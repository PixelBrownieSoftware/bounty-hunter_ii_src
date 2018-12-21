using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class npc_gunnerbot : o_npcharacter, IpoolObject
{
    public AudioClip shootsound;
    public enum AI_STATES
    {
        IDLE,
        ATTACK_PLAYER,
        SHOOTING,
        BACK_AWAY
    };
    public AI_STATES ROUTINES;

    void IpoolObject.SpawnStart()
    {
        experience_to_give = 4;
        initialSpeed = 140;
        attack_pow = 1;
        maxregentime = 0.2f;
        give_ammo = true;
        constant_regen = false;
        
        maxHealth = 7;
        view_distance = 670;
        ROUTINES = AI_STATES.IDLE;
        en_phase.Add(new s_enemyphase(0.7f));
        en_phase.Add(new s_enemyphase(0.4f));

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
                        ROUTINES = AI_STATES.ATTACK_PLAYER;
                        characterstates = CHARACTER_STATEMACHINE.MOVING;
                    }
                }
                break;

            case CHARACTER_STATEMACHINE.MOVING:

                switch (current_health_phase)
                {
                    case 0:
                        constant_regen = false;
                        break;

                    case 1:

                        constant_regen = true;
                        break;
                }

                switch (ROUTINES)
                {

                    case AI_STATES.IDLE:
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
                            if (DistanceToAttack(view_distance, target, false))
                            {
                                SetAnimation(0);
                                direction_vec = LookAtTarget(target);
                                MoveCharacter(direction_vec);

                                //They shoot the player when they are close enough.
                                if (DistanceToAttack(view_distance / 3.8f, target, false))
                                {
                                    ROUTINES = AI_STATES.SHOOTING;
                                }

                                //Strategically backing away so the player can't just constantly shoot at them
                                if (DistanceToAttack(view_distance / 4.2f, target, false))
                                {
                                    if (attkdelay == 0)
                                    {
                                        attkdelay = 1f;
                                        if (current_health_phase == 1)
                                        {
                                            ShootBullet("bmb", 0, 0.1f);
                                        }
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

                        SetAnimation(0);
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

                    case AI_STATES.SHOOTING:

                        StopCharacter();
                        Attack(1.3f, 0.95f);
                        break;
                }
                break;

            case CHARACTER_STATEMACHINE.ATTACKING:
                StartCoroutine(ShootConst());
                characterstates = CHARACTER_STATEMACHINE.ATTACK_ANIMATION;

                break;

            case CHARACTER_STATEMACHINE.DEAD:
                PlayExplosionEffect();
                break;
        }
    }

    new void Update()
    {
        base.Update();
    }

    IEnumerator ShootConst()
    {
        SetAnimation(1);
        direction_vec = LookAtTarget(target);
        yield return new WaitForSeconds(0.8f);

        for (int i = 0; i < 4; i++)
        {
            PlayEffect("shoot_spark");
            SoundManager.SFX.playSound(shootsound);
            ShootBullet("bullet", 0, 0.5f);
            yield return new WaitForSeconds(0.1f);
        }
        ROUTINES = AI_STATES.IDLE;
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
