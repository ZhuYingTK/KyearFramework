using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine;
using System;
using System.IO;
using System.Linq;
using UnityEditor;

namespace Kyear.Graph
{
    class GraphAssetPostProcessor : AssetPostprocessor
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

        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets,
            string[] movedFromAssetPaths)
        {

            // 移动资源
            bool anyMovedassets = movedAssets.Any(val =>
                val.EndsWith(Extension, StringComparison.InvariantCultureIgnoreCase));
            anyMovedassets |= movedAssets.Any(val =>
                val.EndsWith(Extension, StringComparison.InvariantCultureIgnoreCase));
            if (anyMovedassets)
                UpdateAfterAssetChange(movedAssets);

            // 删除资源
            bool anyRemovedassets = deletedAssets.Any(val =>
                val.EndsWith(Extension, StringComparison.InvariantCultureIgnoreCase));
            anyRemovedassets |= deletedAssets.Any(val =>
                val.EndsWith(Extension, StringComparison.InvariantCultureIgnoreCase));
            if (anyRemovedassets)
                DisplayDeletionDialog(deletedAssets);

            //引入的资源
            var changedGraphGuids = importedAssets
                .Where(x => x.EndsWith(Extension, StringComparison.InvariantCultureIgnoreCase) &&
                            AssetDatabase.LoadAssetAtPath<BaseGraphAsset>(x) != null)
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
            //初始化并返回GUID
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
        }
    }
}