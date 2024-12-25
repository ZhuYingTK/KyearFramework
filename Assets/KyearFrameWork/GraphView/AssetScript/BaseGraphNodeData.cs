using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kyear.Graph
{
    [Serializable]
    public class BasePortData
    {
        [SerializeField] public string name;
    }

    [Serializable]
    public class BaseEdgeData
    {
        [SerializeField] public string target;      //目标GUID
        [SerializeField] public int startPortIdx;   //起点的端口ID
        [SerializeField] public int endPortIdx;     //终点的端口ID
    }
    
    [Serializable]
    public class BaseGraphNodeData
    {
        [SerializeField] public Vector2 position = Vector2.zero;
        [SerializeField] public string id = null;
        [SerializeReference] public List<BasePortData> inputPorts = new List<BasePortData>();
        [SerializeReference] public List<BasePortData> outputPorts = new List<BasePortData>();
        [SerializeReference] public List<BaseEdgeData> edges = new List<BaseEdgeData>();

        public virtual Type GetTargetType()
        {
            return typeof(BaseGraphNode);
        }
    }

    [Serializable]
    public class DialogNodeData : BaseGraphNodeData
    {
        [SerializeField] public string content;

        public override Type GetTargetType()
        {
            return typeof(DialogNode);
        }
    }
}