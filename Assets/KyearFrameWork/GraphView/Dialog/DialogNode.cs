using System;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

namespace Kyear.Graph
{
    [Serializable]
    public class DialogNodeData : BaseGraphNodeData
    {
        [SerializeField] public string content;
        [SerializeField] public Texture texture;

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

            TextField testTextField = CreateTextField("对话人");
            TextField textTextField = CreateTextField("内容");
            var textureField = CreateTextureField("图片");
            
            textFoldout.Add(textTextField);
            textFoldout.Add(testTextField);
            textFoldout.Add(textureField);
            textFoldout.AlignmentTextLabel();
            customDataContainer.Add(textFoldout);
            extensionContainer.Add(customDataContainer);
            base.Draw_ExtensionContainer();
        }
    }
}

