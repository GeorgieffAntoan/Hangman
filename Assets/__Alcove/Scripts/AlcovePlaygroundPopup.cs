#if UNITY_EDITOR

using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class AlcovePlaygroundPopup : EditorWindow
{
    public ThirdPartyExperienceType m_ExperienceType;
    Rect buttonRect;

    void OnGUI()
    {
        var style = GUI.skin.GetStyle("boldLabel");
        GUIStyle newStyle = new GUIStyle(style) {fontSize = 24};
        newStyle.wordWrap = true;
        GUILayout.Label("Alcove Playground", newStyle);

        EditorGUILayout.Space();

        style = GUI.skin.GetStyle("label");
        newStyle = new GUIStyle(style) { fontSize = 14 };
        newStyle.wordWrap = true;
        GUILayout.Label("Welcome to the Alcove Playground. Please set your experience name here. This will control your multiplayer grouping for testing as well as your Playground app name and package identifier.", newStyle);

        EditorGUILayout.Space();

        string productName = EditorGUILayout.TextField("Experience Name", PlayerSettings.productName);
        PlayerSettings.productName = productName;
        PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, "com.aarpinnovation." + productName.ToLower().Replace(" ", ""));
    }
}
#endif