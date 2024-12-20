using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Kyear.Graph
{
    public class BaseGraph : GraphView
    {
        public new class UxmlFactory : UxmlFactory<BaseGraph, UxmlTraits> { }

        private EditorWindow parentWindow;
        
        protected BaseGraphAsset m_graphAsset = null;

        public BaseGraph()
        {
            Init();
            AddElement(new BaseGraphNode());
        }

        public void SetParentWindow(EditorWindow window)
        {
            parentWindow = window;
        }
        
        private void Init()
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
 
            // 加载USS风格文件
            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/KyearFramework/GraphView/Resources/GraphViewBackGround.uss");
            var nodeStyleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/KyearFramework/GraphView/Resources/KyearGraphNodeStyles.uss");
            if (styleSheet != null)
                styleSheets.Add(styleSheet);
            if(nodeStyleSheet != null)
                styleSheets.Add(nodeStyleSheet);
            // 添加背景网格
            Insert(0, new GridBackground());

            nodeCreationRequest += context =>
            {
                CreateNode(context);
            };
        }

        public void CreateNode(NodeCreationContext context)
        {
            // 获取鼠标位置
            Vector2 pos = GetLocalMousePosition(context.screenMousePosition);

            // 创建节点并初始化位置
            var node = new BaseGraphNode();
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
    }
}
