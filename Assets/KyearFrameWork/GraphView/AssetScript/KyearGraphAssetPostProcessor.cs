using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Kyear.Graph
{
    using UnityEngine;
using System;
using System.IO;
using System.Linq;
using UnityEditor;

namespace UnityEditor.ShaderGraph
{
    class ShaderGraphAssetPostProcessor : AssetPostprocessor
    {
        private const string Extension = "asset";
        static void UpdateAfterAssetChange(string[] newNames)
        {
            // This will change the title of the window.
            BaseGraphWindow[] windows = Resources.FindObjectsOfTypeAll<BaseGraphWindow>();
            foreach (var matGraphEditWindow in windows)
            {
                for (int i = 0; i < newNames.Length; ++i)
                {
                    if (matGraphEditWindow.selectedGuid == AssetDatabase.AssetPathToGUID(newNames[i]))
                        matGraphEditWindow.UpdateTitle();
                }
            }
        }

        static void DisplayDeletionDialog(string[] deletedAssets)
        {
            BaseGraphWindow[] windows = Resources.FindObjectsOfTypeAll<BaseGraphWindow>();
            foreach (var matGraphEditWindow in windows)
            {
                for (int i = 0; i < deletedAssets.Length; ++i)
                {
                    if (matGraphEditWindow.selectedGuid == AssetDatabase.AssetPathToGUID(deletedAssets[i]))
                        matGraphEditWindow.AssetWasDeleted();
                }
            }
        }

        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {

            // 移动资源
            bool anyMovedShaders = movedAssets.Any(val => val.EndsWith(Extension, StringComparison.InvariantCultureIgnoreCase));
            anyMovedShaders |= movedAssets.Any(val => val.EndsWith(Extension, StringComparison.InvariantCultureIgnoreCase));
            if (anyMovedShaders)
                UpdateAfterAssetChange(movedAssets);

            // 删除资源
            bool anyRemovedShaders = deletedAssets.Any(val => val.EndsWith(Extension, StringComparison.InvariantCultureIgnoreCase));
            anyRemovedShaders |= deletedAssets.Any(val => val.EndsWith(Extension, StringComparison.InvariantCultureIgnoreCase));
            if (anyRemovedShaders)
                DisplayDeletionDialog(deletedAssets);

            var changedGraphGuids = importedAssets
                .Where(x => x.EndsWith(Extension, StringComparison.InvariantCultureIgnoreCase) && AssetDatabase.LoadAssetAtPath<BaseGraphAsset>(x)!= null)
                .Select(LoadAndInitAsset)
                .ToList();
            
            BaseGraphWindow[] windows = null;
            if (changedGraphGuids.Count > 0)
            {
                windows = Resources.FindObjectsOfTypeAll<BaseGraphWindow>();
                foreach (var window in windows)
                {
                    if (changedGraphGuids.Contains(window.selectedGuid))
                    {
                        window.CheckForChanges();
                    }
                }
            }

            string LoadAndInitAsset(string path)
            {
                string guid = AssetDatabase.AssetPathToGUID(path);
                var asset = AssetDatabase.LoadAssetAtPath<BaseGraphAsset>(path);
                if (asset != null)
                {
                    asset.guid = guid;
                }
                return AssetDatabase.AssetPathToGUID(guid);
            }
            // moved or imported subgraphs or HLSL files should notify open shadergraphs that they need to handle them
            //var changedFileGUIDs = movedAssets.Concat(importedAssets).Concat(deletedAssets)
            //    .Where(x =>
            //    {
            //        if (x.EndsWith(Extension, StringComparison.InvariantCultureIgnoreCase) || CustomFunctionNode.s_ValidExtensions.Contains(Path.GetExtension(x)))
            //        {
            //            return true;
            //        }
            //        else
            //        {
            //            var asset = AssetDatabase.GetMainAssetTypeAtPath(x);
            //            return asset is null || asset.IsSubclassOf(typeof(Texture));
            //        }
            //    })
            //    .Select(AssetDatabase.AssetPathToGUID)
            //    .Distinct()
            //    .ToList();
//
            //if (changedFileGUIDs.Count > 0)
            //{
            //    if (windows == null)
            //    {
            //        windows = Resources.FindObjectsOfTypeAll<BaseGraphWindow>();
            //    }
            //    foreach (var window in windows)
            //    {
            //        window.ReloadSubGraphsOnNextUpdate(changedFileGUIDs);
            //    }
            //}
        }
    }
}

}