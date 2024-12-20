using System;
using System.IO;
using System.Text;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace Kyear.Graph
{
    public class BaseGraphWindow : EditorWindow
    {
        //当前窗口对应的资源的GUID
        [SerializeField]
        string m_Selected;
        public string selectedGuid
        {
            get { return m_Selected; }
            private set
            {
                m_Selected = value;
            }
        }

        /// <summary>
        /// 更新Title
        /// </summary>
        public void UpdateTitle()
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
        public void CheckForChanges()
        {
            
        }

        /// <summary>
        /// 有资源被删除
        /// </summary>
        public void AssetWasDeleted()
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
            var graph = root.Q<BaseGraph>("Graph");
            graph.SetParentWindow(this);
        }
        
        [OnOpenAsset(1)]
        private static bool OnOpenAssets(int id, int line)
        {
            if (EditorUtility.InstanceIDToObject(id) is BaseGraphAsset asset)
            {
                foreach (var w in Resources.FindObjectsOfTypeAll<BaseGraphWindow>())
                {
                    if (w.selectedGuid == asset.guid)
                    {
                        w.Focus();
                        return true;
                    }
                }
                var window = CreateWindow<BaseGraphWindow>(typeof(BaseGraphWindow), typeof(SceneView));
                window.Init(asset.guid);
                window.Focus();
                return true;
            }

            return false;
        }
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="assetGuid"></param>
        private void Init(string assetGuid)
        {
            try
            {
                var asset = AssetDatabase.LoadAssetAtPath<Object>(AssetDatabase.GUIDToAssetPath(assetGuid));
                if (asset == null)
                    return;

                if (!EditorUtility.IsPersistent(asset))
                    return;

                if (selectedGuid == assetGuid)
                    return;
                var path = AssetDatabase.GetAssetPath(asset);
                selectedGuid = assetGuid;
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
