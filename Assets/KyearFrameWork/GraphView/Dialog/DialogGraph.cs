using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Kyear.Graph
{
    public class DialogGraph : BaseGraph<DialogSearchWindow,DialogAsset>
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
                },
                new SearchTreeEntry(new GUIContent("对话标题节点", indentationIcon))
                {
                    userData = typeof(DialogTitleNode),
                    level = 1
                },
                new SearchTreeEntry(new GUIContent("图片节点", indentationIcon))
                {
                userData = typeof(DialogTextureNode),
                level = 1 
                },
                new SearchTreeEntry(new GUIContent("选择节点", indentationIcon))
                {
                    userData = typeof(DialogChoiceNode),
                    level = 1 
                },
            };
            return searchTreeEntries;
        }
    }

}
