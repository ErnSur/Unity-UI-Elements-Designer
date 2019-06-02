using System;
using System.Collections;
using System.Collections.Generic;

namespace UnityEngine.UIElements.New
{
    public class Row : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<Row,UxmlTraits> { }
        public new class UxmlTraits : VisualElement.UxmlTraits { }

        public Row()
        {
            style.flexDirection = FlexDirection.Row;
        }
    }

    public class Column : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<Column, UxmlTraits> { }
        public new class UxmlTraits : TextElement.UxmlTraits { }

        public Column()
        {
            style.flexDirection = FlexDirection.Column;
            style.flexGrow = 1;
            style.flexShrink = 0;
        }
    }

    public class ListView<TItem, TData> : ListView where TItem : ListViewItem<TData>, new()
    {
        public ListView(IList<TData> dataSource)
        {
            itemsSource = (IList) dataSource;
            bindItem = (ve, i) => (ve as TItem).BindData(dataSource[i]);
            makeItem = () => new TItem();

            style.flexGrow = 1;
            style.flexShrink = 0;
            style.flexBasis = 1;
        }
    }

    public abstract class ListViewItem<TData> : VisualElement
    {
        //public Action<TData> BindData;
        public abstract void BindData(TData data);
    }

    public class ExpandingFoldout : Foldout
    {
        public ExpandingFoldout()
        {
            this.RegisterValueChangedCallback(OnValueChange);

            contentContainer.style.marginLeft = 0;
            
            style.flexGrow = value ? 1 : 0;

            void OnValueChange(ChangeEvent<bool> e)
            {
                style.flexGrow = e.newValue ? 1 : 0;
            }
        }
    }
    
    public class LabelWithRename : Label
    {
        private readonly RenameField _renameField;

        public LabelWithRename(string text, Action<string> onRename) : base(text)
        {
            style.backgroundColor = Color.green;
            style.flexGrow = 1;
            style.flexShrink = 0;
            style.flexBasis = 1;

            _renameField = new RenameField(this, onRename);
            Add(_renameField);
            RegisterCallback<MouseDownEvent>(OnLabelMouseClickEvent);
        }

        public LabelWithRename()
        {
            style.backgroundColor = Color.green;
            style.flexGrow = 1;
            style.flexShrink = 0;
            style.flexBasis = 1;

            _renameField = new RenameField(this, null);
            Add(_renameField);
            RegisterCallback<MouseDownEvent>(OnLabelMouseClickEvent);
        }

        public LabelWithRename WithOnRename(Action<string> onRename)
        {
            _renameField.OnRename = onRename;
            return this;
        }

        private void OnLabelMouseClickEvent(MouseDownEvent evt)
        {
            if (evt.button == 0 && evt.clickCount == 2)
                _renameField.Show();
        }

        private class RenameField : TextField
        {
            public Action<string> OnRename = (s) => { };

            private readonly LabelWithRename _label;

            private readonly Color _defaultColor = Color.black;
            private readonly Color _renameColor = new Color(0, 0, 0, 0.2f);

            public RenameField(LabelWithRename label, Action<string> onRename)
            {
                focusable = true;
                _label = label;
                OnRename = onRename;
                value = label.text;
                visible = false;
                isDelayed = true;

                this.WithSizeOf(label);
                RegisterCallback<KeyUpEvent>(OnRenameKeyUpEvent);
            }

            void OnRenameKeyUpEvent(KeyUpEvent evt)
            {
                switch (evt.keyCode)
                {
                    case KeyCode.Escape:
                        Hide();
                        break;
                    case KeyCode.Return:
                        DoRename();
                        Hide();
                        break;
                }
            }

            private void DoRename()
            {
                OnRename(value);
                _label.text = value;
            }

            private void Hide()
            {
                visible = false;
                _label.style.color = _defaultColor;
            }

            public void Show()
            {
                Focus();
                value = _label.text;
                visible = true;
                _label.style.color = _renameColor;
            }
        }
    }
}