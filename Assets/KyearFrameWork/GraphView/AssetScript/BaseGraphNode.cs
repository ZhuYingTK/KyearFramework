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

        public virtual void Save()
        {
            data.position = GetPosition().position;
        }
        
        public virtual void Init(BaseGraphNodeData data)
        {
            this.data = data;
            SetPosition(new Rect(this.data.position,Vector2.zero));
            //给每个Node都赋予唯一ID
            mainContainer.style.backgroundColor = new Color(29f / 255f, 29f / 255f, 30f / 255f);
            mainContainer.AddToClassList("kyear-node__main-container");
            Draw_ExtensionContainer();
            Draw_InputContainer();
            Draw_OutputContainer();
            
            //刷新状态，保证UI刷新
            RefreshExpandedState();
            RefreshPorts();
        }

        /// <summary>
        /// 生成新节点
        /// </summary>
        /// <param name="position"></param>
        public virtual void CreateData(Vector2 position)
        {
            BaseGraphNodeData data = new BaseGraphNodeData()
            {
                id = Guid.NewGuid().ToString(),
                position = position,
            };
            Debug.Log($"创建节点:{data.id}");
            Init(data);
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
