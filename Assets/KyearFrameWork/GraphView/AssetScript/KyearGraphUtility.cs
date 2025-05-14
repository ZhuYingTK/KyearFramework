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

    }
}

