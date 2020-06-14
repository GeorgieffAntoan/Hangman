using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class PhysicalHoverEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, IPhysicalInteractable
{
    private const float POINT_HEIGHT = 7.5f;
    private const float TWEEN_DURATION = 0.3f;

    private bool _IsPointedAt;
    private bool _IsLookedAt;

    public void OnPointerEnter(PointerEventData eventData)
    {
        _IsPointedAt = true;
        OnPoint();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _IsPointedAt = false;
        CancelPointEffects();

        if (!_IsLookedAt)
            CancelLookEffects();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Reset();
    }

    void Reset()
    {
        _IsPointedAt = false;
        _IsLookedAt = false;
        CancelLookEffects();
        CancelPointEffects();
    }

    public void OnLook()
    {
        _IsLookedAt = true;
        StartLookEffects();
    }

    public void OnLookAway()
    {
        _IsLookedAt = false;

        if (!_IsPointedAt)
            CancelLookEffects();
    }

    public void OnPoint()
    {
        _IsPointedAt = true;

        StartLookEffects();
        StartPointEffects();
    }

    public void OnReturnToNormal()
    {
        CancelLookEffects();
        CancelPointEffects();
    }

    public virtual void StartPointEffects() {}
    public virtual void CancelPointEffects() { }
    public virtual void StartLookEffects() { }
    public virtual void CancelLookEffects() { }
}
