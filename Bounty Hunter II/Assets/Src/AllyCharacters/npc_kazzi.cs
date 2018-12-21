using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class npc_kazzi : o_npcharacter, IpoolObject {
    
    public AudioClip heal_sound;
    GameObject bite;
    public AudioClip fire;
    public AudioClip dash;

    public enum KAZZI_ACTIONS {
        IDLE,
        ATTACK,
        HEAL_LEADER,
        DASH_BACK,
        SHOCK_GUN
    };
    public KAZZI_ACTIONS ROUTINE;

    void IpoolObject.SpawnStart()
    {
        constant_regen = true;
        destroyOnDeath = false;
        leader_char = "Peast";
        maxHealth = 10;
        attack_pow = 1;
        view_distance = 900;
        initialSpeed = 165f;
        Initialize();

        hurt_del = 0.08f;
        ROUTINE = KAZZI_ACTIONS.IDLE;
    }
    
    IEnumerator Shoot()
    {
        ShootBullet("bullet", 0, 0.5f);
        SoundManager.SFX.playSound(fire);
        yield return new WaitForSeconds(0.08f);
        ShootBullet("bullet", 0, 0.5f);
        SoundManager.SFX.playSound(fire);
        yield return new WaitForSeconds(0.08f);
        ShootBullet("bullet", 0, 0.5f);
        SoundManager.SFX.playSound(fire);
        yield return new WaitForSeconds(0.08f);
    }

    IEnumerator GainConsiousness()
    {
        yield return new WaitForSeconds(10f);
        Health = maxHealth;
        characterstates = CHARACTER_STATEMACHINE.STAND;
    }

    new void Update()
    {
        view_distance = 315;
        base.Update();
        switch (characterstates)
        {
            
            case CHARACTER_STATEMACHINE.STAND:
                CheckForTarget(false);

                if (target != null)
                {
                    if (DistanceToAttack(view_distance, target, false))
                    {
                        if (target != leader_obj)
                            ROUTINE = KAZZI_ACTIONS.ATTACK;

                        characterstates = CHARACTER_STATEMACHINE.MOVING;
                    }

                }
                else
                    target = leader_obj;

                break;

            case CHARACTER_STATEMACHINE.MOVING:

                switch (ROUTINE) {

                    case KAZZI_ACTIONS.IDLE:
                        //Follow player if there's no target in sight
                    
                        if (AquireTarget(false) == null)
                        {
                            if (leader_obj.gameObject.activeSelf) {

                                target = leader_obj;

                                if (!DistanceToAttack(40, leader_obj, false))
                                {
                                    direction_vec = LookAtTarget(target.transform.position);
                                    if (CheckForChasm(direction_vec) == false)
                                        MoveCharacter(direction_vec);
                                    else {
                                        if (CheckLayerCollision(2048, false, leader_obj.gameObject.transform.position)) // If they are still in the ditch for whatever reason.
                                            MoveCharacter(-direction_vec);

                                        StopCharacter();
                                    }
                                }
                                else { characterstates = CHARACTER_STATEMACHINE.STAND; }

                                if (!DistanceToAttack(view_distance, leader_obj, false)) {
                                    if(CheckLayerCollision(2048, false, leader_obj.gameObject.transform.position) == null)
                                        transform.position = leader_obj.gameObject.transform.position;
                                }
                            }
                        } else {
                            target = AquireTarget(false);
                            ROUTINE = KAZZI_ACTIONS.ATTACK;
                        }

                        if (DistanceToAttack(40, leader_obj, false) && leader_obj.Health < leader_obj.maxHealth * 0.75) {
                            GameObject.Find("Peast").GetComponent<o_character>().Health++;
                            SoundManager.SFX.playSound(heal_sound);
                        }
                        break;

                    case KAZZI_ACTIONS.ATTACK:
                        if ((target == null || target.Health <= 0) || target == leader_obj) {

                            Back();
                        }
                        else {

                            if (DistanceToAttack(view_distance, target, false))
                            {
                                tar_last_pos = target.transform.position;
                                direction_vec = LookAtTarget(target.transform.position);
                                if (CheckForChasm(direction_vec) == false)
                                {
                                    MoveCharacter(direction_vec);

                                    if (DistanceToAttack(50, target, false))
                                    {
                                        if (target.characterstates == CHARACTER_STATEMACHINE.ATTACKING || target.characterstates == CHARACTER_STATEMACHINE.DASH_DELAY || target.characterstates == CHARACTER_STATEMACHINE.ATTACK_ANIMATION)
                                        {
                                            SoundManager.SFX.playSound(dash);
                                            direction_vec = -LookAtTarget(target);
                                            Dash(0.25f, 0.50f, 0f);
                                        }
                                    }

                                    if (DistanceToAttack(150, target, false))
                                    {
                                        StopCharacter();
                                        Attack(0.5f, 0.6f);
                                    }
                                }
                                else {
                                    characterstates = CHARACTER_STATEMACHINE.STAND;
                                }
                                
                            }
                            else {
                                Back();
                            }
                        }
                        break;

                    case KAZZI_ACTIONS.DASH_BACK:

                        break;
                }
                break;

            case CHARACTER_STATEMACHINE.ATTACKING:

                //bite.SetActive(true);
                StartCoroutine(Shoot());
                characterstates = CHARACTER_STATEMACHINE.ATTACK_ANIMATION;
                break;

            case CHARACTER_STATEMACHINE.DEAD:
                StartCoroutine(GainConsiousness());
                break;

        }
    }

    void Back()
    {
        target = null; ROUTINE = KAZZI_ACTIONS.IDLE; characterstates = CHARACTER_STATEMACHINE.MOVING;
    }

    public override void AfterDash()
    {
        ROUTINE = KAZZI_ACTIONS.IDLE;
    }

    public override void AfterAttack()
    {
        characterstates = CHARACTER_STATEMACHINE.MOVING;
    }

    public override void DashFunction()
    {
    }

}