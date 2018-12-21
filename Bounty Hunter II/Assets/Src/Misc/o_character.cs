using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class s_anim_clip
{
    public s_anim_clip(dir dire, int anim_num, AnimationClip anim)
    {
        DIR = dire;
        animation_number = anim_num;
        animation = anim;
    }
    public AnimationClip animation;
    public enum dir
    {
        UP,
        DOWN,
        LEFT,
        RIGHT,
        NONE
    }
    public dir DIR;
    public int animation_number;
}

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
[System.Serializable]
public abstract class o_character : MonoBehaviour
{
    /*
     Base class for the characters in the game with health and stuff.
        0 = STAND
        1 = MOVING
        2 = ATTACKING
        3 = DASH_DELAY
        4 = DASHING
        5 = ATTACK_ANIMATION
        6 = DEAD
        7+ = CUSTOM
    */

    #region VARIABLES
    public bool initialized = false;
    public bool Active = true;
    public float Health = 1; //So characters don't die when uninitialized
    public float maxHealth = 1; //Ditto ^
    protected Vector2 shoot_direction;  //for players only
    public Vector3 direction_vec = new Vector3(0,-1);    //a methood to save the direction the character is facing
    protected bool is_arial = false;
    public bool isfalling_trap { get; set; }    //to stop constant fall calls

    public BoxCollider2D CollisionBox;
    protected bool lock_direction;
    protected float dashDelay = 0f;
    protected float dashTime = 0f;
    protected float pre_dash_tim = 0f;

    protected float attkdelay = 0f;
    protected float attk_time = 0;
    protected float hurt_del;
    protected float bullet_position_offset = 18;
    int animationState = 0;

    protected cameraScript cameraStuff;
    protected Vector3 tar_last_pos { get; set; }
    protected Vector3 before_fall_pos;
   
    public LayerMask layer1;
    protected SpriteRenderer shadow;

    [HideInInspector]
    public List<o_character> target_characters = new List<o_character>();
   // [HideInInspector]
    public List<s_anim_clip> animationclips = new List<s_anim_clip>();


    public enum CHARACTER_STATEMACHINE {
        STAND = 0,
        MOVING = 1,
        ATTACKING = 2,
        DASH_DELAY = 3,
        DASHING = 4,
        ATTACK_ANIMATION = 5,
        DEAD = 6,
        NOTHING = 7
    };
    public CHARACTER_STATEMACHINE characterstates;
    protected float attack_pow;

    protected BoxCollider2D hit_box;
    [HideInInspector]
    public float horizontalDir = 1f;
    [HideInInspector]
    public float verticalDir = -1f;
    [HideInInspector]
    public float view_distance { get; set; }

    public Dictionary<string, AudioClip> character_sounds = new Dictionary<string, AudioClip>();

    [HideInInspector]
    public AudioClip hurt;
    [HideInInspector]
    public AudioClip dashcrash;
    public AudioClip[] hurt_sound;

    public enum animations { idle, inProgress, complete }

    public bool Invinciblity;
    bool cutsceneObjCreated = false;
    protected bool constant_regen = false;

    protected bool destroyOnDeath = true;

    protected float Speed;
    protected float initialSpeed;
    protected float charDirection;
    protected SpriteRenderer thisrender;

    public Rigidbody2D rbody2d { get; set; }
    public Animator charAnimator { get; set; }

    public string cur_faction;
    public GameObject CutsceneOndeath;
    internal GameObject attack;

    protected bool hurtdone = true;
    public string type;
    float regentime = 0f;
    Color shadowcol = new Color(1, 1, 1, 0.5f);
    protected float maxregentime = 0.2f;

    #endregion

    #region INITIALIZATION

    protected void SetAttackObject()
    {
        attack = transform.Find("Attack").gameObject;
        attack.GetComponent<BulletClass>().parent = this;
        attack.GetComponent<BulletClass>().InitializeBullet();
        attack.GetComponent<BulletClass>().attackPower = attack_pow;
        attack.SetActive(false);
    }
    protected void SetAttackObject(string nameofobj)
    {
        attack = transform.Find(nameofobj).gameObject;
        attack.GetComponent<BulletClass>().parent = this;
        attack.GetComponent<BulletClass>().InitializeBullet();
        attack.GetComponent<BulletClass>().attackPower = attack_pow;
        attack.SetActive(false);
    }

    protected void CreateAfterimage()
    {
        GameObject obj = ObjectPooler.instance.SpawnObject("afterimage_effect", transform.position, true);
        if (thisrender != null)
        {

            if (Mathf.RoundToInt(direction_vec.x) == 1)
            {
                obj.transform.localRotation = Quaternion.Euler(0, 0, 0);
            }
            if (Mathf.RoundToInt(direction_vec.x) == -1)
            {
                obj.transform.localRotation = Quaternion.Euler(0, 180, 0);
            }
            SpriteRenderer spri = obj.GetComponent<SpriteRenderer>();
            spri.sprite = thisrender.sprite;
            spri.sortingOrder = (int)transform.position.y * -1;
        }
    }

    protected void Initialize() {

        initialized = true;
        hurtdone = true;
        CollisionBox = GetComponent<BoxCollider2D>();
        characterstates = CHARACTER_STATEMACHINE.STAND;
        thisrender = this.gameObject.GetComponent<SpriteRenderer>();
        if (GetComponent<Animator>() != null)
            charAnimator = GetComponent<Animator>();
        if (transform.Find("Shadow") != null)
        {
            shadow = transform.Find("Shadow").gameObject.GetComponent<SpriteRenderer>();
            shadow.color = shadowcol;
        }
        thisrender.color = Color.white;

        hit_box = transform.Find("character_hit_box").GetComponent<BoxCollider2D>();
        hurt = GetSoundEffect("impact_damage");
        dashcrash = GetSoundEffect("impact_crashwall");
        Invinciblity = false;
        Health = maxHealth;

        rbody2d = GetComponent<Rigidbody2D>();
        rbody2d.mass = 0.00001f;
        rbody2d.gravityScale = 0;
        rbody2d.bodyType = RigidbodyType2D.Dynamic;
        rbody2d.angularDrag = 0;
        rbody2d.constraints = RigidbodyConstraints2D.FreezeRotation;

        Speed = initialSpeed;
    }
    protected void InitializeNoHealthChange()
    {
        initialized = true;
        CollisionBox = GetComponent<BoxCollider2D>();
        characterstates = CHARACTER_STATEMACHINE.STAND;
        thisrender = gameObject.GetComponent<SpriteRenderer>();
        if (GetComponent<Animator>() != null)
            charAnimator = GetComponent<Animator>();
        
        if (transform.Find("Shadow") != null)
        {
            shadow = transform.Find("Shadow").gameObject.GetComponent<SpriteRenderer>();
            shadow.color = shadowcol;
        }
        hit_box = transform.Find("character_hit_box").GetComponent<BoxCollider2D>();
        hurt = GetSoundEffect("impact_damage");
        dashcrash = GetSoundEffect("impact_crashwall");
        Invinciblity = false;

        rbody2d = GetComponent<Rigidbody2D>();
        rbody2d.mass = 0.00001f;
        rbody2d.gravityScale = 0;
        rbody2d.bodyType = RigidbodyType2D.Dynamic;
        rbody2d.angularDrag = 0;
        rbody2d.constraints = RigidbodyConstraints2D.FreezeRotation;

        Speed = initialSpeed;
    }

    protected AudioClip GetSoundEffect(string soundname)
    {
        if (SoundManager.SFX == null)
            return null;
        else
            return SoundManager.SFX.LoadAudio(soundname);
    }

    public void AddTargets(List<o_character> targ) {
        List<string> keys = new List<string>(s_factionhandler.factionKeys);

        for (int i = 0; i < keys.Count; i++)
        {
            //They're on our side!
            if (keys[i] == cur_faction)
            {
                continue;
            } else {
                //However these guys aren't!
                for (int e = 0; e < s_factionhandler.faction[keys[i]].Count; e++)
                {
                    //Put the subject in here so I don't have to copy paste that load of crap below when it comes to adding in the enemies.
                    o_character subject = s_factionhandler.faction[keys[i]][e];

                    //Find any enemy that contains the same tag as the target
                    foreach (o_character c in targ) {
                        if (targ.Find(x => c.type.Contains(subject.type)))
                        {
                            target_characters.Add(c);
                        }
                    }

                }

            }
        }

    }
    #endregion

    #region HIT DETECION

    private void BeforeFall()
    {
        if (CheckLayerCollision(11, false) == null)
        {
            switch (characterstates)
            {
                case CHARACTER_STATEMACHINE.ATTACK_ANIMATION:
                case CHARACTER_STATEMACHINE.MOVING:
                case CHARACTER_STATEMACHINE.ATTACKING:
                    before_fall_pos = transform.position - (direction_vec.normalized * 5);
                    break;
            }
        }
    }

    private void DetectFall()
    {
        Collider2D collision = CheckLayerCollision(2048, false);
        if (collision != null && !is_arial)
        {
            if (collision.transform.parent.GetComponent<o_trap>().group_immunity != cur_faction)
            {
                switch (collision.transform.parent.GetComponent<o_trap>().TELEPORT_TRAP)
                {
                    case o_trap.TRAP_TYPE.NON_TELEPORT:
                        collision.gameObject.transform.parent.GetComponent<o_trap>().TeleportCharacter(this, before_fall_pos);
                        break;
                    case o_trap.TRAP_TYPE.TELEPORT_IF_NO_DAHSH:

                        if (shadow != null)
                            shadow.color = Color.clear;

                        if (!isfalling_trap && characterstates != CHARACTER_STATEMACHINE.DASHING
                    && characterstates != CHARACTER_STATEMACHINE.DASH_DELAY)
                        {
                            characterstates = CHARACTER_STATEMACHINE.NOTHING;
                            StopCharacter();
                            collision.gameObject.transform.parent.GetComponent<o_trap>().TeleportCharacter(this, before_fall_pos);

                            isfalling_trap = true;
                        }
                        break;
                }
            }
            
        }
        else if (shadow != null)
            shadow.color = shadowcol;
    }

    /*
    public Collider2D CheckSingleLayerCollision(int lar, bool use_hitbox)
    {
        Collider2D collided;
        //A replacement to constatnly using OnTrigger Functions
        if (hit_box == null)
            return null;

        if (!use_hitbox)
            collided = Physics2D.OverlapBox(new Vector2(transform.position.x + GetComponent<BoxCollider2D>().offset.x,
                transform.position.y + GetComponent<BoxCollider2D>().offset.y), new Vector2(GetComponent<BoxCollider2D>().size.x, 
                GetComponent<BoxCollider2D>().size.y), 0, lar);
        else
            collided = Physics2D.OverlapBox(transform.position, new Vector2(hit_box.size.x, hit_box.size.y), 0, lar);

        if (collided != null)
        {

            //You can't collide with yourself
            if (collided.name == this.name)
                return null;

            //You shouldn't be able to collide with your own bullets 
            if (collided.GetComponent<BulletClass>())
                if (collided.GetComponent<BulletClass>().parent != this)
                    if (collided.GetComponent<BulletClass>().parent.cur_faction != cur_faction)
                        if (!FreindlyFire(collided))
                            return collided;

        }
        
        return null;
    }

    public List<Collider2D> CheckLayerCollision(int lar, bool use_hitbox)
    {
        List<Collider2D> returners = null;
        Collider2D[] collided;
        //A replacement to constatnly using OnTrigger Functions
        if (hit_box == null)
            return null;

        if (!use_hitbox)
            collided = Physics2D.OverlapBoxAll(new Vector2(transform.position.x + GetComponent<BoxCollider2D>().offset.x,
                transform.position.y + GetComponent<BoxCollider2D>().offset.y), new Vector2(GetComponent<BoxCollider2D>().size.x,
                GetComponent<BoxCollider2D>().size.y), 0, lar);
        else
            collided = Physics2D.OverlapBoxAll(transform.position, new Vector2(hit_box.size.x, hit_box.size.y), 0, lar);


        if (collided != null)
        {
            for (int i = 0; i < collided.Length; i++)
            {
                //You can't collide with yourself
                if (collided[i].name == this.name)
                    continue;

                //You shouldn't be able to collide with your own bullets 
                BulletClass col = collided[i].GetComponent<BulletClass>();
                if (col)
                {
                    if (characterstates != CHARACTER_STATEMACHINE.DASH_DELAY)
                        if (col.parent != this)
                            if (col.parent.cur_faction != cur_faction)
                                if (!FreindlyFire(collided[i]))
                                {
                                    print(collided[i].name + " hit User: " + name);
                                    returners.Add(collided[i]);
                                }

                }
                continue;

            }
            return returners;
        }
        else
        return null;
    }

    public List<Collider2D> CheckLayerCollision(int lar, bool use_hitbox, Vector3 position)
    {
        List<Collider2D> returners = null;
        Collider2D[] collided;
        //A replacement to constatnly using OnTrigger Functions
        if (!use_hitbox)
            collided = Physics2D.OverlapBoxAll(new Vector2(position.x + GetComponent<BoxCollider2D>().offset.x,
                position.y + GetComponent<BoxCollider2D>().offset.y), new Vector2(GetComponent<BoxCollider2D>().size.x,
                GetComponent<BoxCollider2D>().size.y), 0, lar);
        else
            collided = Physics2D.OverlapBoxAll(transform.position + (Vector3)position, new Vector2(hit_box.size.x, hit_box.size.y), 0, lar);

        if (collided != null)
        {
            for (int i = 0; i < collided.Length; i++)
            {

                //You can't collide with yourself
                if (collided[i].name == this.name)
                    continue;

                //You shouldn't be able to collide with your own bullets 
                BulletClass col = collided[i].GetComponent<BulletClass>();
                if (col)
                    if (characterstates != CHARACTER_STATEMACHINE.DASHING &&
                        characterstates != CHARACTER_STATEMACHINE.DASH_DELAY)
                        if (col.parent != this)
                            if (col.parent.cur_faction != cur_faction)
                                if (!FreindlyFire(collided[i]))
                                {

                                    returners.Add(collided[i]);
                                }

                continue;
            }
            return returners;
        }

        return null;
    }


    protected bool FreindlyFire(Collider2D col)
    {
        for (int i = 0; i < target_characters.Count; i++)
        {

            if (col.transform.parent.GetComponent<BulletClass>() != null)
            {

                BulletClass bullet = col.transform.parent.GetComponent<BulletClass>();
                if (bullet.parent == target_characters[i])
                {
                    StartCoroutine(TakeDamage(bullet.attackPower));
                    if (characterstates != CHARACTER_STATEMACHINE.DASH_DELAY)
                    {
                        bullet.OnHitEntity();
                        return true;
                    }
                    else
                        return false;
                }
            }
            else
            {
                if (col.GetComponent<BulletClass>())
                {
                    if (col.GetComponent<BulletClass>().parent == target_characters[i])
                    {
                        if (gameObject.activeSelf)
                        {
                            BulletClass bullet = col.GetComponent<BulletClass>();
                            StartCoroutine(TakeDamage(bullet.attackPower));
                            if (characterstates != CHARACTER_STATEMACHINE.DASH_DELAY)
                            {
                                bullet.OnHitEntity();
                                return true;
                            }
                            else
                                return false;
                        }
                    }
                }
            }

        }

        //Otherwise it's a somone in this character's faction

        return true;
    }

    */

    public Collider2D CheckLayerCollision(int lar, bool use_hitbox)
    {
        Collider2D collided;
        //A replacement to constatnly using OnTrigger Functions
        if (hit_box == null)
            return null;

        if (!use_hitbox)
            collided = Physics2D.OverlapBox(new Vector2(transform.position.x + GetComponent<BoxCollider2D>().offset.x,
                transform.position.y + GetComponent<BoxCollider2D>().offset.y), new Vector2(GetComponent<BoxCollider2D>().size.x,
                GetComponent<BoxCollider2D>().size.y), 0, lar);
        else
            collided = Physics2D.OverlapBox(transform.position, new Vector2(hit_box.size.x, hit_box.size.y), 0, lar);

        if (collided != null)
        {
            //You can't collide with yourself
            if (collided.name == this.name)
                return null;

            //You shouldn't be able to collide with your own bullets 
            if (collided.GetComponent<BulletClass>())
                if (collided.GetComponent<BulletClass>().parent == this)
                    if (collided.GetComponent<BulletClass>().parent.cur_faction == cur_faction)
                        if (FreindlyFire(collided))
                            return null;

            return collided;
        }

        return null;
    }
    public Collider2D CheckLayerCollision(int lar, bool use_hitbox, Vector3 position)
    {
        Collider2D collided;
        //A replacement to constatnly using OnTrigger Functions
        if (!use_hitbox)
            collided = Physics2D.OverlapBox(new Vector2(position.x + GetComponent<BoxCollider2D>().offset.x,
                position.y + GetComponent<BoxCollider2D>().offset.y), new Vector2(GetComponent<BoxCollider2D>().size.x,
                GetComponent<BoxCollider2D>().size.y), 0, lar);
        else
            collided = Physics2D.OverlapBox(transform.position + (Vector3)position, new Vector2(hit_box.size.x, hit_box.size.y), 0, lar);

        if (collided != null)
        {
            //You can't collide with yourself
            if (collided.name == this.name)
                return null;

            //You shouldn't be able to collide with your own bullets 
            if (collided.GetComponent<BulletClass>())
                if (characterstates == CHARACTER_STATEMACHINE.DASH_DELAY)
                    if (collided.GetComponent<BulletClass>().parent == this)
                        if (collided.GetComponent<BulletClass>().parent.cur_faction == cur_faction)
                            if (FreindlyFire(collided))
                                return null;

            return collided;
        }

        return null;
    }

    protected void CheckDamage()
    {
        if(hit_box != null)
        {
            if (!hit_box.enabled)
                return;

        }
        //List<Collider2D> col = CheckLayerCollision(4096, true);
        Collider2D col = CheckLayerCollision(4096, true);
        if (col != null)
        {
            //float damage = 0;
            if (!Invinciblity)
            {
                if (!FreindlyFire(col))
                {
                    BulletClass bul = col.GetComponent<BulletClass>();
                    BulletClass bulP = col.gameObject.transform.parent.GetComponent<BulletClass>();
                    //print(col.name);
                    if (bul && gameObject.activeSelf)
                    {
                        StartCoroutine(TakeDamage(bul.attackPower));
                    }
                    else if (gameObject.activeSelf)
                    {
                        bulP.OnHitEntity();
                        StartCoroutine(TakeDamage(bulP.attackPower));
                    }

                }

                /*
                StartCoroutine(TakeDamage(col.transform.parent.GetComponent<BulletClass>().attackPower));
                for (int i = 0; i < col.Count; i++)
                {
                    col[i].GetComponent<BulletClass>().OnHitEntity();
                    print(col[i].name + "hit User:" + name);
                    damage += col[i].transform.parent.GetComponent<BulletClass>().attackPower;
                }
                StartCoroutine(TakeDamage(damage));

            */
            }
        }
    }
    protected bool FreindlyFire(Collider2D col) {
        for (int i = 0; i < target_characters.Count; i++) {
            
            if (col.transform.parent.GetComponent<BulletClass>() != null) { 

                BulletClass bullet = col.transform.parent.GetComponent<BulletClass>();
                if (bullet.parent == target_characters[i])
                {
                    return false;
                }
            }
            else {
                if (col.GetComponent<BulletClass>())
                {
                    BulletClass bullet = col.GetComponent<BulletClass>();
                    if (bullet.parent == target_characters[i])
                    {
                        return false;
                    }
                }
            }
            
        }

        //Otherwise it's a somone in this character's faction
        
        return true;
    }

    public IEnumerator TakeDamage(float dmgAmount)
    {
        if (hurtdone && Health != 0 && !Invinciblity)
        {

            ParticleSystem part = ObjectPooler.instance.SpawnObject("hit_particle", transform.position, Quaternion.identity,true).GetComponent<ParticleSystem>();
            part.Play();
            hurtdone = false;
            Health -= dmgAmount;
            SoundManager.SFX.playSound(hurt);

            if (hurt_sound.Length > 0)
                SoundManager.SFX.playSound(hurt_sound[UnityEngine.Random.Range(0, hurt_sound.Length)]);

            thisrender.color = Color.red;
            yield return new WaitForSeconds(hurt_del);
            thisrender.color = Color.white;
            hurtdone = true;
        }

        yield return null;
    }

    private void OnDrawGizmos()
    {
        
           BoxCollider2D bx = GetComponent<BoxCollider2D>();

        Vector2 offset = new Vector2(bx.transform.position.x + bx.offset.x, bx.transform.position.y + bx.offset.y);
        Gizmos.DrawWireCube((Vector3)offset + (Vector3)(direction_vec * bx.size), bx.size);
    }

    public bool CheckIfCornered(Vector3 direction)
    {
        Vector2 siz = CollisionBox.size * 0.9f;

        Vector2 offset = new Vector2(CollisionBox.transform.position.x + CollisionBox.offset.x, CollisionBox.transform.position.y + CollisionBox.offset.y);
        Collider2D col = Physics2D.OverlapBox((Vector3)offset + (Vector3)(direction_vec * siz), siz, 0, layer1);

        if (col != null)
            return true;
        return false;

    }
    /*
    Debug.DrawLine(offset, (Vector3)offset + (direction * 25), Color.blue);
    if (Physics2D.Linecast((Vector3)offset, (Vector3)offset , layer1)) {
        return true;
    }*/

    #endregion

    #region SHOOT

    protected void EnableAttack()
    {
        if (attack != null)
            attack.SetActive(true);
    }
    protected void DisableAttack()
    {
        if (attack != null)
            attack.SetActive(false);
    }

    public GameObject ShootBullet(string items, float offset, float time, float speed)
    {
        GameObject bullet = ObjectPooler.instance.SpawnObject(items, new Vector3(CollisionBox.transform.position.x, CollisionBox.transform.position.y 
            //- bullet_position_offset
            )
            , Quaternion.identity, true);
        //GameObject bullet = ObjectPooler.instance.SpawnObject(items, new Vector3(transform.position.x, transform.position.y - 60), Quaternion.identity);
        BulletClass bulletdata = bullet.GetComponent<BulletClass>();
        bulletdata.speed = speed;
        bulletdata.attackPower = attack_pow;
        bulletdata.parent = this;
        if (GetComponent<o_plcharacter>())
            bulletdata.SetShootDirection(new Vector2(shoot_direction.x, shoot_direction.y), offset);
        else
            bulletdata.SetShootDirection(new Vector2(direction_vec.x, direction_vec.y), offset);
        bulletdata.inital_del = time;
        bulletdata.hit_multp_enemies = false;
        bulletdata.rotation = charDirection + offset;
        bullet.transform.SetParent(GameObject.Find("Items").transform);
        bulletdata.GetComponent<IpoolObject>().SpawnStart();
        return bullet;
    }
    public GameObject ShootBullet(string items, float offset, float time, float speed, bool hit_multp)
    {
        GameObject bullet = ObjectPooler.instance.SpawnObject(items, new Vector3(transform.position.x, transform.position.y - bullet_position_offset), Quaternion.identity, true);
        //GameObject bullet = ObjectPooler.instance.SpawnObject(items, new Vector3(transform.position.x, transform.position.y - 60), Quaternion.identity);
        BulletClass bulletdata = bullet.GetComponent<BulletClass>();
        bulletdata.speed = speed;
        bulletdata.attackPower = attack_pow;
        bulletdata.parent = this;
        if (GetComponent<o_plcharacter>())
            bulletdata.SetShootDirection(new Vector2(shoot_direction.x, shoot_direction.y), offset);
        else
            bulletdata.SetShootDirection(new Vector2(direction_vec.x, direction_vec.y), offset);
        bulletdata.inital_del = time;
        bulletdata.rotation = charDirection + offset;
        bullet.transform.SetParent(GameObject.Find("Items").transform);
        bulletdata.GetComponent<IpoolObject>().SpawnStart();
        bulletdata.hit_multp_enemies = hit_multp;
        return bullet;
    }

    public GameObject ShootBullet(string items, float offset, float time) {
        //Instantiate((Resources.Load("Prefabs/Bullet prefabs/" + items, typeof(GameObject))), transform.position, Quaternion.identity) as GameObject;

        GameObject bullet = ObjectPooler.instance.SpawnObject(items, new Vector3(transform.position.x, transform.position.y - bullet_position_offset), Quaternion.identity, true);
        BulletClass bulletdata = bullet.GetComponent<BulletClass>();
        if(GetComponent<o_plcharacter>())
        bulletdata.SetShootDirection(new Vector2(shoot_direction.x, shoot_direction.y), offset);
        else
            bulletdata.SetShootDirection(new Vector2(direction_vec.x, direction_vec.y), offset);

        bulletdata.attackPower = attack_pow;
        bulletdata.parent = this;
        bulletdata.hit_multp_enemies = false;
        bulletdata.inital_del = time;
        bulletdata.rotation = charDirection + offset;
        bullet.transform.SetParent(GameObject.Find("Items").transform);
        bulletdata.GetComponent<IpoolObject>().SpawnStart();
        return bullet;
    }
    public GameObject ShootBullet(string items, float offset, float time, bool hit_multip)
    {
        //Instantiate((Resources.Load("Prefabs/Bullet prefabs/" + items, typeof(GameObject))), transform.position, Quaternion.identity) as GameObject;

        GameObject bullet = ObjectPooler.instance.SpawnObject(items, new Vector3(transform.position.x, transform.position.y - bullet_position_offset), Quaternion.identity, true);
        BulletClass bulletdata = bullet.GetComponent<BulletClass>();
        if (GetComponent<o_plcharacter>())
            bulletdata.SetShootDirection(new Vector2(shoot_direction.x, shoot_direction.y), offset);
        else
            bulletdata.SetShootDirection(new Vector2(direction_vec.x, direction_vec.y), offset);

        bulletdata.attackPower = attack_pow;
        bulletdata.parent = this;
        bulletdata.inital_del = time;
        bulletdata.rotation = charDirection + offset;
        bullet.transform.SetParent(GameObject.Find("Items").transform);
        bulletdata.GetComponent<IpoolObject>().SpawnStart();
        bulletdata.hit_multp_enemies = hit_multip;
        return bullet;
    }

    #endregion

    #region CHARACTER PHYSICS

    public virtual void AfterDash()
    {

    }

    protected bool Dash(float dash_tim, float dash_del, float dash_pre)
    {
        if (dashDelay == 0) {
            pre_dash_tim = dash_pre;
            dashDelay = dash_del;
            dashTime = dash_tim;
            thisrender.color = Color.cyan;
            characterstates = CHARACTER_STATEMACHINE.DASHING;
            DashFunction();
            return true;
        }
        return false;
    }

    /// <summary>
    /// Sets the Attacktime which is the time that the attack lasts for
    /// The delay is for how long you need to wait after you are able to do the next attack
    /// </summary>
    protected bool Attack(float attk_time, float attack_del) {
        if (attkdelay == 0) {
            attkdelay = attack_del;
            this.attk_time = attk_time;
            characterstates = CHARACTER_STATEMACHINE.ATTACKING;
            return true;
        }
        return false;
    }
    protected bool Attack(float attack_del)
    {
        if (attkdelay == 0)
        {
            attkdelay = attack_del;
            characterstates = CHARACTER_STATEMACHINE.ATTACKING;
            return true;
        }
        return false;
    }

    public abstract void DashFunction();

    /// <summary>
    /// Character decides what to do after attack. This can be used for enemy AI to perhaps do another attack.
    /// It can also be used for Players to go back to the standing state.
    /// </summary>
    public abstract void AfterAttack();

    protected void StopCharacter() {
        if(rbody2d != null)
            rbody2d.velocity = Vector2.zero;
    }

    public void MoveCharacter(Vector3 currpos)
    {
        horizontalDir = currpos.x;
        verticalDir = currpos.y;
        MoveCharacter();
    }

    public void MoveCharacter()
    {
        rbody2d.velocity = new Vector2(horizontalDir, verticalDir).normalized * Speed;
    }

    public IEnumerator CrashAfterDash()
    {
        characterstates = CHARACTER_STATEMACHINE.NOTHING;
        SoundManager.SFX.playSound(dashcrash);
        ParticleSystem part = ObjectPooler.instance.SpawnObject("hit_particle", transform.position, Quaternion.identity, true).GetComponent<ParticleSystem>();
        part.Play();
        yield return new WaitForSeconds(0.4f);

        dashTime = 0;
        AfterDash();
    }
    #endregion

    #region STATES

    protected void PlayExplosionEffect()
    {
        ParticleSystem part = ObjectPooler.instance.SpawnObject("explosion_particle", transform.position, Quaternion.identity, true).GetComponent<ParticleSystem>();
        part.Play();
        AudioClip cli = SoundManager.SFX.LoadAudio("explode_sound");
        SoundManager.SFX.playSound(cli);
    }
    protected void PlayExplosionEffect(Vector2 pos)
    {
        ParticleSystem part = ObjectPooler.instance.SpawnObject("explosion_particle", transform.position + (Vector3)pos, Quaternion.identity, true).GetComponent<ParticleSystem>();
        part.Play();
        AudioClip cli = SoundManager.SFX.LoadAudio("explode_sound");
        SoundManager.SFX.playSound(cli);
    }
    protected void PlayExplosionEffect(string n)
    {
        ParticleSystem part = ObjectPooler.instance.SpawnObject(n, transform.position, Quaternion.identity, true).GetComponent<ParticleSystem>();
        part.Play();
        AudioClip cli = SoundManager.SFX.LoadAudio("explode_sound");
        SoundManager.SFX.playSound(cli);
    }
    protected void PlayEffect(string n)
    {
        ParticleSystem part = ObjectPooler.instance.SpawnObject(n, transform.position, Quaternion.identity, true).GetComponent<ParticleSystem>();
        part.Play();
    }
    protected void PlayEffect(string n,  string s)
    {
        ParticleSystem part = ObjectPooler.instance.SpawnObject(n, transform.position, Quaternion.identity, true).GetComponent<ParticleSystem>();
        part.Play();
        AudioClip cli = SoundManager.SFX.LoadAudio(s);
        SoundManager.SFX.playSound(cli);
    }
    protected void PlayEffect(string n, Vector2 pos)
    {
        ParticleSystem part = ObjectPooler.instance.SpawnObject(n, transform.position + (Vector3)pos, Quaternion.identity, true).GetComponent<ParticleSystem>();
        part.Play();
    }
    protected void PlayEffect(string n, Vector2 pos, string s)
    {
        ParticleSystem part = ObjectPooler.instance.SpawnObject(n, transform.position + (Vector3)pos, Quaternion.identity, true).GetComponent<ParticleSystem>();
        part.Play();
        AudioClip cli = SoundManager.SFX.LoadAudio(s);
        SoundManager.SFX.playSound(cli);
    }

    protected void ToggleDirectionLock(bool locker)
    {
        lock_direction = locker;
    }

    public void SetAnimation(int state)
    {
        animationState = state;
    }

    public void Update()
    {

        if (thisrender != null)
            if (!is_arial) { thisrender.sortingOrder = (int)transform.position.y * -1; }
        else thisrender.sortingOrder = int.MaxValue;

        if (shadow != null)
            shadow.sortingOrder = (int)(transform.position.y * -1) - 1;

        if (characterstates != CHARACTER_STATEMACHINE.DEAD)
        { }

        
        if (charAnimator != null)
        {
            s_anim_clip anim = null;

            if (!lock_direction)
            {
                verticalDir = Mathf.RoundToInt(direction_vec.y);
                horizontalDir = Mathf.RoundToInt(direction_vec.x);
            }

            if (horizontalDir == 1 && verticalDir == 1 ||
                horizontalDir == 1 && verticalDir == -1|| horizontalDir == 1)
            {
                anim = animationclips.Find(
                    an => an.animation_number == animationState
                    && an.DIR == s_anim_clip.dir.RIGHT);
            }
            else
            if (horizontalDir == -1 && verticalDir == 1 ||
                horizontalDir == -1 && verticalDir == -1 || horizontalDir == -1)
            {
                anim = animationclips.Find(
                    an => an.animation_number == animationState
                    && an.DIR == s_anim_clip.dir.LEFT);
            }else
            if (verticalDir == -1 && horizontalDir == 0)
            {
                anim = animationclips.Find(
                    an => an.animation_number == animationState
                    && an.DIR == s_anim_clip.dir.DOWN);
            } else
            if (verticalDir == 1 && horizontalDir == 0)
            {
                anim = animationclips.Find(
                    an => an.animation_number == animationState
                    && an.DIR == s_anim_clip.dir.UP);
            }
            
            anim = animationclips.Find(
                an => an.animation_number == animationState
                && an.DIR == s_anim_clip.dir.NONE) != null ? animationclips.Find(
                an => an.animation_number == animationState
                && an.DIR == s_anim_clip.dir.NONE) : anim;

            if (anim != null)
                charAnimator.Play(anim.animation.name);
            
        }
        if (Active)
        {
            switch (characterstates)
            {
                case CHARACTER_STATEMACHINE.STAND:

                    if (thisrender != null)
                        thisrender.color = Color.white;

                    if (rbody2d != null)
                        rbody2d.velocity = new Vector2(0, 0);

                    break;

                case CHARACTER_STATEMACHINE.DASHING:

                    if (pre_dash_tim > 0)
                    {
                        Invinciblity = false;
                        pre_dash_tim = pre_dash_tim - Time.deltaTime;
                    }
                    else
                    {

                        thisrender.color = Color.white;

                        Invinciblity = true;
                        rbody2d.velocity = new Vector2(direction_vec.x, direction_vec.y).normalized * Speed * 5;

                        characterstates = CHARACTER_STATEMACHINE.DASH_DELAY;
                    }

                    break;

                case CHARACTER_STATEMACHINE.DASH_DELAY:
                    if (dashTime >= 0)
                    {
                        CreateAfterimage();
                        if (!CollisionBox.isTrigger && CheckIfCornered(new Vector3(direction_vec.x, direction_vec.y).normalized))
                        {
                            if (this.gameObject == cameraScript.camOptions.player)
                                cameraScript.camOptions.cameraShake(9, 9, 0.4f);

                            Invinciblity = false;
                            dashDelay = 0.65f;
                            StopCharacter();
                            
                            StartCoroutine(CrashAfterDash());
                        }
                        dashTime = dashTime - Time.deltaTime;
                    }
                    else
                    {
                        Invinciblity = false; Speed = initialSpeed;
                        if (CheckLayerCollision(11, false) == null)
                            characterstates = CHARACTER_STATEMACHINE.MOVING;
                    }
                    break;

                case CHARACTER_STATEMACHINE.ATTACK_ANIMATION:

                    if (attk_time >= 0)
                    {
                        attk_time = attk_time - Time.deltaTime;
                    }
                    else
                    {
                        //What happens after the attack is decided by the character themselves
                        //Hence we give a nothing state.
                        AfterAttack();
                    }
                    break;

                case CHARACTER_STATEMACHINE.DEAD:
                    if (CutsceneOndeath != null)
                    {
                        if (!cutsceneObjCreated)
                        {
                            GameObject cut = Instantiate(CutsceneOndeath, transform.position, Quaternion.identity);
                            cut.transform.parent = GameObject.Find("Cutscenes").transform;
                            cutsceneObjCreated = true;
                        }
                    }
                    if (destroyOnDeath)
                    {
                        this.gameObject.SetActive(false);
                    }
                    break;

            }
            DetectFall();
            BeforeFall();

            dashDelay = dashDelay > 0 ? dashDelay - Time.deltaTime : 0;
            attkdelay = attkdelay > 0 ? attkdelay - Time.deltaTime : 0;

        }

        if (Health > 0)
        {
            if (constant_regen)
            {
                if (regentime > 0)
                {
                    regentime -= Time.deltaTime;
                }
                else
                {
                    Health++;
                    regentime = maxregentime;
                }
            }
        }


        if (Health >= maxHealth)
        {
            Health = maxHealth;
        }

        if (Health <= 0)
        {
            Health = 0;
            StopCharacter();
            characterstates = CHARACTER_STATEMACHINE.DEAD;
            
        }

    }

    IEnumerator DashingTimer()
    {
        while (dashTime >= 0)
        {
            if (!CollisionBox.isTrigger && CheckIfCornered(new Vector3(direction_vec.x, direction_vec.y).normalized))
            {
                if (this.gameObject == cameraScript.camOptions.player)
                {
                    cameraScript.camOptions.cameraShake(9, 9, 0.4f);
                }
                Invinciblity = false;
                dashDelay = 0.65f;
                StopCharacter();
                StartCoroutine(CrashAfterDash());
            }
            dashTime = dashTime - Time.deltaTime;
            yield return new WaitForSeconds(Time.deltaTime);

        }
        Invinciblity = false; Speed = initialSpeed;
        characterstates = CHARACTER_STATEMACHINE.MOVING;
    }
    #endregion

}
