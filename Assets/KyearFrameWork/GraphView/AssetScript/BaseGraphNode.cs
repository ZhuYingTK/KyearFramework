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
        }
    }
}
