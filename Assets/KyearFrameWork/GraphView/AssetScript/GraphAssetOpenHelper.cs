using System;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace Kyear.Graph
{
    public class GraphAssetOpenHelper : EditorWindow
    {
        [OnOpenAsset(1)]
        private static bool OnOpenAssets(int id, int line)
        {
            if (EditorUtility.InstanceIDToObject(id) is BaseGraphAsset asset)
            {
                foreach (var w in Resources.FindObjectsOfTypeAll<AbstractGraphWindow>())
                {
                    if (w.selectedGuid == asset.guid)
                    {
                        w.Focus();
                        return true;
                    }
                }
                CreateWindow(asset);
                return true;
            }

            return false;
        }

        public static void CreateWindow(BaseGraphAsset asset)
        {
            AbstractGraphWindow window;
            switch (asset)
            {
                case DialogAsset:
                    window = CreateWindow<DialogWindow>(typeof(AbstractGraphWindow));
                    break;
                default:
                    window = CreateWindow<AbstractGraphWindow>(typeof(AbstractGraphWindow));
                    break;
            }
            window.Init(asset);
            window.Focus();
        }
    }
}

