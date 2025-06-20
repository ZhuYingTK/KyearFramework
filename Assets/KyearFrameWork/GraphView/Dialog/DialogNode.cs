using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Kyear.Graph
{
    [Serializable]
    public class DialogNodeData : BaseGraphNodeData
    {
        [SerializeField] public string content;

        public override Type GetTargetType()
        {
            return typeof(DialogNode);
        }
    }
    
    public class DialogNode : BaseGraphNode<DialogNodeData>
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
            title = "对话节点";
        }


        public override void CreateData(Vector2 position, AbstractGraph parent)
        {
            DialogNodeData data = new DialogNodeData()
            {
                id = Guid.NewGuid().ToString(),
                position = position,
                inputPorts = new List<BasePortData>()
                {
                    new("输入", GeneratePortID(PortType.Input), Port.Capacity.Multi)
                },
                outputPorts = new List<BasePortData>()
                {
                    new("输出", GeneratePortID(PortType.Output), Port.Capacity.Multi)
                },
            };
            Debug.Log($"[KyearGraphError]  创建节点:{data.id}");
            Init(data, parent);
        }

        public override void Draw_ExtensionContainer()
        {
            VisualElement customDataContainer = new VisualElement();
            customDataContainer.AddToClassList("kyear-node__custom-data-container");
            Foldout textFoldout = new Foldout()
            {
                text = "展开",
                value = true
            };

            textTextField = CreateTextField("对话");
            textFoldout.Add(textTextField);
            textFoldout.AlignmentTextLabel();
            customDataContainer.Add(textFoldout);
            extensionContainer.Add(customDataContainer);
            base.Draw_ExtensionContainer();
        }
    }
}

