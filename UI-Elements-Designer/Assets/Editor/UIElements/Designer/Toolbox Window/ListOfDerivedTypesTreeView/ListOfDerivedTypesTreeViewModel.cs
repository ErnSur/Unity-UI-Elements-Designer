using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.IMGUI.Controls;

namespace UIDesigner.Toolbox.TreeView
{
    public partial class ListOfDerivedTypesTreeView
    {   
        private readonly Dictionary<int,string> _idNamespaceLookup = new Dictionary<int, string>();
        private readonly Dictionary<int,Type> _idTypeLookup = new Dictionary<int,Type>();

        protected override TreeViewItem BuildRoot()
        {
            var root = new TreeViewItem {id = 0, depth = -1};
            
            _idNamespaceLookup.Clear();
            _idTypeLookup.Clear();
            
            CreateRowItems(root);

            SetupDepthsFromParentsAndChildren(root);
            
            return root;
        }

        private void CreateRowItems(TreeViewItem root)
        {
            var derivedTypes = GetSubTypesDictionary(_baseClassOfDerivedTypes);
            var rows = new List<TreeViewItem>();
            
            foreach (var @namespace in derivedTypes.Keys)
            {
                var parent = root;

                if (@namespace != "")
                {
                    parent = CreateNameSpaceRow(@namespace, parent);
                    rows.Add(parent);
                }
                
                foreach (var type in derivedTypes[@namespace])
                {
                    var item = CreateTypeRow(type, parent);
                    rows.Add(item);
                }
            }
            
            SortSearchResult(rows);
        }

        private TreeViewItem CreateTypeRow(Type type, TreeViewItem parent)
        {
            var id = type.GetHashCode();
            var item = new TreeViewItem(id)
            {
                displayName = type.Name,
                icon = Styles.UIElementIcon.Value
            };
            
            parent.AddChild(item);
            _idTypeLookup.Add(id, type);
            
            return item;
        }

        private TreeViewItem CreateNameSpaceRow(string @namespace, TreeViewItem parent)
        {
            var id = @namespace.GetHashCode();
            var newNamespaceItem = new TreeViewItem(id)
            {
                displayName = @namespace,
                icon = Styles.NamespaceIcon.Value
            };
            
            parent.AddChild(newNamespaceItem);
            _idNamespaceLookup.Add(id, @namespace);

            return newNamespaceItem;
        }

        private void SortSearchResult(List<TreeViewItem> rows)
        {
            rows.Sort((x, y) => EditorUtility.NaturalCompare(x.displayName, y.displayName));
        }
        
        private Dictionary<string,List<Type>> GetSubTypesDictionary(Type baseClass)
        {
            var subTypes = (from domainAssembly in AppDomain.CurrentDomain.GetAssemblies()
                where !domainAssembly.IsDynamic
                from assemblyType in domainAssembly.GetExportedTypes()
                where !assemblyType.IsGenericType
                where assemblyType.GetConstructor(Type.EmptyTypes) != null
                where baseClass.IsAssignableFrom(assemblyType) && (_includeInheritedTypeInList || assemblyType != baseClass)
                select assemblyType);
            
            var namespaceTypes = new Dictionary<string,List<Type>>();

            foreach (var type in subTypes)
            {
                var @namespace = type.Namespace ?? "";
                
                if(!namespaceTypes.ContainsKey(@namespace))
                {
                    namespaceTypes.Add(@namespace,new List<Type>{type});
                }
                else
                {
                    namespaceTypes[@namespace].Add(type);
                }
            }
            
            return namespaceTypes;
        }
    }
}