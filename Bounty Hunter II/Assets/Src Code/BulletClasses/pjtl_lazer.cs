using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pjtl_lazer : BulletClass, IpoolObject
{
    bool lazerdest;
	new void Update ()
    {
        base.Update();
    }

    public override void InitializeBullet()
    {
        sprite.color = Color.white;
        hit_collision.enabled = false;
        timed_obj = true;
        delay = 0.5f;
        lazerdest = false;
        speed = 0;
        attackPower = 2;
        SetShootAngle();
    }

    IEnumerator Lazer_activate()
    {
        print("Boom");
        AudioClip cli = SoundManager.SFX.LoadAudio("shoot_lazer");
        SoundManager.SFX.playSound(cli);
        sprite.color = Color.blue;
        hit_collision.enabled = true;
        yield return new WaitForSeconds(0.1f);

        DestoryBullet();
    }
    public override void OnBulletDestroy()
    {
        if (!lazerdest)
        {
            StartCoroutine(Lazer_activate());
            lazerdest = true;
        }
    }

    public override void OnHitEntity()
    {
    }
}
