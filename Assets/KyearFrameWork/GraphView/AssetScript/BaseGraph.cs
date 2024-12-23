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
        //当前聚焦对象
        private object m_currentFocusObject;
        
        protected BaseGraphAsset m_graphAsset = null;

        public void SetParentWindow(EditorWindow window)
        {
            parentWindow = window;
        }
        
        public void Init(BaseGraphAsset asset)
        {
            m_graphAsset = asset;
            AddManipulators();
            AddStyles();
            nodeCreationRequest += context =>
            {
                CreateNode(context);
            };
            RegisterCallback<PointerDownEvent>(OnPointerDown);
            graphViewChanged += OnGraphViewChanged;
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
                    var sourcePort = edge.input;
                    var destPort = edge.output;
                }
            }
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

        private void OnPointerDown(PointerDownEvent evt)
        {
            if (evt.target != m_currentFocusObject)
            {
                m_graphAsset?.MarkDirty();
                m_currentFocusObject = evt.target;
            }
        }

        public void CreateNode(NodeCreationContext context)
        {
            // 获取鼠标位置
            Vector2 pos = GetLocalMousePosition(context.screenMousePosition);

            // 创建节点并初始化位置
            var node = new DialogNode();
            node.Init(pos);
            AddElement(node);
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
