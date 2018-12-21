using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class npc_engineer : o_npcharacter, IpoolObject
{
    public AudioClip attacksnd;
    public enum AI_STATES
    {
        ATTACK_PLAYER,
        BACK_AWAY,
        WRENCH,
        GUN
    };
    public AI_STATES ROUTINES;

    void IpoolObject.SpawnStart()
    {
        maxHealth = 8;
        experience_to_give = 5;
        initialSpeed = 100f;

        give_ammo = true;
        view_distance = 500;
        hurt_sound = new AudioClip[3];
        hurt_sound[0] = GetSoundEffect("engineer_hurt1");
        hurt_sound[1] = GetSoundEffect("engineer_hurt2");
        hurt_sound[2] = GetSoundEffect("engineer_hurt3");

        attack_pow = 2;
        SetAttackObject();
        Initialize();
        en_phase.Add(new s_enemyphase(0.5f));
        en_phase.Add(new s_enemyphase(0.33f));
        //en_phase.Add(new s_enemyphase(0.25f));
    }
	
	new void Update ()
    {
        base.Update();
	}

    public override void AIFunction()
    {
        switch (characterstates)
        {
            case CHARACTER_STATEMACHINE.STAND:

                SetAnimation(0);
                CheckForTarget(false);
                if (target != null)
                    if (DistanceToAttack(view_distance, target, false))
                        characterstates = CHARACTER_STATEMACHINE.MOVING;
                break;

            case CHARACTER_STATEMACHINE.MOVING:
                switch (ROUTINES)
                {
                    case AI_STATES.ATTACK_PLAYER:

                        if (target.characterstates == CHARACTER_STATEMACHINE.DEAD)
                        {
                            StopCharacter();
                            characterstates = CHARACTER_STATEMACHINE.STAND;
                        }

                        direction_vec = LookAtTarget(target);
                        MoveCharacter(direction_vec);
                        SetAnimation(1);

                        if (DistanceToAttack(180, target, false))
                        {
                            SetAnimation(0);
                            switch (current_health_phase)
                            {
                                case 0:
                                    ChooseAttack(0);

                                    break;

                                case 1:
                                    ChooseAttack(UnityEngine.Random.Range(0, 2));
                                    break;

                            }
                        }

                        if (DistanceToAttack(60, target, false))
                        {
                            if (attkdelay <= 0)
                            {
                                direction_vec = -(LookAtTarget(target) + (Vector3)new Vector2(direction_vec.x + Random.Range(-0.6f, 0.6f), direction_vec.y + Random.Range(-0.6f, 0.6f))).normalized;
                                attkdelay = 0.4f;
                                StartCoroutine(WalkBack(0.8f));
                                ROUTINES = AI_STATES.BACK_AWAY;
                            }
                        }
                        break;

                    case AI_STATES.BACK_AWAY:
                        
                        MoveCharacter(direction_vec);

                        if (CheckIfCornered(direction_vec))
                        {
                            Speed = initialSpeed;
                            StopCharacter();
                            ROUTINES = AI_STATES.ATTACK_PLAYER;
                        }

                        if (!walkback)
                        {
                            StopCharacter();
                            ROUTINES = AI_STATES.ATTACK_PLAYER;
                        }
                        break;

                    case AI_STATES.WRENCH:
                        StopCharacter();
                        Dash(0.4f, 1.2f, 0.6f);
                        break;

                    case AI_STATES.GUN:

                        SetAnimation(0);
                        StopCharacter();
                        SetAnimation(2);
                        Attack(0.9f, 0.6f);
                        break;

                }
                break;

            case CHARACTER_STATEMACHINE.ATTACKING:

                switch (ROUTINES)
                {
                    case AI_STATES.WRENCH:

                        StartCoroutine(WrenchWhack());
                        break;
                    case AI_STATES.GUN:

                        StartCoroutine(Gun());
                        break;
                }
                characterstates = CHARACTER_STATEMACHINE.ATTACK_ANIMATION;
                break;
        }
    }

    IEnumerator Gun()
    {
        //Set animation to gun
        attack_pow = 1;
        StopCharacter();
        direction_vec = LookAtTarget(target);
        yield return new WaitForSeconds(0.2f);
        PlayEffect("shoot_spark");
        ShootBullet("bullet", UnityEngine.Random.Range(-5, 5), 2.3f);
        yield return new WaitForSeconds(0.1f);
        attack_pow = 2;
    }

    IEnumerator WrenchWhack()
    {
        SetAnimation(5);
        attack.SetActive(true);
        Speed = initialSpeed + 325;
        direction_vec = LookAtTarget(target);
        MoveCharacter(direction_vec);
        yield return new WaitForSeconds(0.2f);
        Speed = initialSpeed;
        StopCharacter();
        SetAnimation(0);
    }

    public override void AfterAttack()
    {
        switch (ROUTINES)
        {
            case AI_STATES.WRENCH:

                attack.SetActive(false);
                ROUTINES = AI_STATES.ATTACK_PLAYER;
                break;

            case AI_STATES.GUN:

                attack.SetActive(false);
                ROUTINES = AI_STATES.ATTACK_PLAYER;
                break;
        }
        characterstates = CHARACTER_STATEMACHINE.MOVING;
    }

    public override void AfterDash()
    {
        DisableAttack();
        StopCharacter();
        SetAnimation(0);
        if (!walkback)
        {
            SetAnimation(1);
            direction_vec = -(LookAtTarget(target) + (Vector3)new Vector2(direction_vec.x + Random.Range(-0.6f, 0.6f), direction_vec.y + Random.Range(-0.6f, 0.6f))).normalized;

            ROUTINES = AI_STATES.BACK_AWAY;
            StartCoroutine(WalkBack(0.8f));
        }
        else
            ROUTINES = AI_STATES.ATTACK_PLAYER;
        characterstates = CHARACTER_STATEMACHINE.MOVING;

    }

    public override void DashFunction()
    {
        SoundManager.SFX.playSound(attacksnd);
        SetAnimation(5);
        EnableAttack();
    }

    public override void ChooseAttack(int attack_enum)
    {
        base.ChooseAttack(attack_enum);

        switch (attack_enum)
        {
            case 0:

                ROUTINES = AI_STATES.WRENCH;
                break;

            case 1:

                ROUTINES = AI_STATES.GUN;
                break;
        }
    }
}
