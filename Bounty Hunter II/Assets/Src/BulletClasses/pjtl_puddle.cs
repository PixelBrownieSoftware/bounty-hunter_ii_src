using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pjtl_puddle : BulletClass {

    public override void InitializeBullet()
    {
        timed_obj = true;
        delay = 3.4f;
        attackPower = 1;
    }

    new void Update () {
        base.Update();
	}

    public override void OnHitEntity()
    {
    }
}
