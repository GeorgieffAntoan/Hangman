using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class AlcoveVrInteractable : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public UnityEvent PointerEnter;
    public UnityEvent PointerExit;
    public UnityEvent PointerClick;

    void Awake()
    {
        gameObject.layer = LayerMask.NameToLayer("PhysicsInteractable");

        if (!GetComponent<Collider>())
            Debug.Log("GameObject " + gameObject.name + " has a VrInteractor component but no collider. For interactions to work a collider is required.");
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        PointerEnter.Invoke();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        PointerExit.Invoke();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        PointerClick.Invoke();
    }
}
