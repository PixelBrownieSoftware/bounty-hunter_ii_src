using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class npc_slime : o_npcharacter, IpoolObject
{
    GameObject bite;
    int slimecount = 0;
    const int maxslime = 3;

    public enum AI_STATES
    {
        IDLE,
        ATTACK,
        MULTIPLY
    }
    public AI_STATES ROUTINES;

    void IpoolObject.SpawnStart()
    {
        slimecount = 0;
        maxHealth = 3;
        attack_pow = 1;
        view_distance = 450;
        initialSpeed = 150;
        experience_to_give = 2;
        SetAttackObject("BiteAttk");
        Initialize();

        en_phase.Add(new s_enemyphase(0.7f));
        en_phase.Add(new s_enemyphase(1f));
        
    }

    public override void AIFunction()
    {
        //if (bite.activeSelf == true)
         //   bite.transform.rotation = Quaternion.Euler(0, 0, charDirection + 90);


        switch (characterstates)
        {
            case CHARACTER_STATEMACHINE.STAND:
                StopCharacter();
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
                        if (target.characterstates == CHARACTER_STATEMACHINE.DEAD)
                        {
                            characterstates = CHARACTER_STATEMACHINE.STAND;
                            ROUTINES = AI_STATES.IDLE;
                        }
                        SetAnimation(0);
                        if (DistanceToAttack(view_distance, target, false))
                        {
                            direction_vec = LookAtTarget(target);
                            MoveCharacter(direction_vec);

                            if (current_health_phase == 1)
                            {
                                ChooseAttack(UnityEngine.Random.Range(0, 2));
                            }
                            else
                            {
                                ChooseAttack(0);
                            }
                            

                        }
                        break;

                    case AI_STATES.IDLE:

                        StopCharacter();
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

                    case AI_STATES.MULTIPLY:
                        GameObject objtospawn = ObjectPooler.instance.SpawnObject("Small Slime", transform.position, Quaternion.identity, true);
                        objtospawn.GetComponent<o_npcharacter>().target_characters = target_characters;
                        objtospawn.GetComponent<o_npcharacter>().cur_faction = cur_faction;
                        objtospawn.transform.SetParent(GameObject.Find("Entities").transform);
                        Attack(0.5f, 0.46f);
                        break;


                }
                break;

            case CHARACTER_STATEMACHINE.ATTACKING:
                
                characterstates = CHARACTER_STATEMACHINE.ATTACK_ANIMATION;
                break;
                

        }
    }

    new void Update()
    {
        base.Update();

    }

    public override void ChooseAttack(int attack_enum)
    {
        base.ChooseAttack(attack_enum);
        switch (attack_enum)
        {
            case 0:
                if (DistanceToAttack(view_distance / 4, target, false))
                {
                    StopCharacter();
                    SetAnimation(3);
                    Dash(0.15f, 0.58f, 0.35f);
                }
                else
                    return;

                break;

            case 1:

                if (slimecount < maxslime)
                {
                    StopCharacter();
                    ROUTINES = AI_STATES.MULTIPLY;
                    slimecount++;
                }
                else
                    return;

                break;

        }
    }


    public override void AfterAttack()
    {
        ROUTINES = AI_STATES.ATTACK;
        characterstates = CHARACTER_STATEMACHINE.MOVING;
    }

    public override void AfterDash()
    {
        DisableAttack();
        ROUTINES = AI_STATES.ATTACK;
        characterstates = CHARACTER_STATEMACHINE.MOVING;
    }

    public override void DashFunction()
    {
        EnableAttack();
    }
}
