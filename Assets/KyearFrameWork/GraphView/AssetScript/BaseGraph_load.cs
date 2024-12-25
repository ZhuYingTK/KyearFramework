using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

            //加载边
            foreach (BaseGraphNodeData nodeData in m_graphAsset.nodeDataList)
            {
                foreach (BaseEdgeData edgeData in nodeData.edges)
                {
                    Node startNode = m_nodeDic[nodeData.id];
                    Port startPort = (Port)startNode.outputContainer.Children().ToArray()[edgeData.startPortIdx];
                    Node endNode = m_nodeDic[edgeData.target];
                    Port endPort = (Port)endNode.inputContainer.Children().ToArray()[edgeData.endPortIdx];
                    Edge edge = startPort.ConnectTo(endPort);
                    AddElement(edge);
                    startNode.RefreshPorts();
                }
            }
        }

        /// <summary>
        /// 创建节点
        /// </summary>
        /// <returns></returns>
        public BaseGraphNode CreateNode(Type nodeType,Vector2 position)
        {
            BaseGraphNode node = (BaseGraphNode)Activator.CreateInstance(nodeType);
            node.CreateData(position);
            m_graphAsset.AddNode(node);
            AddNode(node);
            return node;
        }
        ///视图内添加节点
        private void AddNode(BaseGraphNode node)
        {
            AddElement(node);
            m_nodeDic[node.ID] = node;
        }
    }
}

