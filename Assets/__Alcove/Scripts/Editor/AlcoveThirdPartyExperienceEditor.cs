using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AlcoveThirdPartyExperience))]
public class AlcoveThirdPartyExperienceEditor : Editor
{
    // OnInspector GUI
    public override void OnInspectorGUI() //2
    {
        base.DrawDefaultInspector();

        if (GUILayout.Button("SetPosition")) //8
        {
            AlcoveThirdPartyExperience script = (AlcoveThirdPartyExperience)target;
            AlcoveThirdPartyExperienceController experienceController = FindObjectOfType<AlcoveThirdPartyExperienceController>();
            if (experienceController == null)
                Debug.LogError("No Third Party Experience Controller found in scene.");

            if (script.m_Data.ExperienceType == ThirdPartyExperienceType.Room && experienceController.m_RoomExperienceTransform == null)
                Debug.LogError("No transform for Room type assigned to the ThirdPartyExperienceController.");
            else if (script.m_Data.ExperienceType == ThirdPartyExperienceType.Room)
            {
                script.transform.position = experienceController.m_RoomExperienceTransform.position;
                script.transform.rotation= experienceController.m_RoomExperienceTransform.rotation;
            }

            else if (script.m_Data.ExperienceType == ThirdPartyExperienceType.Tabletop && experienceController.m_RoomExperienceTransform == null)
                Debug.LogError("No transform for Tabletop type assigned to the ThirdPartyExperienceController.");
            else if (script.m_Data.ExperienceType == ThirdPartyExperienceType.Tabletop)
            {
                script.transform.position = experienceController.m_TableExperienceTransform.position;
                script.transform.rotation = experienceController.m_TableExperienceTransform.rotation;
            }
        }
    }


}
