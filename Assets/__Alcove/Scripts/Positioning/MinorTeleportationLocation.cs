using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class MinorTeleportationLocation : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, IPhysicalInteractable
{
    public static HashSet<MinorTeleportationLocation> MinorTeleportationLocations = new HashSet<MinorTeleportationLocation>();
    public static Collider CurrentPositionCollider;

    private const float POINT_HEIGHT = 7.5f;
    private const float TWEEN_DURATION = 0.3f;

    public UserPosition m_AssociatedPosition;
    public AlcovePositionController m_AlcovePos;

    private Transform _LocationsContainer;
    private Collider _PositionCollider;
    private Tweener _CylinderSizeTween;
    private GameObject _Cylinder;

    private bool _IsPointedAt;
    private bool _IsLookedAt;

    void Awake()
    {
        _PositionCollider = GetComponent<Collider>();
        _LocationsContainer = transform.Find("Locations");
        _Cylinder = transform.Find("TeleportationIndicator/Cylinder").gameObject;

        MinorTeleportationLocations.Add(this);
    }

    void OnDestroy()
    {
        MinorTeleportationLocations.Remove(this);
    }

    private void CreateTween()
    {
        _CylinderSizeTween = _Cylinder.transform.DOScaleZ(POINT_HEIGHT, TWEEN_DURATION)
            .SetAutoKill(false).OnRewind(delegate { _Cylinder.SetActive(false); })
            .Pause();
    }

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
        m_AlcovePos.SetUserPosition(m_AssociatedPosition, delegate
        {
            _PositionCollider.enabled = false;
            CurrentPositionCollider = _PositionCollider;
            Reset();
        });
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

        StartPointEffects();
        StartLookEffects();
    }

    public void OnReturnToNormal()
    {
        CancelLookEffects();
        CancelPointEffects();
    }

    public void StartPointEffects()
    {
        m_AlcovePos.ShowMinorTeleportationMarkerAtTransform(m_AssociatedPosition, transform);
    }

    public void CancelPointEffects()
    {
        m_AlcovePos.HideMinorTeleportationMarker();
    }

    public void StartLookEffects()
    {
        if (_CylinderSizeTween == null)
            CreateTween();

        _Cylinder.gameObject.SetActive(true);
        _CylinderSizeTween.PlayForward();
    }

    public void CancelLookEffects()
    {
        if (_CylinderSizeTween == null)
            CreateTween();

        _CylinderSizeTween.SmoothRewind();
    }

    public static Transform GetTransformForPosition(UserPosition position)
    {
        foreach (MinorTeleportationLocation minorTeleportationLocation in MinorTeleportationLocations)
        {
            if (position == minorTeleportationLocation.m_AssociatedPosition)
                return minorTeleportationLocation._LocationsContainer;
        }

        return null;
    }
}
