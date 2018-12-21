using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class npc_planttwins : o_npcharacter, IpoolObject
{

    public enum AI_STATES
    {
        IDLE,
        SLASH,
        DOUBLE_SLASH,
        TRIPLE_SLASH,
        THROW
    };
    public AI_STATES ROUTINES;
    public AudioClip shoot;

    void Start ()
    {
    }

    void IpoolObject.SpawnStart()
    {
        gui_player.othercharacter = new o_npcharacter[2];
        gui_player.othercharacter[0] = GameObject.Find("PlantTwin").GetComponent<o_npcharacter>();

        gui_player.othercharacter[1] = GameObject.Find("PlantTwin_2").GetComponent<o_npcharacter>();
        maxHealth = 75;
        initialSpeed = 90;
        attack_pow = 1;
        destroyOnDeath = true;
        experience_to_give = 20;
        SetAttackObject();

        Initialize();
        en_phase.Add(new s_enemyphase(0.95f));
        en_phase.Add(new s_enemyphase(0.55f));
    }

    public override void AIFunction()
    {
        switch (characterstates)
        {
            case CHARACTER_STATEMACHINE.STAND:
                if (target == null)
                    target = target_characters[0];

                switch (ROUTINES)
                {
                    case AI_STATES.SLASH:
                        direction_vec = LookAtTarget(target);
                        ROUTINES = AI_STATES.DOUBLE_SLASH;
                        break;

                    case AI_STATES.DOUBLE_SLASH:

                        direction_vec = LookAtTarget(target);
                        Dash(1f, 0.3f, 0.3f);
                        ROUTINES = AI_STATES.TRIPLE_SLASH;
                        break;

                    case AI_STATES.TRIPLE_SLASH:
                        direction_vec = LookAtTarget(target);
                        ROUTINES = AI_STATES.IDLE;
                        break;
                }

                if (target != null)
                {
                    characterstates = CHARACTER_STATEMACHINE.MOVING;
                }


                break;

            case CHARACTER_STATEMACHINE.MOVING:
                switch (ROUTINES)
                {
                    case AI_STATES.IDLE:
                        SetAnimation(1);
                        direction_vec = LookAtTarget(target);
                        MoveCharacter(direction_vec);
                        if (DistanceToAttack(175, target, false))
                        {
                            switch (current_health_phase)
                            {
                                case 0:
                                    ChooseAttack(0);
                                    break;

                                case 1:
                                    ChooseAttack(UnityEngine.Random.Range(0, 3));
                                    break;
                            }
                        }

                        break;

                    case AI_STATES.SLASH:

                        StopCharacter();
                        direction_vec = LookAtTarget(target);
                        Dash(1.5f, 0.7f, 0.3f);
                        break;

                    case AI_STATES.DOUBLE_SLASH:
                        StopCharacter();
                        direction_vec = LookAtTarget(target);
                        Dash(1f, 0.1f, 0.1f);
                        break;

                    case AI_STATES.TRIPLE_SLASH:

                        StopCharacter();
                        direction_vec = LookAtTarget(target);
                        Dash(0.4f, 0.1f, 0.1f);
                        break;
                }

                break;

            case CHARACTER_STATEMACHINE.ATTACKING:
                StopCharacter();
                StartCoroutine(Throw());
                characterstates = CHARACTER_STATEMACHINE.ATTACK_ANIMATION;
                break;

        }
    }

    new void Update ()
    {
        base.Update();

	}

    IEnumerator Throw()
    {
        SetAnimation(5);
        yield return new WaitForSeconds(0.9f);
        SoundManager.SFX.playSound(shoot);
        ShootBullet("bullet", 0, 0.5f);
        yield return new WaitForSeconds(0.07f);
        SoundManager.SFX.playSound(shoot);
        ShootBullet("bullet", 0, 0.5f);
        yield return new WaitForSeconds(0.07f);
        SoundManager.SFX.playSound(shoot);
        ShootBullet("bullet", 0, 0.5f);
        yield return new WaitForSeconds(0.07f);
    }

    public override void ChooseAttack(int attack_enum)
    {
        base.ChooseAttack(attack_enum);

        switch (attack_enum)
        {
            case 0:
                StopCharacter();
                Dash(1.5f, 0.7f, 0.3f);
                break;

            case 1:

                ROUTINES = AI_STATES.SLASH;
                break;

            case 2:
                Attack(2.5f, 1f);
                break;
        }
    }

    public override void AfterAttack()
    {
        characterstates = CHARACTER_STATEMACHINE.MOVING;
    }

    public override void AfterDash()
    {
        StopCharacter();
        switch (ROUTINES)
        {
            case AI_STATES.SLASH:
                ROUTINES = AI_STATES.DOUBLE_SLASH;
                break;

            case AI_STATES.DOUBLE_SLASH:

                ROUTINES = AI_STATES.TRIPLE_SLASH;
                break;

            case AI_STATES.TRIPLE_SLASH:
                ROUTINES = AI_STATES.IDLE;

                break;
        }

        DisableAttack();
        Invinciblity = false;
        characterstates = CHARACTER_STATEMACHINE.MOVING;
    }

    public override void DashFunction()
    {
        EnableAttack();
        SetAnimation(3);
    }
}
