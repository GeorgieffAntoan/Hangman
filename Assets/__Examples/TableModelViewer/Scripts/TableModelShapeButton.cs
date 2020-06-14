using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using UnityEngine;
using UnityEngine.EventSystems;
using Hashtable = ExitGames.Client.Photon.Hashtable;

namespace TableViewerExperience
{
    public class TableModelShapeButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        private MeshRenderer _Renderer;
        private Color _OriginalColor;

        void Awake()
        {
            _Renderer = GetComponent<MeshRenderer>();
            _OriginalColor = _Renderer.material.color;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _Renderer.material.color = Color.green;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _Renderer.material.color = _OriginalColor;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            PhotonNetwork.room.SetCustomProperties(new Hashtable()
            {
                { "TableModelViewer_ObjectMesh", gameObject.name }
            });
        }
    }
}
