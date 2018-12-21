using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class npc_milbert2 : o_npcharacter, IpoolObject
{
    const int max_shotgun_fires = 3;
    int dash_number = 0;
    public bool pickchargedirection = false;       //A small workaround for the dash in wall problem
    

    AudioClip dodge_sound;
    public enum AI_STATES
    {
        IDLE,
        WALK_BACK,
        DASH_BACK,
        //SHOGUN_FRENZY,
        //SHOGUN_FRENZY_RECOIL,
        DOUBLE_DASH,
        DASH,
        PEACEMAKER,
        MACHINE_GUN,
        MACHINE_GUN_DASH_BACK,
        NONE
    };
    public AI_STATES ROUTINES;

    void Start()
    {
    }

    void IpoolObject.SpawnStart()
    {
        gui_player.othercharacter = new o_npcharacter[1];
        gui_player.othercharacter[0] = this;
        attack_pow = 1;
        maxHealth = 180f;
        initialSpeed = 180f;
        view_distance = 900;
        destroyOnDeath = false;
        
        dodge_sound = SoundManager.SFX.LoadAudio("milbert_dash_attack1");
        SetAttackObject();
        DisableAttack();

        Initialize();
        en_phase.Add(new s_enemyphase(0.83f));
        en_phase.Add(new s_enemyphase(0.65f));
        en_phase.Add(new s_enemyphase(0.25f));
        en_phase.Add(new s_enemyphase(0.15f));
    }

    new private void Update()
    {
        base.Update();
    }

    public override void AIFunction()
    {
        switch (characterstates)
        {
            case CHARACTER_STATEMACHINE.STAND:

                target = AquireTarget(false);
                if (target != null)
                {
                    characterstates = CHARACTER_STATEMACHINE.MOVING;
                }

                switch (ROUTINES)
                {

                    case AI_STATES.DOUBLE_DASH:
                    case AI_STATES.DASH_BACK:
                    case AI_STATES.DASH:
                    case AI_STATES.MACHINE_GUN:

                        ROUTINES = AI_STATES.IDLE;
                        characterstates = CHARACTER_STATEMACHINE.MOVING;
                        break;
                }

                break;

            case CHARACTER_STATEMACHINE.MOVING:


                switch (ROUTINES)
                {
                    case AI_STATES.IDLE:

                        SetAnimation(1);
                        if (target == null)
                            target = AquireTarget(false);
                        direction_vec = LookAtTarget(target);
                        MoveCharacter(direction_vec);

                        switch (current_health_phase)
                        {
                            case 0:

                                if (DistanceToAttack(60, target, false))
                                {

                                }
                                if (DistanceToAttack(130, target, false))
                                {
                                    ChooseAttack(Random.Range(0, 2));
                                }
                                break;

                            case 1:
                                if (DistanceToAttack(80, target, false))
                                {
                                    ROUTINES = AI_STATES.DASH_BACK;
                                }
                                if (DistanceToAttack(175, target, false))
                                {
                                    ChooseAttack(Random.Range(0, 3));
                                }
                                break;

                            case 2:
                                if (DistanceToAttack(80, target, false))
                                {
                                    ROUTINES = AI_STATES.DASH_BACK;
                                }
                                if (DistanceToAttack(175, target, false))
                                {
                                    ChooseAttack(Random.Range(0, 4));
                                }

                                break;
                            case 3:
                                if (DistanceToAttack(80, target, false))
                                {
                                    ROUTINES = AI_STATES.DASH_BACK;
                                }
                                if (DistanceToAttack(175, target, false))
                                {
                                    ChooseAttack(Random.Range(0, 5));
                                }

                                break;
                        }

                        break;

                    case AI_STATES.DASH_BACK:
                        Dash(0.25f, 0.65f, 0.07f);
                        break;

                    case AI_STATES.DASH:
                    case AI_STATES.DOUBLE_DASH:

                        StopCharacter();
                        direction_vec += (Vector3)new Vector2(direction_vec.x + Random.Range(-0.6f, 0.6f), direction_vec.y + Random.Range(-0.6f, 0.6f)).normalized;
                        Dash(0.25f, 0.65f, 0.07f);
                        break;

                    case AI_STATES.PEACEMAKER:

                        Attack(1.3f, 0.6f);
                        break;

                    case AI_STATES.MACHINE_GUN_DASH_BACK:

                        StopCharacter();
                        SetAnimation(8);
                        StartCoroutine(Shotgun());
                        break;

                    case AI_STATES.MACHINE_GUN:

                        Attack(0.6f);
                        break;

                        

                    /*
                case AI_STATES.SHOGUN_FRENZY:

                    direction_vec = (Vector3)new Vector2(direction_vec.x + Random.Range(-0.6f, 0.6f), direction_vec.y + Random.Range(-0.6f, 0.6f)).normalized;

                    Speed = initialSpeed + 90;
                    Dash(0.4f, 0.65f, 0.01f);
                    break;
                    */


                    case AI_STATES.WALK_BACK:

                        SetAnimation(1);
                        MoveCharacter(direction_vec);

                        if (CheckIfCornered(direction_vec))
                        {
                            ROUTINES = AI_STATES.IDLE;
                        }
                        if (!walkback)
                            ROUTINES = AI_STATES.IDLE;
                        break;
                }

                break;

            case CHARACTER_STATEMACHINE.ATTACKING:
                switch (ROUTINES)
                {
                    case AI_STATES.IDLE:

                        direction_vec = LookAtTarget(target);
                        MoveCharacter(direction_vec);

                        break;

                    case AI_STATES.MACHINE_GUN:

                        StopCharacter();
                        Speed = initialSpeed + 120;
                        MoveCharacter(-direction_vec);
                        StartCoroutine(MachineGun());
                        break;

                    case AI_STATES.PEACEMAKER:

                        StopCharacter();
                        direction_vec = LookAtTarget(target);
                        StartCoroutine(Peacemaker());
                        break;

                }
                characterstates = CHARACTER_STATEMACHINE.ATTACK_ANIMATION;
                break;

        }
    }

    IEnumerator Peacemaker()
    {
        attack_pow = 3;
        for (int i = 0; i != 5; i++)
        {
            SetAnimation(2);
            direction_vec = LookAtTarget(target);
            PlayEffect("shoot_spark");
            SoundManager.SFX.playSound(SoundManager.SFX.LoadAudio("Colt Python_shoot"));
            ShootBullet("bullet", 0, 0.3f,90);
            yield return new WaitForSeconds(0.3f);
        }
    }

    IEnumerator MachineGun()
    {
        while (!CheckIfCornered(direction_vec) || characterstates != CHARACTER_STATEMACHINE.STAND || characterstates != CHARACTER_STATEMACHINE.MOVING)
        {
            attack_pow = 1;
            direction_vec = -LookAtTarget(target);
            SoundManager.SFX.playSound(SoundManager.SFX.LoadAudio("Colt Python_shoot"));
            ShootBullet("bullet", 0, 0.9f, 50);
            yield return new WaitForSeconds(0.1f);
        }
        attkdelay = 0;
        characterstates = CHARACTER_STATEMACHINE.STAND;

    }

    IEnumerator Shotgun()
    {
        ROUTINES = AI_STATES.NONE;
        attack_pow = 3;
        direction_vec = LookAtTarget(target);
        yield return new WaitForSeconds(0.3f);
        ShootBullet("bullet", 0, 0.6f, 150f);
        SoundManager.SFX.playSound(SoundManager.SFX.LoadAudio("shotgun"));
        direction_vec *= -1;

        Dash(1f, 0.3f, 0.2f);
        ROUTINES = AI_STATES.MACHINE_GUN_DASH_BACK;
        yield return new WaitForSeconds(1f);
        direction_vec = LookAtTarget(target);
        lock_direction = false;
    }

    public override void AfterAttack()
    {
        switch (ROUTINES)
        {

            case AI_STATES.PEACEMAKER:
            case AI_STATES.MACHINE_GUN:
                ROUTINES = AI_STATES.IDLE;
                break;
                
        }
        characterstates = CHARACTER_STATEMACHINE.MOVING;
    }

    public override void AfterDash()
    {
        DisableAttack();
        StopCharacter();
        switch (ROUTINES)
        {
            case AI_STATES.DASH:

                if (!walkback)
                {
                    direction_vec = -LookAtTarget((Vector3)new Vector2(direction_vec.x + Random.Range(-0.6f, 0.6f), direction_vec.y + Random.Range(-0.6f, 0.6f)).normalized);
                    ROUTINES = AI_STATES.WALK_BACK;
                    StartCoroutine(WalkBack(0.5f));
                }
                break;

            case AI_STATES.DASH_BACK:

                ROUTINES = AI_STATES.IDLE;
                break;

            case AI_STATES.DOUBLE_DASH:

                if (dash_number >= 1)
                {
                    SetAnimation(3);
                    dash_number = 0;
                    if (!walkback)
                    {
                        ROUTINES = AI_STATES.WALK_BACK;
                        direction_vec = new Vector2(direction_vec.x + Random.Range(-0.6f, 0.6f), direction_vec.y + Random.Range(-0.6f, 0.6f)).normalized;
                        StartCoroutine(WalkBack(0.5f));
                    }
                    else
                        ROUTINES = AI_STATES.IDLE;
                }
                else
                    characterstates = CHARACTER_STATEMACHINE.MOVING;
                break;

            case AI_STATES.MACHINE_GUN:

                ROUTINES = AI_STATES.IDLE;
                break;

            case AI_STATES.MACHINE_GUN_DASH_BACK:
                ROUTINES = AI_STATES.IDLE;
                //ROUTINES = AI_STATES.MACHINE_GUN;
                break;
                
        }
        Invinciblity = false;
        characterstates = CHARACTER_STATEMACHINE.MOVING;
    }

    public override void ChooseAttack(int attack_enum)
    {
        SetAnimation(0);
        base.ChooseAttack(attack_enum);
        switch (attack_enum)
        {
            case 0:

                direction_vec = LookAtTarget(target);
                ROUTINES = AI_STATES.DASH;
                break;
                
            case 1:

                direction_vec = LookAtTarget(target);
                ROUTINES = AI_STATES.DOUBLE_DASH;
                break;
                
            case 2:
                direction_vec = -LookAtTarget(target);
                ROUTINES = AI_STATES.MACHINE_GUN_DASH_BACK;
                break;

            case 3:
                ROUTINES = AI_STATES.PEACEMAKER;
                break;
        }
    }

    public override void DashFunction()
    {
        switch (ROUTINES)
        {
            case AI_STATES.MACHINE_GUN_DASH_BACK:

                StartCoroutine(Shotgun());
                break;

            case AI_STATES.DOUBLE_DASH:
                EnableAttack();
                SetAnimation(3);
                dash_number++;
                SoundManager.SFX.playSound(dodge_sound);
                direction_vec = LookAtTarget(target);
                Dash(0.25f, 0.65f, 0.07f);
                break;

            case AI_STATES.DASH:

                EnableAttack();
                SetAnimation(3);
                SoundManager.SFX.playSound(dodge_sound);
                direction_vec = LookAtTarget(target);
                Dash(0.25f, 0.65f, 0.2f);
                break;

            case AI_STATES.DASH_BACK:

                EnableAttack();
                SoundManager.SFX.playSound(dodge_sound);
                direction_vec = LookAtTarget(target);
                direction_vec *= -1;
                Dash(0.25f, 0.65f, 0.07f);
                break;
                
                
        }
    }
}
