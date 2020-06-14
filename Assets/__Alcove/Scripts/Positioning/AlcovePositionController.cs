using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public enum UserPosition
{
    Entertainment,
    Family,
    Health,
    Travel,
    InitialSpawn,
    Hallway1,
    Hallway2,
    Hallway3,
    Hallway4,
    Hallway5,
    Balcony1,
    Balcony2,
    Balcony3,
    Balcony4,
    Balcony5,
    MediaPlayer,
    ThirdPartyRoom,
    ThirdPartyTable,
    None
}

public class AlcovePositionController : SingletonMonoBehaviour<AlcovePositionController>
{
    public static UserPosition CurrentUserPosition = UserPosition.Entertainment;
    public static int CurrentUserPositionIndex = 0;

    [NonSerialized]
    public Action<UserPosition, int> m_OnPositionChanged;

    [Header("Spawn Points")]
    public GameObject[] m_HealthSpawnPoints;
    public GameObject[] m_EntertainmentSpawnPoints;
    public GameObject[] m_FamilySpawnPoints;
    public GameObject[] m_TravelSpawnPoints;
    public GameObject[] m_ThirdPartyTableSpawnPoints;
    public GameObject[] m_ThirdPartyRoomSpawnPoints;

    private GameObject[] _OriginalThirdPartyTableSpawnPoints;
    private GameObject[] _OriginalThirdPartyRoomSpawnPoints;

    [Header("Third Party")]
    public GameObject m_ThirdPartyExperienceSelectionCanvas;

    [Header("Teleportation")]
    public float m_FloorHeight;
    public GameObject m_TeleportationContainer;
    public GameObject m_MinorTeleportationContainer;
    public GameObject m_TeleportationGlow;
    public Collider m_FamilyRoomCollider;
    public Collider m_EntertainmentAreaCollider;
    public Collider m_TravelAreaCollider;
    public Collider m_HealthAreaCollider;
    public Collider m_ThirdPartyTableCollider;
    public Collider m_ThirdPartyRoomCollider;
    public Collider m_ThirdPartyRoomExitCollider;

    [NonSerialized]
    public Transform m_CurrentUserPositionTransform;

    private bool _InitialPositionSet;

    void Awake()
    {
        gInstance = this;

        _OriginalThirdPartyTableSpawnPoints = m_ThirdPartyTableSpawnPoints;
        _OriginalThirdPartyRoomSpawnPoints = m_ThirdPartyRoomSpawnPoints;
    }

    void Start()
    {
        SetUserPosition(CurrentUserPosition);
    }

    public void ShowTeleportationMarkerForPosition(UserPosition position)
    {
        Transform objectPosTransform = SetObjectPosition(m_TeleportationContainer, position, GetPositionIndex(position));
        Vector3 floorPos = m_TeleportationContainer.transform.position;
        floorPos.y = m_FloorHeight;
        m_TeleportationGlow.transform.position = floorPos;
        m_TeleportationContainer.SetActive(true);
        m_TeleportationContainer.transform.rotation = objectPosTransform.transform.rotation;
    }

    public void ShowMinorTeleportationMarkerAtTransform(UserPosition position, Transform potentialLocationsContainer)
    {
        m_MinorTeleportationContainer.SetActive(true);
        
        m_MinorTeleportationContainer.transform.position = potentialLocationsContainer.transform.position;
        m_MinorTeleportationContainer.transform.rotation = potentialLocationsContainer.transform.rotation;
    }

    public void HideMinorTeleportationMarker()
    {
        m_MinorTeleportationContainer.SetActive(false);
    }

    public void HideTeleportationMarker()
    {
        m_TeleportationContainer.SetActive(false);
    }

    public void HideAllTeleportationGraphics()
    {
        HideMinorTeleportationMarker();
        HideTeleportationMarker();
    }

    public int SetUserPosition(UserPosition position, Action callback = null)
    {
        if ((position == UserPosition.ThirdPartyTable || position == UserPosition.ThirdPartyRoom) &&
            AlcoveThirdPartyExperienceController.CurrentThirdPartyExperience == null)
        {
            if (m_ThirdPartyExperienceSelectionCanvas)
                m_ThirdPartyExperienceSelectionCanvas.SetActive(true);
            CameraController.getInstance().PositionObjectInFrontOfCamera(m_ThirdPartyExperienceSelectionCanvas.gameObject, 1.25f);
            return -1;
        }
        else
        {
            if (m_ThirdPartyExperienceSelectionCanvas)
                m_ThirdPartyExperienceSelectionCanvas.SetActive(false);
        }

        int availablePositionIndex = GetPositionIndex(position);
        SetUserPosition(position, availablePositionIndex, callback);

        if (OVRPlugin.GetSystemHeadsetType().ToString().Contains("Quest") || OVRPlugin.GetSystemHeadsetType().ToString().Contains("Rift"))
            CameraController.CameraContainer.transform.position = new Vector3(CameraController.CameraContainer.transform.position.x, m_FloorHeight, CameraController.CameraContainer.transform.position.z);

        return availablePositionIndex;
    }

    public int GetPositionIndex(UserPosition position)
    {
        if (PhotonNetwork.room == null)
            return 0;

        //Iterate through players ignoring the local player and any who aren't in the location we are moving to. Find the first available spot.
        bool[] takenIndexes = new bool[MultiplayerController.MAX_PLAYERS];
        foreach (PhotonPlayer photonPlayer in PhotonNetwork.playerList)
        {
            if (!photonPlayer.IsLocal && photonPlayer.CustomProperties.ContainsKey("EnvironmentLocation") && (int)photonPlayer.CustomProperties["EnvironmentLocation"] == (int)position)
            {
                if (photonPlayer.CustomProperties.ContainsKey("EnvironmentPositionIndex"))
                    takenIndexes[(int)photonPlayer.customProperties["EnvironmentPositionIndex"]] = true;
            }
        }
        for (int i = 0; i < takenIndexes.Length; i++)
        {
            if (!takenIndexes[i])
            {
                return i;
            }
        }

        return 0;
    }

    public void SetUserPosition(UserPosition position, int posIndex, Action callback = null)
    {
        //Check if we need to destroy the room experience
        if ((CurrentUserPosition == UserPosition.ThirdPartyRoom ||
            CurrentUserPosition == UserPosition.ThirdPartyTable) && position != CurrentUserPosition && AlcoveThirdPartyExperienceController.CurrentThirdPartyExperience != null && AlcoveThirdPartyExperienceController.CurrentThirdPartyExperience.photonView.isMine)
        {
            ThirdPartyExperienceType type = CurrentUserPosition == UserPosition.ThirdPartyRoom
                ? ThirdPartyExperienceType.Room
                : ThirdPartyExperienceType.Tabletop;

            NotificationPanel.NotificationData notificationData = new NotificationPanel.NotificationData();
            notificationData.AcceptButtonText = "Ok";
            notificationData.CancelButtonText = "Cancel";
            notificationData.HeaderText = "Leaving game";
            notificationData.MessageText = "If you leave the table, the game will end!";
            notificationData.AcceptCallback = delegate
            {
                AlcoveThirdPartyExperienceController.getInstance().DestroyThirdPartyExperience(type);
                SetUserPosition(position, callback);
            };
            
            NotificationPanel.getInstance().ShowNotification(notificationData, true);
            return;
        }

        if (MinorTeleportationLocation.CurrentPositionCollider)
            MinorTeleportationLocation.CurrentPositionCollider.enabled = true;

        CurrentUserPosition = position;
        CurrentUserPositionIndex = posIndex;

        m_CurrentUserPositionTransform = SetObjectPosition(CameraController.CameraContainer, position, posIndex);

        //Setup room colliders
        m_FamilyRoomCollider.enabled = CurrentUserPosition != UserPosition.Family;
        m_EntertainmentAreaCollider.enabled = CurrentUserPosition != UserPosition.Entertainment;
        m_TravelAreaCollider.enabled = CurrentUserPosition != UserPosition.Travel;
        m_HealthAreaCollider.enabled = CurrentUserPosition != UserPosition.Health;
        m_ThirdPartyTableCollider.enabled = CurrentUserPosition != UserPosition.ThirdPartyTable;
        m_ThirdPartyRoomCollider.enabled = CurrentUserPosition != UserPosition.ThirdPartyRoom;
        m_ThirdPartyRoomExitCollider.enabled = CurrentUserPosition == UserPosition.ThirdPartyRoom;

        HideAllTeleportationGraphics();

        callback?.Invoke();

        if (m_OnPositionChanged != null)
            m_OnPositionChanged(CurrentUserPosition, CurrentUserPositionIndex);
    }

    public Transform SetObjectPosition(GameObject go, UserPosition position, int positionIndex)
    {
        Transform potentialLocationsContainer = MinorTeleportationLocation.GetTransformForPosition(position);

        //Set position and rotation
        Transform positionTransform;
        switch (position)
        {
            case UserPosition.Health:
                positionTransform = m_HealthSpawnPoints[positionIndex].transform;
                break;
            case UserPosition.Entertainment:
                positionTransform = m_EntertainmentSpawnPoints[positionIndex].transform;
                break;
            case UserPosition.Travel:
                positionTransform = m_TravelSpawnPoints[positionIndex].transform;
                break;
            case UserPosition.Family:
                positionTransform = m_FamilySpawnPoints[positionIndex].transform;
                break;
            case UserPosition.ThirdPartyTable:
                positionTransform = m_ThirdPartyTableSpawnPoints[positionIndex].transform;
                break;
            case UserPosition.ThirdPartyRoom:
                positionTransform = m_ThirdPartyRoomSpawnPoints[positionIndex].transform;
                break;
            default:
                if (potentialLocationsContainer)
                {
                    positionTransform = potentialLocationsContainer.GetChild(positionIndex);
                    go.transform.position = positionTransform.position;
                    return positionTransform; //Do not set rotation for minor teleportation
                }
                else
                    positionTransform = m_EntertainmentSpawnPoints[0].transform;
                break;
        }
        go.transform.position = positionTransform.position;
        go.transform.rotation = positionTransform.rotation;

        return positionTransform;
    }

    public void ResetPosition()
    {
        if (CurrentUserPosition != UserPosition.MediaPlayer)
            SetUserPosition(CurrentUserPosition);
    }

    public void QuitToOculusHome()
    {
        OVRManager.PlatformUIConfirmQuit();
    }

    #region THIRDPARTY
    public void SetThirdPartySpawnPoints(GameObject[] spawnPoints, ThirdPartyExperienceType type)
    {
        if (type == ThirdPartyExperienceType.Tabletop && spawnPoints != null && spawnPoints.Length > 3)
            m_ThirdPartyTableSpawnPoints = spawnPoints;
        if (type == ThirdPartyExperienceType.Room && spawnPoints != null && spawnPoints.Length > 3)
            m_ThirdPartyRoomSpawnPoints = spawnPoints;
    }

    public void ResetThirdPartySpawnPoints(ThirdPartyExperienceType type)
    {
        if (type == ThirdPartyExperienceType.Room)
            m_ThirdPartyRoomSpawnPoints = _OriginalThirdPartyRoomSpawnPoints;
        else if (type == ThirdPartyExperienceType.Tabletop)
            m_ThirdPartyTableSpawnPoints = _OriginalThirdPartyTableSpawnPoints;
    }
    #endregion

#if UNITY_EDITOR
    protected virtual void OnDrawGizmos()
    {
        //GameObject[] thirdPartySpawnPoints = m_ThirdPartyRoomSpawnPoints ?? m_ThirdPartyTableSpawnPoints;
        if (m_ThirdPartyTableSpawnPoints != null)
        {
            foreach (GameObject go in m_ThirdPartyTableSpawnPoints)
            {
                Gizmos.DrawWireSphere(go.transform.position, 0.1f);
            }
        }

        if (m_ThirdPartyRoomSpawnPoints != null)
        {
            foreach (GameObject go in m_ThirdPartyRoomSpawnPoints)
            {
                Gizmos.DrawWireSphere(go.transform.position, 0.1f);
            }
        }

        foreach (GameObject go in m_FamilySpawnPoints)
        {
            Gizmos.DrawWireSphere(go.transform.position, 0.1f);
        }
        foreach (GameObject go in m_EntertainmentSpawnPoints)
        {
            Gizmos.DrawWireSphere(go.transform.position, 0.1f);
        }
        foreach (GameObject go in m_HealthSpawnPoints)
        {
            Gizmos.DrawWireSphere(go.transform.position, 0.1f);
        } 
        foreach (GameObject go in m_TravelSpawnPoints)
        {
            Gizmos.DrawWireSphere(go.transform.position, 0.1f);
        }
    }
#endif
}
