using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class npc_demonmilbert : o_npcharacter, IpoolObject
{
    int dashnumber = 0;
    GameObject smackOBJ;

    public enum AI_STATES
    {
        IDLE,
        TRIPLE_DASH,
        PROJECTILES,
        MELEE_ATTACK_LONG,
        MELEE_ATTACK_FRENZY,
        CHARGE,
        FIRE_BREATH,
        JUMP_UP,
        JUMP_AIM,
        JUMP_IMPACT
    };
    public AI_STATES ROUTINES;

    public AudioClip[] dashsounds;
    public AudioClip jump;
    public AudioClip land;
    public AudioClip fire;
    public AudioClip projectilesound;

    void IpoolObject.SpawnStart()
    {
        dashnumber = 0;
        attack_pow = 1;
        maxHealth = 250f;
        initialSpeed = 230f;
        view_distance = 900;
        destroyOnDeath = false;

        smackOBJ = transform.GetChild(2).gameObject;
        SetAttackObject();
        smackOBJ.SetActive(false);
        
        Initialize();
        en_phase.Add(new s_enemyphase(0.83f));
        en_phase.Add(new s_enemyphase(0.65f));
        en_phase.Add(new s_enemyphase(0.45f));
        en_phase.Add(new s_enemyphase(0.27f));
    }

    void Start()
    {
        gui_player.othercharacter = new o_npcharacter[1];
        gui_player.othercharacter[0] = this;
    }

    public override void AIFunction()
    {
        switch (characterstates)
        {
            case CHARACTER_STATEMACHINE.STAND:
                target = AquireTarget(false);
                SetAnimation(0);
                if (target != null)
                {
                    characterstates = CHARACTER_STATEMACHINE.MOVING;
                }
                switch (ROUTINES)
                {
                    case AI_STATES.MELEE_ATTACK_LONG:
                        ROUTINES = AI_STATES.IDLE;
                        break;
                }
                break;

            case CHARACTER_STATEMACHINE.MOVING:
                switch (ROUTINES)
                {
                    case AI_STATES.IDLE:

                        SetAnimation(1);
                        direction_vec = LookAtTarget(target);
                        MoveCharacter(direction_vec);
                        if (current_health_phase >= 2 && DistanceToAttack(90, GameObject.FindWithTag("Bullet"), false))
                        {
                            if (!walkback)
                            {
                                //       StartCoroutine(WalkBack(1.2f));
                            }
                        }
                        switch (current_health_phase)
                        {
                            case 0:

                                if (DistanceToAttack(120, target, false))
                                {
                                    ChooseAttack(0);
                                }
                                break;
                            case 1:

                                if (DistanceToAttack(150, target, false))
                                {
                                    ChooseAttack(Random.Range(0, 3));
                                }


                                break;
                            case 2:

                                if (DistanceToAttack(240, target, false))
                                {
                                    ChooseAttack(Random.Range(1, 4));
                                }
                                break;

                            case 3:

                                if (DistanceToAttack(400, target, false))
                                {
                                    ChooseAttack(Random.Range(1, 5));
                                }
                                break;
                        }
                        break;

                    case AI_STATES.TRIPLE_DASH:

                        StopCharacter();
                        dashnumber++;
                        direction_vec = LookAtTarget(target);
                        SetAnimation(3);
                        Dash(0.2f, 0.1f, 0.5f);
                        break;

                    case AI_STATES.MELEE_ATTACK_LONG:
                        StopCharacter();
                        SetAnimation(3);
                        Dash(0.5f, 1.7f, 0.3f);
                        break;

                    case AI_STATES.MELEE_ATTACK_FRENZY:
                        StopCharacter();
                        Attack(4f, 0.2f);
                        break;

                    case AI_STATES.JUMP_UP:

                        Attack(0.5f, 2.5f);
                        MoveCharacter(direction_vec);
                        break;

                    case AI_STATES.JUMP_AIM:

                        direction_vec = LookAtTarget(target);
                        MoveCharacter(direction_vec);
                        break;

                    case AI_STATES.JUMP_IMPACT:
                        StopCharacter();
                        break;

                    case AI_STATES.PROJECTILES:

                        SetAnimation(8);
                        Attack(1.2f, 1.2f);
                        break;

                    case AI_STATES.FIRE_BREATH:
                        Attack(8.5f, 1.5f);
                        break;
                }
                break;

            case CHARACTER_STATEMACHINE.ATTACKING:
                switch (ROUTINES)
                {
                    case AI_STATES.PROJECTILES:

                        StartCoroutine(Beams());
                        break;

                    case AI_STATES.JUMP_UP:
                        SetAnimation(9);
                        StopCharacter();
                        StartCoroutine(Jump());
                        break;

                    case AI_STATES.FIRE_BREATH:

                        StartCoroutine(FireBreath(Random.Range(1, 3)));
                        break;

                    case AI_STATES.MELEE_ATTACK_FRENZY:

                        StartCoroutine(MeleAttkFrenzy());
                        break;
                }
                characterstates = CHARACTER_STATEMACHINE.ATTACK_ANIMATION;
                break;

            case CHARACTER_STATEMACHINE.DEAD:
                DisableAttack();
                smackOBJ.SetActive(false);

                break;
        }
    }

    new void Update()
    {
        base.Update();
        shadow.transform.localScale = new Vector3(1.4f, 1, 1);   
    }

    IEnumerator MeleAttkFrenzy()
    {
        attack_pow = 1;
        int loop = 5;
        while (loop > -1)
        {
            if (loop == 0)
            {
                attack_pow = 4;
                Speed = initialSpeed + 500 * 3;
            }
            else
            {
                attack_pow = 1;
                Speed = initialSpeed + 325 * 2;
            }

            EnableAttack();
            direction_vec = LookAtTarget(target);
            MoveCharacter(direction_vec);

            if (loop == 0)
                yield return new WaitForSeconds(0.5f);
            else
                yield return new WaitForSeconds(0.4f);
            StopCharacter();

            yield return new WaitForSeconds(0.1f);
            loop--;
        }
        Speed = initialSpeed;
        StopCharacter();
    }

    IEnumerator FireBreath(int rounds)
    {
        SetAnimation(8);
        int roundss = rounds;
        while (roundss != 0)
        {
            StopCharacter();
            int i = 10;
            float angleoffset = 90;
            const float dist = 0.9f, speed = 100;

            if (characterstates == CHARACTER_STATEMACHINE.DEAD)
                break;
            yield return new WaitForSeconds(0.37f);

            if (characterstates == CHARACTER_STATEMACHINE.DEAD)
                break;
            while (i > -10)
            {
                if (characterstates == CHARACTER_STATEMACHINE.DEAD)
                    break;
                direction_vec = -LookAtTarget(target);

                angleoffset += 15 * Mathf.Sign(i);
                SoundManager.SFX.playSound(fire);
                ShootBullet("fire", angleoffset + Random.Range(-2, 2), dist, speed);
                yield return new WaitForSeconds(0.12f);
                if (characterstates == CHARACTER_STATEMACHINE.DEAD)
                    break;
                i--;
            }
            roundss--;
        }
    }

    IEnumerator JumpAnim()
    {
        yield return new WaitForSeconds(0.3f);
    }

    IEnumerator Jump()
    {
        DisableAttack();
        hit_box.enabled = false;
        //GO INTO THE NOTHING STATE AND PLAY A JUMPING UP ANIMATION
        thisrender.color = Color.black;

        Speed = initialSpeed + 200;

        yield return new WaitForSeconds(0.3f);
        SoundManager.SFX.playSound(jump);
        ROUTINES = AI_STATES.JUMP_AIM;
        const float timerstart = 3;
        float jumptimer = timerstart;
        while (jumptimer > 0)
        {
            //SET SHADOW APLHA TO JUMPTIMER DIVIDED BY TIMERSTART

            jumptimer = jumptimer - Time.deltaTime;
            yield return new WaitForSeconds(Time.deltaTime);
        }

        smackOBJ.SetActive(false);
        //GO INTO THE NOTHING STATE AND PLAY AN ANIMATION
        thisrender.color = Color.white;
        hit_box.enabled = true;

        ROUTINES = AI_STATES.JUMP_IMPACT;
        SoundManager.SFX.playSound(land);
        Speed = initialSpeed;
        yield return new WaitForSeconds(0.1f);

        PlayExplosionEffect(new Vector2(Random.Range(10, 45), Random.Range(10, 45)));
        PlayExplosionEffect(new Vector2(-Random.Range(10,25), -Random.Range(35, 45)));
        PlayExplosionEffect(new Vector2(Random.Range(10, 45), -Random.Range(10, 45)));
        PlayExplosionEffect(new Vector2(Random.Range(10, 45), Random.Range(26, 35)));
        PlayExplosionEffect(new Vector2(-Random.Range(10, 45), Random.Range(10, 45)));
        PlayExplosionEffect();

        smackOBJ.SetActive(true);
        SetAnimation(7);
        yield return new WaitForSeconds(0.05f);
        smackOBJ.SetActive(false);
        yield return new WaitForSeconds(1.25f);
        SetAnimation(1);
        ROUTINES = AI_STATES.IDLE;
    }

    IEnumerator Beams()
    {
        SetAnimation(8);
        attack_pow = 1;
        StopCharacter();
        const float dist = 0.9f, speed = 40;
        yield return new WaitForSeconds(0.5f);
        SoundManager.SFX.playSound(projectilesound);
        direction_vec = LookAtTarget(target);
        ShootBullet("bullet", 0, dist, speed);
        ShootBullet("bullet", 20, dist, speed);
        ShootBullet("bullet", -70, dist, speed);
        ShootBullet("bullet", -20, dist, speed);
        ShootBullet("bullet", -10, dist, speed);
        ShootBullet("bullet", 70, dist, speed);
        yield return new WaitForSeconds(0.3f);

        direction_vec = LookAtTarget(target);
        SoundManager.SFX.playSound(projectilesound);
        ShootBullet("bullet", 0, dist, speed);
        ShootBullet("bullet", 10, dist, speed);
        ShootBullet("bullet", 20, dist, speed);
        ShootBullet("bullet", 30, dist, speed);
        ShootBullet("bullet", 70, dist, speed);
        ShootBullet("bullet", -10, dist, speed);
        ShootBullet("bullet", -30, dist, speed);
        ShootBullet("bullet", -70, dist, speed);
        yield return new WaitForSeconds(0.3f);

        direction_vec = LookAtTarget(target);
        SoundManager.SFX.playSound(projectilesound);
        ShootBullet("bullet", 0, dist, speed);
        ShootBullet("bullet", 20, dist, speed);
        ShootBullet("bullet", -20, dist, speed);
        ShootBullet("bullet", -10, dist, speed);
        yield return new WaitForSeconds(0.3f);
        //SoundManager.SFX.playSound(SoundManager.SFX.LoadAudio("peacemaker_shoot"));

        direction_vec = LookAtTarget(target);
        
    }

    public override void AfterAttack()
    {
        switch (ROUTINES)
        {
            case AI_STATES.MELEE_ATTACK_FRENZY:
            case AI_STATES.FIRE_BREATH:
            case AI_STATES.JUMP_IMPACT:
            case AI_STATES.PROJECTILES:

                StopCharacter();
                ROUTINES = AI_STATES.IDLE;
                break;
        }
        characterstates = CHARACTER_STATEMACHINE.MOVING;
    }

    public override void AfterDash()
    {
        StopCharacter();
        switch (ROUTINES)
        {
            case AI_STATES.MELEE_ATTACK_LONG:

                StopCharacter();
                ROUTINES = AI_STATES.IDLE;
                break;

            case AI_STATES.TRIPLE_DASH:
                StopCharacter();
                if (dashnumber >= 3)
                {
                    dashnumber = 0;
                    ROUTINES = AI_STATES.IDLE;
                }
                break;
        }
        DisableAttack();
        characterstates = CHARACTER_STATEMACHINE.MOVING;
    }

    public override void ChooseAttack(int attack_enum)
    {
        switch (attack_enum)
        {
            case 0:

                ROUTINES = AI_STATES.MELEE_ATTACK_LONG;
                break;
                
            case 1:
                ROUTINES = AI_STATES.TRIPLE_DASH;
                break;
                
            case 2:

                ROUTINES = AI_STATES.PROJECTILES;
                break;

            case 3:

                ROUTINES = AI_STATES.JUMP_UP;
                break;

            case 4:

                ROUTINES = AI_STATES.FIRE_BREATH;
                break;

            case 5:

                ROUTINES = AI_STATES.MELEE_ATTACK_FRENZY;
                break;
                
        }

    }

    public override void DashFunction()
    {
        SoundManager.SFX.playSound(dashsounds[UnityEngine.Random.Range(0, dashsounds.Length)]);
        attack.SetActive(true);

    }
}
