using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class npc_arrowshooter : o_npcharacter, IpoolObject {

    void IpoolObject.SpawnStart()
    {
        maxHealth = 5;
        is_arial = true;
        constant_regen = true;
        attack_pow = 1;
        view_distance = 900f;
        initialSpeed = 0f;
        Initialize();

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

    new void Update ()
    {
        base.Update();
        switch (characterstates)
        {
            case CHARACTER_STATEMACHINE.STAND:
                CheckForTarget(false);
                if (target != null)
                    if (DistanceToAttack(view_distance, target, false))
                        if (target != leader_obj)
                            Attack(0.3f, 0.7f);
                break;

            case CHARACTER_STATEMACHINE.ATTACKING:
                ShootBullet("bullet", UnityEngine.Random.Range(-3, 3), 0.8f);
                characterstates = CHARACTER_STATEMACHINE.ATTACK_ANIMATION;
                break;
        }
    }
}
