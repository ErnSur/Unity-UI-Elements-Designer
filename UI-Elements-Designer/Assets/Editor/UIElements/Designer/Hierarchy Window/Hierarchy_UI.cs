using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;
using UnityEngine.UIElements.New;

namespace UIDesigner
{
    public class Hierarchy_UI : EditorWindow
    {
        private VeTreeView _treeView;

        [SerializeField]
        private Vector2 _scrollViewPos;

        private StyleSheet PreviewUss => ussField.value as StyleSheet;
        private VisualTreeAsset PreviewUxml => uxmlField.value as VisualTreeAsset;

        private ObjectField ussField, uxmlField;

        [MenuItem("Window/Designer/Hierarchy")]
        public static void CreateWindow()
        {
            UIDesignerScene.Hierarchy = GetWindow<Hierarchy_UI>(nameof(Hierarchy_UI), typeof(SceneView_UI));
        }

        private void OnEnable()
        {
            UIDesignerScene.Hierarchy = this;
            rootVisualElement.styleSheets.Add(Resources.Load<StyleSheet>("Hierarchy_UI"));

            rootVisualElement.Add(CreateTopBar());
            rootVisualElement.Add(CreateTreeView());
        }

        private VisualElement CreateTopBar()
        {
            var bar = new Row().WithName("top-bar");
            
            var buttonsColumn = new VisualElement().WithName("top-bar-button-column");
            {
                var loadButton = Create_Top_Bar_Button(LoadVisualTree).WithText("Load");
                var saveButton = Create_Top_Bar_Button(SaveVisualTreeAsset).WithText("Save");
                
                buttonsColumn.ADD(saveButton, loadButton);
            }
            
            var spacer = new VisualElement {name = "top-bar-spacer"};

            var fieldsColumn = new Column();
            {
                uxmlField = Create_Top_Bar_Field<VisualTreeAsset>();
                
                ussField = Create_Top_Bar_Field<StyleSheet>();
    
                fieldsColumn.ADD(uxmlField, ussField);
            }

            return bar.WithChildren(buttonsColumn,fieldsColumn);
        }
        
        private ObjectField Create_Top_Bar_Field<T>() => new ObjectField {allowSceneObjects = false, objectType = typeof(T)}.WithClasses("top-bar-field");
        private TextElement Create_Top_Bar_Button(Action handler) => new TextElement().WithOnClick(handler).WithClasses("top-bar-button");

        private VisualElement CreateTreeView()
        {
            _treeView = new VeTreeView(new TreeViewState(), UIDesignerScene.UIRoot);

            return new IMGUIContainer(TreeViewOnGui).WithStyleOf(new Column()).WithName("tree-view");

            void TreeViewOnGui()
            {
                _scrollViewPos = GUILayout.BeginScrollView(_scrollViewPos);
                GUILayout.EndScrollView();

                var rect = Rect.zero;

                if (Event.current.type == EventType.Repaint)
                    rect = GUILayoutUtility.GetLastRect();

                _treeView.OnGUI(rect);
            }
        }

        private void LoadVisualTree()
        {
            if (PreviewUxml)
            {
                UIDesignerScene.SceneView.CreateNewRoot();
                PreviewUxml.CloneTree(UIDesignerScene.UIRoot);
                Refresh();
            }

            if (PreviewUss)
            {
                UIDesignerScene.UIRoot.styleSheets.Clear();
                UIDesignerScene.UIRoot.styleSheets.Add(PreviewUss);
                Refresh();
            }
        }
        
        private void SaveVisualTreeAsset()
        {
            throw new NotImplementedException();
        }

        public void Refresh()
        {
            _treeView.Refresh(UIDesignerScene.UIRoot);
        }
    }
}