using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pjtl_rocket : BulletClass {

    bool exploded;
    public AudioClip sound;

    public override void InitializeBullet()
    {
        sprite.sprite = bullet;
        timed_obj = true;
        StartCoroutine(muzzle(0.06f));
        rbody.velocity = shoot_direction.normalized * speed * 14 * Time.deltaTime;
        SetShootAngleChild();
        hit_collision.size = new Vector2(25,25);

        exploded = false;
        attackPower = 6;
    }

    new void Update()
    {
        base.Update();
        if (isStarted)
            transform.Translate(rbody.velocity);
    }

    public override void OnBulletDestroy()
    {
        if (!exploded)
        {
            sprite.sprite = muzz;
            hit_collision.enabled = true;
            AudioClip cli = SoundManager.SFX.LoadAudio("shootfx");
            SoundManager.SFX.playSound(cli);
            ParticleSystem part = ObjectPooler.instance.SpawnObject("explosion_particle", transform.position, Quaternion.identity, true).GetComponent<ParticleSystem>();
            part.Play();

            StartCoroutine(Explosion());
            exploded = true;
        }
    }
    
    IEnumerator Explosion()
    {
        rbody.velocity = Vector2.zero;
        hit_collision.size *= 5;
        yield return new WaitForSeconds(0.2f);
        DestoryBullet();
    }

    public override void OnHitEntity()
    {
        OnBulletDestroy();
    }
}
