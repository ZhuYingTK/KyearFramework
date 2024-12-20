using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Kyear.Graph
{
    public class BaseGraphNode : Node
    {
        public string ID => data?.id;
        public BaseGraphNodeData data;
        public BaseGraphNode()
        {
            title = "Sample";
        }
        public virtual void Init(Vector2 posotion)
        {
            SetPosition(new Rect(posotion,Vector2.zero));
            string ID = Guid.NewGuid().ToString();
            data = new BaseGraphNodeData()
            {
                id = ID,
                position = posotion,
            };
            mainContainer.style.backgroundColor = new Color(29f / 255f, 29f / 255f, 30f / 255f);
            mainContainer.AddToClassList("kyear-node__main-container");
            Draw_ExtensionContainer();
        }

        public virtual void Draw_ExtensionContainer()
        {
            extensionContainer.AddToClassList("kyear-node__extension-container");
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
        }
    }
}
