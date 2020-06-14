using UnityEngine;
using UnityEngine.EventSystems;

public class AlcovePosition : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public UserPosition m_AssociatedPosition;
    private AlcovePositionController _PositionController;

    void Awake()
    {
        _PositionController = FindObjectOfType<AlcovePositionController>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _PositionController.ShowTeleportationMarkerForPosition(m_AssociatedPosition);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _PositionController.HideTeleportationMarker();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        _PositionController.SetUserPosition(m_AssociatedPosition);
    }

#if UNITY_EDITOR
    protected virtual void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position,0.2f);
    }
#endif
}
