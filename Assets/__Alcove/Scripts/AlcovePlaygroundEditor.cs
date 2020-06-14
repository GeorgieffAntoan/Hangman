#if UNITY_EDITOR

using System;
using UnityEngine;
using UnityEditor;
[InitializeOnLoad]
public class AlcovePlaygroundEditor : ScriptableObject
{
    public static AlcovePlaygroundEditor m_Instance = null;
    public static EditorWindow m_WindowInstance = null;

    [UnityEditor.MenuItem("Alcove/Settings")]
    public static void ShowPopup()
    {
        EditorWindow window = EditorWindow.CreateInstance<AlcovePlaygroundPopup>();
        window.minSize = new Vector2(50f, 50f);
        window.maxSize = new Vector2(600f, 700f);
        window.titleContent = new GUIContent("Alcove Playground Settings");
        window.Show();
    }

    static AlcovePlaygroundEditor()
    {
        EditorApplication.update += OnInit;
    }
    static void OnInit()
    {
        EditorApplication.update -= OnInit;

        if (!PlayerSettings.productName.Equals("AlcovePlayground", StringComparison.InvariantCultureIgnoreCase))
            return;

        if (m_Instance == null && m_WindowInstance == null)    
        {
            m_Instance = CreateInstance<AlcovePlaygroundEditor>();
            DontDestroyOnLoad(m_Instance);
             
            m_WindowInstance = EditorWindow.GetWindow<AlcovePlaygroundPopup>();
            m_WindowInstance.minSize = new Vector2(100f, 100f);
            m_WindowInstance.maxSize = new Vector2(400f, 300f);
            m_WindowInstance.titleContent = new GUIContent("Alcove Playground Settings");
            m_WindowInstance.Show();
        }
    }
}
#endif