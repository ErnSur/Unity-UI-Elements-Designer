using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace UIDesigner.Toolbox.TreeView
{
    //Styles
    public partial class ListOfDerivedTypesTreeView
    {
        private static class Styles
        {
            public static readonly Lazy<Texture2D> NamespaceIcon = new Lazy<Texture2D>(()=>EditorGUIUtility.IconContent("Folder Icon").image as Texture2D);
            public static readonly Lazy<Texture2D> UIElementIcon = new Lazy<Texture2D>(()=>EditorGUIUtility.IconContent("Prefab Icon").image as Texture2D);
        }
    }
    
    //Drag
    public partial class ListOfDerivedTypesTreeView
    {
        public static readonly string DragGenericData ="Toolbox-VisualElementTypeList";

        protected override void SetupDragAndDrop(SetupDragAndDropArgs args)
        {
            DragAndDrop.PrepareStartDrag();
            var draggedTypes = args.draggedItemIDs.Select(id => _idTypeLookup[id]).ToList();
            DragAndDrop.SetGenericData(DragGenericData, draggedTypes);

            DragAndDrop.StartDrag("Dragging VisualElements Types");
            
            Event.current.Use();
        }

        protected override bool CanStartDrag(CanStartDragArgs args)
        {
            var nonDragableIds = _idNamespaceLookup.Keys;

            foreach (var draggedId in args.draggedItemIDs)
            {
                if (nonDragableIds.Contains(draggedId))
                    return false;
            }

            return true;
        }

        protected override DragAndDropVisualMode HandleDragAndDrop(DragAndDropArgs args)
        {
            return DragAndDropVisualMode.None;
        }
    }
}