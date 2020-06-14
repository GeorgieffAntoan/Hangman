using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AlcoveThirdPartyExperienceController : SingletonMonoBehaviour<AlcoveThirdPartyExperienceController>
{
    public static AlcoveThirdPartyExperience CurrentThirdPartyExperience;
    public static Dictionary<PhotonPlayer, int> PlayersInExperience = new Dictionary<PhotonPlayer, int>();
    public static string HomeEnvironmentSceneName;

    [Header("Experiences")]
    public GameObject[] m_AlcoveThirdPartyExperiences;

    [Header("Buttons")]
    public Transform m_BoardGameSelectionContainer;
    public Transform m_OtherExperienceSelectionContainer;
    public GameObject m_ExperienceButtonPrefab;

    [Header("Positioning")]
    public Transform m_TableExperienceTransform;
    public Transform m_RoomExperienceTransform;

    void Awake()
    {
        HomeEnvironmentSceneName = SceneManager.GetActiveScene().name;

        foreach (GameObject alcoveThirdPartyExperience in m_AlcoveThirdPartyExperiences)
        {
            if (!alcoveThirdPartyExperience)
                continue;

            AlcoveThirdPartyExperience experience =
                alcoveThirdPartyExperience.GetComponent<AlcoveThirdPartyExperience>();

            if (experience == null)
            {
                Debug.LogException(new Exception("Experience " + alcoveThirdPartyExperience.name + " was assigned to the AlcoveThirdPartyExperienceController without a AlcoveThirdPartyExperience component."));
                continue;
            }

            CreateExperienceButton(experience.m_Data, experience.gameObject.name);
        }
    }

    public void CreateExperienceButton(ThirdPartyExperienceData data, string experiencePrefabOrSceneName)
    {
        GameObject experienceBtnObj = Instantiate(m_ExperienceButtonPrefab, data.ExperienceType == ThirdPartyExperienceType.Tabletop ? m_BoardGameSelectionContainer : m_OtherExperienceSelectionContainer);
        ExperienceButton expBtn = experienceBtnObj.GetComponent<ExperienceButton>();
        expBtn.SetupForExperience(data);
        expBtn.m_Btn.onClick.AddListener(delegate
        {
            if (CurrentThirdPartyExperience)
                return;

            if (data.ExperienceType != ThirdPartyExperienceType.NewScene)
                InstantiateThirdPartyExperienceWithNameAndType(experiencePrefabOrSceneName, data.ExperienceType);

            if (data.ExperienceType == ThirdPartyExperienceType.Tabletop)
                AlcovePositionController.getInstance().SetUserPosition(UserPosition.ThirdPartyTable);
            else if (data.ExperienceType == ThirdPartyExperienceType.Room)
                AlcovePositionController.getInstance().SetUserPosition(UserPosition.ThirdPartyRoom);
            else if (data.ExperienceType == ThirdPartyExperienceType.NewScene)
            {
                PhotonNetwork.Disconnect();
                SceneManager.LoadScene(experiencePrefabOrSceneName);
            }
        });
    }

    public void InstantiateThirdPartyExperienceWithNameAndType(string experienceName, ThirdPartyExperienceType type)
    {
        Transform instantiationTransform = type == ThirdPartyExperienceType.Tabletop
            ? m_TableExperienceTransform
            : m_RoomExperienceTransform;

        GameObject newExperienceObj = PhotonNetwork.Instantiate(experienceName, instantiationTransform.position, instantiationTransform.rotation, 0, null);

        AlcoveThirdPartyExperience newExperience = newExperienceObj.GetComponent<AlcoveThirdPartyExperience>();
        if (newExperience == null)
            Debug.LogError("The root GameObject for an instantiated experience must be assigned the AlcoveThirdPartyExperience component.");
        else
            CurrentThirdPartyExperience = newExperience;
    }

    public void DestroyThirdPartyExperience(ThirdPartyExperienceType type)
    {
        AlcovePositionController.getInstance().ResetThirdPartySpawnPoints(CurrentThirdPartyExperience.GetComponent<AlcoveThirdPartyExperience>().m_Data.ExperienceType);
        if (CurrentThirdPartyExperience.gameObject != null)
            PhotonNetwork.Destroy(CurrentThirdPartyExperience.gameObject);
        CurrentThirdPartyExperience = null;
    }

    public static void AssignPlayerToThirdPartyExperiencePosition(PhotonPlayer player, int positionIndex)
    {
        if (!CurrentThirdPartyExperience)
            return;

        PlayersInExperience[player] = positionIndex;

        //If the experience is owned by the scene and a player has just entered the experience, set that player to be the owner.
        if (PhotonNetwork.isMasterClient && CurrentThirdPartyExperience && CurrentThirdPartyExperience.photonView.ownerId == 0)
        {
            CurrentThirdPartyExperience.photonView.TransferOwnership(player);
        }

        if (CurrentThirdPartyExperience)
            CurrentThirdPartyExperience.PlayerJoinedAtPosition(player, positionIndex);
    }

    public static void RemovePlayerFromThirdPartyExperience(PhotonPlayer player)
    {
        if (!CurrentThirdPartyExperience)
            return;

        if (PlayersInExperience.ContainsKey(player))
            PlayersInExperience.Remove(player);

        //If the player who is leaving is the owner of the experience, set the owner to be the first player who is still in the experience, or the scene itself if there is no player left.
        if (player.Equals(CurrentThirdPartyExperience.photonView.owner))
        {
            if (PlayersInExperience != null && PlayersInExperience.Count > 0)
                CurrentThirdPartyExperience.photonView.TransferOwnership(PlayersInExperience.Keys.First());
            else
                CurrentThirdPartyExperience.photonView.TransferOwnership(0);
        }

        if (CurrentThirdPartyExperience)
            CurrentThirdPartyExperience.PlayerLeft(player);
    }

    public static void ReturnToHomeEnvironment()
    {
        SceneManager.LoadScene(HomeEnvironmentSceneName);
    }

    public static void ExitExperience()
    {
        if (HomeEnvironmentSceneName != SceneManagerHelper.ActiveSceneName)
            SceneManager.LoadScene(HomeEnvironmentSceneName);
        else if (getInstance() && AlcovePositionController.getInstance())
        {
            getInstance().DestroyThirdPartyExperience(
                CurrentThirdPartyExperience.m_Data.ExperienceType);
            AlcovePositionController.getInstance().SetUserPosition(UserPosition.Entertainment);
        }
    }
}
