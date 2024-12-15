using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class BaseGraphWindow : EditorWindow
{
    [MenuItem("Window/KyearFramework/BaseGraphWindow")]
    public static void ShowExample()
    {
        BaseGraphWindow window = (BaseGraphWindow)EditorWindow.GetWindow(typeof(BaseGraphWindow));
        window.titleContent = new GUIContent("测试窗口");
    }
    public void CreateGUI()
    {
        VisualElement root = rootVisualElement;
        
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/KyearFramework/GraphView/AssetScript/Window/BaseGraphWindow.uxml");
        visualTree.CloneTree(root);
        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/KyearFramework/GraphView/AssetScript/Window/BaseGraphWindow.uss");
        root.styleSheets.Add(styleSheet);
    }
}
