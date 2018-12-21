using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class WeaponBase
{
    public string name;
    public string bulletName;

    public int ammoitemquant_max;
    public int ammoitemquant_min;
    public int level;
    public float attackPow;
    public float[] levelAttackPow;
    public int[] expToNextLevel;
    public int exp;

    public bool auto;
    public float delay;
    public float attacktime;
    public bool noLevel;public int ammo_max;
    public bool ammoLimits;
    public int ammoCap;

    public WeaponBase(string name, string bulletname, int[] exp_to_nl, float attktime, float delay, int ammocap, float[] levelAttackPow, int maxammo)
    {
        ammoCap = ammocap;
        this.delay = delay;
        this.name = name;
        this.levelAttackPow = levelAttackPow != null ? levelAttackPow : null;
        bulletName = bulletname;
        expToNextLevel = exp_to_nl;
        attacktime = attktime;
        ammoLimits = ammocap != 0 ? true : false;
        noLevel = expToNextLevel != null ? false : true;
        attackPow = levelAttackPow != null ? levelAttackPow[0] : 1;
        ammo_max = maxammo;
        level = 1;
    }
    public WeaponBase(string name, string bulletname, int[] exp_to_nl, float attktime, float delay, int ammocap, float[] levelAttackPow, int attackpow, int maxammo)
    {
        ammoCap = ammocap;
        this.delay = delay;
        this.name = name;
        this.levelAttackPow = levelAttackPow != null ? levelAttackPow : null;
        bulletName = bulletname;
        expToNextLevel = exp_to_nl;
        attacktime = attktime;
        ammoLimits = ammocap != 0 ? true : false;
        noLevel = expToNextLevel != null ? false : true;
        attackPow = attackpow;
        ammo_max = maxammo;
        level = 1;
    }

    public void Update()
    {
        if (!noLevel)
        {
            if (level < expToNextLevel.Length)
            {
                if (exp >= expToNextLevel[level])
                {
                    LevelUp();
                }
            }
        }

        if (ammoLimits && ammo_max <= ammoCap)
        {
            ammoCap = ammo_max;
        }
    }

    public void LevelUp()
    {
        if (level < expToNextLevel.Length + 1)
        {
            level++;
            attackPow = levelAttackPow[level];
        }
    }

    public int AmmoUse()
    {
        if (ammoLimits && ammoCap > 0) { ammoCap--; }
        return ammoCap;
    }
}
[System.Serializable]
public abstract class o_plcharacter : o_character, IpoolObject {

    protected float player_invinciblity_time = 0f;
    protected bool temp_invinciblity;
    [HideInInspector]
    public int weaponNum = 0;
    [HideInInspector]
    public WeaponBase selectedWeapon;
    public List<WeaponBase> weapons = new List<WeaponBase>();


    float dash_tim_midpoint;
    public AudioClip dash;
    
    void IpoolObject.SpawnStart()
    {
        Initialize();
    }

    public sealed override void AfterAttack()
    {
        characterstates = CHARACTER_STATEMACHINE.STAND;
    }
    
    public void bulletSwitch() {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (weaponNum == 0) {
                weaponNum = weapons.Count + 1;
            } else {
                weaponNum--;
            }
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            weaponNum++;
        }
        
        weaponNum = weaponNum % weapons.Count;
        selectedWeapon = weapons[weaponNum];
        selectedWeapon.Update();
    }

    protected void Dash(float dash_tim, float dash_del)
    {
        //This is for storing the dash timer start variable
        dash_tim_midpoint = dash_tim;
        //So that the player can cancel their dash quickly when the dash time reaches half that variable
        base.Dash(dash_tim,dash_del, 0f);
    }

    public new void Update()
    {
        base.Update();
        
        attack_pow = selectedWeapon.attackPow;
        CheckDamage();
        int horiz = (int)Input.GetAxisRaw("Horizontal");
        int vert = (int)Input.GetAxisRaw("Vertical");

        if (weapons.Count > 0)
            bulletSwitch();
        
        if (Active)
        {
            switch (characterstates)
            {
                case CHARACTER_STATEMACHINE.STAND:
                    if (horiz != 0 || vert != 0)
                    {
                        direction_vec = new Vector2(horiz, vert);
                    }
                    ToggleDirectionLock(false);
                    SetAnimation(0);
                    if (new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")) != new Vector2(0, 0) && (attkdelay < 0.1))
                    {
                        characterstates = CHARACTER_STATEMACHINE.MOVING;
                    }

                    ShootGun();
                    break;

                case CHARACTER_STATEMACHINE.MOVING:

                    if (horiz != 0 || vert != 0)
                    {
                        direction_vec = new Vector2(horiz, vert);
                    }
                    ToggleDirectionLock(false);
                    MoveCharacter(new Vector2(direction_vec.x, direction_vec.y));

                    SetAnimation(1);
                    ShootGun();

                    if (Input.GetKeyDown(KeyCode.Space) && dashDelay <= 0 && !temp_invinciblity)
                    {
                        if (!Input.GetKeyDown(KeyCode.LeftArrow) && !Input.GetKeyDown(KeyCode.RightArrow))
                        {
                            SetAnimation(4);
                            ToggleDirectionLock(true);
                            Dash(0.35f, 0.50f);
                        }
                    }
                    if (new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")) == new Vector2(0, 0))
                        characterstates = CHARACTER_STATEMACHINE.STAND;

                    break;

                case CHARACTER_STATEMACHINE.DASH_DELAY:
                    if (dashTime >= 0)
                    {
                        if (dashTime <= dash_tim_midpoint * 0.75)
                        {
                            if (!Input.GetKey(KeyCode.Space))
                            {
                                dashTime = 0;
                                Invinciblity = false;
                                characterstates = CHARACTER_STATEMACHINE.MOVING;
                            }
                        }
                    }
                    break;

                case CHARACTER_STATEMACHINE.ATTACKING:
                    StopCharacter();
                    FireGun();
                    ToggleDirectionLock(true);
                    characterstates = CHARACTER_STATEMACHINE.ATTACK_ANIMATION;
                    break;

                case CHARACTER_STATEMACHINE.DEAD:

                    SetAnimation(6);
                    break;

            }
        }
    }

    void ShootGun()
    {
        if (selectedWeapon.ammoLimits && selectedWeapon.ammoCap == 0)
            return;

        Vector2 vectorshoot = (Camera.main.transform.GetChild(0).GetComponent<Camera>().ScreenToWorldPoint(Input.mousePosition) - transform.position).normalized;
        if (selectedWeapon.auto)
        {
            if (Input.GetMouseButton(0))
            {
                shoot_direction = vectorshoot;
                Attack(selectedWeapon.attacktime, selectedWeapon.delay); direction_vec = vectorshoot;
            }
        }
        else
        {
            if (Input.GetMouseButtonDown(0))
            {
                shoot_direction = vectorshoot;
                Attack(selectedWeapon.attacktime, selectedWeapon.delay); direction_vec = vectorshoot;
            }
        }
    }

    

    
    public abstract void FireGun();
    
}
