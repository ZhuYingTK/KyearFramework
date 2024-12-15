using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kyear.Graph
{
    public abstract class BaseGraphNodeAsset : ScriptableObject
    {
        [HideInInspector] public Vector2 position = Vector2.zero;
        [HideInInspector] public string id = null;
        public abstract string nodeTitle { get; }
        public abstract string[] outputItems { get; set; }
        
        [HideInInspector, SerializeField] 
        private List<LinkChild> m_children = new List<LinkChild>();
    }
    
    [Serializable]
    public class LinkChild
    {
        public string key;
        public BaseGraphNodeAsset child;
    };
}