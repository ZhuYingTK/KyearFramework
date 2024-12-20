using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Kyear.Graph
{
    public class DialogNode : BaseGraphNode
    {
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

