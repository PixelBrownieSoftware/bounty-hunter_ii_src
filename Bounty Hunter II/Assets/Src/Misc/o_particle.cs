using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class o_particle : MonoBehaviour {
    
    ParticleSystem part;

    void Start () {
        part = GetComponent<ParticleSystem>();
    }

    public void Disable()
    {
        gameObject.SetActive(false);
    }

    void Update () {

        if (part != null)
        {
            if (part.isStopped)
            {
                gameObject.SetActive(false);
            }
        }
	}
}
