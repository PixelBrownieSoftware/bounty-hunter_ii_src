using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class npc_assasin : o_npcharacter, IpoolObject
{
    GameObject slash;
    public enum AI_STATES
    {
        SLASH,
        SECOND_SLASH,
        IDLE,
        SHRUKIEN,
        ATTACK_PLAYER,
        JUMP_BACK,
        WALK_BACK
    };
    public AI_STATES ROUTINES;

    void IpoolObject.SpawnStart()
    {
        view_distance = 300;
        attack_pow = 1;
        experience_to_give = 4;
        initialSpeed = 165f;
        maxHealth = 7;

        hurt_sound = new AudioClip[2];
        hurt_sound[0] = SoundManager.SFX.LoadAudio("assasin_hurt_1");
        hurt_sound[1] = SoundManager.SFX.LoadAudio("assasin_hurt_2");
        
        slash = transform.Find("Attack").gameObject;
        Initialize();
        ROUTINES = AI_STATES.IDLE;

        en_phase.Add(new s_enemyphase(1f));
        en_phase.Add(new s_enemyphase(0.5f));

        slash.GetComponent<BulletClass>().parent = this;
        slash.GetComponent<BulletClass>().InitializeBullet();
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
                    case AI_STATES.ATTACK_PLAYER:
                        /*
                        The main reason they look at the target's hitbox rather than the target itself is so that they don't
                        just stand there, allowing the player to freely attack
                         */
                        direction_vec = LookAtTarget(target);
                        SetAnimation(1);
                        MoveCharacter(direction_vec);

                        if (!DistanceToAttack(view_distance, target, false))
                        {
                            characterstates = CHARACTER_STATEMACHINE.STAND;
                        }

                        if (DistanceToAttack(60, target, false))
                        {
                            if (attkdelay == 0)
                            {
                                WalkBack();
                            }
                        }

                        if (DistanceToAttack(view_distance / 4.5f, target, false))
                        {
                            StopCharacter();
                            SetAnimation(0);
                            Dash(0.26f, 0.95f, 0.3f);
                        }
                        break;

                    case AI_STATES.JUMP_BACK:
                    case AI_STATES.SLASH:
                        break;

                    case AI_STATES.SECOND_SLASH:
                        StopCharacter();
                        direction_vec = LookAtTarget(target);
                        Dash(0.22f, 1.2f, 0.05f);

                        break;

                    case AI_STATES.WALK_BACK:
                        SetAnimation(1);
                        MoveCharacter(direction_vec);

                        if (CheckIfCornered(direction_vec))
                        {
                            StopCharacter();
                            SetAnimation(0);
                            Speed = initialSpeed;
                            ROUTINES = AI_STATES.ATTACK_PLAYER;
                        }

                        if (attkdelay == 0)
                        {
                            StopCharacter();
                            SetAnimation(0);
                            Speed = initialSpeed;
                            ROUTINES = AI_STATES.ATTACK_PLAYER;
                        }
                        break;

                }
                break;

            case CHARACTER_STATEMACHINE.ATTACKING:

                SetAnimation(5);
                PlayEffect("shoot_spark");
                ShootBullet("bullet", 0, 0.5f);
                characterstates = CHARACTER_STATEMACHINE.ATTACK_ANIMATION;
                break;
        }
    }

    public new void Update()
    {
        base.Update();
    }

    void WalkBack()
    {
        Vector2 vec = -LookAtTarget(target) + (Vector3)new Vector2(direction_vec.x + Random.Range(-0.6f, 0.6f), direction_vec.y + Random.Range(-0.6f, 0.6f));
        direction_vec = vec.normalized;
        attkdelay = 0.7f;
        ROUTINES = AI_STATES.WALK_BACK;
    }

    public override void ChooseAttack(int attack_enum)
    {
        StopCharacter();
        direction_vec = LookAtTarget(target);
        /*
        switch (attack_enum)
        {
            case 0:
                break;

            case 1:
                //THROW SHRUIKEN
                
                ROUTINES = AI_STATES.ATTACK_PLAYER;
                switch (current_health_phase)
                {
                    case 0:
                        StopCharacter();
                        SetAnimation(0);
                        SetAnimation(5);
                        Attack(0.2f, 0.4f);
                        break;

                    case 1:
                        if (CheckForChasm(direction_vec * 3) == true)
                            return;
                        StopCharacter();
                        ROUTINES = AI_STATES.JUMP_BACK;
                        Attack(0.2f, 0.15f);
                        break;
                }

        break;
        }
        */
    }

    void DoubleSlash()
    {
        characterstates = CHARACTER_STATEMACHINE.MOVING;
        ROUTINES = AI_STATES.SECOND_SLASH;
    }

    public override void AfterDash()
    {
        SetAnimation(0);
        slash.SetActive(false);
        switch (ROUTINES)
        {
            case AI_STATES.SLASH:
                DoubleSlash();
                break;

            case AI_STATES.JUMP_BACK:
            case AI_STATES.SECOND_SLASH:
                if (attkdelay == 0)
                {
                    WalkBack();
                }
                else
                {
                    ROUTINES = AI_STATES.ATTACK_PLAYER;
                }
                break;
        }
        characterstates = CHARACTER_STATEMACHINE.MOVING;

    }

    public override void AfterAttack()
    {
        switch (ROUTINES)
        {
            case AI_STATES.JUMP_BACK:

                direction_vec = -LookAtTarget(target);
                Dash(0.1f, 0.4f, 0.5f);
                return;
        }
        ROUTINES = AI_STATES.ATTACK_PLAYER;
        characterstates = CHARACTER_STATEMACHINE.MOVING;
    }

    public override void DashFunction()
    {
        if (ROUTINES == AI_STATES.ATTACK_PLAYER)
            ROUTINES = AI_STATES.SLASH;

        SetAnimation(5);
        slash.SetActive(true);
    }
}
