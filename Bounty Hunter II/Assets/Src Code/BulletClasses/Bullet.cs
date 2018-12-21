using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : BulletClass
{
    public AudioClip sound;

    public override void InitializeBullet()
    {
        timed_obj = true;
        playerBullet = true;
        StartCoroutine(muzzle(0.06f));
        rbody.velocity = shoot_direction.normalized * speed * 14 * Time.deltaTime;
        SetShootAngleChild();
    }

    new void Update ()
    {
        base.Update();

        if (isStarted)
            transform.Translate(rbody.velocity);
    }

    protected void BulletCollide()
    {
        o_particle par = ObjectPooler.instance.SpawnObject("bullet_destroy", transform.position, true).GetComponent<o_particle>();
        Animator part = par.GetComponent<Animator>();
        part.Play("bullet_destory");
        SoundManager.SFX.playSound(sound);
    }
    public override void OnBulletDestroy()
    {
        BulletCollide();
        DestoryBullet();
    }

    public override void OnHitEntity()
    {
        if (!hit_multp_enemies)
        {
            BulletCollide();
            DestoryBullet();
        }
    }
}
