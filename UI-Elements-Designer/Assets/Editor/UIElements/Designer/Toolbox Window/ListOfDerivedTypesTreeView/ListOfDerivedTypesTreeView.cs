using System;
using UnityEditor.IMGUI.Controls;

namespace UIDesigner.Toolbox.TreeView
{
    public partial class ListOfDerivedTypesTreeView : UnityEditor.IMGUI.Controls.TreeView
    {
        private readonly Type _baseClassOfDerivedTypes;

        private readonly bool _includeInheritedTypeInList;

        public ListOfDerivedTypesTreeView(TreeViewState state, Type baseClassOfDerivedTypes,
            bool includeInheritedTypeInList) : base(state)
        {
            _includeInheritedTypeInList = includeInheritedTypeInList;
            _baseClassOfDerivedTypes = baseClassOfDerivedTypes;
            Reload();
        }
    }
}