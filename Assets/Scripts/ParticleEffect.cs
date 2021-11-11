using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleEffect : MonoBehaviour
{
    public float duration;

    // Start is called before the first frame update
    void Start()
    {
        Invoke("destroy", duration);
    }

    void destroy()
    {
        Destroy(gameObject);
    }
}
