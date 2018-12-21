using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class o_afterimage : MonoBehaviour, IpoolObject {

    public SpriteRenderer rend { get; set; }
    //Color clr = new Color(1, 1, 1, 0);  //Unity's clear colour makes it all black and ugly
    //float timer = 0;
    Animator anim;
    public AnimationClip cli;

    void IpoolObject.SpawnStart()
    {
        if(rend == null)
            rend = GetComponent<SpriteRenderer>();

        if(anim == null)
            anim = GetComponent<Animator>();

        anim.Play(cli.name);
        gameObject.SetActive(true);
    }

    void Deactivate()
    {
        gameObject.SetActive(false);
    }
    
}
