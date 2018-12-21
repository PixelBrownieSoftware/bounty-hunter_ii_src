using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class npc_airshipcannon : o_npcharacter, IpoolObject
{

    void IpoolObject.SpawnStart()
    {
        maxHealth = 1;
        is_arial = true;
        constant_regen = true;
        attack_pow = 1;
        view_distance = 900f;
        initialSpeed = 0f;
        Initialize();
        Invinciblity = true;

    }

    public override void AfterAttack()
    {

        characterstates = CHARACTER_STATEMACHINE.STAND;
    }
    public override void AfterDash()
    {

    }
    public override void DashFunction()
    {

    }

    new void Update()
    {
        base.Update();
        switch (characterstates)
        {
            case CHARACTER_STATEMACHINE.STAND:
                CheckForTarget(false);
                if (target != null)
                    if (DistanceToAttack(view_distance, target, false))
                        if (target != leader_obj)
                            Attack(0.7f, 1.5f);
                break;

            case CHARACTER_STATEMACHINE.ATTACKING:
                StartCoroutine(MachineGun());
                characterstates = CHARACTER_STATEMACHINE.ATTACK_ANIMATION;
                break;
        }
    }

    IEnumerator MachineGun()
    {
        for (int i = 0; i < 5; i++)
        {
            PlayEffect("shoot_spark");
            ShootBullet("bullet", UnityEngine.Random.Range(-3, 3), 0.5f, 50f).GetComponent<Bullet>().go_through = true;
            yield return new WaitForSeconds(0.08f);
        }
    }
}
