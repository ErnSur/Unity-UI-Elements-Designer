//#define DEBUG_TV

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using UnityEngine.UIElements;
using UIDesigner.Toolbox.TreeView;

namespace UIDesigner
{
    public partial class VeTreeView : TreeView
    {
        private VisualElement _visualElementRoot;

        private readonly Dictionary<int, TreeViewItem> _treeViewItems = new Dictionary<int, TreeViewItem>();
        private readonly Dictionary<VisualElement, int> _visualElementIdLookup = new Dictionary<VisualElement, int>();
        private readonly Dictionary<int, VisualElement> _idVisualElementLookup = new Dictionary<int, VisualElement>();
        private int _lastItemIndex;

        private Rect _onGUIRect;

        private IList<VisualElement> _selected;

        public VeTreeView(TreeViewState state, VisualElement visualElementRoot) : base(state)
        {
            Refresh(visualElementRoot);
        }

        public override void OnGUI(Rect rect)
        {
            _onGUIRect = rect;
            base.OnGUI(rect);
        }

        public void Refresh(VisualElement root)
        {
            _visualElementRoot = root;
            Refresh();
        }

        public void Refresh()
        {
            Reload();
            UIDesignerScene.RefreshPreview();
        }

        protected override TreeViewItem BuildRoot()
        {
            var root = new TreeViewItem {id = -1, depth = -1};

            _lastItemIndex = -1;

            ClearTreeData();

            var visualRoot = CreateTreeViewItemHierarchy(_visualElementRoot);
            root.AddChild(visualRoot);

            SetupDepthsFromParentsAndChildren(root);

            return root;
        }

        private void ClearTreeData()
        {
            _treeViewItems.Clear();
            _idVisualElementLookup.Clear();
            _visualElementIdLookup.Clear();
        }

        private TreeViewItem CreateTreeViewItemHierarchy(VisualElement ve)
        {
            var id = ++_lastItemIndex;
            var itemName = string.IsNullOrEmpty(ve.name) ? "No Name" : ve.name;
#if DEBUG_TV
            itemName += $" (id: {id})";
#endif
            var item = new TreeViewItem(id) {displayName = itemName, icon = Styles.RegularItemIcon};

            _treeViewItems[id] = item;
            _idVisualElementLookup[id] = ve;
            _visualElementIdLookup[ve] = id;

            if (ve.childCount > 0)
            {
                foreach (var child in ve.Children())
                {
                    item.AddChild(CreateTreeViewItemHierarchy(child));
                }
            }

            return item;
        }
    }

    //VisualElement Manipulation
    public partial class VeTreeView
    {
        private void RemoveVeFormHierarchy(IEnumerable<VisualElement> elements)
        {
            foreach (var ve in elements)
            {
                if (ve != _visualElementRoot)
                    ve.RemoveFromHierarchy();
            }
        }

        private List<VisualElement> DuplicateVe(IEnumerable<VisualElement> elements)
        {
            var newElements = new List<VisualElement>();

            foreach (var ve in elements)
            {
                if (ve == _visualElementRoot)
                    continue;

                // Ideally we would create a DeepCopy of 've' here
                var newElement = ToolboxWindow.CreateVisualElement(ve.GetType());

                newElements.Add(newElement);
                ve.parent.Add(newElement);
            }

            return newElements;
        }
    }

    //OnGui
    public partial class VeTreeView
    {
        private static class Styles
        {
            public static readonly Texture2D RootItemIcon =
                EditorGUIUtility.IconContent("GameObject Icon").image as Texture2D;

            public static readonly Texture2D RegularItemIcon =
                EditorGUIUtility.IconContent("GameObject Icon").image as Texture2D;

            public static readonly Lazy<GUIStyle> RootRowStyle =
                new Lazy<GUIStyle>(() => new GUIStyle("TV LineBold") {alignment = TextAnchor.MiddleLeft});

            public static Color RootRowBackgroundColor => EditorGUIUtility.isProSkin
                ? new Color(0.3f, 0.3f, 0.3f)
                : new Color(.86f, .86f, .86f);
            
            public static Color RootRowShadowColor => EditorGUIUtility.isProSkin
                ? new Color(0.1f, 0.1f, 0.1f)
                : new Color(.56f, .56f, .56f);
        }

        protected override void RowGUI(RowGUIArgs args)
        {
            if (args.row == 0)
            {
                DrawRootRowItem(args);
            }
            else
            {
                base.RowGUI(args);
            }
        }

        protected override void BeforeRowsGUI()
        {
            var rootRowRect = new Rect(0, 0, _onGUIRect.width, 17);
            EditorGUI.DrawRect(rootRowRect, Styles.RootRowBackgroundColor);
            rootRowRect.yMin = rootRowRect.height - 1;
            EditorGUI.DrawRect(rootRowRect, Styles.RootRowShadowColor);
            base.BeforeRowsGUI();
        }

        private void DrawRootRowItem(RowGUIArgs args)
        {
            if (Event.current.rawType != EventType.Repaint)
                return;

            var rect = args.rowRect;
            rect.xMin += GetContentIndent(args.item);

            GUI.Label(rect, new GUIContent(args.label, Styles.RootItemIcon), Styles.RootRowStyle.Value);
        }
    }

    //Click Events
    public partial class VeTreeView
    {
        protected override void KeyEvent()
        {
            if (GotKeyEvent(KeyCode.Delete))
            {
                RemoveVeFormHierarchy(_selected);

                RefreshAndSelect(new List<int>());
            }

            if (GotKeyEvent(KeyCode.D, true))
            {
                RefreshAndSelect(DuplicateVe(_selected));
            }
        }

        private bool GotKeyEvent(KeyCode key, bool withModifier = false)
        {
            var evt = Event.current;

#if UNITY_EDITOR_WIN
            if (withModifier && !evt.control)
                return false;
#elif UNITY_EDITOR_OSX
            if (withModifier && !evt.command)
                return false;
#endif

            if (evt.isKey && evt.keyCode == key && evt.type == EventType.KeyDown)
            {
                evt.Use();
                return true;
            }

            return false;
        }
    }

    //Selection
    public partial class VeTreeView
    {
        protected override void SelectionChanged(IList<int> selectedIds)
        {
            _selected = selectedIds.Select(id => _idVisualElementLookup[id]).ToList();
        }

        private void RefreshAndSelect(IList<int> ids)
        {
            Refresh();
            SetSelection(ids, TreeViewSelectionOptions.RevealAndFrame);
            SelectionChanged(ids);
        }

        private void RefreshAndSelect(List<VisualElement> elements)
        {
            Refresh();
            var ids = elements.Select(ve => _visualElementIdLookup[ve]).ToList();
            SetSelection(ids, TreeViewSelectionOptions.RevealAndFrame);
            SelectionChanged(ids);
        }

        private void FrameElements(IEnumerable<VisualElement> addedElements)
        {
            var oldExpanded = GetExpanded();
            Refresh();
            var newSelection = addedElements.Select(ve => _visualElementIdLookup[ve]).ToList();
            var newExpanded = oldExpanded.Union(newSelection.Select(id => _treeViewItems[id].parent.id)).ToList();

            SetExpanded(newExpanded);
            SetSelection(newSelection, TreeViewSelectionOptions.FireSelectionChanged);
        }
    }

    //Drag
    public partial class VeTreeView
    {
        private const string _hierarchyDragData = "UIDesigner.Hierarchy Drag";


        protected override bool CanStartDrag(CanStartDragArgs args)
        {
            return !args.draggedItemIDs.Contains(0);
        }

        protected override void SetupDragAndDrop(SetupDragAndDropArgs args)
        {
            DragAndDrop.PrepareStartDrag();
            var draggedItems = args.draggedItemIDs.Select(id => _idVisualElementLookup[id]).ToList();
            DragAndDrop.SetGenericData(_hierarchyDragData, draggedItems);

            DragAndDrop.StartDrag("Dragging VisualElements");

            Event.current.Use();
        }

        protected override DragAndDropVisualMode HandleDragAndDrop(DragAndDropArgs args)
        {
            var dropIndex = args.parentItem == null || args.parentItem.id == -1 ? 0 : args.parentItem.id;

            var dragData = DragAndDrop.GetGenericData(ListOfDerivedTypesTreeView.DragGenericData);

            if (dragData is List<Type> toolboxData)
            {
                return HandleToolboxDrag(toolboxData, args.performDrop, dropIndex);
            }

            dragData = DragAndDrop.GetGenericData(_hierarchyDragData);

            if (dragData is List<VisualElement> hierarchyData)
            {
                return HandleHierarchyDrag(hierarchyData, args);
            }

            return DragAndDropVisualMode.None;
        }

        private DragAndDropVisualMode HandleToolboxDrag(List<Type> data, bool dropPreformed, int dropIndex)
        {
            if (!dropPreformed)
                return DragAndDropVisualMode.Copy;

            var parent = _idVisualElementLookup[dropIndex];
            var newElements = new List<VisualElement>();
            foreach (var type in data)
            {
                var newElement = ToolboxWindow.CreateVisualElement(type);
                newElements.Add(newElement);
                parent.Add(newElement);
            }

            FrameElements(newElements);
            DragAndDrop.AcceptDrag();

            return DragAndDropVisualMode.Copy;
        }

        private DragAndDropVisualMode HandleHierarchyDrag(List<VisualElement> draggedElements, DragAndDropArgs args)
        {
            var (dropTarget, insertIndex) = GetDropTarget(args);

            foreach (var ve in draggedElements)
            {
                if (ve.Contains(dropTarget) || ve == dropTarget)
                    return DragAndDropVisualMode.Rejected;
            }

            if (!args.performDrop)
                return DragAndDropVisualMode.Move;

            for (var i = 0; i < draggedElements.Count; i++)
            {
                var ve = draggedElements[i];

                if (dropTarget == ve.parent)
                {
                    var oldIndex = dropTarget.IndexOf(ve);
                    var newIndex = i + insertIndex > oldIndex ? insertIndex - 1 : insertIndex;
                    ve.RemoveFromHierarchy();

                    dropTarget.Insert(newIndex, ve);
                }
                else
                {
                    ve.RemoveFromHierarchy();
                    dropTarget.Insert(i + insertIndex, ve);
                }
            }

            FrameElements(draggedElements);
            DragAndDrop.AcceptDrag();

            return DragAndDropVisualMode.Move;
        }

        private (VisualElement target, int insertIndex) GetDropTarget(DragAndDropArgs args)
        {
            if (args.parentItem == null || args.parentItem == rootItem)
            {
                var veRoot = _idVisualElementLookup[0];
                return (veRoot, veRoot.childCount); // Set drop index to last index of preview root item
            }

            var insertIndex = args.insertAtIndex;
            if (args.insertAtIndex == -1)
            {
                insertIndex = args.parentItem.hasChildren ? args.parentItem.children.Count - 1 : 0;
            }

            var targetId = args.parentItem.id;

            return (_idVisualElementLookup[targetId], insertIndex);
        }
    }
}