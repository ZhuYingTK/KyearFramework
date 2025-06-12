using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Kyear.Graph
{
    public abstract class AbstractGraphNode : Node
    {
        public Dictionary<uint, Port> inputPortDic = new Dictionary<uint, Port>();
        public Dictionary<uint, Port> outputPortDic = new Dictionary<uint, Port>();
        
        public abstract void Save();
        public abstract uint GetPortDataID(Port port);
        public abstract string GetID();
        public abstract void Init(BaseGraphNodeData data, AbstractGraph parent);
        public abstract BaseGraphNodeData GetData();
        public abstract IEnumerable<Edge> GetAllInPutEdges();
        public abstract IEnumerable<Edge> GetAllOutPutEdges();
        public abstract void CreateData(Vector2 position,AbstractGraph parent);
        public abstract void AddEdge(Edge edge);
        public abstract IEnumerable<AbstractGraphNode> GetBeforeNodes();
        public abstract IEnumerable<AbstractGraphNode> GetAfterNodes();
    }
    public abstract class BaseGraphNode<TNodeData> : AbstractGraphNode
    where TNodeData : BaseGraphNodeData
    {
        public string ID => data?.id;
        public TNodeData data;
        public AbstractGraph parent;
        public BaseGraphNode()
        {
            title = "Sample";
        }

        public override void Save()
        {
            data.position = GetPosition().position;
        }
        
        public override void Init(BaseGraphNodeData data,AbstractGraph parent)
        {
            this.data = (TNodeData)data;
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
        

        public override string GetID()
        {
            return ID;
        }

        public override BaseGraphNodeData GetData()
        {
            return data;
        }
        
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
                AddPort(data.outputPorts[i],PortType.Output);
            }
        }

        public virtual void Draw_ExtensionContainer()
        {
            extensionContainer.AddToClassList("kyear-node__extension-container");
            extensionContainer.style.flexDirection = FlexDirection.Column;
            extensionContainer.style.flexShrink = 0;
        }

        #region 端口

        ///生成PortID
        public uint GeneratePortID(PortType type)
        {
            //0记为错误标记
            uint id = 1;
            var Dic = type == PortType.Input ? inputPortDic : outputPortDic;
            while (true)
            {
                if (!Dic.ContainsKey(id))
                {
                    //占用
                    Dic[id] = null;
                    return id;
                }
                id++;
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

        public override uint GetPortDataID(Port port)
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

        public override void AddEdge(Edge edge)
        {
            var sourcePort = edge.output;
            var destPort = edge.input;
            AbstractGraphNode destNode = destPort.node as AbstractGraphNode;
            if (destNode == null)
            {
                Debug.LogError("[KyearGraphError]  目标节点不是BaseGraphNode");
                return;
            }
            var edgeData = new BaseEdgeData()
            {
                startPortID = GetPortDataID(sourcePort),
                endPortID = destNode.GetPortDataID(destPort),
                target = destNode.GetID()
            };
            edge.userData = edgeData;
            data.edges.Add(edgeData);
        }
        /// <summary>
        /// 输入该节点的边
        /// </summary>
        /// <returns></returns>
        public override IEnumerable<Edge> GetAllInPutEdges()
        {
            return inputPortDic.Values.SelectMany(e => e.connections);
        }
        /// <summary>
        /// 该节点输出的边
        /// </summary>
        /// <returns></returns>
        public override IEnumerable<Edge> GetAllOutPutEdges()
        {
            return outputPortDic.Values.SelectMany(e => e.connections);
        }
        


        #endregion

        #region 创建元素

        public TextField CreateTextField(string label, string value = "", bool multiline = true, int labelWidth = -1)
        {
            TextField textTextField = new TextField()
            {
                label = label,
                value = value
            };
            textTextField.RegisterCallback<FocusOutEvent>(e => Save());
            textTextField.multiline = multiline;
            textTextField.AddToClassList("kyear-node__text-field");
            textTextField.AddToClassList("kyear-node__quote-text-field");
            textTextField.labelElement.SetToDefaultStyle();
            textTextField.MarkDirtyRepaint();
            return textTextField;
        }

        public ObjectField CreateTextureField(string label)
        {
            ObjectField textureField = new ObjectField()
            {
                label = label,
                objectType = typeof(Texture)
            };
            textureField.RegisterCallback<FocusOutEvent>(e => Save());
            textureField.RegisterValueChangedCallback(e =>Save());
            textureField.labelElement.SetToDefaultStyle();
            textureField.MarkDirtyRepaint();
            return textureField;
        }

        #endregion
        
        #region 访问工具

        /// <summary>
        /// 获得所有前序节点
        /// </summary>
        /// <returns></returns>
        public override IEnumerable<AbstractGraphNode> GetBeforeNodes()
        {
            foreach (Edge edge in GetAllInPutEdges())
            {
                if (edge.TryGetStartNode(out var target))
                {
                    yield return target;
                }
            }
        }

        /// <summary>
        /// 获得所有后序节点
        /// </summary>
        /// <returns></returns>
        public override IEnumerable<AbstractGraphNode> GetAfterNodes()
        {
            foreach (Edge edge in GetAllOutPutEdges())
            {
                if (edge.TryGetEndNode(out var target))
                {
                    yield return target;
                }
            }
        }
        
        #endregion

    }
}
