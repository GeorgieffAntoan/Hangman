using UnityEngine;

public class CustomReturnToAlcove : MonoBehaviour
{
    void Awake()
    {
        ReturnToAlcove.ReturnReturnAutomatically = false;
    }

    void OnApplicationQuit()
    {
        //Do any necessary cleanup or other work here

        ReturnToAlcove.ReturnNow(); //This line will exit and launch Alcove

        //You can also send a string back to Alcove, or send a multiplayer room id
        ReturnToAlcove.ReturnNow(extraData: "customDataString", multiplayerRoomId: "154652343");
    }
}