using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pjtl_boomerang : BulletClass, IpoolObject
{
    o_character targ;

    public override void InitializeBullet()
    {
        timed_obj = true;
        delay = 0.5f;
        attackPower = 1;
        speed = 110f;
        print(parent);
        rbody.velocity = shoot_direction.normalized * speed * 14 * Time.deltaTime;
        if (parent != null)
        {
            if (AquireTarget() != null)
            {

                targ = AquireTarget();
                shoot_direction = (targ.transform.position - transform.position).normalized;
                rbody.velocity = shoot_direction.normalized * speed * 14 * Time.deltaTime;
            }
        }
        

    }

    public override void OnHitEntity()
    {
        shoot_direction = (parent.transform.position - transform.position).normalized;
        rbody.velocity = shoot_direction.normalized * speed * 14 * Time.deltaTime;
        hit_collision.enabled = false;
    }

    public override void OnBulletDestroy()
    {
        if (parent.GetComponent<o_plcharacter>()) {
            for (int i = 0; i < parent.GetComponent<o_plcharacter>().weapons.Count; i++)
            {
                if (parent.GetComponent<o_plcharacter>().weapons[i].name == "Boomerang")
                {
                    parent.GetComponent<o_plcharacter>().weapons[i].ammoCap++;
                }
            }
        }
        
    }

    new void Update () {
        base.Update();

        if (delay <= 0.2f)
        {
            shoot_direction = (parent.transform.position - transform.position).normalized;
            rbody.velocity = shoot_direction.normalized * speed * 14 * Time.deltaTime;
        }
        transform.Translate(rbody.velocity);
        
    }

    public o_character AquireTarget()
    {
        if (parent.target_characters.Count > 0)
        {
            for (int i = 0; i < parent.target_characters.Count; i++)
            {
                o_character pot_target = parent.target_characters[i];
                if (pot_target.characterstates == o_character.CHARACTER_STATEMACHINE.DEAD)
                {
                    continue;
                }
                if (Mathf.Abs(pot_target.transform.position.x - transform.position.x) < 250 && Mathf.Abs(pot_target.transform.position.y - transform.position.y) < 250)
                {
                    return pot_target;
                }
            }
        }
        
        return null;
    }

}
