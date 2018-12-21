using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class npc_gunnerfrog : o_npcharacter,IpoolObject
{

    public enum AI_STATES
    {
        IDLE,
        ATTACK_PLAYER,
        BACK_AWAY
    };
    public AI_STATES ROUTINES;
    public AudioClip shoot;

    void IpoolObject.SpawnStart()
    {
        experience_to_give = 2;
        initialSpeed = 140;
        attack_pow = 1;
        maxHealth = 4;
        give_ammo = true;
        view_distance = 670;
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
                        SetAnimation(0);
                        if (target != null)
                        {
                            if (DistanceToAttack(view_distance, target, false))
                            {
                                ROUTINES = AI_STATES.ATTACK_PLAYER;
                            }
                        }
                        break;

                    case AI_STATES.ATTACK_PLAYER:
                        if (target.characterstates != CHARACTER_STATEMACHINE.DEAD)
                        {
                            SetAnimation(1);
                            if (DistanceToAttack(view_distance, target, false))
                            {

                                direction_vec = LookAtTarget(target);
                                MoveCharacter(direction_vec);

                                //They shoot the player when they are close enough.
                                if (DistanceToAttack(view_distance / 4.2f, target, false))
                                {
                                    if (attkdelay == 0)
                                    {
                                        attkdelay = 0.7f;
                                        Vector2 vec = -LookAtTarget(target) + (Vector3)new Vector2(direction_vec.x + Random.Range(-0.6f, 0.6f), direction_vec.y + Random.Range(-0.6f, 0.6f));
                                        direction_vec = vec.normalized;

                                        SetAnimation(1);
                                        Speed = initialSpeed + 50;
                                        ROUTINES = AI_STATES.BACK_AWAY;
                                    }
                                }

                                //Strategically backing away so the player can't just constantly shoot at them
                                if (DistanceToAttack(view_distance / 3.8f, target, false))
                                {
                                    SetAnimation(0);
                                    StopCharacter();
                                    Attack(0.95f, 0.55f);
                                }
                            }
                        }
                        else
                        {
                            //If the target is defeated/dead, no need to keep attacking them
                            target = null;
                            ROUTINES = AI_STATES.IDLE;
                            characterstates = CHARACTER_STATEMACHINE.STAND;
                        }
                        break;

                    case AI_STATES.BACK_AWAY:

                        SetAnimation(1);
                        MoveCharacter(direction_vec);

                        if (CheckIfCornered(direction_vec))
                        {
                            Speed = initialSpeed;
                            ROUTINES = AI_STATES.ATTACK_PLAYER;
                        }

                        if (attkdelay == 0)
                        {
                            Speed = initialSpeed;
                            ROUTINES = AI_STATES.ATTACK_PLAYER;
                        }
                        break;
                }

                break;

            case CHARACTER_STATEMACHINE.ATTACKING:

                PlayEffect("shoot_spark");
                SetAnimation(5);
                SoundManager.SFX.playSound(shoot);
                ShootBullet("bullet", 0, 0.4f, 50);
                characterstates = CHARACTER_STATEMACHINE.ATTACK_ANIMATION;
                break;
        }

    }

    new void Update()
    {
        base.Update();
    }
    

    public override void AfterDash() {

    }

    public override void AfterAttack()
    {
        characterstates = CHARACTER_STATEMACHINE.MOVING;
    }

    public override void DashFunction() {
    }

}