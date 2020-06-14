using UnityEngine;
using UnityEngine.EventSystems;

public class AlcoveExperienceExitButton : MonoBehaviour, IPointerDownHandler
{
    public void OnPointerDown(PointerEventData eventData)
    {
        AlcoveThirdPartyExperienceController.ExitExperience();
    }
}
