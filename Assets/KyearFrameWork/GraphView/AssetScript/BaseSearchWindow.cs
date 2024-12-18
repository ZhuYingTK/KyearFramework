using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Kyear.Graph
{
    public class BaseSearchWindow : ScriptableObject, ISearchWindowProvider
    {
        private BaseGraph graphView;
        private Texture2D indentationIcon;
        public void Initialize(BaseGraph dsGraphView)
        {
            graphView = dsGraphView;

            indentationIcon = new Texture2D(1, 1);
            indentationIcon.SetPixel(0, 0, Color.clear);
            indentationIcon.Apply();
        }
        //创建搜索树
        public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
        {
            throw new System.NotImplementedException();
        }
        
        //选择元素
        public bool OnSelectEntry(SearchTreeEntry SearchTreeEntry, SearchWindowContext context)
        {
            throw new System.NotImplementedException();
        }
    }

}
