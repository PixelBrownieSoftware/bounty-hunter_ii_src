using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class npc_yeti : o_npcharacter, IpoolObject
{
    public AudioClip attackyell;

    public enum AI_STATES
    {
        ATTACK_PLAYER,
        BACK_AWAY
    };
    public AI_STATES ROUTINES;

    void IpoolObject.SpawnStart()
    {
        maxHealth = 10;
        experience_to_give = 4;
        initialSpeed = 90;
        view_distance = 670;
        attack_pow = 3;
        Initialize();
        SetAttackObject();
        DisableAttack();
    }

    public override void AIFunction()
    {
        switch (characterstates)
        {
            case CHARACTER_STATEMACHINE.STAND:
                CheckForTarget(false);
                if (target != null)
                {
                    if (DistanceToAttack(view_distance, target, false))
                    {
                        characterstates = CHARACTER_STATEMACHINE.MOVING;
                    }
                }
                break;

            case CHARACTER_STATEMACHINE.MOVING:
                switch (ROUTINES)
                {
                    case AI_STATES.ATTACK_PLAYER:

                        SetAnimation(1);
                        direction_vec = LookAtTarget(target);
                        MoveCharacter(direction_vec);

                        if (DistanceToAttack(150, target, false))
                        {
                            if (dashDelay <= 0)
                            {
                                StopCharacter();
                                SetAnimation(5);
                                SoundManager.SFX.playSound(attackyell);
                                Dash(0.2f, 1.3f, 0.3f);
                            }
                        }
                        break;

                    case AI_STATES.BACK_AWAY:

                        SetAnimation(1);
                        Speed = initialSpeed + 50;
                        //tar_last_pos = target.transform.position;
                        MoveCharacter(direction_vec);

                        if (CheckIfCornered(direction_vec))
                        {
                            Speed = initialSpeed;
                            ROUTINES = AI_STATES.ATTACK_PLAYER;
                        }
                        if (!walkback)
                        {
                            ROUTINES = AI_STATES.ATTACK_PLAYER;
                        }
                        break;
                }


                break;
        }
    }

    new void Update()
    {
        base.Update();
    }

    public override void AfterAttack()
    {

    }

    public override void AfterDash()
    {
        DisableAttack();
        StopCharacter();
        SetAnimation(0);
        if (!walkback)
        {
            direction_vec = -(LookAtTarget(target) + (Vector3)new Vector2(direction_vec.x + Random.Range(-0.6f, 0.6f), direction_vec.y + Random.Range(-0.6f, 0.6f))).normalized;
            ROUTINES = AI_STATES.BACK_AWAY;
            StartCoroutine(WalkBack(0.7f));
        }
        characterstates = CHARACTER_STATEMACHINE.MOVING;
    }

    public override void DashFunction()
    {
        EnableAttack();

    }
}
