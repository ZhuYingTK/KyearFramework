using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
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

    public class DialogTitleNode : BaseGraphNode<DialogTitleNodeData>
    {
        private TextField textTextField;
        public override void Save()
        {
            data.content = textTextField.value;
            base.Save();
        }

        public override void Init(BaseGraphNodeData data, AbstractGraph parent)
        {
            base.Init(data, parent);
            textTextField.value = base.data.content;
            title = "对话标题节点";
        }

        public override void CreateData(Vector2 position, AbstractGraph parent)
        {
            DialogTitleNodeData data = new DialogTitleNodeData()
            {
                id = Guid.NewGuid().ToString(),
                position = position,
                outputPorts = new List<BasePortData>()
                {
                    new("输出", GeneratePortID(PortType.Output), Port.Capacity.Multi)
                }
            };
            Debug.Log($"[KyearGraphError]  创建节点:{data.id}");
            Init(data, parent);
        }

        public override void Draw_ExtensionContainer()
        {
            VisualElement customDataContainer = new VisualElement();
            customDataContainer.AddToClassList("kyear-node__custom-data-container");
            textTextField = CreateTextField("标题");
            extensionContainer.Add(textTextField);
            base.Draw_ExtensionContainer();
        }
    }
}
