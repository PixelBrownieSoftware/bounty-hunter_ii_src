using UnityEngine;
using System;

public class npc_snanake : o_npcharacter, IpoolObject
{
    GameObject bite;

    public enum states
    {
        IDLE,
        BACK_AWAY,
        SEARCHFORTARGET,
        ATTACK
    };
    public states state;

    void IpoolObject.SpawnStart()
    {
        state = states.IDLE;
        attack_pow = 1;
        maxHealth = 3;
        initialSpeed = 165;
        experience_to_give = 1;
        SetAttackObject("BiteAttk");
        Initialize();
    }

    public override void AfterDash() {
        attack.SetActive(false);
        characterstates = CHARACTER_STATEMACHINE.STAND;
    }

    public override void DashFunction()
    {
        SetAnimation(3);
    }

    public override void AIFunction()
    {
        if (attack.activeSelf == true)
        {
            attack.transform.rotation = Quaternion.Euler(0, 0, charDirection + 90);
        }

        switch (characterstates)
        {

            case CHARACTER_STATEMACHINE.STAND:
                SetAnimation(0);
                CheckForTarget(false);
                if (target != null)
                {
                    if (DistanceToAttack(view_distance, target, false))
                    {
                        state = states.ATTACK;
                        characterstates = CHARACTER_STATEMACHINE.MOVING;
                    }
                }
                break;

            case CHARACTER_STATEMACHINE.MOVING:
                SetAnimation(1);
                switch (state)
                {
                    case states.IDLE:

                        if (target.characterstates == CHARACTER_STATEMACHINE.DEAD)
                        {
                            characterstates = CHARACTER_STATEMACHINE.STAND;
                        }
                        Walking(); CheckForTarget(false);
                        if (!DistanceToAttack(view_distance, target, false))
                        {
                            target = null;
                            characterstates = CHARACTER_STATEMACHINE.STAND;
                        }

                        if (DistanceToAttack(view_distance, target, false))
                        {
                            if (CheckForChasm(direction_vec) == false)
                                state = states.ATTACK;
                        }
                        break;

                    case states.ATTACK:
                        if (target.characterstates != CHARACTER_STATEMACHINE.DEAD)
                        {
                            tar_last_pos = target.transform.position;

                            direction_vec = LookAtTarget(target);
                            MoveCharacter(direction_vec);

                            if (CheckForChasm(direction_vec * 94))
                            {
                                if (attkdelay == 0)
                                {
                                    attkdelay = 1.27f;
                                    state = states.BACK_AWAY;
                                }
                            }

                            if (DistanceToAttack(150, target, false))
                            {
                                StopCharacter();
                                Dash(0.35f, 1.3f, 0.45f);
                            }

                            if (DistanceToAttack(120, target, false))
                            {
                                if (attkdelay == 0)
                                {

                                    attkdelay = 0.85f;
                                    state = states.BACK_AWAY;
                                }
                            }
                        }
                        else
                        {

                            //If the target is defeated/dead, no need to keep attacking them
                            state = states.IDLE;
                            characterstates = CHARACTER_STATEMACHINE.STAND;
                        }
                        break;

                    case states.BACK_AWAY:

                        Speed = initialSpeed + 50;
                        tar_last_pos = target.transform.position;
                        direction_vec = -LookAtTarget(target);
                        MoveCharacter(direction_vec);

                        if (CheckIfCornered(direction_vec))
                        {
                            Speed = initialSpeed;
                            state = states.ATTACK;
                        }

                        if (attkdelay == 0)
                        {
                            Speed = initialSpeed;
                            state = states.ATTACK;
                        }

                        break;
                        
                }
                break;

            case CHARACTER_STATEMACHINE.DASH_DELAY:
                attack.SetActive(true);
                break;
        }
    }

    public override void AfterAttack()
    {
    }

    new void Update()
    {
        view_distance = 460;
        base.Update();

        
    }

    void Sighted() {
        target = null;
        state = states.IDLE; characterstates = CHARACTER_STATEMACHINE.STAND;
    }

}