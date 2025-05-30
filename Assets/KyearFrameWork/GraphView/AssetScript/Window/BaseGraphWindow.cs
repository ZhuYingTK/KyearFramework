using System;
using System.IO;
using System.Text;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace Kyear.Graph
{
    public class AbstractGraphWindow : EditorWindow
    {
        public virtual string selectedGuid { get; }
        public virtual void UpdateTitle(){}
        public virtual void AssetWasDeleted(){}
        public virtual void CheckForChanges(){}
        public virtual void Init(BaseGraphAsset asset){}
    }
    public class BaseGraphWindow<TGraph,TGraphAsset> : AbstractGraphWindow
    where TGraph : AbstractGraph,new()
    where TGraphAsset : BaseGraphAsset
    {
        //当前窗口资源
        [SerializeReference]
        private TGraphAsset m_asset;

        protected TGraph m_graph;
        public override string selectedGuid
        {
            get { return m_asset?.guid; }
        }

        private bool isWindowFocused = false;
        protected Toolbar m_toolbar;

        /// <summary>
        /// 更新Title
        /// </summary>
        public override void UpdateTitle()
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(selectedGuid);
            string assetName = Path.GetFileNameWithoutExtension(assetPath);
            title = assetName;
            Texture2D icon;
            string theme = EditorGUIUtility.isProSkin ? "_dark" : "_light";
            icon = Resources.Load<Texture2D>("Icons/sg_subgraph_icon_gray" + theme);

            titleContent = new GUIContent(title, icon);
        }

        /// <summary>
        /// 检查更新
        /// </summary>
        public override void CheckForChanges()
        {
            
        }

        /// <summary>
        /// 有资源被删除
        /// </summary>
        public override void AssetWasDeleted()
        {
            Close();
        }
        
        /// <summary>
        /// 创建GUI,来自UIToolkit
        /// </summary>
        public void CreateGUI()
        {
            VisualElement root = rootVisualElement;
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/KyearFramework/GraphView/AssetScript/Window/BaseGraphWindow.uxml");
            visualTree.CloneTree(root);
            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/KyearFramework/GraphView/AssetScript/Window/BaseGraphWindow.uss");
            root.styleSheets.Add(styleSheet);
            //添加自定义根元素
            var rootSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/KyearFramework/GraphView/Resources/KyearVariableStyles.uss");
            root.styleSheets.Add(rootSheet);
        }

        /// <summary>
        /// 焦点响应
        /// </summary>
        public void OnFocus()
        {
            
        }

        /// <summary>
        /// 失去焦点
        /// </summary>
        public void OnLostFocus()
        {
            
        }

        private void OnDisable()
        {
            m_graph.Save();
        }

        protected virtual void SetToolButtons()
        {
            
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="assetGuid"></param>
        public override void Init(BaseGraphAsset asset)
        {
            if (asset == null)
                return;

            if (!EditorUtility.IsPersistent(asset))
                return;

            if (selectedGuid != null && selectedGuid == asset.guid)
                return;
            var path = AssetDatabase.GetAssetPath(asset);
            m_asset = asset as TGraphAsset;
            AbstractGraph oldGraph = rootVisualElement.Q<AbstractGraph>("Graph");
            VisualElement parent = oldGraph.parent; // 获取父节点
            int index = parent.IndexOf(oldGraph);   // 获取原来子元素的位置

            // 从父节点移除旧图表
            parent.Remove(oldGraph);

            // 创建新的 TGraph 对象
            TGraph graph = new TGraph();  // 假设 TGraph 有一个默认构造函数
            graph.name = oldGraph.name;   // 如果需要保留旧对象的名字或其他属性
            graph.style.flexGrow = 1;
            // 将新对象插入到原来的位置
            parent.Insert(index, graph);
            graph.SetParentWindow(this);
            graph.Init(m_asset);
            m_graph = graph;
            
            Toolbar oldToolbar = rootVisualElement.Q<Toolbar>("Toolbar");
            parent = oldToolbar.parent; // 获取父节点
            index = parent.IndexOf(oldToolbar);
            parent.Remove(oldToolbar);
            m_toolbar = new Toolbar();
            parent.Insert(index,m_toolbar);
            
            
            UpdateTitle();
            Repaint();
            SetToolButtons();
        }
    }
    
}
