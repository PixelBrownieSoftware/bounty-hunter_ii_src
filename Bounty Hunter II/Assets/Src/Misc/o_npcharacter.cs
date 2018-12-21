using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class o_npcharacter : o_character, IpoolObject
{

    protected bool walkback = false;
    float walk_timer;   //a way to get the character to randomly move direction
    public o_character target;
    protected string leader_char;
    protected int experience_to_give;
    protected bool give_exp = true;
    protected bool give_ammo = false;
    bool spawneditemondeath = false;

    public int current_health_phase; //{ get; set; }

    internal List<s_enemyphase> en_phase = new List<s_enemyphase>();

    internal struct o_enemyweapon {
        public float damage;
        public float time;
    }

    [System.Serializable]
    internal struct s_enemyphase {

        public float divider;
        public s_enemyphase(float div) {
            divider = div;
        } 
    }

    o_enemyweapon cur_weap;

    protected o_character leader_obj {
        get {
            if (GameObject.Find(leader_char) != null){
                return GameObject.Find(leader_char).GetComponent<o_character>();
            }
            else {
                return null;
            }
        }

    }

    protected GameObject FindBullet()
    {
        return GameObject.FindWithTag("Bullet");
    }

    protected IEnumerator WalkBack(float timer)
    {
        walkback = true;
        float time = timer;
        Speed = initialSpeed + 60;
        while (time > 0)
        {
            if (walkback == false)
                break;
            time = time - Time.deltaTime;
            yield return new WaitForSeconds(Time.deltaTime);
        }
        Speed = initialSpeed;
        walkback = false;
    }

    protected void Walking() {
        if (walk_timer < 0) {
            direction_vec = new Vector2(Random.Range(-1, 1), Random.Range(-1, 1));
            if (direction_vec == new Vector3(0, 0)) {
                characterstates = CHARACTER_STATEMACHINE.STAND;
            }

        } else { walk_timer = Random.Range(0.5f, 2); }
    }

    public new void Initialize()
    {
        base.Initialize();
        hurt_del = 0.06f;
        en_phase.Clear();
    }

    void IpoolObject.SpawnStart()
    {
        spawneditemondeath = true;
        walkback = false;
        rbody2d = this.GetComponent<Rigidbody2D>();
    }

    public Vector2 RepelFromOthers()
    {
        Collider2D[] coll = Physics2D.OverlapBoxAll(transform.position, new Vector2(GetComponent<BoxCollider2D>().size.x, GetComponent<BoxCollider2D>().size.y), 0);

        foreach (Collider2D col in coll)
        {
            if (col != this.GetComponent<Collider2D>()) {
                Vector2 offset = (col.gameObject.transform.position - transform.position);
                Vector2 dir = Vector2.zero;
                dir -= offset.normalized / offset.sqrMagnitude;

                dir.Normalize();
                return dir;
            }
        }
        return Vector2.zero;
    }

    public bool CheckForChasm(Vector3 direction)
    {
        if (Physics2D.Linecast(transform.position, transform.position + (direction * 95), 2048))
        {
            return true;
        }
        return false;
    }

    public virtual void AIFunction()
    {

    }

    /// <summary>
    /// Put in a switch-case statement to choose the attack routine.
    /// </summary>
    /// <param name="attack_enum">The attack enum should also contain a random ranged number.</param>
    public virtual void ChooseAttack(int attack_enum) { }

    bool CheckForPlayerWeapons()
    {
        o_plcharacter p = GameObject.Find("Peast").GetComponent<o_plcharacter>();
        foreach (WeaponBase weap in p.weapons)
        {
            if (weap.ammoLimits)
            {
                return true;
            }
        }
        return false;
    }


    public new void Update()
    {
        /*
        if (Input.GetKeyDown(KeyCode.Z))
        {
            if(initialized)
            Health = 0;
        }*/
        base.Update();
        CheckDamage();
        //CollidingWithBullet();

        if (en_phase.Count > 0 && current_health_phase != en_phase.Count - 1)
        {
            if (Health < (maxHealth * en_phase[current_health_phase].divider))
            {
                current_health_phase++;
            }
        }
        if (Active)
        {
            AIFunction();
        }

        switch (characterstates) {
            case CHARACTER_STATEMACHINE.DASH_DELAY:
                if (dashTime <= 0)
                {
                    Invinciblity = false;
                    AfterDash();
                }
                break;

            case CHARACTER_STATEMACHINE.DEAD:
                if (!spawneditemondeath)
                {
                    if (destroyOnDeath )
                    {
                        PlayEffect("defeat_particle");
                        if (give_exp)
                        {
                            Transform items = GameObject.Find("Items").transform;
                            GameObject exp = ObjectPooler.instance.SpawnObject("Energy Orb", transform.position, Quaternion.identity, true);
                            exp.GetComponent<o_item>().QUANT = experience_to_give;
                            exp.transform.SetParent(items);
                            spawneditemondeath = true;
                            if (Random.Range(0, 50) < 6)
                            {
                                o_plcharacter p = GameObject.Find("Peast").GetComponent<o_plcharacter>();
                                print(p.maxHealth / p.Health);
                                if (p.maxHealth / p.Health < 15)
                                {
                                    GameObject medkit = ObjectPooler.instance.SpawnObject("Medical", transform.position, Quaternion.identity, true);
                                    medkit.transform.SetParent(items);
                                }

                            }
                        }

                        if (give_ammo)
                        {
                            if (CheckForPlayerWeapons())
                            {
                                GameObject exp = ObjectPooler.instance.SpawnObject("Ammo", transform.position, Quaternion.identity, true);
                                exp.transform.parent.SetParent(GameObject.Find("Items").transform);
                                
                                exp.GetComponent<o_item>().QUANT = Random.Range(1, 8);
                                spawneditemondeath = true;
                            }
                        }
                    }
                }

                break;
        }

        
    }

    internal bool CollidingWithBullet()
    {
        Collider2D col = CheckLayerCollision(4096, true);
        if (col != null)
        {
            if (!Invinciblity)
            {
                BulletClass bul = col.GetComponent<BulletClass>();
                target = bul.parent;
                bul.OnHitEntity();
            }
        }
        return false;
    }


    public void CheckForTarget(bool throughwalls) {
        if (target == null) {
            target = AquireTarget(throughwalls);
        } else {
            if (target.Health > 0)
            {
                direction_vec = LookAtTarget(target.CollisionBox.transform.position);
            } else {
                target = null;
            }
        }

    }

    //Take out the target that can be seen
    public o_character AquireTarget(bool seethroughwalls) {

        float lastdist = Mathf.Infinity;
        o_character targ = null;

        for (int i = 0; i < target_characters.Count; i++)
        {
            o_character pot_target = target_characters[i];
            if (pot_target.characterstates == CHARACTER_STATEMACHINE.DEAD) {
                continue;
            }
            if (DistanceToAttack(view_distance, pot_target, seethroughwalls)) {
                if (lastdist > Vector2.Distance(pot_target.transform.position, gameObject.transform.position))
                {
                    if (!pot_target.Active)
                        continue;
                    targ = pot_target;
                    lastdist = Vector2.Distance(pot_target.transform.position, gameObject.transform.position);
                }
            }
        }
        if(targ == null)
            return null;
        return targ;
    }

    public o_character AquireTarget(List<o_character> target_characters, bool throughwall)
    {
        o_character new_target = null;
        for (int i = 0; i < target_characters.Count; i++)
        {
            o_character pot_target = target_characters[i];
            if (DistanceToAttack(150, pot_target, throughwall) )
            {
                new_target = pot_target;
                return new_target;
            }
        }
        return null;
    }

    /// <summary>
    /// Checks if target is not obstructed by a wall.
    /// </summary>
    /*
    public bool CanSeeTarget(o_character target)
    {
        if (target == null)
            return false;
        if (target.CollisionBox == null)
            return false;
        Debug.DrawRay(transform.position , target.transform.position , Color.green);
        if (!Physics2D.Linecast(CollisionBox.transform.position + (Vector3)CollisionBox.offset, target.transform.position, layer1))
        {
            return true;
        }
        return false;
    }*/


    public Vector2 CalcDist(o_character o)
    {
        GameObject obj = o.gameObject;
        return obj.transform.position - transform.position;
    }
    public Vector2 CalcDist(GameObject obj)
    {
        return obj.transform.position - transform.position;
    }

    public Vector2 CalcDist(Vector3 obj)
    {
        return obj - transform.position;
    }
    
    public bool DistanceToAttack(float dist, GameObject obj, bool seethruwalls)
    {
        if (obj != null)
        {
            if (dist > Mathf.Abs(CalcDist(obj).x) && dist > Mathf.Abs(CalcDist(obj).y))
            {
                return true;
            }
        }
        return false;
    }
    public bool DistanceToAttack(float dist, o_character obj, bool seethruwalls)
    {
        if (obj != null)
        {
            if (dist > Vector2.Distance(obj.transform.position, transform.position))
            {
                return true;
            }
        }
        return false;
    }

    public float SetCharDirection(Vector3 currpos)
    {
        return 360 - Mathf.Atan2(currpos.x, currpos.y) * Mathf.Rad2Deg;
    }

    //Basicaly returns the whole direciton the character is looking in terms of vectors
    public Vector3 LookAtTarget(Vector3 target)
    {
        charDirection = SetCharDirection(new Vector3(target.x - transform.position.x, target.y - transform.position.y).normalized);
        return new Vector3(target.x - transform.position.x, target.y - transform.position.y).normalized;
    }
    public Vector3 LookAtTarget(o_character cha)
    {
        GameObject target = cha.gameObject;
        charDirection = SetCharDirection(new Vector3(target.transform.position.x - transform.position.x, target.transform.position.y - transform.position.y).normalized);
        return new Vector3(target.transform.position.x - transform.position.x, target.transform.position.y - transform.position.y).normalized;
    }

}