using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class p_kazzi : o_plcharacter,IpoolObject {

    /* 
      Real name: Kazzi Chuck
      Born: June 3rd 1992
    */

    ObjectPooler obj;
    public AudioClip fire;


    public IEnumerator TripShoot()
    {
        ShootBullet("bullet", 0, 0.5f);
        SoundManager.SFX.playSound(fire);
        yield return new WaitForSeconds(0.08f);
        ShootBullet("bullet", 0, 0.5f);
        SoundManager.SFX.playSound(fire);
        yield return new WaitForSeconds(0.08f);
        ShootBullet("bullet", 0, 0.5f);
        SoundManager.SFX.playSound(fire);
        yield return new WaitForSeconds(0.08f);
    }
    public p_peast peast;

    void IpoolObject.SpawnStart()
    {
        destroyOnDeath = false;
        maxHealth = 15;
        initialSpeed = 165f;

        hurt_del = 0.95f;

        Initialize();

        WeaponBase shockgun = new WeaponBase("Triple Usp", "bullet", null, 0.5f, 0.7f, 0, null,1);
        weapons.Add(shockgun);

    }

    new void Update()
    {
        base.Update();
    }

    public override void DashFunction()
    {
    }


    public override void FireGun()
    {
        if (selectedWeapon.name == "Triple Usp")
        {

            SoundManager.SFX.playSound(fire);
            StartCoroutine(TripShoot());
            cameraStuff.cameraShake(1.1f, 1.1f, 0.75f);
        }
    }
    

}
