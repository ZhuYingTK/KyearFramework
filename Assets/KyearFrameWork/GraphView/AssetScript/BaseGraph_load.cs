using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Kyear.Graph
{
    public partial class BaseGraph<TSearchWindow,TGraphAsset,TRootNode>: AbstractGraph
    {
        public override void Save()
        {
            foreach (Node node in nodes)
            {
                if (node is AbstractGraphNode baseGraphNode)
                {
                    baseGraphNode.Save();
                }
            }
            MGraphGraphAsset?.MarkDirty();
            Debug.Log("<color=green>[KyearGraphError]  已保存</color>");
        }
        /// <summary>
        /// 加载
        /// </summary>
        public void Load()
        {
            //加载节点
            foreach (BaseGraphNodeData nodeData in MGraphGraphAsset.nodeDataList)
            {
                AbstractGraphNode node = (AbstractGraphNode)Activator.CreateInstance(nodeData.GetTargetType());
                node.Init(nodeData,this);
                AddNode(node);
            }

            //加载边
            foreach (BaseGraphNodeData nodeData in MGraphGraphAsset.nodeDataList)
            {
                foreach (BaseEdgeData edgeData in nodeData.edges)
                {
                    AbstractGraphNode startNode = m_nodeDic[nodeData.id];
                    Port startPort = startNode.outputPortDic[edgeData.startPortID];
                    AbstractGraphNode endNode = m_nodeDic[edgeData.target];
                    Port endPort = endNode.inputPortDic[edgeData.endPortID];
                    Edge edge = startPort.ConnectTo(endPort);
                    edge.userData = edgeData;
                    AddElement(edge);
                    startNode.RefreshPorts();
                }
            }
        }

        /// <summary>
        /// 创建节点
        /// </summary>
        /// <returns></returns>
        public override AbstractGraphNode CreateNode(Type nodeType,Vector2 position)
        {
            AbstractGraphNode node = (AbstractGraphNode)Activator.CreateInstance(nodeType);
            node.CreateData(position,this);
            MGraphGraphAsset.AddNode(node);
            AddNode(node);
            return node;
        }
        ///视图内添加节点
        private void AddNode(AbstractGraphNode node)
        {
            AddElement(node);
            m_nodeDic[node.GetID()] = node;
        }
    }
}

