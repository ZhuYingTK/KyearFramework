using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Kyear.Graph
{
    public class BaseGraphAsset : ScriptableObject
    {
        [ReadOnly]
        public string guid = default;
        //支持BaseGraphNode扩展
        [SerializeReference][ReadOnly]
        public List<BaseGraphNodeData> nodeDataList = new List<BaseGraphNodeData>()
        { };

        #region 添加节点

        /// <summary>
        /// 添加节点
        /// </summary>
        /// <param name="nodeData"></param>
        public void AddNode(BaseGraphNodeData nodeData)
        {
            if(nodeDataList.Exists(e => e.id == nodeData.id))
                return;
            nodeDataList.Add(nodeData);
        }

        public void AddNode(AbstractGraphNode node)
        {
            AddNode(node.GetData());
        }

        #endregion

        
        /// <summary>
        /// 需要更新持久化数据时调用
        /// </summary>
        public void MarkDirty()
        {
            EditorUtility.SetDirty(this);
        }
    }
}
