using System;
using System.Collections.Generic;

namespace UnityEngine.UIElements
{
    public static class VisualElementFlowExtensions
    {
        public static T2 ADD<T, T2>(this T parent, T2 child)
            where T : VisualElement
            where T2 : VisualElement
        {
            parent.Add(child);
            return child;
        }

        public static T2[] ADD<T, T2>(this T parent, params T2[] children)
            where T : VisualElement
            where T2 : VisualElement
        {
            foreach (var child in children)
            {
                parent.Add(child);
            }

            return children;
        }

        public static TChild[] AddForEach<TItem, TChild, T>(this T parent, Func<TItem, TChild> makeChild,
            IList<TItem> collection)
            where TChild : VisualElement
            where T : VisualElement
        {
            var children = new TChild[collection.Count];

            for (int i = 0; i < collection.Count; i++)
            {
                var hierarchyChild = makeChild(collection[i]);
                parent.Add(hierarchyChild);
                children[i] = hierarchyChild;
            }

            return children;
        }

        public static TParent WithChildrenForEach<TElement, TResult, TParent>(this TParent parent,
            Func<TElement, TResult> makeChild,
            ICollection<TElement> collection) where TParent : VisualElement where TResult : VisualElement
        {
            var children = new TResult[collection.Count];

            foreach (var element in collection)
            {
                var hierarchyChild = makeChild(element);
                parent.Add(hierarchyChild);
            }

            return parent;
        }

        public static VisualElement[] ForEach(this VisualElement[] elements, Action<VisualElement, int> action)
        {
            for (int i = 0; i < elements.Length; i++)
            {
                action(elements[i], i);
            }

            return elements;
        }

        #region WITH

        public static T WithOnClick<T>(this T ve, Action onClick) where  T : VisualElement
        {
            ve.AddManipulator(new Clickable(onClick));
            return ve;
        }

        public static T WithText<T>(this T textElement, string text) where T : TextElement
        {
            textElement.text = text;
            return textElement;
        }

        public static T WithName<T>(this T visualElement, string name) where T : VisualElement
        {
            visualElement.name = name;
            return visualElement;
        }
        
        public static T WithBackgroundImageOf<T>(this T visualElement, VisualElement ve) where T : VisualElement
        {
            visualElement.style.backgroundImage = ve.style.backgroundImage;
            return visualElement;
        }

        public static TOwner WithClasses<TOwner>(this TOwner visualElement, params string[] classCollection)
            where TOwner : VisualElement
        {
            foreach (var className in classCollection)
            {
                visualElement.AddToClassList(className);
            }

            return visualElement;
        }

        public static TOwner WithChild<TOwner, TChild>(this TOwner parent, TChild child)
            where TOwner : VisualElement
            where TChild : VisualElement
        {
            parent.Add(child);
            return parent;
        }

        public static TOwner WithChildren<TOwner, TChild>(this TOwner parent, params TChild[] children)
            where TOwner : VisualElement
            where TChild : VisualElement
        {
            foreach (var child in children)
            {
                parent.Add(child);
            }

            return parent;
        }

        public static VisualElement WithSizeOf(this VisualElement ve, VisualElement element)
        {
            ve.style.width = element.style.width;
            ve.style.height = element.style.height;

            ve.style.maxWidth = element.style.maxWidth;
            ve.style.maxHeight = element.style.maxHeight;
            ve.style.minWidth = element.style.minWidth;
            ve.style.minHeight = element.style.minHeight;

            ve.style.left = element.style.left;
            ve.style.top = element.style.top;
            ve.style.right = element.style.right;
            ve.style.bottom = element.style.bottom;

            ve.style.marginLeft = element.style.marginLeft;
            ve.style.marginTop = element.style.marginTop;
            ve.style.marginRight = element.style.marginRight;
            ve.style.marginBottom = element.style.marginBottom;
            ve.style.paddingLeft = element.style.paddingLeft;
            ve.style.paddingTop = element.style.paddingTop;
            ve.style.paddingRight = element.style.paddingRight;
            ve.style.paddingBottom = element.style.paddingBottom;

            return ve;
        }

        public static VisualElement WithStyleOf(this VisualElement ve, VisualElement element)
        {
            ve.style.width = element.style.width;
            ve.style.height = element.style.height;
            ve.style.maxWidth = element.style.maxWidth;
            ve.style.maxHeight = element.style.maxHeight;
            ve.style.minWidth = element.style.minWidth;
            ve.style.minHeight = element.style.minHeight;
            ve.style.flexBasis = element.style.flexBasis;
            ve.style.flexGrow = element.style.flexGrow;
            ve.style.flexShrink = element.style.flexShrink;
            ve.style.flexDirection = element.style.flexDirection;
            ve.style.flexWrap = element.style.flexWrap;
            ve.style.overflow = element.style.overflow;
            ve.style.left = element.style.left;
            ve.style.top = element.style.top;
            ve.style.right = element.style.right;
            ve.style.bottom = element.style.bottom;
            ve.style.marginLeft = element.style.marginLeft;
            ve.style.marginTop = element.style.marginTop;
            ve.style.marginRight = element.style.marginRight;
            ve.style.marginBottom = element.style.marginBottom;
            ve.style.paddingLeft = element.style.paddingLeft;
            ve.style.paddingTop = element.style.paddingTop;
            ve.style.paddingRight = element.style.paddingRight;
            ve.style.paddingBottom = element.style.paddingBottom;
            ve.style.position = element.style.position;
            ve.style.alignSelf = element.style.alignSelf;
            ve.style.unityTextAlign = element.style.unityTextAlign;
            ve.style.unityFontStyleAndWeight = element.style.unityFontStyleAndWeight;
            ve.style.unityFont = element.style.unityFont;
            ve.style.fontSize = element.style.fontSize;
            ve.style.whiteSpace = element.style.whiteSpace;
            ve.style.color = element.style.color;
            ve.style.backgroundColor = element.style.backgroundColor;
            ve.style.borderColor = element.style.borderColor;
            ve.style.backgroundImage = element.style.backgroundImage;
            ve.style.unityBackgroundScaleMode = element.style.unityBackgroundScaleMode;
            ve.style.alignItems = element.style.alignItems;
            ve.style.alignContent = element.style.alignContent;
            ve.style.justifyContent = element.style.justifyContent;
            ve.style.borderLeftWidth = element.style.borderLeftWidth;
            ve.style.borderTopWidth = element.style.borderTopWidth;
            ve.style.borderRightWidth = element.style.borderRightWidth;
            ve.style.borderBottomWidth = element.style.borderBottomWidth;
            ve.style.borderTopLeftRadius = element.style.borderTopLeftRadius;
            ve.style.borderTopRightRadius = element.style.borderTopRightRadius;
            ve.style.borderBottomRightRadius = element.style.borderBottomRightRadius;
            ve.style.borderBottomLeftRadius = element.style.borderBottomLeftRadius;
            ve.style.unitySliceLeft = element.style.unitySliceLeft;
            ve.style.unitySliceTop = element.style.unitySliceTop;
            ve.style.unitySliceRight = element.style.unitySliceRight;
            ve.style.unitySliceBottom = element.style.unitySliceBottom;
            ve.style.opacity = element.style.opacity;
            ve.style.visibility = element.style.visibility;
            ve.style.cursor = element.style.cursor;
            ve.style.display = element.style.display;

            return ve;
        }

        #endregion
    }
}