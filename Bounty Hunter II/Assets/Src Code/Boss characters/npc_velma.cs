using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class npc_velma : o_npcharacter, IpoolObject {

    o_npcharacter[] characters = new o_npcharacter[3];
    bool ai_delay = false;
    public AudioClip shotgun;
    public AudioClip pistol;

    public enum AI_STATES
    {
        IDLE,
        COUNTER_ATTK,
        PEACEMAKER_GUN,
        SHOT_GUN,
        MACHINE_GUN,
        BACK_AWAY
    };
    public AI_STATES ROUTINES;

    void IpoolObject.SpawnStart()
    {
        constant_regen = true;
        destroyOnDeath = false;
        view_distance = 900;
        maxHealth = 130;
        initialSpeed = 150f;
        attack_pow = 2;
        maxregentime = 0.002f;

        Initialize();

        en_phase.Add(new s_enemyphase(0.85f));
        en_phase.Add(new s_enemyphase(0.65f));
        en_phase.Add(new s_enemyphase(0.45f));
    }

    void Start () {

        gui_player.othercharacter = new o_npcharacter[4];
        gui_player.othercharacter[0] = this;
        gui_player.othercharacter[1] = GameObject.Find("Support Bots_1").GetComponent<o_npcharacter>();
        gui_player.othercharacter[2] = GameObject.Find("Support Bots_2").GetComponent<o_npcharacter>();
        gui_player.othercharacter[3] = GameObject.Find("Support Bots_3").GetComponent<o_npcharacter>();

        characters[0] = GameObject.Find("Support Bots_1").GetComponent<o_npcharacter>();
        characters[1] = GameObject.Find("Support Bots_2").GetComponent<o_npcharacter>();
        characters[2] = GameObject.Find("Support Bots_3").GetComponent<o_npcharacter>();
    }

    bool checkifcharadead()
    {

        int deadcount = 0;
        for (int i = 0; i < characters.Length; i++)
        {
            if (characters[i] == null)
                return false;

            if (characters[i].characterstates == CHARACTER_STATEMACHINE.DEAD)
                deadcount++;
        }
        //print(deadcount);
        if (deadcount == 3) return true; else return false;
    }

    public override void AIFunction()
    {
        switch (characterstates)
        {

            case CHARACTER_STATEMACHINE.STAND:
                if (checkifcharadead())
                {
                    constant_regen = false;
                    target = AquireTarget(false);
                    if (target != null)
                    {
                        characterstates = CHARACTER_STATEMACHINE.MOVING;
                    }
                }
                break;

            case CHARACTER_STATEMACHINE.MOVING:

                switch (ROUTINES)
                {
                    case AI_STATES.IDLE:
                        SetAnimation(1);
                        direction_vec = LookAtTarget(target);
                        MoveCharacter(direction_vec);

                        if (DistanceToAttack(80, target, false))
                        {
                            direction_vec = -LookAtTarget(target);

                            Dash(0.5f, 0.3f, 0.1f);
                        }
                        if (DistanceToAttack(90, GameObject.FindWithTag("Bullet"), false))
                        {
                            if (!ai_delay)
                            {
                                ROUTINES = AI_STATES.COUNTER_ATTK;
                                Dash(0.3f, 0.3f, 0.1f);
                            }
                        }
                        if (DistanceToAttack(175, target, false))
                        {
                            StopCharacter();
                            switch (current_health_phase)
                            {
                                case 0:
                                    ChooseAttack(0);
                                    break;

                                case 1:
                                    ChooseAttack(Random.Range(0, 2));
                                    break;

                                case 2:
                                    ChooseAttack(Random.Range(0, 3));
                                    break;
                            }
                        }
                        break;

                    case AI_STATES.PEACEMAKER_GUN:

                    case AI_STATES.COUNTER_ATTK:
                        Attack(0.5f, 0.7f);
                        break;

                    case AI_STATES.MACHINE_GUN:

                        StopCharacter();
                        Attack(3.2f, 1f);
                        break;

                    case AI_STATES.SHOT_GUN:

                        StopCharacter();
                        Attack(0.7f, 1.5f);
                        break;

                    case AI_STATES.BACK_AWAY:

                        SetAnimation(1);
                        Speed = initialSpeed + 50;
                        //tar_last_pos = target.transform.position;
                        MoveCharacter(direction_vec);

                        if (CheckIfCornered(direction_vec))
                        {
                            Speed = initialSpeed;
                            ROUTINES = AI_STATES.IDLE;
                        }
                        if (!walkback)
                        {
                            ROUTINES = AI_STATES.IDLE;
                        }
                        break;
                }
                break;

            case CHARACTER_STATEMACHINE.ATTACKING:
                switch (ROUTINES)
                {
                    case AI_STATES.PEACEMAKER_GUN:
                    case AI_STATES.COUNTER_ATTK:
                        StartCoroutine(Peacemaker());
                        break;

                    case AI_STATES.MACHINE_GUN:
                        StartCoroutine(MachineGun());
                        break;

                    case AI_STATES.SHOT_GUN:
                        StartCoroutine(Shotgun());
                        break;
                }

                characterstates = CHARACTER_STATEMACHINE.ATTACK_ANIMATION;
                break;

            case CHARACTER_STATEMACHINE.DEAD:
                SetAnimation(6);
                break;

        }
    }

    new void Update ()
    {
        if (!ai_delay)
        {
            StartCoroutine(EvadeDelay());
        }
        base.Update();
	}

    float CalcDashTime()
    {
        return (CalcDist(target)).magnitude / initialSpeed;
    }

    IEnumerator EvadeDelay()
    {
        ai_delay = true;
        float loop = 5f;
        while (loop > 0)
        {
            loop = loop - Time.deltaTime;
            yield return new WaitForSeconds(Time.deltaTime);
        }
        ai_delay = false;
    }

    IEnumerator Peacemaker()
    {
        SetAnimation(0);
        yield return new WaitForSeconds(0.1f);
        SetAnimation(5);
        attack_pow = 2;
        PlayEffect("shoot_spark");
        SoundManager.SFX.playSound(pistol);
        ShootBullet("bullet", Random.Range(-2, 2), 0.4f, 50);
        yield return new WaitForSeconds(0.1f);
        SetAnimation(0);
    }

    IEnumerator MachineGun()
    {
        SetAnimation(5);
        attack_pow = 1;
        yield return new WaitForSeconds(0.4f);
        for (int i = 0; i < 15; i++)
        {
            PlayEffect("shoot_spark");
            SoundManager.SFX.playSound(SoundManager.SFX.LoadAudio("Kazzi_shoot"));
            ShootBullet("bullet", Random.Range(-0.7f, 0.7f), 0.4f, 110f);
            direction_vec = LookAtTarget(target);
            yield return new WaitForSeconds(0.1f);
        }
        SetAnimation(0);
        yield return new WaitForSeconds(0.3f);
    }

    IEnumerator Shotgun()
    {
        yield return new WaitForSeconds(0.1f);
        SetAnimation(5);
        attack_pow = 4;
            PlayEffect("shoot_spark");
        SoundManager.SFX.playSound(shotgun);

        ShootBullet("bullet", -15f, 0.45f, 70, true);
        ShootBullet("bullet", 15f, 0.45f, 70, true);
        ShootBullet("bullet", 0, 0.45f, 70, true);
        yield return new WaitForSeconds(0.5f);
    }

    public override void ChooseAttack(int attack_enum)
    {
        base.ChooseAttack(attack_enum);
        switch (attack_enum)
        {
            case 0:
                
                ROUTINES = AI_STATES.PEACEMAKER_GUN;
                break;

            case 1:
                
                ROUTINES = AI_STATES.MACHINE_GUN;
                break;

            case 2:

                ROUTINES = AI_STATES.SHOT_GUN;
                break;
                
        }
    }

    public override void AfterAttack()
    {
        SetAnimation(0);
        switch (ROUTINES)
        {
            case AI_STATES.COUNTER_ATTK:
                ROUTINES = AI_STATES.IDLE;
                break;
        }
        if (!walkback)
        {
            direction_vec = -(LookAtTarget(target) + (Vector3)new Vector2(direction_vec.x + Random.Range(-0.6f, 0.6f), direction_vec.y + Random.Range(-0.6f, 0.6f))).normalized;
            ROUTINES = AI_STATES.BACK_AWAY;
            StartCoroutine(WalkBack(0.7f));
        }else
        ROUTINES = AI_STATES.IDLE;
        characterstates = CHARACTER_STATEMACHINE.MOVING;
    }

    public override void AfterDash()
    {
        SetAnimation(0);
        ROUTINES = AI_STATES.IDLE;
        switch (ROUTINES)
        {
            case AI_STATES.COUNTER_ATTK:
                StopCharacter();
                direction_vec = LookAtTarget(target);

                Attack(0.5f, 0.7f);
                break;

        }
        if (!walkback)
        {
            direction_vec = -(LookAtTarget(target) + (Vector3)new Vector2(direction_vec.x + Random.Range(-0.6f, 0.6f), direction_vec.y + Random.Range(-0.6f, 0.6f))).normalized;
            ROUTINES = AI_STATES.BACK_AWAY;
            StartCoroutine(WalkBack(0.7f));
        }
        characterstates = CHARACTER_STATEMACHINE.MOVING;
    }

    public override void DashFunction()
    {
        SetAnimation(4);
    }
}
