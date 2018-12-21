using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class boss_milbert : o_npcharacter, IpoolObject
{

	public enum GOBLIN_AI
    {
        NONE,
        DASH_ATTACK,
        DASH_COUNTER ,
        BACK_UP,
    }
	public GOBLIN_AI AI;

	GameObject bulletToDodge;
	public GameObject CutsceneUponDeath;

    AudioClip hurtGob;
    AudioClip dieGob;
    AudioClip dodgeGob;
    AudioClip attkGob;

    AudioClip dodge_sound;

    float attacktimer = 0;

    private void Start()
    {
        attack_pow = 1;
        SetAttackObject();
        DisableAttack();
    }

    void IpoolObject.SpawnStart()
    {
        gui_player.othercharacter = new o_npcharacter[1];
        gui_player.othercharacter[0] = this;
        maxHealth = 35f;
        initialSpeed = 150f;
        destroyOnDeath = false;


        dodge_sound = SoundManager.SFX.LoadAudio("milbert_dash_attack1");

        Initialize();
        en_phase.Add(new s_enemyphase(0.65f));
        en_phase.Add(new s_enemyphase(0.35f));
    }

    public override void AIFunction()
    {
        switch (characterstates)
        {
            case CHARACTER_STATEMACHINE.STAND:
                target = target_characters[0];

                SetAnimation(0);
                switch (AI)
                {
                    case GOBLIN_AI.DASH_COUNTER:

                        AI = GOBLIN_AI.NONE;
                        break;
                }

                if (target != null)
                {
                    //For now this will be set to none
                    characterstates = CHARACTER_STATEMACHINE.MOVING;
                }
                break;

            case CHARACTER_STATEMACHINE.MOVING:

                switch (AI)
                {
                    //Follow the player, this is basicaly the core of all the states Milbert's statemachine
                    case GOBLIN_AI.NONE:
                        if (target != null)
                        {
                            direction_vec = LookAtTarget(target);
                            MoveCharacter(direction_vec);

                            SetAnimation(1);
                            if (DistanceToAttack(175, target, false))
                            {
                                switch (current_health_phase)
                                {
                                    case 0:
                                        ChooseAttack(0);
                                        break;

                                    case 1:
                                        ChooseAttack(Random.Range(0, 2));
                                        break;
                                }

                            }
                        }
                        break;

                    case GOBLIN_AI.DASH_COUNTER:
                        
                        StopCharacter();
                        Dash(0.3f, 1.05f, 0.25f);

                        break;

                    case GOBLIN_AI.BACK_UP:

                        SetAnimation(1);
                        Speed = initialSpeed + 60;
                        MoveCharacter(direction_vec);

                        if (CheckIfCornered(direction_vec))
                        {
                            Speed = initialSpeed;
                            AI = GOBLIN_AI.NONE;
                        }

                        if (!walkback)
                        {
                            Speed = initialSpeed;
                            AI = GOBLIN_AI.NONE;
                        }
                        break;
                }
                break;

        }
    }

    void WalkBackInit()
    {
        attkdelay = 0.85f;
        direction_vec = -(LookAtTarget(target) + (Vector3)new Vector2(direction_vec.x + Random.Range(-0.6f, 0.6f), direction_vec.y + Random.Range(-0.6f, 0.6f))).normalized; 
    }

    new private void Update()
    {
        base.Update();

    }

    public override void ChooseAttack(int attack_enum)
    {
        base.ChooseAttack(attack_enum);

        switch (attack_enum)
        {
            case 0:
                switch (current_health_phase)
                {
                    case 0:
                    case 1:
                        //For the most part have a short dash unless Milbert is on very low health

                        direction_vec = LookAtTarget(target);
                        StopCharacter();
                        Dash(0.25f, 1.05f, 0.25f);
                        break;

                    case 2:
                        //Quicker dash on low health

                        direction_vec = LookAtTarget(target);
                        StopCharacter();
                        Dash(0.25f, 0.65f, 0.07f);
                        break;
                }
                break;


            case 1:
                //This is when Milbert dashes back, if he sees a wall in front of him, he will not preform the dash.
                if (CheckIfCornered(direction_vec * 9))
                {
                    return;
                }
                direction_vec = (LookAtTarget(target) + (Vector3)new Vector2(direction_vec.x + Random.Range(-0.6f, 0.6f), direction_vec.y + Random.Range(-0.6f, 0.6f))).normalized;
                AI = GOBLIN_AI.DASH_COUNTER;
                break;


            case 2:

                break;
        }

    }


    public override void AfterDash() {
        SetAnimation(0);
        StopCharacter();
        DisableAttack();
        if (!walkback)
        {
            AI = GOBLIN_AI.BACK_UP;
            direction_vec = -(LookAtTarget(target) + (Vector3)new Vector2(direction_vec.x + Random.Range(-0.6f, 0.6f), direction_vec.y + Random.Range(-0.6f, 0.6f))).normalized;

            StartCoroutine(WalkBack(0.5f)); 
        }
        characterstates = CHARACTER_STATEMACHINE.MOVING;
    }

    public override void DashFunction()
    {
        SetAnimation(3);
        EnableAttack();
        SoundManager.SFX.playSound(dodge_sound);

    }

    public override void AfterAttack()
    {
        characterstates = CHARACTER_STATEMACHINE.STAND;
    }
}