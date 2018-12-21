using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BulletClass : MonoBehaviour, IpoolObject
{
    protected cameraScript cameraStuff;
    public o_character parent;
	public float speed;
    public float inital_del;
	protected float delay;
    public bool hit_multp_enemies = false;
	public float attackPower;

    public BoxCollider2D hit_collision;
    public BoxCollider2D real_collision;

    protected Rigidbody2D rbody;
    public bool playerBullet;
    protected bool isStarted;

    protected SpriteRenderer sprite;

    public Sprite muzz;
    public Sprite bullet;
    public Animator animbull;
    public bool go_through = false;

    protected bool timed_obj;

    public float rotation;
    public LayerMask layer1;
    public LayerMask collision;
    public Vector3 shoot_direction;

    public abstract void InitializeBullet();
    public void Start()
    {
        delay = inital_del;
        isStarted = false;
        if (transform.Find("Hitbox"))
        {
            animbull = transform.Find("HitBox").gameObject.GetComponent<Animator>();
            sprite = transform.Find("HitBox").gameObject.GetComponent<SpriteRenderer>();
            hit_collision = transform.Find("HitBox").gameObject.GetComponent<BoxCollider2D>();
        }
        else
        {
            hit_collision = GetComponent<BoxCollider2D>();
        }
        real_collision = GetComponent<BoxCollider2D>();
        cameraStuff = GameObject.Find("Main Camera").GetComponent<cameraScript>();
        InitializeBullet();
    }
    void IpoolObject.SpawnStart()
    {
        rbody = GetComponent<Rigidbody2D>();
        delay = inital_del;
        hit_multp_enemies = false;
        isStarted = false; go_through = false;
        if (transform.Find("HitBox"))
        {
            animbull = transform.Find("HitBox").gameObject.GetComponent<Animator>();
            sprite = transform.Find("HitBox").gameObject.GetComponent<SpriteRenderer>();
            hit_collision = transform.Find("HitBox").gameObject.GetComponent<BoxCollider2D>();
        }
        else
        {
            sprite = GetComponent<SpriteRenderer>();
            hit_collision = GetComponent<BoxCollider2D>();
        }
        hit_collision.enabled = true;
        real_collision = GetComponent<BoxCollider2D>();
        cameraStuff = GameObject.Find("Main Camera").GetComponent<cameraScript>();
        InitializeBullet();
    }

    public void SetShootDirection(Vector2 dir, float off)
    {
        shoot_direction = dir;
        transform.rotation = Quaternion.Euler(0, 0, off);
    }

    public void SetShootAngle()
    {
        float angle = 360 - Mathf.Atan2(shoot_direction.x, shoot_direction.y) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    public void SetShootAngleChild()
    {
        float angle = 360 - Mathf.Atan2(shoot_direction.x, shoot_direction.y) * Mathf.Rad2Deg;
        transform.GetChild(0).rotation = Quaternion.Euler(0, 0, angle);
    }

    public void Update ()
    {
        if (timed_obj) {
            if (delay >= 0)
                delay = delay - Time.deltaTime;
            else
                OnBulletDestroy();
        }

        if(!go_through)
            CheckCollision();
	}

    public virtual void OnBulletDestroy() { }

    public void CheckCollision()
    {

        Collider2D bound_col = Physics2D.OverlapBox(real_collision.transform.position +  (Vector3)real_collision.offset, new Vector2(real_collision.size.x, real_collision.size.y), 0, collision);
        if (bound_col != null)
        {
            if (bound_col.CompareTag("Collision"))
            {
                //SoundManager.SFX.playSound(sound);
                OnBulletDestroy();
            }
        }
    }

    public abstract void OnHitEntity();

    public void DestoryBullet() {
        delay = 0;
        transform.SetParent(GameObject.Find("OBj pool").transform);
        gameObject.SetActive(false);
    }

    public IEnumerator muzzle(float delay)
    {
        //sprite.sprite = muzz;

        yield return new WaitForSeconds(delay);
        isStarted = true;

        if (animbull != null) {
            animbull.SetBool("isMuzz", true);
        }
        //sprite.sprite = bullet;

        yield return null;
    }

}

/*
Collider2D col = Physics2D.OverlapBox(transform.position, new Vector2(hit_collision.size.x, hit_collision.size.y), 0, layer1);

if (Physics2D.OverlapBox(transform.position, new Vector2(hit_collision.size.x, hit_collision.size.y), 0, layer1) != null)
{
    //Checks "Hitbox layer"
    if (col.gameObject.layer == layer1)
    {
        o_character charater = col.transform.parent.gameObject.GetComponent<o_character>();
        print(parent.cur_faction);
        print(charater.cur_faction);

        if (charater.cur_faction != parent.cur_faction) {
            if (charater.characterstates != o_character.CHARACTER_STATEMACHINE.DASH_DELAY) {
                print("no");
                charater.StartCoroutine(charater.TakeDamage(attackPower));
                OnHitEntity();
                return;
            }
        }
    }
}*/
