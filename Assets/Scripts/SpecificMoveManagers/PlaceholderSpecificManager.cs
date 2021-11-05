using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceholderSpecificManager : SpecificMoveManager
{
    public Transform bulletLocation;
    public GameObject bullet_prefab;

    private void Start()
    {
        moveController = GetComponentInParent<MyCharacterController>();
    }

    public override void DoCoolStuff(int i)
    {
        switch(i)
        {
            case 1:
                Gun1();
                break;
        }
    }

    public void Gun1()
    {



    }
}
