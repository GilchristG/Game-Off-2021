using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitboxManager : MonoBehaviour
{
    public BasicMoveTester parentController;

    public void SetParent(BasicMoveTester bmt)
    {
        parentController = bmt;
    }

    public void ProcessHit(Move incomingMove, Vector3 pointOfContact)
    {
        parentController.OnContact(incomingMove, pointOfContact);
    }

}
