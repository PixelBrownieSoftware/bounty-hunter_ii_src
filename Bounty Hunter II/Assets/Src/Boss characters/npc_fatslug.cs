using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class npc_fatslug : o_npcharacter, IpoolObject
{
    public enum AI_STATES
    {
        IDLE,
        CHARGE,
        PUDDLE,
        SLIMESHOT_SMALL,
        SLIMESHOT_BIG
    };
    AI_STATES ROUTINES;

    public AudioClip deadsound;

    bool isdead = false;
    GameObject bite;

    void IpoolObject.SpawnStart()
    {
        gui_player.othercharacter = new o_npcharacter[1];
        gui_player.othercharacter[0] = this;
        destroyOnDeath = false;
        maxHealth = 100;
        initialSpeed = 100f;

        experience_to_give = 15;
        attack_pow = 2;
        Initialize();

        en_phase.Add(new s_enemyphase(0.75f));
        en_phase.Add(new s_enemyphase(0.5f));
        en_phase.Add(new s_enemyphase(0.25f));
        SetAttackObject();

    }

    private void Start()
    {
    }

    public override void AIFunction()
    {
        switch (characterstates)
        {
            case CHARACTER_STATEMACHINE.STAND:
                StopCharacter();
                switch (ROUTINES)
                {
                    case AI_STATES.IDLE:
                        target = target_characters[0];
                        characterstates = CHARACTER_STATEMACHINE.MOVING;
                        break;

                    case AI_STATES.CHARGE:
                        bite.SetActive(false);
                        ROUTINES = AI_STATES.IDLE;
                        break;
                }

                break;

            case CHARACTER_STATEMACHINE.MOVING:

                switch (ROUTINES)
                {
                    case AI_STATES.IDLE:
                        if (target.characterstates == CHARACTER_STATEMACHINE.DEAD)
                        {
                            ROUTINES = AI_STATES.IDLE;
                            characterstates = CHARACTER_STATEMACHINE.STAND;
                        }

                        SetAnimation(1);
                        direction_vec = LookAtTarget(target);
                        MoveCharacter(direction_vec);
                        
                        switch (current_health_phase)
                        {
                            case 2:
                                initialSpeed = 140f;
                                if (DistanceToAttack(235, target, false))
                                    ChooseAttack(UnityEngine.Random.Range(0, 1));
                                break;

                            case 1:
                                initialSpeed = 125f;
                                if (DistanceToAttack(185, target, false))
                                {
                                    ChooseAttack(UnityEngine.Random.Range(0, 2));
                                }
                                break;

                            case 0:
                                if (DistanceToAttack(150, target, false))
                                {
                                    ChooseAttack(0);
                                }
                                break;
                        }

                        break;

                    case AI_STATES.CHARGE:
                        StopCharacter();
                        Speed = initialSpeed + 50;
                        direction_vec = LookAtTarget(target);
                        Dash(0.63f, 0.15f, 0.48f);
                        break;
                }

                break;

            case CHARACTER_STATEMACHINE.ATTACKING:

                StopCharacter();
                switch (ROUTINES)
                {
                    case AI_STATES.PUDDLE:
                        ShootBullet("slime_puddle", 0, 0);
                        break;

                    case AI_STATES.SLIMESHOT_SMALL:
                        ShootBullet("bullet", 0, 0.5f).GetComponent<BulletClass>().attackPower = 1;
                        break;

                    case AI_STATES.SLIMESHOT_BIG:
                        ShootBullet("bullet", 0, 0.3f).GetComponent<BulletClass>().attackPower = 2;
                        break;
                }
                characterstates = CHARACTER_STATEMACHINE.ATTACK_ANIMATION;
                break;

            case CHARACTER_STATEMACHINE.DEAD:

                if (!isdead)
                {
                    StartCoroutine(Death());
                }
                break;
        }
    }
    
    IEnumerator Death()
    {
        isdead = true;
        SetAnimation(6);
        SoundManager.SFX.playSound(deadsound);
        gui_player gui = GameObject.Find("GUI").GetComponent<gui_player>();
        gui.ToggleBossGUI(false);
        yield return new WaitForSeconds(1.5f);
        gameObject.SetActive(false);
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
                ROUTINES = AI_STATES.CHARGE;
                break;

            case 1:
                ROUTINES = AI_STATES.PUDDLE;
                Attack(0.5f, 0.1f);
                break;
                /*
            case 1:

                switch (current_health_phase)
                {
                    case 1:

                        ROUTINES = AI_STATES.SLIMESHOT_SMALL;
                        Attack(0.5f, 0.1f);
                        break;

                    case 2:

                        ROUTINES = (AI_STATES)UnityEngine.Random.Range(3, 5);
                        Attack(0.5f, 0.1f);
                        break;

                }

                break;
                */


        }

    }

    public override void AfterDash()
    {
        Speed = initialSpeed;
        characterstates = CHARACTER_STATEMACHINE.MOVING;
        ROUTINES = AI_STATES.IDLE;
        bite.SetActive(false);
    }

    public override void DashFunction()
    {
        SetAnimation(3);
        bite.SetActive(true);
    }

    public override void AfterAttack()
    {
        ROUTINES = AI_STATES.IDLE;
        characterstates = CHARACTER_STATEMACHINE.MOVING;
    }
}