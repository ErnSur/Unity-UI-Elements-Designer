using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UIElements.New;

namespace UIDesigner
{
    public class SceneView_UI : EditorWindow
    {
        [MenuItem("Window/Designer/Scene")]
        public static void CreateWindow()
        {
            var window = GetWindow<SceneView_UI>(nameof(SceneView_UI));
            UIDesignerScene.SceneView = window;
            window.CreateNewRoot();
        }

        public void CreateNewRoot()
        {
            var previewRoot = new Column {name = "preview-root"};
            rootVisualElement.Clear();
            rootVisualElement.Add(previewRoot);
            UIDesignerScene.UIRoot = previewRoot;
            UIDesignerScene.RefreshHierarchy();
        }

        private void OnEnable()
        {
            UIDesignerScene.SceneView = this;
            CreateNewRoot();
        }
    }
}