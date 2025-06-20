using System;
using System.Collections;
using System.Collections.Generic;
using Kyear.Graph;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Kyear.Graph
{
    [Serializable]
    public class DialogChoiceNodeData : BaseGraphNodeData
    {
        [SerializeField] public List<string> content = new List<string>();

        public override Type GetTargetType()
        {
            return typeof(DialogChoiceNode);
        }
    }

    public class DialogChoiceNode : BaseGraphNode<DialogChoiceNodeData>
    {
        private CustomListView<StringListViewElement, string> _customListView;
        public override void Save()
        {
            base.Save();
            if (_customListView != null)
            {
                var stringList = _customListView.GetData();
                data.content = stringList;
            }
        }

        public override void Init(BaseGraphNodeData data, AbstractGraph parent)
        {
            base.Init(data, parent);
            title = "对话节点";
        }
        
        public override void CreateData(Vector2 position, AbstractGraph parent)
        {
            DialogChoiceNodeData data = new DialogChoiceNodeData()
            {
                id = Guid.NewGuid().ToString(),
                position = position,
                inputPorts = new List<BasePortData>()
                {
                    new("输入", GeneratePortID(PortType.Input), Port.Capacity.Multi)
                },
                outputPorts = new List<BasePortData>()
                {
                    new("输出", GeneratePortID(PortType.Output), Port.Capacity.Multi)
                },
            };
            Debug.Log($"[KyearGraphError]  创建节点:{data.id}");
            Init(data, parent);
        }

        public override void Draw_ExtensionContainer()
        {
            VisualElement customDataContainer = new VisualElement();
            customDataContainer.AddToClassList("kyear-node__custom-data-container");
            Foldout textFoldout = new Foldout()
            {
                text = "展开",
                value = true
            };
            textFoldout.AlignmentTextLabel();


            _customListView = new CustomListView<StringListViewElement, string>(data.content);
            textFoldout.Add(_customListView);
            customDataContainer.Add(textFoldout);
            extensionContainer.Add(customDataContainer);
            base.Draw_ExtensionContainer();
        }

    }
}

