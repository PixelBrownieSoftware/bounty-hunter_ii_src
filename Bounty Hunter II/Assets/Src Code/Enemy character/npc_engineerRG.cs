using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class npc_engineerRG : o_npcharacter, IpoolObject {


    public enum AI_STATES
    {
        IDLE,
        ATTACK_PLAYER,
        BACK_AWAY,
        ROCKET_LAUNCHER,
        GERNADE
    };
    public AI_STATES ROUTINES;

    void IpoolObject.SpawnStart()
    {
        experience_to_give = 6;
        give_ammo = true;
        initialSpeed = 55;
        attack_pow = 5;
        maxHealth = 9;
        view_distance = 550;
        ROUTINES = AI_STATES.IDLE;
        Initialize();
    }

    public override void AIFunction()
    {
        switch (characterstates)
        {
            case CHARACTER_STATEMACHINE.STAND:

                CheckForTarget(false);
                SetAnimation(0);
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

                    case AI_STATES.IDLE:
                        SetAnimation(1);
                        if (target != null)
                        {
                            if (DistanceToAttack(view_distance, target, false))
                            {
                                ROUTINES = AI_STATES.ATTACK_PLAYER;
                            }
                        }
                        break;

                    case AI_STATES.ATTACK_PLAYER:

                        if (target.Health >= 0)
                        {
                            if (DistanceToAttack(view_distance, target, false))
                            {
                                direction_vec = LookAtTarget(target);
                                MoveCharacter(direction_vec);

                                //They shoot the player when they are close enough.
                                if (DistanceToAttack(view_distance / 3.8f, target, false))
                                {
                                    StopCharacter(); ROUTINES = (AI_STATES)Random.Range(3, 5);
                                }

                                //Strategically backing away so the player can't just constantly shoot at them
                                if (DistanceToAttack(view_distance / 4.2f, target, false))
                                {
                                    if (attkdelay == 0)
                                    {
                                        attkdelay = 1f;
                                        ROUTINES = AI_STATES.BACK_AWAY;
                                    }
                                }
                            }
                        }
                        else
                        {
                            //If the target is defeated/dead, no need to keep attacking them
                            ROUTINES = AI_STATES.IDLE;
                            characterstates = CHARACTER_STATEMACHINE.STAND;
                        }
                        break;

                    case AI_STATES.BACK_AWAY:

                        direction_vec = -LookAtTarget(target);
                        MoveCharacter(direction_vec);

                        if (CheckIfCornered(direction_vec))
                        {
                            ROUTINES = AI_STATES.ATTACK_PLAYER;
                        }

                        if (attkdelay == 0)
                        {
                            ROUTINES = AI_STATES.ATTACK_PLAYER;
                        }

                        break;

                    case AI_STATES.GERNADE:

                        Attack(0.95f, 2.65f);
                        break;

                    case AI_STATES.ROCKET_LAUNCHER:

                        Attack(0.95f, 2.65f);
                        break;
                }

                break;

            case CHARACTER_STATEMACHINE.ATTACKING:
                switch (ROUTINES)
                {
                    case AI_STATES.GERNADE:

                        SetAnimation(0);
                        ShootBullet("rkt", 0, 0.4f, 90);
                        break;

                    case AI_STATES.ROCKET_LAUNCHER:

                        ShootBullet("bmb", 0, 0.4f, 0);
                        break;
                }
                characterstates = CHARACTER_STATEMACHINE.ATTACK_ANIMATION;
                break;
        }
    }

    new void Update()
    {
        base.Update();
    }

    public void RetalitateGUN()
    {
        direction_vec = LookAtTarget(target);
        StopCharacter();
        Attack(0.35f, 0.65f);
    }


    public override void AfterDash()
    {

    }

    public override void AfterAttack()
    {
        characterstates = CHARACTER_STATEMACHINE.MOVING;
    }

    public override void DashFunction()
    {
    }

}
