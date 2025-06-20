using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Kyear.Graph
{
    public class AbstractGraph : GraphView
    {
        public new class UxmlFactory : UxmlFactory<AbstractGraph, UxmlTraits> { }
        public virtual Vector2 GetLocalMousePosition(Vector2 screenPosition){return Vector2.zero;}

        public virtual AbstractGraphNode CreateNode(Type nodeType, Vector2 position) { return null;}
        public virtual void Save(){}
        public virtual void SetParentWindow(EditorWindow window){}
        public virtual void Init(BaseGraphAsset graphAsset){}
    }
    public partial class BaseGraph<TSearchWindow,TGraphAsset>: AbstractGraph
    where TSearchWindow : BaseSearchWindow
    where TGraphAsset : BaseGraphAsset
    {
        private EditorWindow parentWindow;
        private TSearchWindow searchWindow;
        
        protected TGraphAsset MGraphGraphAsset = null;
        protected Dictionary<string, AbstractGraphNode> m_nodeDic = new Dictionary<string, AbstractGraphNode>();

        public override void SetParentWindow(EditorWindow window)
        {
            parentWindow = window;
        }
        
        public override void Init(BaseGraphAsset graphAsset)
        {
            MGraphGraphAsset = graphAsset as TGraphAsset;
            Load();
            AddManipulators();
            AddStyles();
            AddSearchWindow();
            deleteSelection += OnDeleteSelection;
            graphViewChanged += OnGraphViewChanged;
        }
        /// <summary>
        /// 响应删除元素
        /// </summary>
        /// <param name="operationname"></param>
        /// <param name="askuser"></param>
        private void OnDeleteSelection(string operationname, AskUser askuser)
        {
            List<AbstractGraphNode> nodeToDelete = new List<AbstractGraphNode>();
            List<Edge> edgeToDelete = new List<Edge>();
            foreach (GraphElement graphElement in selection)
            {
                switch (graphElement)
                {
                    case AbstractGraphNode:
                    {
                        nodeToDelete.Add((AbstractGraphNode)graphElement);
                        continue;
                    }
                    case Edge:
                    {
                        edgeToDelete.Add((Edge)graphElement);
                        continue;
                    }
                }
            }

            //删除边
            //只要删除存储该边的数据即可
            foreach (Edge edge in edgeToDelete)
            {
                edge.RemoveData();
            }
            //要调用这个方法，会自动做一些事情
            DeleteElements(edgeToDelete);

            foreach (AbstractGraphNode node in nodeToDelete)
            {
                //删除所有输入的边数据
                foreach (Edge inPutEdge in node.GetAllInPutEdges())
                {
                    inPutEdge.RemoveData();
                }
                //由于数据的边数据就存在当前节点上，所以不用删
                DeleteElements(node.GetAllInPutEdges());
                DeleteElements(node.GetAllOutPutEdges());
                MGraphGraphAsset.nodeDataList.Remove(node.GetData());
            }
            DeleteElements(nodeToDelete);
            
            Save();
        }

        /// <summary>
        /// 视图变化响应
        /// </summary>
        /// <param name="graphViewChange"></param>
        /// <returns></returns>
        private GraphViewChange OnGraphViewChanged(GraphViewChange graphViewChange)
        {
            //对边进行判断
            if (graphViewChange.edgesToCreate != null)
            {
                foreach (Edge edge in graphViewChange.edgesToCreate)
                {
                    var sourcePort = edge.output;
                    AbstractGraphNode sourceNode = sourcePort.node as AbstractGraphNode;
                    if (sourceNode == null)
                    {
                        Debug.LogError("[KyearGraphError]  起源节点不是BaseGraphNode");
                        continue;
                    }
                    sourceNode.AddEdge(edge);
                }
            }
            
            if (graphViewChange.elementsToRemove != null)
            {
                foreach (GraphElement element in graphViewChange.elementsToRemove)
                {
                    if (element is Edge edge)
                    {
                        edge.RemoveData();
                    }
                }
            }
            
            MGraphGraphAsset.MarkDirty();
            return graphViewChange;
        }
        
        private void AddManipulators()
        {
            // 开启Graph缩放
            this.AddManipulator(new ContentZoomer(){minScale = 0.3f,maxScale = 2});
            // 添加拖动Content功能
            this.AddManipulator(new ContentDragger());
            // 添加拖拽选中功能
            this.AddManipulator(new SelectionDragger());
            // 添加框选功能
            this.AddManipulator(new RectangleSelector());
        }
        

        private void AddStyles()
        {
            // 加载USS风格文件
            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/KyearFramework/GraphView/Resources/GraphViewBackGround.uss");
            var nodeStyleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/KyearFramework/GraphView/Resources/KyearGraphNodeStyles.uss");
            if (styleSheet != null)
                styleSheets.Add(styleSheet);
            if(nodeStyleSheet != null)
                styleSheets.Add(nodeStyleSheet);
            // 添加背景网格
            Insert(0, new GridBackground());
        }

        public override Vector2 GetLocalMousePosition(Vector2 screenPosition)
        {
            // 第一步：将屏幕坐标转换为 EditorWindow 坐标系
            Vector2 editorWindowPosition = screenPosition - parentWindow.position.position;

            // 第二步：将 EditorWindow 坐标转换为 GraphView 的本地坐标
            float scale = contentContainer.resolvedStyle.scale.value.x;
            Vector2 localPosition = contentViewContainer.WorldToLocal(editorWindowPosition);

            // 考虑缩放
            localPosition /= scale;

            return localPosition;
        }
        
        private void AddSearchWindow()
        {
            if (searchWindow == null)
            {
                searchWindow = ScriptableObject.CreateInstance<TSearchWindow>();
            }

            searchWindow.Initialize(this);

            nodeCreationRequest = context => SearchWindow.Open(new SearchWindowContext(context.screenMousePosition), searchWindow);
        }
        
        /// <summary>
        /// 返回可连接对象
        /// </summary>
        /// <param name="startPort"></param>
        /// <param name="nodeAdapter"></param>
        /// <returns></returns>
        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            List<Port> compatiblePorts = new List<Port>();

            ports.ForEach(port =>
            {
                if (startPort == port)
                {
                    return;
                }

                if (startPort.node == port.node)
                {
                    return;
                }

                if (startPort.direction == port.direction)
                {
                    return;
                }

                compatiblePorts.Add(port);
            });

            return compatiblePorts;
        }

        #region 访问工具

        public IEnumerable<AbstractGraphNode> Nodes
        {
            get => m_nodeDic.Values;
        }
        public AbstractGraphNode GetRootNode()
        {
            AbstractGraphNode rootNode = null;
            foreach (var node in Nodes)
            {

                if (!node.GetBeforeNodes().Any())
                {
                    if (rootNode != null)
                    {
                        Debug.Log("<color=red>[KyearGraphError]  出现多个根节点</color>");
                        return null;
                    }
                    rootNode = node;
                }
            }
            return rootNode;
        }

        #endregion
    }
}
