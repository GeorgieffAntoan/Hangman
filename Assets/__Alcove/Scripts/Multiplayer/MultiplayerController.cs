using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Oculus.Platform;
using Oculus.Platform.Models;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using Application = Oculus.Platform.Application;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using HashTable = ExitGames.Client.Photon.Hashtable;
using Random = System.Random;

public class MultiplayerController : SingletonMonoBehaviour<MultiplayerController>
{
    public static string RoomName;
    public static string MyAvatarId = "Test";

    public static string[] CrossPlatformExpressiveAvatarIds =
    {
        "10150030458727564", "10150030458738922", "10150030458747067", "10150030458756715", "10150030458762178",
        "10150030458769900", "10150030458775732", "10150030458785587", "10150030458806683", "10150030458820129",
        "10150030458827644", "10150030458843421"
    };

    public const int MAX_PLAYERS = 4;

    public enum AlcovePhotonEventCodes
    {
        InstantiateVrAvatarEventCode = 123
    }

    [Header("Prefabs")]
    public GameObject m_LocalAvatarPrefab;
    public GameObject m_RemoteAvatarPrefab;

    [Header("Positioning")]
    public AlcovePositionController m_PositionController;

    [NonSerialized]
    public Dictionary<int, GameObject> m_PlayerAvatars;

    private PhotonVoiceRecorder _LocalVoiceRecorder;
    private GameObject _LocalAvatar;

    void Awake()
    {
        if (gInstance != null)
        {
            DestroyImmediate(gameObject);
            return;
        }

        gInstance = this;

        RoomName = UnityEngine.Application.identifier;

        PhotonNetwork.OnEventCall += OnEvent;

        m_PositionController.m_OnPositionChanged += delegate (UserPosition newPos, int posIndex)
        {
            if (_LocalAvatar)
                m_PositionController.SetObjectPosition(_LocalAvatar, newPos, posIndex);
            if (_LocalVoiceRecorder)
                m_PositionController.SetObjectPosition(_LocalVoiceRecorder.gameObject, newPos, posIndex);

            PhotonNetwork.player.SetCustomProperties(new ExitGames.Client.Photon.Hashtable() {
                { "EnvironmentLocation", (int)newPos },
                { "EnvironmentPositionIndex", posIndex }
            });
        };

        SceneManager.sceneLoaded += SceneManagerOnSceneLoaded;
    }

    public void Start()
    {
        SetupLocalUser();
        Connect();
        InvokeRepeating("Reconnect", 1f, 1f);
    }

    private void OnDestroy()
    {
        PhotonNetwork.OnEventCall -= OnEvent;
        SceneManager.sceneLoaded -= SceneManagerOnSceneLoaded;
    }

    private void SceneManagerOnSceneLoaded(Scene loadedScene, LoadSceneMode mode)
    {
        m_PositionController = FindObjectOfType<AlcovePositionController>();

        PhotonNetwork.player.SetCustomProperties(new HashTable() { { "CurrentScene", loadedScene.name } });

        if (loadedScene.name.Equals(AlcoveThirdPartyExperienceController.HomeEnvironmentSceneName))
            SetLocalAvatarParentToCameraRig();

        if (PhotonNetwork.room != null)
        {
            foreach (var photonPlayer in PhotonNetwork.playerList)
            {
                if (photonPlayer.IsLocal)
                    continue;

                if (photonPlayer.CustomProperties.ContainsKey("CurrentScene") &&
                    loadedScene.name.Equals(photonPlayer.CustomProperties["CurrentScene"] as string,
                        StringComparison.InvariantCultureIgnoreCase) && m_PlayerAvatars.ContainsKey(photonPlayer.ID))
                {
                    m_PlayerAvatars[photonPlayer.ID].GetComponent<OvrAvatar>().ShowThirdPerson = true;

                    if (string.Equals(AlcoveThirdPartyExperienceController.HomeEnvironmentSceneName, photonPlayer.CustomProperties["CurrentScene"] as string, StringComparison.InvariantCultureIgnoreCase))
                        SetPlayerPosition(photonPlayer);
                }
                else if (m_PlayerAvatars.ContainsKey(photonPlayer.ID))
                    m_PlayerAvatars[photonPlayer.ID].GetComponent<OvrAvatar>().ShowThirdPerson = false;
            }
        }

        if (PhotonNetwork.room != null)
            Fader.FadeInAll();
    }


    void SetupLocalUser()
    {
        int randomAvatarIndex = UnityEngine.Random.Range(0, CrossPlatformExpressiveAvatarIds.Length - 1);
        MyAvatarId = CrossPlatformExpressiveAvatarIds[randomAvatarIndex];

        TimeSpan t = DateTime.UtcNow - new DateTime(1970, 1, 1);
        long secondsSinceEpoch = (long)t.TotalMilliseconds;

        PhotonNetwork.playerName = MyAvatarId + secondsSinceEpoch;
        PhotonNetwork.player.SetCustomProperties(new HashTable() { { "OculusId", MyAvatarId } });
    }

    private void Connect()
    {
        PhotonNetwork.ConnectUsingSettings("0.1");

        PhotonNetwork.player.SetCustomProperties(new ExitGames.Client.Photon.Hashtable() {
            { "EnvironmentLocation", (int)UserPosition.Entertainment },
            { "EnvironmentPositionIndex", 0 }
        });
    }

    //In this case we have just left the Photon room. We need to reset state.
    public void OnLeftRoom()
    {
        if (_LocalAvatar)
            Destroy(_LocalAvatar.gameObject);

        //Destroy player avatars
        foreach (var playerAvatar in m_PlayerAvatars)
        {
            Destroy(playerAvatar.Value.gameObject);
        }

        m_PlayerAvatars = new Dictionary<int, GameObject>();
    }

    public void OnPhotonPlayerConnected(PhotonPlayer player)
    {
        if (PhotonNetwork.room != null && PhotonNetwork.isMasterClient)
        {
            string delimitedUserList = "";

            if (PhotonNetwork.room.CustomProperties.ContainsKey("UserList"))
                delimitedUserList = PhotonNetwork.room.CustomProperties["UserList"] as string;

            List<string> userList = string.IsNullOrEmpty(delimitedUserList) ? delimitedUserList.Split('|').ToList() : new List<string>();

            userList.Add(player.NickName);

            delimitedUserList = string.Join("|", userList.ToArray());

            HashTable props = new Hashtable()
            {
                { "OwnerOculusId", MyAvatarId },
                { "UserList", delimitedUserList },
            };

            PhotonNetwork.room.SetCustomProperties(props);
        }
    }

    public void OnPhotonPlayerDisconnected(PhotonPlayer player)
    {
        if (m_PlayerAvatars.ContainsKey(player.ID))
        {
            Destroy(m_PlayerAvatars[player.ID].gameObject);
            m_PlayerAvatars.Remove(player.ID);
        }

        if (PhotonNetwork.room != null && PhotonNetwork.isMasterClient)
        {
            string delimitedUserList = ""; 

            if (PhotonNetwork.room.CustomProperties.ContainsKey("UserList")) 
                delimitedUserList = PhotonNetwork.room.CustomProperties["UserList"] as string;

            List<string> userList = string.IsNullOrEmpty(delimitedUserList) ? delimitedUserList.Split('|').ToList() : new List<string>();
            
            userList.Remove(player.NickName);
            delimitedUserList = string.Join("|", userList.ToArray());

            HashTable props = new Hashtable()
            {
                { "OwnerOculusId", MyAvatarId },
                { "UserList", delimitedUserList },
            };

            PhotonNetwork.room.SetCustomProperties(props);
        }
    }

    void Reconnect()
    {
        if (PhotonNetwork.connectionState == ConnectionState.Disconnected && RoomName != null)
        {
            PhotonNetwork.ConnectUsingSettings("0.1");
        }
    }

    void OnJoinedLobby()
    {
        CreateOrJoinPhotonRoom();
    }

    public void CreateOrJoinPhotonRoom()
    {
        Debug.Log("Joining or creating Photon room");
        if (PhotonNetwork.room != null)
        {
            PhotonNetwork.LeaveRoom();
            return;
        }

        RoomOptions roomOptions = new RoomOptions();
        roomOptions.CleanupCacheOnLeave = true;

        PhotonNetwork.JoinOrCreateRoom(RoomName, roomOptions, TypedLobby.Default);
    }

    public void OnJoinedRoom()
    {
        Debug.Log("Joined room with id " + PhotonNetwork.room.Name);

        m_PlayerAvatars = new Dictionary<int, GameObject>();

        m_PositionController.ResetPosition();

        //Send create avatar event
        object[] avatarData = new object[2];
        int viewId = PhotonNetwork.AllocateViewID();
        avatarData[0] = viewId;
        avatarData[1] = MyAvatarId;
        PhotonNetwork.RaiseEvent((byte)AlcovePhotonEventCodes.InstantiateVrAvatarEventCode, avatarData, true, new RaiseEventOptions() { CachingOption = EventCaching.AddToRoomCache, Receivers = ReceiverGroup.All });
    }

#region AvatarInstantiation
    private void OnEvent(byte eventcode, object content, int senderid)
    {
        if (eventcode == (byte)AlcovePhotonEventCodes.InstantiateVrAvatarEventCode)
        {
            StartCoroutine(DelayedAvatarInstantiation(content, senderid));
        }
    }

    IEnumerator DelayedAvatarInstantiation(object content, int senderid)
    {
        yield return new WaitForSecondsRealtime(0.1f);

        object[] instantiationData = (object[])content;

        //Create panel
        PhotonPlayer photonPlayer = PhotonPlayer.Find(senderid);

        GameObject go = null;
        int viewId = (int)instantiationData[0];
        string oculusId = (string)instantiationData[1];

        if (PhotonNetwork.player.ID == senderid)
        {
            go = Instantiate(m_LocalAvatarPrefab);
            m_PlayerAvatars.Add(photonPlayer.ID, go);

            _LocalAvatar = go;

            _LocalVoiceRecorder = PhotonNetwork.Instantiate("PhotonPlayerVoice", Vector3.zero, Quaternion.identity, 0).GetComponent<PhotonVoiceRecorder>();
            _LocalVoiceRecorder.transform.SetParent(go.transform);
            _LocalAvatar.transform.SetParent(CameraController.CameraContainer.transform);
        }
        else
        {
            go = Instantiate(m_RemoteAvatarPrefab);
            m_PlayerAvatars.Add(photonPlayer.ID, go);
        }

        if (go != null)
        {
            PhotonView pView = go.GetComponent<PhotonView>();

            if (pView != null)
            {
                pView.viewID = viewId;
                pView.ownerId = senderid;
            }

            OvrAvatar avatar = go.GetComponent<OvrAvatar>();
            avatar.oculusUserID = oculusId;

            if (photonPlayer.IsLocal)
            {
                Fader.FadeInAll();
            }

            m_PositionController.SetObjectPosition(go, (UserPosition)photonPlayer.CustomProperties["EnvironmentLocation"],
                (int)photonPlayer.CustomProperties["EnvironmentPositionIndex"]);

            if (!photonPlayer.IsLocal)
                SetPlayerPosition(photonPlayer);
        }
    }

    public void OnPhotonPlayerPropertiesChanged(object[] playerAndUpdatedProps)
    {
        PhotonPlayer player = playerAndUpdatedProps[0] as PhotonPlayer;
        ExitGames.Client.Photon.Hashtable props = playerAndUpdatedProps[1] as ExitGames.Client.Photon.Hashtable;
        if (player == null || props == null)
        {
            return;
        }

        if (!player.IsLocal && props.ContainsKey("CurrentScene"))
        {
            string playerNewScene = props["CurrentScene"] as string;
            if (!string.IsNullOrEmpty(playerNewScene))
                SetPlayerAvatarVisibility(player, playerNewScene.Equals(SceneManager.GetActiveScene().name, StringComparison.InvariantCultureIgnoreCase));
        }

        if (!player.IsLocal && props.ContainsKey("EnvironmentLocation") || player.CustomProperties.ContainsKey("EnvironmentPositionIndex"))
        {
            SetPlayerPosition(player);
        }
    }

    public void SetPlayerPosition(PhotonPlayer player)
    {
        if (m_PlayerAvatars == null || player == null)
            return;

        int posIndex = 0;
        if (player.CustomProperties.ContainsKey("EnvironmentPositionIndex"))
            posIndex = (int)player.CustomProperties["EnvironmentPositionIndex"];

        int locationIndex = 0;
        if (player.CustomProperties.ContainsKey("EnvironmentLocation"))
            locationIndex = (int)player.CustomProperties["EnvironmentLocation"];

        UserPosition userPosition = (UserPosition)locationIndex;

        if (m_PlayerAvatars.ContainsKey(player.ID) && m_PlayerAvatars[player.ID] != null && m_PositionController != null)
            m_PositionController.SetObjectPosition(m_PlayerAvatars[player.ID].gameObject, userPosition, posIndex);

        if (userPosition == UserPosition.ThirdPartyRoom || userPosition == UserPosition.ThirdPartyTable)
            AlcoveThirdPartyExperienceController.AssignPlayerToThirdPartyExperiencePosition(player,locationIndex);
        else if (AlcoveThirdPartyExperienceController.PlayersInExperience.ContainsKey(player))
            AlcoveThirdPartyExperienceController.RemovePlayerFromThirdPartyExperience(player);
    }

    public void SetPlayerAvatarVisibility(PhotonPlayer player, bool shouldBeVisible)
    {
        if (m_PlayerAvatars == null || player == null)
            return;

        if (m_PlayerAvatars.ContainsKey(player.ID) && m_PlayerAvatars[player.ID] != null)
            m_PlayerAvatars[player.ID].GetComponent<OvrAvatar>().ShowThirdPerson = shouldBeVisible;
    }

    public void SetLocalAvatarToNoParent()
    {
        if (_LocalAvatar)
        {
            _LocalAvatar.transform.SetParent(null);
        }
    }

    public void SetLocalAvatarParentToCameraRig()
    {
        if (_LocalAvatar)
            _LocalAvatar.transform.SetParent(CameraController.CameraContainer.transform);
    }
    #endregion
}
