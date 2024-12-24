using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Kyear.Graph
{
    public partial class BaseGraph
    {
        public void Save()
        {
            foreach (Node node in nodes)
            {
                if (node is BaseGraphNode baseGraphNode)
                {
                    baseGraphNode.Save();
                }
            }
            m_graphAsset?.MarkDirty();
        }
        /// <summary>
        /// 加载
        /// </summary>
        public void Load()
        {
            //加载节点
            foreach (BaseGraphNodeData nodeData in m_graphAsset.nodeDataList)
            {
                BaseGraphNode node = (BaseGraphNode)Activator.CreateInstance(nodeData.GetTargetType());
                node.Init(nodeData);
                AddNode(node);
            }
        }
        ///添加节点
        public void AddNode(BaseGraphNode node)
        {
            AddElement(node);
        }
    }
}

