using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pjtl_gernade : BulletClass, IpoolObject
{
    bool exploded;
    public Sprite norm;
    public Sprite glow;
    public AudioClip tick;

    new public void Update()
    {
        base.Update();
    }

    public override void InitializeBullet()
    {
        sprite.sprite = bullet;
        hit_collision.enabled = false;
        timed_obj = true;
        delay = 0.1f;
        speed = 0f;

        exploded = false;
        attackPower = 6;
    }

    public override void OnBulletDestroy()
    {
        if (!exploded) {

            StartCoroutine(Explosion());
            exploded = true;
        }
    }

    void SetAnim(int i)
    {
        switch (i)
        {
            case 0:
                sprite.sprite = norm;
                break;

            case 1:
                SoundManager.SFX.playSound(tick);
                sprite.sprite = glow;
                break;
        }
    }

    IEnumerator Explosion()
    {
        for (int i = 0; i < 5; i++)
        {
            SetAnim(0);
            yield return new WaitForSeconds(0.2f);
            SetAnim(1);
            yield return new WaitForSeconds(0.1f);
        }
        for (int i = 0; i < 10; i++)
        {
            SetAnim(0);
            yield return new WaitForSeconds(0.1f);
            SetAnim(1);
            yield return new WaitForSeconds(0.05f);
        }
        for (int i = 0; i < 25; i++)
        {
            SetAnim(0);
            yield return new WaitForSeconds(0.05f);
            SetAnim(1);
            yield return new WaitForSeconds(0.01f);
        }

        SetAnim(0);
        hit_collision.enabled = true;

        ParticleSystem part = ObjectPooler.instance.SpawnObject("explosion_particle", transform.position, Quaternion.identity, true).GetComponent<ParticleSystem>();
        part.Play();
        sprite.sprite = null;
        AudioClip cli = SoundManager.SFX.LoadAudio("explode_sound");
        SoundManager.SFX.playSound(cli);
        yield return new WaitForSeconds(0.7f);
        DestoryBullet();
    }

    public override void OnHitEntity()
    {
    }
}
