using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class p_peast : o_plcharacter, IpoolObject {

    /*
      Real name: Alan Woodford
      Born: August 16th 1994
    */

    public AudioClip fire;
    public AudioClip fireshotgun;
    public AudioClip firemachinegun;

    void IpoolObject.SpawnStart()
    {
        destroyOnDeath = false;
        initialSpeed = 150f;
        hurt_del = 0.95f;
        destroyOnDeath = false;


        InitializeNoHealthChange();
        hurt_sound = new AudioClip[3];

        hurt_sound[0] = GetSoundEffect("peast_hurt_1ech");
        hurt_sound[1] = GetSoundEffect("peast_hurt_2ech");
        hurt_sound[2] = GetSoundEffect("peast_hurt_3ech");

        hurt = GetSoundEffect("impact_damage2");
        if (!gui_main_menu.isLoadgame && weapons.Count == 0)
        {
            weapons.Clear();
            WeaponBase peace_maker = new WeaponBase("Pistol", 
                "bullet", 
                new int[] { 0, 25, 295, 365 },
                0.14f,
                0.03f, 
                0 ,
                new float[] { 1, 1, 2, 3 },0);
            peace_maker.attackPow = 1;
            weapons.Add(peace_maker);
        }
    }

    new void Update() {
        base.Update();
    }

    public override void DashFunction()
    {
        SoundManager.SFX.playSound(dash);
    }

    public override void AfterDash()
    {
        characterstates = CHARACTER_STATEMACHINE.STAND;
    }

    public override void FireGun()
    {
        if (selectedWeapon.name == "Pistol")
        {
            SetAnimation(5);
            SoundManager.SFX.playSound(fire);
            PlayEffect("shoot_spark");
            ShootBullet(selectedWeapon.bulletName, Random.Range(-2, 2), 0.4f, 50);


            // ShootBullet(selectedWeapon.bulletName, Random.Range(-2, 2), 0.4f, 50);
        }

        if (selectedWeapon.name == "Machine Gun")
        {
            cameraScript.camOptions.cameraShake(4, 4, 0.3f);
            selectedWeapon.AmmoUse();
            SetAnimation(9);

            SoundManager.SFX.playSound(firemachinegun);
            PlayEffect("shoot_spark");
            ShootBullet(selectedWeapon.bulletName, Random.Range(-5, 5), 0.3f, 100);
        }

        if (selectedWeapon.name == "Shotgun") {
            attkdelay = 0.53f;
            SoundManager.SFX.playSound(fireshotgun);
            SetAnimation(8);
            PlayEffect("shoot_spark");
            cameraScript.camOptions.cameraShake(10, 10, 0.3f);
            selectedWeapon.AmmoUse();
            ShootBullet(selectedWeapon.bulletName, -15f, 0.45f, 70, true);
            ShootBullet(selectedWeapon.bulletName, 15f, 0.45f, 70,true);
            ShootBullet(selectedWeapon.bulletName, 0, 0.45f, 70, true);
        }
        /*
        if (selectedWeapon.name == "Boomerang")
        {
            attkdelay = 0.04f;
            SoundManager.SFX.playSound(fireshotgun);
            selectedWeapon.AmmoUse();
            ShootBullet(selectedWeapon.bulletName, 0, 0.45f, 80);
        }*/
    }


}
