using UnityEngine;
using System.Collections;
using System;

public class FowardAttack : BulletClass, IpoolObject
{
    

    public override void InitializeBullet()
    {
        //delay = 99;
        timed_obj = false;
    }

    public new void Update() {
        base.Update();
    }
    public override void OnHitEntity()
    {
    }
}
