using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class npc_eagles : o_npcharacter, IpoolObject
{
    public enum AI_STATES
    {
        ATTACK_PLAYER,
        BACK_AWAY
    };
    public AI_STATES ROUTINES;

    void IpoolObject.SpawnStart()
    {
        maxHealth = 4;
        experience_to_give = 3;
        view_distance = 670;
        attack_pow = 2;
        initialSpeed = 215;
        is_arial = true;
        Initialize();
        SetAttackObject();

        hurt_sound = new AudioClip[2];
        hurt_sound[0] = SoundManager.SFX.LoadAudio("eagle_cry_hurt1");
        hurt_sound[1] = SoundManager.SFX.LoadAudio("eagle_cry_hurt2");
        en_phase.Add(new s_enemyphase(0.2f));
    }

    public override void AIFunction()
    {
        base.AIFunction();
        switch (characterstates)
        {
            case CHARACTER_STATEMACHINE.STAND:
                CheckForTarget(true);
                SetAnimation(1);
                if (target != null)
                {
                    if (DistanceToAttack(view_distance, target, true))
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
                        SetAnimation(1);
                        if (target.Health <= 0)
                        {
                            ROUTINES = AI_STATES.BACK_AWAY;
                            characterstates = CHARACTER_STATEMACHINE.STAND;
                        }

                        if (DistanceToAttack(view_distance, target, true))
                        {
                            direction_vec = LookAtTarget(target);
                            MoveCharacter(direction_vec);
                        }

                        switch (current_health_phase)
                        {
                            case 0:
                                if (DistanceToAttack(45, target, true))
                                {
                                    if (attkdelay == 0)
                                    {
                                        attkdelay = 0.45f;
                                        ROUTINES = AI_STATES.BACK_AWAY;
                                    }
                                }
                                break;

                            case 1:
                                if (DistanceToAttack(90, target, true))
                                {
                                    if (attkdelay == 0)
                                    {
                                        attkdelay = 0.45f;
                                        ROUTINES = AI_STATES.BACK_AWAY;
                                    }
                                }
                                break;

                        }

                        break;

                    case AI_STATES.BACK_AWAY:

                        Speed = initialSpeed + 50;
                        tar_last_pos = target.transform.position;
                        direction_vec = -LookAtTarget(target);
                        MoveCharacter(direction_vec);

                        if (attkdelay == 0)
                        {
                            StopCharacter();
                            Speed = initialSpeed;
                            direction_vec = LookAtTarget(target);
                            Dash(0.2f, 0.5f, 0.6f);
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
        attack.SetActive(true);
        Invinciblity = false;
        ROUTINES = AI_STATES.ATTACK_PLAYER;
        characterstates = CHARACTER_STATEMACHINE.MOVING;
    }

    public override void DashFunction()
    {
        SetAnimation(2);
        attack.SetActive(true);
    }
}
