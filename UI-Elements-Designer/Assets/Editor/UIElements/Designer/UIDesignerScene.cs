using UnityEditor;
using UnityEngine.UIElements;

namespace UIDesigner
{
    public static class UIDesignerScene
    {
        [MenuItem("Window/Designer/All")]
        public static void Open()
        {
            Hierarchy_UI.CreateWindow();
            SceneView_UI.CreateWindow();
            ToolboxWindow.CreateWindow();
        }
        
        public static Hierarchy_UI Hierarchy { get; set; }
        public static SceneView_UI SceneView { get; set; }
        public static ToolboxWindow Toolbox { get; set; }

        public static VisualElement UIRoot { get; set; } = new VisualElement{name = "default"};
        
        public static void RefreshPreview()
        {
            if (SceneView != null)
                SceneView.Repaint();
        }
        
        public static void RefreshHierarchy()
        {
            if (Hierarchy != null)
                Hierarchy.Refresh();
        }
    }
}