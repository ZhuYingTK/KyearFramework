using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;

namespace Kyear.Graph
{
    public class DialogWindow : BaseGraphWindow<DialogGraph,DialogAsset>
    {
        protected override void SetToolButtons()
        {
            var button = new ToolbarButton(() => { PrintData(); }) { text = "导出" };
            m_toolbar.Add(button);
        }

        private void PrintData()
        {
            m_graph.GetRootNode();
        }
    }

}
