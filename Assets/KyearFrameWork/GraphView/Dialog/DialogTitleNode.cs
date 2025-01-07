using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kyear.Graph
{
    [Serializable]
    public class DialogTitleNodeData : BaseGraphNodeData
    {
        [SerializeField] public string content;

        public override Type GetTargetType()
        {
            return typeof(DialogTitleNode);
        }
    }

    public class DialogTitleNode : BaseGraphNode
    {
        public override void CreateData(Vector2 position, AbstractGraph parent)
        {
            DialogNodeData data = new DialogNodeData()
            {
                id = Guid.NewGuid().ToString(),
                position = position,
                inputPorts = new List<BasePortData>(){new BasePortData(){name = "输入",ID = GeneratePortID(PortType.Input)}},
                outputPorts = new List<BasePortData>(){new BasePortData(){name = "输出",ID = GeneratePortID(PortType.Input)}},
            };
            Debug.Log($"[KyearGraphError]  创建节点:{data.id}");
            Init(data,parent);
        }
    }
}
