using System;
using UIDesigner;
using UIDesigner.Toolbox.TreeView;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UIElements.New;

public class ToolboxWindow : EditorWindow
{
    [MenuItem("Window/Designer/Toolbox")]
    public static void CreateWindow()
    {
        UIDesignerScene.Toolbox = GetWindow<ToolboxWindow>(nameof(ToolboxWindow), typeof(SceneView_UI));
    }

    private ListOfDerivedTypesTreeView _treeView;
    private Vector2 _scrollViewPos;

    private ToolbarSearchField _searchField;

    private void OnEnable()
    {
        _searchField = new ToolbarSearchField();
        rootVisualElement.Add(new Toolbar().WithChild(_searchField));
        rootVisualElement.Add(CreateTreeView());
    }

    private VisualElement CreateTreeView()
    {
        _treeView = new ListOfDerivedTypesTreeView(new TreeViewState(), typeof(VisualElement), true);

        return new IMGUIContainer(TreeViewOnGui).WithStyleOf(new Column()).WithName("tree-view");

        void TreeViewOnGui()
        {
            _scrollViewPos = GUILayout.BeginScrollView(_scrollViewPos);
            GUILayout.EndScrollView();

            var rect = Rect.zero;

            if (Event.current.type == EventType.Repaint)
                rect = GUILayoutUtility.GetLastRect();

            _treeView.searchString = _searchField.value;
            _treeView.OnGUI(rect);
        }
    }

    public static VisualElement CreateVisualElement(Type type)
    {
        if (!(Activator.CreateInstance(type) is VisualElement ve))
            return null;
        
        ve.name = type.Name;

        return ve;
    }
}