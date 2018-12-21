using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class npc_scorpion : o_npcharacter, IpoolObject
{

    public enum AI_STATES
    {
        IDLE,
        SLASH,
        HIDING,
        VENOM
    };
    public AI_STATES ROUTINES;

    void IpoolObject.SpawnStart()
    {
        maxHealth = 5;
        initialSpeed = 90;
        attack_pow = 1;
        experience_to_give = 4;
        view_distance = 450;
        SetAttackObject();
        Initialize();
    }

    public override void AIFunction()
    {
        switch (characterstates)
        {
            case CHARACTER_STATEMACHINE.STAND:
                CheckForTarget(false);
                SetAnimation(0);
                if (target != null)
                {
                    if (DistanceToAttack(view_distance, target, false))
                    {
                        SetAnimation(1);
                        ROUTINES = AI_STATES.IDLE;
                        characterstates = CHARACTER_STATEMACHINE.MOVING;
                    }
                }
                break;

            case CHARACTER_STATEMACHINE.MOVING:

                switch (ROUTINES)
                {
                    case AI_STATES.IDLE:

                        if(target.characterstates == CHARACTER_STATEMACHINE.DEAD)
                        {
                            StopCharacter();
                            characterstates = CHARACTER_STATEMACHINE.STAND;
                        }

                        SetAnimation(1);
                        direction_vec = LookAtTarget(target);
                        MoveCharacter(direction_vec);

                        if (DistanceToAttack(120, target, false))
                        {
                            ChooseAttack(Random.Range(0,2));
                        }

                        break;

                    case AI_STATES.HIDING:

                        shadow.gameObject.SetActive(false);
                        SetAnimation(7);
                        Invinciblity = true;
                        MoveCharacter(direction_vec * 2);
                        
                        if (attkdelay <= 0)
                        {
                            direction_vec = LookAtTarget(target);
                            shadow.gameObject.SetActive(true);
                            ChooseAttack(0);
                        }


                        break;
                }

                break;

            case CHARACTER_STATEMACHINE.ATTACKING:
                ShootBullet("bullet", 0, 0.5f);
                characterstates = CHARACTER_STATEMACHINE.ATTACK_ANIMATION;
                break;
        }
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
                if (dashDelay > 0)
                    return;

                Invinciblity = false;
                thisrender.color = Color.white;
                StopCharacter();
                Dash(1.2f, 0.4f, 0.2f);
                break;

            case 1:
                if (attkdelay > 0)
                    return;
                direction_vec = -(LookAtTarget(target) + (Vector3)new Vector2(direction_vec.x + Random.Range(-0.6f, 0.6f), direction_vec.y + Random.Range(-0.6f, 0.6f))).normalized;
                attkdelay = 1.26f;
                ROUTINES = AI_STATES.HIDING;
                break;

            case 2:

                if (attkdelay <= 0)
                    return;

                Invinciblity = false;
                thisrender.color = Color.white;
                StopCharacter();
                Attack(0.8f, 0.5f);
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
        DisableAttack();
        SetAnimation(0);
        ROUTINES = AI_STATES.IDLE;
        characterstates = CHARACTER_STATEMACHINE.MOVING;
    }

    public override void DashFunction()
    {
        EnableAttack();
        SetAnimation(5);
    }
    
}
