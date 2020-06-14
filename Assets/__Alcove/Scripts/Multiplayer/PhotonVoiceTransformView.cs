using UnityEngine;

public class PhotonVoiceTransformView : MonoBehaviour, IPunObservable
{
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            stream.SendNext(transform.position);
        }

        if (stream.isReading)
        {
            Vector3 newPos = (Vector3)stream.ReceiveNext();
            transform.position = newPos;
        }
    }
}