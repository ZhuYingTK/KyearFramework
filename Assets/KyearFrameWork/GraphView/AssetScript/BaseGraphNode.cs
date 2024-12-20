using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Kyear.Graph
{
    public abstract class BaseGraphNode : Node
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
            Draw_InputContainer();
            Draw_OutputContainer();
            
            //刷新状态，保证UI刷新
            RefreshExpandedState();
            RefreshPorts();
        }
        
        public virtual void Draw_InputContainer()
        {
        }

        public virtual void Draw_OutputContainer()
        {
            
        }

        public virtual void Draw_ExtensionContainer()
        {
            extensionContainer.AddToClassList("kyear-node__extension-container");
        }
    }
}
