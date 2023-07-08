using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputEvents : MonoBehaviour
{
    public void OnDeviceLost()
    {
        Destroy(gameObject);
    }
}
