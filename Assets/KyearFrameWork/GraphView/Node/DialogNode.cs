using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Kyear.Graph
{
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

        public override void CreateData(Vector2 position)
        {
            DialogNodeData data = new DialogNodeData()
            {
                id = Guid.NewGuid().ToString(),
                position = position,
                inputPorts = new List<BasePortData>(){new BasePortData(){name = "输入"}},
                outputPorts = new List<BasePortData>(){new BasePortData(){name = "输出"}},
            };
            Debug.Log($"[KyearGraphError]  创建节点:{data.id}");
            Init(data);
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

            TextField textTextField = new TextField(){
                value = "value",
            };
            textTextField.multiline = true;
            textTextField.AddToClassList("kyear-node__text-field");
            textTextField.AddToClassList("kyear-node__quote-text-field");
            textFoldout.Add(textTextField);
            
            
            customDataContainer.Add(textFoldout);
            extensionContainer.Add(customDataContainer);
            base.Draw_ExtensionContainer();
        }
    }
}

