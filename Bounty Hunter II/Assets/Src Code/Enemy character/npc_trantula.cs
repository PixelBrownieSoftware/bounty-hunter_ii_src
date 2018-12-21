using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class npc_trantula : o_npcharacter,IpoolObject {
    
    GameObject bite;
    public enum AI_STATES
    {
        IDLE,
        ATTACK,
        SILK,
        BACK_AWAY
    }
    public AI_STATES ROUTINES;

    void IpoolObject.SpawnStart()
    {
        maxHealth = 4;
        attack_pow = 1;
        initialSpeed = 130f;
        view_distance = 250f;
        bite = transform.Find("BiteAttk").gameObject;
        bite.GetComponent<BulletClass>().parent = this;
        bite.GetComponent<BulletClass>().InitializeBullet();
        Initialize();
    }

    public override void AIFunction()
    {
        switch (characterstates)
        {
            case CHARACTER_STATEMACHINE.STAND:
                CheckForTarget(false);

                if (target != null)
                    if (DistanceToAttack(view_distance, target, false))
                    {
                        ROUTINES = AI_STATES.ATTACK;
                        characterstates = CHARACTER_STATEMACHINE.MOVING;
                    }
                break;

            case CHARACTER_STATEMACHINE.MOVING:
                switch (ROUTINES)
                {
                    case AI_STATES.ATTACK:

                        direction_vec = LookAtTarget(target);
                        MoveCharacter(direction_vec);

                        if (DistanceToAttack(150, target, false))
                        {
                            attack_pow = 2;
                            StopCharacter();
                            Dash(0.15f, 0.3f, 0.2f);
                        }

                        if (DistanceToAttack(250, target, false))
                        {
                            attack_pow = 1;
                            StopCharacter();
                            Attack(0.3f, 0.8f);
                        }
                        break;

                    case AI_STATES.BACK_AWAY:
                        Speed = initialSpeed + 50;
                        direction_vec = -LookAtTarget(target);
                        MoveCharacter(direction_vec);

                        if (attkdelay == 0)
                        {
                            Speed = initialSpeed;
                            ROUTINES = AI_STATES.ATTACK;
                        }

                        break;
                }
                break;
            case CHARACTER_STATEMACHINE.ATTACKING:

                ShootBullet("bullet", 0, 0.6f);
                characterstates = CHARACTER_STATEMACHINE.ATTACK_ANIMATION;
                break;
        }
    }

    new void Update ()
    {
        base.Update();
    }

    public override void AfterAttack()
    {
        ROUTINES = AI_STATES.ATTACK;
        characterstates = CHARACTER_STATEMACHINE.MOVING;
    }

    public override void AfterDash()
    {
        bite.gameObject.SetActive(false);
    }

    public override void DashFunction()
    {
        bite.gameObject.SetActive(true);
    }
}
