using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Kyear.Graph
{
    [Serializable]
    public class BasePortData
    {
        [SerializeField] public string name;
        [SerializeField] public uint ID;
        [SerializeField] public Port.Capacity capacity;

        public BasePortData(string name, uint id, Port.Capacity capacity)
        {
            this.name = name;
            ID = id;
            this.capacity = capacity;
        }
    }
  
    [Serializable]
    public class BaseEdgeData
    {
        [SerializeField] public string target;      //目标GUID
        [SerializeField] public uint startPortID;   //起点的端口ID
        [SerializeField] public uint endPortID;     //终点的端口ID
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
            return typeof(AbstractGraphNode);
        }
    }
}