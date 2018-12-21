using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class npc_supportbot : o_npcharacter, IpoolObject
{
    public enum AI_STATES
    {
        IDLE,
        LAZERS,
        BROKEN_LAZERS,
        BACK_AWAY
    };
    AI_STATES ROUTINES;

    void IpoolObject.SpawnStart()
    {
        maxHealth = 35;
        initialSpeed = 50f;
        attack_pow = 2;
        experience_to_give = 25;
        view_distance = 900;
        give_ammo = true;

        Initialize();
        en_phase.Add(new s_enemyphase(0.6f));
        en_phase.Add(new s_enemyphase(0.5f));
        en_phase.Add(new s_enemyphase(0.15f));
    }

    public override void AIFunction()
    {
        switch (characterstates)
        {
            case CHARACTER_STATEMACHINE.STAND:

                target = AquireTarget(false);
                if (target != null)
                {
                    ROUTINES = AI_STATES.IDLE;
                    characterstates = CHARACTER_STATEMACHINE.MOVING;
                }
                break;

            case CHARACTER_STATEMACHINE.MOVING:

                switch (ROUTINES)
                {
                    case AI_STATES.IDLE:

                        direction_vec = LookAtTarget(target);

                        switch (current_health_phase)
                        {
                            case 0:
                            case 1:
                                Speed = 100f;
                                MoveCharacter(direction_vec);
                                break;
                            case 2:
                                MoveCharacter(direction_vec + (Vector3)new Vector2(Random.Range(0, 2), Random.Range(0, 2)).normalized);

                                Speed = 175f; break;
                        }
                        if (DistanceToAttack(250, target, false))
                        {
                            switch (current_health_phase)
                            {
                                case 0:
                                case 1:
                                    ROUTINES = AI_STATES.LAZERS;
                                    break;

                                case 2:
                                    ChooseAttack(Random.Range(1, 3));
                                    break;
                            }
                        }
                        break;

                    case AI_STATES.BROKEN_LAZERS:
                        Attack(1.4f, 0.5f);
                        break;

                    case AI_STATES.LAZERS:
                        Attack(0.2f, 0.8f);
                        break;

                    case AI_STATES.BACK_AWAY:

                        SetAnimation(0);
                        MoveCharacter(direction_vec);

                        if (CheckIfCornered(direction_vec))
                            walkback = false;

                        if (!walkback)
                        {
                            StopCharacter();
                            ROUTINES = AI_STATES.IDLE;
                        }

                        break;
                }

                break;

            case CHARACTER_STATEMACHINE.ATTACKING:
                switch (ROUTINES)
                {
                    case AI_STATES.LAZERS:

                        StartCoroutine(ShootLazer());
                        break;

                    case AI_STATES.BROKEN_LAZERS:

                        StartCoroutine(ShootBrokenLazer());
                        break;
                }
                characterstates = CHARACTER_STATEMACHINE.ATTACK_ANIMATION;
                break;

            case CHARACTER_STATEMACHINE.DEAD:
                PlayExplosionEffect();
                break;
        }
    }

    public override void ChooseAttack(int attack_enum)
    {
        base.ChooseAttack(attack_enum);
        switch (attack_enum)
        {
            case 0:

                ROUTINES = AI_STATES.LAZERS;
                break;

            case 1:

                ROUTINES = AI_STATES.BROKEN_LAZERS;
                break;
            case 2:
                Vector2 vec = -LookAtTarget(target) + (Vector3)new Vector2(direction_vec.x + Random.Range(-0.6f, 0.6f), direction_vec.y + Random.Range(-0.6f, 0.6f));
                StartCoroutine(WalkBack(1));

                ROUTINES = AI_STATES.BACK_AWAY;
                break;
        }


    }

    new void Update ()
    {
        base.Update();
    }

    IEnumerator ShootBrokenLazer()
    {
        int it = 0;
        attack_pow = 1;
        StopCharacter();
        while (it < 4)
        {
            PlayEffect("shoot_spark");
            AudioClip cli = SoundManager.SFX.LoadAudio("Colt Python_shoot");
            SoundManager.SFX.playSound(cli);
            direction_vec = LookAtTarget(target);
            ShootBullet("bullet", Random.Range(-10, 10), 2.3f);
            yield return new WaitForSeconds(0.2f);
            it++;
        }
        attack_pow = 2;
    }

    IEnumerator ShootLazer()
    {
        StopCharacter();
        ShootBullet("lazr", 0, 0.2f);
        yield return new WaitForSeconds(Random.Range(0.1f, 0.6f));
        switch (current_health_phase)
        {
            case 0:
            case 1:
                yield return new WaitForSeconds(0.1f);
                break;

            case 2:
                yield return new WaitForSeconds(0.05f);
                break;
        }
    }

    public override void AfterAttack()
    {
        ROUTINES = AI_STATES.IDLE;
        characterstates = CHARACTER_STATEMACHINE.MOVING;
    }

    public override void AfterDash()
    {
    }

    public override void DashFunction()
    {
    }
}
