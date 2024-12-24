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
        {
            new DialogNodeData()
            {
                position = new Vector2(1,1)
            },
            new DialogNodeData()
            {
                position = new Vector2(6,6)
            }
        };
        
        /// <summary>
        /// 需要更新持久化数据时调用
        /// </summary>
        public void MarkDirty()
        {
            EditorUtility.SetDirty(this);
        }
    }
}
