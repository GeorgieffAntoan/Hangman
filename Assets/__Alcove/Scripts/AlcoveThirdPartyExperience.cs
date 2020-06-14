using System;
using System.Collections.Generic;
using System.Linq;
using Photon;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(PhotonView))]
public class AlcoveThirdPartyExperience : PunBehaviour
{
    public ThirdPartyExperienceData m_Data;

    //This array can be used to specify a custom set of positions for users in your experience. Experiences must have set locations for exactly four players. Players can be spectators, but must still be positioned correctly.
    public GameObject[] m_UserPositions;

    public UnityAction<PhotonPlayer, int> m_OnPlayerJoinedAtPosition;
    public UnityAction<PhotonPlayer> m_OnPlayerLeft;

    // Start is called before the first frame update
    void Awake()
    {
        if (AlcoveThirdPartyExperienceController.CurrentThirdPartyExperience && PhotonNetwork.isMasterClient)
            PhotonNetwork.Destroy(AlcoveThirdPartyExperienceController.CurrentThirdPartyExperience.photonView);

        AlcoveThirdPartyExperienceController.CurrentThirdPartyExperience = this;

        if (m_UserPositions != null && m_UserPositions.Length == 4)
            AlcovePositionController.getInstance().SetThirdPartySpawnPoints(m_UserPositions, m_Data.ExperienceType);
        else
            AlcovePositionController.getInstance().ResetThirdPartySpawnPoints(m_Data.ExperienceType);
    }

    void OnDestroy()
    {
        if (AlcovePositionController.getInstance())
            AlcovePositionController.getInstance().ResetThirdPartySpawnPoints(m_Data.ExperienceType);
    }

    public void PlayerJoinedAtPosition(PhotonPlayer player, int position)
    {
        m_OnPlayerJoinedAtPosition?.Invoke(player,position);
    }

    public void PlayerLeft(PhotonPlayer player)
    {
        m_OnPlayerLeft?.Invoke(player);
    }
}
