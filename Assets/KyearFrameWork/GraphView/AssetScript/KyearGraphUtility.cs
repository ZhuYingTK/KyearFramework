using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Edge = UnityEditor.Experimental.GraphView.Edge;

namespace Kyear.Graph
{
    public static class KyearGraphUtility
    {
        public static AbstractGraphNode GetStartNode(this Edge @this)
        {
            return @this.output.node as AbstractGraphNode;
        }
        public static bool TryGetStartNode(this Edge @this, out AbstractGraphNode startNode)
        {
            startNode = @this.GetStartNode();
            if (startNode == null)
                return false;
            return true;
        }
        
        public static AbstractGraphNode GetEndNode(this Edge @this)
        {
            return @this.output.node as AbstractGraphNode;
        }
        public static bool TryGetEndNode(this Edge @this, out AbstractGraphNode endNode)
        {
            endNode = @this.GetEndNode();
            if (endNode == null)
                return false;
            return true;
        }

        public static void RemoveData(this Edge @this)
        {
            BaseEdgeData edgeData = (BaseEdgeData) @this.userData;
            if(edgeData == null)
                return;
            if (@this.TryGetStartNode(out var startNode))
            {
                startNode.GetData().edges.Remove(edgeData);
            }
        }

        #region 排版

        //对齐Text的Label
        public static void AlignmentTextLabel(this Foldout @this)
        {
            @this.MarkDirtyRepaint();
            @this.schedule.Execute(() =>
            {
                var texts = @this.Children().OfType<TextField>();
                float maxwidth = -1;
                foreach (var text in texts)
                {
                    if (maxwidth < text.labelElement.contentContainer.resolvedStyle.width)
                    {
                        maxwidth = text.labelElement.resolvedStyle.width;
                    }
                }

                foreach (var text in texts)
                {
                    text.labelElement.style.minWidth = maxwidth;
                }
            });
        }

        public static void SetToDefaultStyle(this Label @this)
        {
            @this.style.minWidth  = 0;
            @this.MarkDirtyRepaint();
            @this.style.fontSize = 14;
            @this.style.unityFontStyleAndWeight = FontStyle.Bold;
        }

        #endregion

        #region GUID加载

        /// <summary>
        /// 获取Unity资源的GUID
        /// </summary>
        /// <param name="assetObject">Unity资源对象</param>
        /// <returns>资源的GUID，如果无效返回null</returns>
        public static string GetGuidFromObject(Object assetObject)
        {
#if UNITY_EDITOR
            if (assetObject == null)
            {
                return null;
            }

            string path = AssetDatabase.GetAssetPath(assetObject);
            if (string.IsNullOrEmpty(path))
            {
                return null;
            }

            return AssetDatabase.AssetPathToGUID(path);
#else
        Debug.LogError("GetGuidFromObject is only available in the Unity Editor");
        return null;
#endif
        }

        /// <summary>
        /// 通过GUID加载Unity资源
        /// </summary>
        /// <param name="guid">资源GUID</param>
        /// <returns>资源对象，如果无效返回null</returns>
        public static T LoadObjectFromGuid<T>(string guid) where T : Object
        {
#if UNITY_EDITOR
            if (string.IsNullOrEmpty(guid))
            {
                return null;
            }

            string path = AssetDatabase.GUIDToAssetPath(guid);
            if (string.IsNullOrEmpty(path))
            {
                return null;
            }

            return AssetDatabase.LoadAssetAtPath<T>(path);
#else
        Debug.LogError("LoadObjectFromGuid is only available in the Unity Editor");
        return null;
#endif
        }

        #endregion

    }
}

