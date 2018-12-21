using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class npc_wasp : o_npcharacter, IpoolObject {
    

    GameObject bite;
    public enum AI_STATES
    {
        IDLE,
        ATTACK,
        BACK_AWAY
    }
    public AI_STATES ROUTINES;

    void IpoolObject.SpawnStart()
    {
        maxHealth = 2;
        attack_pow = 1;
        experience_to_give = 1;
        view_distance = 300f;
        initialSpeed = 230f;
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
                {
                    if (DistanceToAttack(view_distance, target, false))
                    {
                        ROUTINES = AI_STATES.ATTACK;
                        characterstates = CHARACTER_STATEMACHINE.MOVING;
                    }
                }
                break;

            case CHARACTER_STATEMACHINE.MOVING:

                switch (ROUTINES)
                {
                    case AI_STATES.ATTACK:

                        direction_vec = LookAtTarget(target);
                        MoveCharacter(direction_vec);


                        if (target.characterstates ==  CHARACTER_STATEMACHINE.DEAD)
                        {
                            characterstates = CHARACTER_STATEMACHINE.STAND;
                        }

                        if (DistanceToAttack(20, target, false))
                        {
                            Attack(0.3f, 0.6f);
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

                bite.SetActive(true);
                characterstates = CHARACTER_STATEMACHINE.ATTACK_ANIMATION;
                break;
        }
    }

    new void Update()
    {
        base.Update();
    }

    public override void AfterAttack()
    {
        bite.SetActive(false);
        ROUTINES = AI_STATES.BACK_AWAY;
        characterstates = CHARACTER_STATEMACHINE.MOVING;
    }

    public override void AfterDash()
    {
    }

    public override void DashFunction()
    {
    }
}
