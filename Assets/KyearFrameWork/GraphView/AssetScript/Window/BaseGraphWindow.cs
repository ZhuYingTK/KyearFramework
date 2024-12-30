using System;
using System.IO;
using System.Text;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.Experimental.GraphView;
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
    [AssetWindowMapping(typeof(BaseGraphAsset))]
    public class BaseGraphWindow<TGraph,TGraphAsset> : AbstractGraphWindow
    where TGraph : BaseGraph<TGraphAsset,BaseGraphNode>
    where TGraphAsset : BaseGraphAsset
    {
        //当前窗口资源
        [SerializeReference]
        private TGraphAsset m_asset;

        private TGraph m_graph;
        public override string selectedGuid
        {
            get { return m_asset?.guid; }
        }

        private bool isWindowFocused = false;

        /// <summary>
        /// 更新Title
        /// </summary>
        public override void UpdateTitle()
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(selectedGuid);
            string assetName = Path.GetFileNameWithoutExtension(assetPath);
            title = assetName;
            Texture2D icon;
            {
                string theme = EditorGUIUtility.isProSkin ? "_dark" : "_light";
                icon = Resources.Load<Texture2D>("Icons/sg_subgraph_icon_gray" + theme);
            }

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

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="assetGuid"></param>
        public override void Init(BaseGraphAsset asset)
        {
            try
            {
                if (asset == null)
                    return;

                if (!EditorUtility.IsPersistent(asset))
                    return;

                if (selectedGuid != null && selectedGuid == asset.guid)
                    return;
                var path = AssetDatabase.GetAssetPath(asset);
                m_asset = asset as TGraphAsset;
                var graph = rootVisualElement.Q<TGraph>("Graph");
                graph.SetParentWindow(this);
                graph.Init(m_asset);
                m_graph = graph;
                string graphName = Path.GetFileNameWithoutExtension(path);
                UpdateTitle();
                Repaint();
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }
    }
    
}
