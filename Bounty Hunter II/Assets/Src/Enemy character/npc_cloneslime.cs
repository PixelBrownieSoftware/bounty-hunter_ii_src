using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class npc_cloneslime : o_npcharacter, IpoolObject
{
    public npc_slime parent_slime;

    public enum AI_STATES
    {
        ATTACK_PLAYER,
        BACK_AWAY
    };
    public AI_STATES ROUTINES;

    void IpoolObject.SpawnStart()
    {
        hurtdone = true;
        give_exp = false;
        Invinciblity = false;
        maxHealth = 1f;
        attack_pow = 0;
        initialSpeed = 190;
        view_distance = 450;
        experience_to_give = 0;
        Initialize();
    }

    public override void AIFunction()
    {
        if (cur_faction == "")
        {
            //cur_faction = parent_slime.cur_faction;
            //AddTargets(s_levelmanager.enemies_in_level);
        }

        switch (characterstates)
        {
            case CHARACTER_STATEMACHINE.STAND:
                CheckForTarget(false);
                if (target != null)
                {
                    if (DistanceToAttack(view_distance, target, false))
                    {
                        ROUTINES = AI_STATES.ATTACK_PLAYER;
                        characterstates = CHARACTER_STATEMACHINE.MOVING;
                    }
                }
                break;

            case CHARACTER_STATEMACHINE.MOVING:
                switch (ROUTINES)
                {
                    case AI_STATES.ATTACK_PLAYER:
                        if (target.characterstates != CHARACTER_STATEMACHINE.DEAD)
                        {
                            direction_vec = LookAtTarget(target);
                            MoveCharacter(direction_vec);

                            if (DistanceToAttack(45, target, false))
                            {
                                if (attkdelay == 0)
                                {
                                    Vector2 vec = -LookAtTarget(target) + (Vector3)new Vector2(direction_vec.x + Random.Range(-0.6f, 0.6f), direction_vec.y + Random.Range(-0.6f, 0.6f));
                                    direction_vec = vec.normalized;

                                    attkdelay = 0.45f;
                                    ROUTINES = AI_STATES.BACK_AWAY;
                                }
                            }
                        }
                        break;

                    case AI_STATES.BACK_AWAY:

                        Speed = initialSpeed + 50;
                        tar_last_pos = target.transform.position;
                        MoveCharacter(direction_vec);

                        if (attkdelay == 0)
                        {
                            Speed = initialSpeed;
                            ROUTINES = AI_STATES.ATTACK_PLAYER;
                        }
                        break;
                }


                break;
        }
    }

    new void Update ()
    {
        base.Update();


    }

    public override void AfterAttack()
    {
    }

    public override void AfterDash()
    {
    }

    public override void DashFunction()
    {
    }
}
