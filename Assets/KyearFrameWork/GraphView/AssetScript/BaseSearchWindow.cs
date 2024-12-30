using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Kyear.Graph
{
    public class BaseSearchWindow : ScriptableObject, ISearchWindowProvider
    {
        private IBaseGraph graphView;
        private Texture2D indentationIcon;
        public void Initialize(IBaseGraph dsGraphView)
        {
            graphView = dsGraphView;

            indentationIcon = new Texture2D(1, 1);
            indentationIcon.SetPixel(0, 0, Color.clear);
            indentationIcon.Apply();
        }
        //创建搜索树
        public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
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
        
        //当选择元素
        public bool OnSelectEntry(SearchTreeEntry SearchTreeEntry, SearchWindowContext context)
        {
            Vector2 localMousePosition = graphView.GetLocalMousePosition(context.screenMousePosition);
            switch (SearchTreeEntry.userData)
            {
                //如果传入类型,就生成节点
                case Type:
                    graphView.CreateNode((Type)SearchTreeEntry.userData, localMousePosition);
                    return true;
                default:
                    return false;
            }
        }
    }

}
