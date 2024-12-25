using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Kyear.Graph
{
    [CreateAssetMenu(menuName = "测试脚本/Kyear", fileName = "新文件", order = 81)]
    public class BaseGraphAsset : ScriptableObject
    {
        [HideInInspector]
        public string guid = default;
        //支持BaseGraphNode扩展
        [SerializeReference]
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

        public void AddNode(BaseGraphNode node)
        {
            nodeDataList.Add(node.data);
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
