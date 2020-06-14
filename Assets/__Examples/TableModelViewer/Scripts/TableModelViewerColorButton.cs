using Newtonsoft.Json;
using Photon;
using UnityEngine;
using UnityEngine.EventSystems;

public class TableModelViewerColorButton : PunBehaviour, IPointerClickHandler, IPointerExitHandler, IPointerEnterHandler
{
    public Color m_Color;

    private TableModelExperienceController _TableModelExperienceController;
    private MeshRenderer _Renderer;

    void Awake()
    {
        _TableModelExperienceController = FindObjectOfType<TableModelExperienceController>();
        m_Color = JsonConvert.DeserializeObject<Color>(photonView.instantiationData[0] as string);
        _Renderer = GetComponent<MeshRenderer>();
        _Renderer.material.color = m_Color;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        photonView.RPC("SetColor", PhotonTargets.All);
    }

    [PunRPC]
    void SetColor()
    {
        _TableModelExperienceController.SetColor(m_Color);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _Renderer.material.color = m_Color;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _Renderer.material.color = Color.green;
    }
}
