using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPhysicalInteractable
{
    void OnLook();
    void OnLookAway();
    void OnPoint();
    void OnReturnToNormal();
}
