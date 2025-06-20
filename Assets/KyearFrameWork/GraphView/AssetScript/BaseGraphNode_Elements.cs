using System;
using System.Collections.Generic;
using System.Linq;
using Kyear.Graph;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Kyear.Graph
{
    public abstract partial class BaseGraphNode<TNodeData>
        where TNodeData : BaseGraphNodeData
    {
        #region 创建元素
        /// <summary>
        /// 创建文字输入框
        /// </summary>
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
        /// <summary>
        /// 创建图片输入框
        /// </summary>
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
    }
    
    /// <summary>
    /// 绘制列表元素
    /// </summary>
    public abstract class AbstractListViewElement<TData> : VisualElement
    {
        protected TData _data;
        protected bool canMove = true;
        protected Color borderColor = Color.black;
        
        public Action<AbstractListViewElement<TData>, int> OnMoveRequested;
        public Action<AbstractListViewElement<TData>> OnDeleteRequested;

        public virtual TData GetData()
        {
            return _data;
        }

        public virtual void Init(
            TData data,
            Action<AbstractListViewElement<TData>, int> OnMoveRequested,
            Action<AbstractListViewElement<TData>> OnDeleteRequested)
        {
            _data = data;
            this.OnMoveRequested = OnMoveRequested;
            this.OnDeleteRequested = OnDeleteRequested;
            DrawView();
        }

        public virtual void Init(
            Action<AbstractListViewElement<TData>, int> OnMoveRequested,
            Action<AbstractListViewElement<TData>> OnDeleteRequested)
        {
            _data = default;
            focusable = true;
            this.OnMoveRequested = OnMoveRequested;
            this.OnDeleteRequested = OnDeleteRequested;
            DrawView();
        }

        /// <summary>
        /// 绘制基础布局
        /// </summary>
        public virtual void DrawView()
        {
            if (ColorUtility.TryParseHtmlString("#101010", out var borderColor))
            {
                this.borderColor = borderColor;
            }
            Clear();
            style.flexDirection = FlexDirection.Row;
            style.borderTopWidth = 0.5f;
            style.borderBottomWidth = 0.5f;
            style.borderTopColor = borderColor;
            style.borderBottomColor = borderColor;
            DrawLeftElements();
            
            // 中间自适应元素（填充剩余空间）
            VisualElement elementContainer = DrawMainElement();
            contentContainer.Add(elementContainer);

            DrawRightElements();
        }

        protected virtual VisualElement DrawMainElement()
        {
            VisualElement elementContainer = new VisualElement();
            TextField textField = new TextField();
            elementContainer.Add(textField);
            return elementContainer;
        }

        private void DrawLeftElements()
        {
            VisualElement leftPort = new VisualElement();
            Button deleteBtn = new Button(() => OnDeleteRequested(this)) { text = "-" };
            SetVisualElementFixed(leftPort, 20);
            SetVisualElementFixed(deleteBtn, 20);
            contentContainer.Add(leftPort);
            contentContainer.Add(deleteBtn);
        }

        private void DrawRightElements()
        {
            Button upBtn  = new Button(() => { OnMoveRequested(this, -1); }){text = "\u2191"};
            Button downBtn  = new Button(() => { OnMoveRequested(this, 1); }){text = "\u2193"};
            VisualElement rightProt = new VisualElement();
            SetVisualElementFixed(rightProt, 20);
            SetVisualElementFixed(upBtn,20);
            SetVisualElementFixed(downBtn,20);
            contentContainer.Add(upBtn);
            contentContainer.Add(downBtn);
            contentContainer.Add(rightProt);
        }

        private void SetVisualElementFixed(VisualElement element,int width)
        {
            element.style.width = width;
            element.style.height = 30;
            element.style.backgroundColor = new Color(0.5f, 1, 0.5f, 1);
            element.style.marginLeft = 0;
            element.style.marginRight = 0;
        }
    }
    
    
    public class StringListViewElement : AbstractListViewElement<string>
    {
        private TextField textField = new TextField();
        public override string GetData()
        {
            return textField.value;
        }
        protected override VisualElement DrawMainElement()
        {
            VisualElement elementContainer = new VisualElement();
            textField.value = _data;
            elementContainer.Add(textField);
            return elementContainer;
        }
    }

    public class CustomListView<TViewElement,TData> : VisualElement
        where TViewElement : AbstractListViewElement<TData>,new()
    {
        private List<TViewElement> _list = new();
        private VisualElement listPanel;
        private Button addButton;
        private VisualElement ButtonParent;

        private Color bgColor = Color.black;
        float borderWidth = 0.01f;
        Color borderColor = Color.black;
        
        public CustomListView(List<TData> dataList)
        {
            if (ColorUtility.TryParseHtmlString("#303030", out var bgColor))
            {
                this.bgColor = bgColor;
            }
            if (ColorUtility.TryParseHtmlString("#101010", out var borderColor))
            {
                this.borderColor = borderColor;
            }
            for (int i = 0; i < dataList.Count; i++)
            {
                var element = new TViewElement();
                element.Init(dataList[i],MoveElement,DeleteElement);
                _list.Add(element);
            }

            listPanel = new VisualElement();
            
            ButtonParent = new VisualElement();
            addButton = new Button(OnClickAdd)
            {
                text = "+"
            };
            ButtonParent.Add(addButton);

            SetStyle();
            DrawView();
            
            contentContainer.Add(listPanel);
            contentContainer.Add(ButtonParent);
        }

        private void OnClickAdd()
        {
            var element = new TViewElement();
            element.Init(MoveElement,DeleteElement);
            _list.Add(element);
            Refresh();
            Focus();
        }

        public void MoveElement(AbstractListViewElement<TData> target, int offset)
        {
            if (_list == null || !_list.Contains(target))
            {
                Debug.LogError("[KyearGraphError]  移动元素时列表为空或没有该元素");
                return;
            }

            if (target is TViewElement targetElement)
            {
                int index = _list.IndexOf(targetElement);
                int targetIndex = index + offset;
                if (targetIndex < 0 || targetIndex >= _list.Count)
                    return;
                (_list[index], _list[targetIndex]) = (_list[targetIndex], _list[index]);
            }
            Refresh();
        }

        public void DeleteElement(AbstractListViewElement<TData> target)
        {
            if (_list == null || !_list.Contains(target))
            {
                Debug.LogError("[KyearGraphError]  删除元素时列表为空或没有该元素");
                return;
            }

            if (target is TViewElement targetElement)
            {
                _list.Remove(targetElement);
            }
            Refresh();
        }

        private void SetStyle()
        {
            int radius = 4;

            listPanel.style.backgroundColor = bgColor;
            listPanel.style.borderBottomLeftRadius = radius;
            listPanel.style.borderTopRightRadius = radius;
            listPanel.style.borderTopLeftRadius = radius;

            listPanel.style.paddingTop = 2f;
            listPanel.style.paddingBottom = 2f;
            
            listPanel.style.borderBottomWidth = borderWidth;
            listPanel.style.borderTopWidth = borderWidth;
            listPanel.style.borderLeftWidth = borderWidth;
            listPanel.style.borderRightWidth = borderWidth;
            listPanel.style.borderBottomColor = borderColor;
            listPanel.style.borderTopColor = borderColor;
            listPanel.style.borderLeftColor = borderColor;
            listPanel.style.borderRightColor = borderColor;
            
            ButtonParent.style.paddingLeft = 0;
            ButtonParent.style.paddingTop = 0;
            ButtonParent.style.height = 20;
            
            addButton.style.width = 25;
            addButton.style.position = Position.Absolute;
            addButton.style.right = 0;
            addButton.style.bottom = 0;
            addButton.style.marginRight = 0;
            addButton.style.backgroundColor = bgColor;
            addButton.style.borderTopLeftRadius = 0;
            addButton.style.borderTopRightRadius = 0;
            addButton.style.borderBottomWidth = borderWidth;
            addButton.style.borderTopWidth = borderWidth;
            addButton.style.borderLeftWidth = borderWidth;
            addButton.style.borderRightWidth = borderWidth;
            addButton.style.borderBottomColor = borderColor;
            addButton.style.borderTopColor = borderColor;
            addButton.style.borderLeftColor = borderColor;
            addButton.style.borderRightColor = borderColor;
        }

        /// <summary>
        /// 清空List并重新绘制
        /// </summary>
        private void Refresh()
        {
            DrawView();
        }

        private void DrawView()
        {
            listPanel.Clear();
            for(int i = 0; i < _list.Count; i++)
            {
                listPanel.contentContainer.Add(_list[i]);
            }
        }
        
        /// <summary>
        /// 获取全部数据
        /// </summary>
        public List<TData> GetData()
        {
            return _list.Select(e => e.GetData()).ToList();
        }
    }
}