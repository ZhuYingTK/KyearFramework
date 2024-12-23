using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kyear.Graph
{
    [Serializable]
    public class BaseGraphNodeData
    {
        [SerializeField] public Vector2 position = Vector2.zero;
        [SerializeField] public string id = null;
    }

    [Serializable]
    public class DialogNodeData
    {
        [SerializeField] public string content;
    }
}