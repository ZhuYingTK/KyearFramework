using System;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace Kyear.Graph
{
    [System.AttributeUsage(System.AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class AssetWindowMappingAttribute : Attribute
    {
        public Type WindowType { get; }
    
        public AssetWindowMappingAttribute(Type windowType)
        {
            WindowType = windowType;
        }
    }
    
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
                var window = CreateWindow<AbstractGraphWindow>(typeof(AbstractGraphWindow), typeof(SceneView));
                window.Init(asset);
                window.Focus();
                return true;
            }

            return false;
        }
    }
}

