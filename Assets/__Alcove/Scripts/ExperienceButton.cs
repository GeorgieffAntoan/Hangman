using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ExperienceButton : MonoBehaviour
{
    public Button m_Btn;
    public TextMeshProUGUI m_NameText;
    public RawImage m_ThumbnailImage;

    public ThirdPartyExperienceData m_ExperienceData;

    public void SetupForExperience(ThirdPartyExperienceData experienceData)
    {
        m_NameText.text = experienceData.ExperienceName;
        m_ThumbnailImage.texture = experienceData.ExperienceThumbnail;
    }
}
