using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class npc_viper : o_npcharacter, IpoolObject
{

    public enum AI_STATES
    {
        IDLE,
        ATTACK,
        VENOM,
        BACK_AWAY
    }
    public AI_STATES ROUTINES;

    void IpoolObject.SpawnStart()
    {
        attack_pow = 1;
        maxHealth = 5;
        view_distance = 460;
        experience_to_give = 3;
        initialSpeed = 175;
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
                        ROUTINES = AI_STATES.ATTACK;
                        characterstates = CHARACTER_STATEMACHINE.MOVING;
                    }
                }
                break;

            case CHARACTER_STATEMACHINE.MOVING:

                switch (ROUTINES)
                {
                    case AI_STATES.ATTACK:

                        if (target.characterstates == CHARACTER_STATEMACHINE.DEAD)
                        {
                            characterstates = CHARACTER_STATEMACHINE.STAND;
                            ROUTINES = AI_STATES.IDLE;
                        }
                        direction_vec = LookAtTarget(target);
                        MoveCharacter(direction_vec);
                        SetAnimation(1);

                        if (DistanceToAttack(view_distance / 2, target, false))
                        {
                            ChooseAttack(Random.Range(0, 2));
                        }
                        break;

                    case AI_STATES.VENOM:
                        SetAnimation(0);
                        Attack(0.3f, 1.2f);
                        break;

                    case AI_STATES.BACK_AWAY:
                        SetAnimation(1);
                        Speed = initialSpeed + 50;
                        direction_vec = -LookAtTarget(target);
                        MoveCharacter(direction_vec);

                        if (attkdelay == 0)
                        {
                            Speed = initialSpeed;
                            ROUTINES = AI_STATES.ATTACK;
                        }

                        break;
                }

                break;

            case CHARACTER_STATEMACHINE.ATTACKING:
                StartCoroutine(Venom());
                characterstates = CHARACTER_STATEMACHINE.ATTACK_ANIMATION;
                break;
        }
    }

    new private void Update()
    {
        base.Update();
    }

    public override void ChooseAttack(int attack_enum)
    {
        switch (attack_enum)
        {
            case 0:

                StopCharacter();
                Dash(0.3f, 1.4f, 0.5f);
                break;

            case 1:
                if (DistanceToAttack(130, target, false))
                {
                    StopCharacter();
                    ROUTINES = AI_STATES.VENOM;
                }
                else return;
                break;
        }
    }

    IEnumerator Venom()
    {
        SetAnimation(5);
        yield return new WaitForSeconds(0.3f);
        ShootBullet("bullet", 0, 0.6f);
    }

    public override void AfterAttack()
    {
        SetAnimation(5);
        switch (ROUTINES)
        {
            case AI_STATES.VENOM:

                ROUTINES = AI_STATES.BACK_AWAY;
                break;
        }
        characterstates = CHARACTER_STATEMACHINE.MOVING;
    }

    public override void AfterDash()
    {
        ROUTINES = AI_STATES.ATTACK;
        characterstates = CHARACTER_STATEMACHINE.MOVING;
    }

    public override void DashFunction()
    {
        SetAnimation(3);

    }
}