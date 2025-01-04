using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Kyear.Graph
{
    public class DialogGraph : BaseGraph<DialogSearchWindow,DialogAsset,DialogNode>
    {
    }

    public class DialogSearchWindow : BaseSearchWindow
    {
        public override List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
        {
            List<SearchTreeEntry> searchTreeEntries = new List<SearchTreeEntry>()
            {
                new SearchTreeGroupEntry(new GUIContent("创建节点")),
                new SearchTreeEntry(new GUIContent("对话节点", indentationIcon))
                {
                    userData = typeof(DialogNode),
                    level = 1
                }
            };
            return searchTreeEntries;
        }
    }

}
