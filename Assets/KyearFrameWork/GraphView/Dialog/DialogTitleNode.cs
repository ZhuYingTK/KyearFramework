using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

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
            DialogTitleNodeData data = new DialogTitleNodeData()
            {
                id = Guid.NewGuid().ToString(),
                position = position,
                outputPorts = new List<BasePortData>(){new BasePortData(){name = "输出",ID = GeneratePortID(PortType.Input)}},
            };
            Debug.Log($"[KyearGraphError]  创建节点:{data.id}");
            Init(data,parent);
        }

        public override void Draw_ExtensionContainer()
        {
            VisualElement customDataContainer = new VisualElement();
            customDataContainer.AddToClassList("kyear-node__custom-data-container");
            TextField textTextField = CreateTextField("标题");
            extensionContainer.Add(textTextField);
            base.Draw_ExtensionContainer();
        }
    }
}
