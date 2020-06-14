using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using HashTable = ExitGames.Client.Photon.Hashtable;

public class TableModelExperienceController : MonoBehaviour
{
    #region VARS
    [Header("Viewable Object")]
    public MeshFilter m_ViewableObjectMeshFilter;
    public MeshRenderer m_ViewableObjectMeshRenderer;

    [Header("Meshes")]
    public Mesh m_CubeMesh;
    public Mesh m_SphereMesh;
    public Mesh m_CylinderMesh;

    [Header("Colors")]
    public GameObject m_ColorButtonPrefab;
    public Color[] m_Colors;

    private List<GameObject> _InstantiatedObjects = new List<GameObject>();
    #endregion

    private void Start()
    {
        SetupColorButtons();
    }

    public void SetCube()
    {
        m_ViewableObjectMeshFilter.mesh = m_CubeMesh;
    }

    public void SetSphere()
    {
        m_ViewableObjectMeshFilter.mesh = m_SphereMesh;
    }

    public void SetCylinder()
    {
        m_ViewableObjectMeshFilter.mesh = m_CylinderMesh;
    }

    public void SetColor(Color newColor)
    {
        m_ViewableObjectMeshRenderer.material.color = newColor;
    }

    void SetupColorButtons()
    {
        Vector3 center = transform.position;
        for (int i = 0; i < m_Colors.Length; i++)
        {
            int a = 360 / m_Colors.Length * i;
            Vector3 pos = PositionOnCircle(center, .45f, a);

            object[] instantiationData = new object[1];
            instantiationData[0] = JsonConvert.SerializeObject(m_Colors[i]);

            GameObject colorButtonObj = PhotonNetwork.Instantiate(m_ColorButtonPrefab.name, pos, Quaternion.identity, 0, instantiationData);
            _InstantiatedObjects.Add(colorButtonObj);
        }
    }

    Vector3 PositionOnCircle(Vector3 center, float radius, int a)
    {
        float ang = a;
        Vector3 pos;
        pos.x = center.x + radius * Mathf.Sin(ang * Mathf.Deg2Rad);
        pos.y = center.y;
        pos.z = center.z + radius * Mathf.Cos(ang * Mathf.Deg2Rad);
        return pos;
    }

    private void OnDestroy()
    {
        foreach (GameObject instantiatedObject in _InstantiatedObjects)
        {
            if (instantiatedObject != null && instantiatedObject.GetComponent<PhotonView>().isMine)
                PhotonNetwork.Destroy(instantiatedObject);
        }
    }

    #region NETWORK
    public void OnPhotonCustomRoomPropertiesChanged(Hashtable propertiesThatChanged)
    {
        if (propertiesThatChanged.ContainsKey("TableModelViewer_ObjectMesh"))
        {
            string newMeshName = propertiesThatChanged["TableModelViewer_ObjectMesh"] as string;
            switch (newMeshName)
            {
                case "Cube":
                    SetCube();
                    break;
                case "Sphere":
                    SetSphere();
                    break;
                case "Cylinder":
                    SetCylinder();
                    break;
            }
        }
    }
    #endregion
}
