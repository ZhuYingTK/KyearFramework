using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Kyear.Graph
{
    public abstract class BaseGraphNode : Node
    {
        public string ID => data?.id;
        public BaseGraphNodeData data;
        public AbstractGraph parent;
        public BaseGraphNode()
        {
            title = "Sample";
        }

        public virtual void Save()
        {
            data.position = GetPosition().position;
        }
        
        public virtual void Init(BaseGraphNodeData data,AbstractGraph parent)
        {
            this.data = data;
            this.parent = parent;
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
            RegisterCallback<BlurEvent>(e => Save());
        }

        /// <summary>
        /// 生成新节点
        /// </summary>
        /// <param name="position"></param>
        public abstract void CreateData(Vector2 position,AbstractGraph parent);
        
        public virtual void Draw_InputContainer()
        {
            for (int i = 0; i < data.inputPorts.Count; i++)
            {
                AddPort(data.inputPorts[i],PortType.Input);
            }
        }

        public virtual void Draw_OutputContainer()
        {
            for (int i = 0; i < data.outputPorts.Count; i++)
            {
                AddPort(data.inputPorts[i],PortType.Output);
            }
        }

        public virtual void Draw_ExtensionContainer()
        {
            extensionContainer.AddToClassList("kyear-node__extension-container");
        }

        #region 端口

        public Dictionary<uint, Port> inputPortDic = new Dictionary<uint, Port>();
        public Dictionary<uint, Port> outputPortDic = new Dictionary<uint, Port>();

        ///生成PortID
        public uint GeneratePortID(PortType type)
        {
            //0记为错误标记
            uint id = 1;
            var Dic = type == PortType.Input ? inputPortDic : outputPortDic;
            while (true)
            {
                if (!Dic.ContainsKey(id))
                    return id;
            }
        }
        
        public enum PortType
        {
            Output,
            Input
        }

        protected virtual void AddPort(BasePortData data,PortType type)
        {
            var direction = type == PortType.Input ? Direction.Input : Direction.Output;
            var container = type == PortType.Input ? inputContainer : outputContainer;
            var dic = type == PortType.Input ? inputPortDic : outputPortDic;
            var port = Port.Create<Edge>(Orientation.Horizontal, direction, Port.Capacity.Single, typeof(Port));
            port.name = data.name;
            port.userData = data;
            container.Add(port);
            dic[data.ID] = port;
        }

        public uint GetPortDataID(Port port)
        {
            var protData = port.userData as BasePortData;
            if (protData == null)
            {
                Debug.LogError("[KyearGraphError]  port没有数据");
                return 0;
            }

            return protData.ID;
        }

        #endregion

        #region 边

        public void AddEdge(Edge edge)
        {
            var sourcePort = edge.output;
            var destPort = edge.input;
            BaseGraphNode destNode = destPort.node as BaseGraphNode;
            if (destNode == null)
            {
                Debug.LogError("[KyearGraphError]  目标节点不是BaseGraphNode");
                return;
            }
            var edgeData = new BaseEdgeData()
            {
                startPortID = GetPortDataID(sourcePort),
                endPortID = destNode.GetPortDataID(destPort),
                target = destNode.ID
            };
            edge.userData = edgeData;
            data.edges.Add(edgeData);
        }
        
        public List<Edge> GetAllInPutEdges()
        {
            return inputPortDic.Values.SelectMany(e => e.connections.ToList()).ToList();
        }

        public List<Edge> GetAllOutPutEdges()
        {
            return outputPortDic.Values.SelectMany(e => e.connections.ToList()).ToList();
        }
        


        #endregion
    }
}
