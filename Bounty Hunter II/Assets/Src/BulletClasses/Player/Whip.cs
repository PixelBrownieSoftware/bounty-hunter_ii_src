using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Whip : BulletClass {

	new void Start () {

        playerBullet = true;
		parent = GameObject.Find ("Uberdry").GetComponent<o_character>();
		transform.SetParent (parent.transform);
		delay = 0.12f;
		attackPower = 5;
	}
	
	new void Update () {
		base.Update ();
	}

    public override void InitializeBullet()
    {
        throw new NotImplementedException();
    }

    public override void OnHitEntity()
    {
        DestoryBullet();
    }
}
