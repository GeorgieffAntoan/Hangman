using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitOnEscape : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (AlcoveThirdPartyExperienceController.HomeEnvironmentSceneName != SceneManagerHelper.ActiveSceneName)
                SceneManager.LoadScene(AlcoveThirdPartyExperienceController.HomeEnvironmentSceneName);
            else if (AlcoveThirdPartyExperienceController.getInstance() && AlcovePositionController.getInstance())
            {
                AlcoveThirdPartyExperienceController.getInstance().DestroyThirdPartyExperience(
                    AlcoveThirdPartyExperienceController.CurrentThirdPartyExperience.m_Data.ExperienceType);
                AlcovePositionController.getInstance().SetUserPosition(UserPosition.Entertainment);
            }
        }
    }
}
