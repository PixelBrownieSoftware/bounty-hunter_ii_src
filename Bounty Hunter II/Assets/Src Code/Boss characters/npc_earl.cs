using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class npc_earl : o_npcharacter, IpoolObject
{
    public AudioClip fire;

    public enum AI_STATES
    {
        ATTACK_PLAYER,
        BACK_AWAY,
        SLASH_FIRST,
        SLASH_SECOND,
        LUNGE_FIRST,
        LUNGE_SECOND,
        MACHINEGUN,
        GERNADE
    };
    public AI_STATES ROUTINES;

    void IpoolObject.SpawnStart()
    {
        maxHealth = 120;
        initialSpeed = 160f;
        attack_pow = 2;
        SetAttackObject();
        destroyOnDeath = false;
        Initialize();
        en_phase.Add(new s_enemyphase(0.7f));
        en_phase.Add(new s_enemyphase(0.65f));
        en_phase.Add(new s_enemyphase(0.4f));
        en_phase.Add(new s_enemyphase(0.25f));

        hurt_sound = new AudioClip[5];
        hurt_sound[0] = SoundManager.SFX.LoadAudio("earl_hurt1");
        hurt_sound[1] = SoundManager.SFX.LoadAudio("earl_hurt2");
        hurt_sound[2] = SoundManager.SFX.LoadAudio("earl_hurt3");
        hurt_sound[3] = SoundManager.SFX.LoadAudio("earl_hurt4");
    }

    private void Start()
    {
        gui_player.othercharacter = new o_npcharacter[1];
        gui_player.othercharacter[0] = this;
    }

    new void Update ()
    {
        base.Update();

        switch (characterstates)
        {
            case CHARACTER_STATEMACHINE.STAND:

                if (target == null)
                {
                    target = target_characters[0];
                }
                characterstates = CHARACTER_STATEMACHINE.MOVING;


                break;

            case CHARACTER_STATEMACHINE.MOVING:
                Invinciblity = false;
                switch (ROUTINES)
                {
                    case AI_STATES.ATTACK_PLAYER:

                        direction_vec = LookAtTarget(target);
                        MoveCharacter(direction_vec);

                        if (DistanceToAttack(125, target, false))
                        {
                            switch (current_health_phase)
                            {
                                case 0:

                                    ChooseAttack(Random.Range(0, 2));
                                    break;

                                case 1:

                                    ChooseAttack(Random.Range(0, 2));
                                    break;

                                case 2:
                                    ChooseAttack(Random.Range(0, 3));
                                    break;
                                case 3:

                                    ChooseAttack(Random.Range(0, 4));
                                    break;
                            }
                        }
                            break;

                    case AI_STATES.SLASH_FIRST:
                        StopCharacter();
                        ROUTINES = AI_STATES.SLASH_SECOND;
                        break;

                    case AI_STATES.SLASH_SECOND:
                        StopCharacter();
                        if (current_health_phase == 0)
                            Attack(0.2f, 0.8f);
                        else
                            Attack(0.3f, 0.4f);

                        break;

                    case AI_STATES.LUNGE_FIRST:

                        StopCharacter();
                        Speed = initialSpeed + 50;
                        direction_vec = LookAtTarget(target);
                        if (current_health_phase == 0)
                            Dash(0.35f, 0.15f, 0.4f);
                        else
                            Dash(0.38f, 0.15f, 0.2f);

                        break;

                    case AI_STATES.LUNGE_SECOND:

                        StopCharacter();
                        Speed = initialSpeed + 50;
                        direction_vec = LookAtTarget(target);
                        if (current_health_phase == 0)
                            Dash(0.35f, 0.6f, 0.4f);
                        else
                            Dash(0.38f, 0.4f, 0.2f);
                        break;

                    case AI_STATES.MACHINEGUN:
                        StopCharacter();
                        break;
                    case AI_STATES.GERNADE:
                        Attack(0.2f, 0.8f);
                        break;
                }
                break;

            case CHARACTER_STATEMACHINE.ATTACKING:
                switch (ROUTINES)
                {
                    case AI_STATES.SLASH_FIRST:
                        StartCoroutine(SlashAttack());
                        break;
                    case AI_STATES.SLASH_SECOND:
                        StartCoroutine(SlashAttack());
                        break;
                    case AI_STATES.MACHINEGUN:
                        StartCoroutine(MachineGun());
                        break;
                    case AI_STATES.GERNADE:
                        LayGernade();
                        break;

                }
                characterstates = CHARACTER_STATEMACHINE.ATTACK_ANIMATION;
                break;
                
        }

	}

    public override void ChooseAttack(int attack_enum)
    {
        base.ChooseAttack(attack_enum);
        switch (attack_enum)
        {
            case 0:

                ROUTINES = AI_STATES.SLASH_FIRST;
                if (current_health_phase == 0)
                    Attack(0.05f, 0.01f);
                else
                    Attack(0.1f, 0.01f);
                break;

            case 1:

                ROUTINES = AI_STATES.LUNGE_FIRST;
                break;

            case 2:

                if (!Attack(3f, 0.3f))
                {
                    return;
                }
                else { ROUTINES = AI_STATES.MACHINEGUN; }
                break;

            case 3:

                ROUTINES = AI_STATES.GERNADE;
                break;
        }
        
    }

    public override void AfterAttack()
    {
        switch (ROUTINES)
        {

            case AI_STATES.SLASH_SECOND:
            case AI_STATES.GERNADE:
            case AI_STATES.MACHINEGUN:
                ROUTINES = AI_STATES.ATTACK_PLAYER;
                StopCharacter();
                break;
        }
        characterstates = CHARACTER_STATEMACHINE.MOVING;
    }

    void LayGernade()
    {
        StopCharacter();
        ShootBullet("bmb", 0, 2.3f);

    }

    IEnumerator SlashAttack()
    {
        StopCharacter();
        yield return new WaitForSeconds(0.02f);
        attack.SetActive(true);
        Speed = initialSpeed + 325;
        direction_vec = LookAtTarget(target);
        MoveCharacter(direction_vec);
        yield return new WaitForSeconds(0.12f);
        Speed = initialSpeed;
        attack.SetActive(false);
        StopCharacter();
    }

    IEnumerator MachineGun()
    {
        attack_pow = 1;
        StopCharacter();
        for (int i = 0; i < Random.Range(20,50); i++)
        {
            direction_vec = LookAtTarget(target);
            SoundManager.SFX.playSound(fire);
            ShootBullet("bullet", Random.Range(-5, 5), 2.3f);
            yield return new WaitForSeconds(0.1f);
        }
        attack_pow = 2;
    }

    public override void AfterDash()
    {
        attack.SetActive(false);
        switch (ROUTINES)
        {

            case AI_STATES.LUNGE_FIRST:
                ROUTINES = AI_STATES.LUNGE_SECOND;
                StopCharacter();
                break;

            case AI_STATES.LUNGE_SECOND:

                ROUTINES = AI_STATES.ATTACK_PLAYER;
                StopCharacter();
                break;
        }
        characterstates = CHARACTER_STATEMACHINE.MOVING;
    }

    public override void DashFunction()
    {
        attack.SetActive(true);

    }
}
