using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace Kyear.Graph
{
    public class BaseGraph : GraphView
    {
        public new class UxmlFactory : UxmlFactory<BaseGraph, UxmlTraits> { }
        
        
        protected BaseGraphAsset m_graphAsset = null;
        public BaseGraph()
        {
            Init();
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
 
            // 加载uss风格文件
            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Editor/GraphView/Resources/GraphViewBackGround.uss");
            if(styleSheet != null) 
                styleSheets.Add(styleSheet);
            // 添加背景网格
            Insert(0, new GridBackground());
        }
    }
}

