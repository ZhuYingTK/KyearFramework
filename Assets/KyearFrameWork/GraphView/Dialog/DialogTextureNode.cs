using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Kyear.Graph
{
    [Serializable]
    public class DialogTextureNodeData : BaseGraphNodeData
    {
        [SerializeField] public string path;

        public override Type GetTargetType()
        {
            return typeof(DialogTextureNode);
        }
    }
    
    public class DialogTextureNode : BaseGraphNode<DialogTextureNodeData>
    {
        private ObjectField objectField;
        private Image texturePreview;
        public override void Save()
        {
            //data.content = objectField.value;
            base.Save();
            if (objectField != null)
            {
                texturePreview.image = (Texture)objectField.value;
                string guid = KyearGraphUtility.GetGuidFromObject(objectField.value);
                if (guid != null)
                {
                    data.path = guid;
                }
            }
        }

        public override void Init(BaseGraphNodeData data, AbstractGraph parent)
        {
            base.Init(data, parent);
            title = "图片节点";

            var texture = KyearGraphUtility.LoadObjectFromGuid<Texture>(base.data.path);
            if (texture != null)
            {
                objectField.value = texture;
                texturePreview.image = texture;
            }
        }


        public override void CreateData(Vector2 position,AbstractGraph parent)
        {
            DialogTextureNodeData data = new DialogTextureNodeData()
            {
                id = Guid.NewGuid().ToString(),
                position = position,
                inputPorts = new List<BasePortData>()
                {
                    new (){name = "输入",ID = GeneratePortID(PortType.Input)}
                },
                outputPorts = new List<BasePortData>()
                {
                    new (){name = "输出",ID = GeneratePortID(PortType.Output)},
                },
            };
            Debug.Log($"[KyearGraphError]  创建节点:{data.id}");
            Init(data,parent);
        }

        public override void Draw_ExtensionContainer()
        {
            VisualElement customDataContainer = new VisualElement();
            customDataContainer.AddToClassList("kyear-node__custom-data-container");
            Foldout textFoldout = new Foldout()
            {
                text = "输入",
                value = true
            };

            objectField = CreateTextureField("图片");
            textFoldout.Add(objectField);

            Foldout imagePreviewFoldout = new Foldout()
            {
                text = "预览",
            };
            texturePreview = new Image
            {
                image = (Texture)objectField.value
            };
            imagePreviewFoldout.Add(texturePreview);
            textFoldout.Add(imagePreviewFoldout);
            
            textFoldout.AlignmentTextLabel();
            customDataContainer.Add(textFoldout);
            extensionContainer.Add(customDataContainer);
            base.Draw_ExtensionContainer();
        }
    }
}
