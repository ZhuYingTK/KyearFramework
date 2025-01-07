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
    
    public class DialogNode : BaseGraphNode
    {
        public override void Draw_InputContainer()
        {
            base.Draw_InputContainer();
        }
        
        public override void Draw_OutputContainer()
        {
            base.Draw_OutputContainer();
        }

        public override void CreateData(Vector2 position,AbstractGraph parent)
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

        public override void Draw_ExtensionContainer()
        {
            VisualElement customDataContainer = new VisualElement();
            customDataContainer.AddToClassList("kyear-node__custom-data-container");
            Foldout textFoldout = new Foldout()
            {
                text = "输入",
                value = true
            };

            TextField textTextField = CreateTextField("对话");
            TextField testTextField = CreateTextField("测试111");
            textFoldout.Add(textTextField);
            textFoldout.Add(testTextField);
            textFoldout.AlignmentTextLabel();
            customDataContainer.Add(textFoldout);
            extensionContainer.Add(customDataContainer);
            base.Draw_ExtensionContainer();
        }
    }
}

