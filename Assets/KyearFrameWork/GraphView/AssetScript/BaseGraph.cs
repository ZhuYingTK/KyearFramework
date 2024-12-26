using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Kyear.Graph
{
    public partial class BaseGraph : GraphView
    {
        public new class UxmlFactory : UxmlFactory<BaseGraph, UxmlTraits> { }

        private EditorWindow parentWindow;
        private BaseSearchWindow searchWindow;
        
        protected BaseGraphAsset m_graphAsset = null;
        protected Dictionary<string, BaseGraphNode> m_nodeDic = new Dictionary<string, BaseGraphNode>();

        public void SetParentWindow(EditorWindow window)
        {
            parentWindow = window;
        }
        
        public void Init(BaseGraphAsset asset)
        {
            m_graphAsset = asset;
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
                    BaseGraphNode sourceNode = sourcePort.node as BaseGraphNode;
                    if (sourceNode == null)
                    {
                        Debug.LogError("[KyearGraphError]  起源节点不是BaseGraphNode");
                        continue;
                    }
                    sourceNode.AddEdge(edge);
                }
            }
            
            m_graphAsset.MarkDirty();
            return graphViewChange;
        }
        

        private void AddManipulators()
        {
            // 开启Graph缩放
            SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);
            this.AddManipulator(new ContentZoomer());
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

        public Vector2 GetLocalMousePosition(Vector2 screenPosition)
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
                searchWindow = ScriptableObject.CreateInstance<BaseSearchWindow>();
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
    }
}
