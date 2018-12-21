#region KAZZI/UBERDRY CODE

/*
 * Added on 12/05/2018
 * This is Kazzi's old code, most likely before the NPC rewrite
 * Fun fact: He was actually called "Uberdry" for the first half of development, until I decided to change his name to "Kazzi"
 * He was also meant to play a bigger role as Peast's old freind and as a doctor/medic.
 * I decided that he was probably not going to do much to the game's story; I also didn't want him to break the whole concept that
 * Peast (the main character) goes on this adventure solo.
 * 
 * float heal_delay = 0;

	new void Start () {
        base.Start();
        player_leader = GameObject.Find("Peast");
        ai_states = artificial_intelligence.follow;
	}

    void IpoolObject.SpawnStart() {

        base.Start();
        view_distance = 600;
        player_leader = GameObject.Find("Peast");
        ai_states = artificial_intelligence.follow;
    }

    void OnTriggerEnter2D(Collider2D hitbx)
    {
        BulletClass bullet = hitbx.GetComponent<BulletClass>();
        if (hitbx.isTrigger == true && ((hitbx.CompareTag("EnemyBullet"))))
        {
            if (!Invinciblity)
            {
                cameraStuff.cameraShake(2, 2, 0.2f); SoundManager.SFX.playSound(hurt_sound[Random.Range(0, hurt_sound.Length)]);
                StartCoroutine(TakeDamage(hitbx.GetComponent<BulletClass>().attackPower));
            }
        }
    }


    new void Update () {
        base.Update();

        switch (ai_states) {
            case artificial_intelligence.heal_player:

                o_plcharacter target_heal = player_leader.GetComponent<o_plcharacter>();
                SoundManager.SFX.playSound(Resources.Load("Sound/Heal") as AudioClip);
                target_heal.Health += 2;
                ai_states = artificial_intelligence.follow;

                break;
            case artificial_intelligence.follow:
                if (heal_delay == 0) {
                    if (AILibary.AILibary.DistanceToAttack(45f, player_leader, this.gameObject))
                    {
                        if (player_leader.GetComponent<o_plcharacter>().Health <= 3)
                        {
                            heal_delay = 4f;
                            ai_states = artificial_intelligence.heal_player;
                        }
                    }
                }



                if (target_characters.Count == 0) {
                    SetTargChars();
                }
                if (target == null)
                {
                    target = (EnemyClass)AILibary.AILibary.AquireTarget(this.gameObject, target_characters);
                }
                else
                {
                    currpos = AILibary.AILibary.LookAtTarget(this.gameObject, target.gameObject);
                    charDirection = 360 - Mathf.Atan2(currpos.x, currpos.y) * Mathf.Rad2Deg;
                    if (AILibary.AILibary.LineOfSight(this.gameObject, target.gameObject, currpos, 65))
                    {
                        attackdelay = 1.5f;
                        ai_states = artificial_intelligence.walk_back;
                    }
                    if (AILibary.AILibary.LineOfSight(this.gameObject, target.gameObject, currpos, 200))
                    {
                        if (attackdelay == 0)
                        {
                            ShootBullet("bullet", 0, 2);
                            attackdelay = 1f;
                        }
                    }
                }

                AILibary.AILibary.MoveCharacter(this, currpos);
                

                break;

            case artificial_intelligence.walk_back:
                //switch_Dir = true;
                currpos = -AILibary.AILibary.LookAtTarget(this.gameObject, target.gameObject);
                AILibary.AILibary.MoveCharacter(this, currpos);

                if (AILibary.AILibary.DistanceToAttack(90, target.gameObject, this.gameObject))
                {
                    if (dashDelay == 0)
                    {
                        dashTime = 0.03f;
                        state = allyState.dash;
                    }
                }
                if (AILibary.AILibary.CheckIfCornered(this.gameObject, -currpos * 2))
                {
                    print("I'm blocked");
                    attackdelay = 0.5f;
                    ai_states = artificial_intelligence.follow;
                }

                if (attackdelay == 0) {
                    switch_Dir = false;
                    ai_states = artificial_intelligence.follow;
                }

                break;

        }

        heal_delay = heal_delay > 0 ? heal_delay -= Time.deltaTime : 0;

    }
 * 
 * 
*/

#endregion

#region UNUSED FUNCTIONS

/* Added on 23/04/2018
 * For o_character, same functionality as initialize
 * 
 * void IpoolObject.SpawnStart() {
    characterstates = CHARACTER_STATEMACHINE.STAND;
    thisrender = this.gameObject.GetComponent<SpriteRenderer>();
    thisrender.color = Color.white;
    if (GetComponent<Animator>() != null)
    {
        charAnimator = GetComponent<Animator>();
    }
    hurt = Resources.Load("Sound/hitsfx2") as AudioClip;
    Invinciblity = false;
    Health = maxHealth;
    rbody2d = GetComponent<Rigidbody2D>();
    cameraStuff = GameObject.Find("Main Camera").GetComponent<cameraScript>();
    Speed = initialSpeed;
}*/


/* Added on 23/04/2018
 * For o_character, same functionality as checkforlayercollision
 * 
public Collider2D CheckBulletCollision()
{
    Collider2D collided = Physics2D.OverlapBox(transform.position, new Vector2(hit_box.size.x, hit_box.size.y), 0, 4096);
    if (collided != null)
    {

        if (collided.name == this.name)
        {
            return null;
        }
        if (collided.GetComponent<BulletClass>())
        {
            if (collided.GetComponent<BulletClass>().parent == this)
            {
                return null;
            }
        }
        if (!FreindlyFire(collided))
            if (hurtdone)
                return collided;
    }
    return null;
}
*/
#endregion

#region SCRIPTED KNIFE CODE
/* Added on 21/04/2018
 * 
 * This knife was meant to be in a now removed cutscene after Peast defeats Milbert.
 * Basically, this knife would be thrown by Milbert
 * It was specificaly scripted to damage Peast (via calculations) to 1 health no matter if he had 4 or 200 hit points.
 * This can't kill Peast though... 
 * For instance, if he has 1 health before this hurt him, he would still be alive after the knife hit him, because you can't progress the game
 * when Peast is unconsious.
 * 
public class KnifeCutscene : BulletClass
{
    o_plcharacter peast;
    new void Start()
    {
        speed = 90f;
        playerBullet = false;
    }

    public override void InitializeBullet()
    {
        throw new NotImplementedException();
    }

    new void Update()
    {
        peast = GameObject.Find("Peast").GetComponent<o_plcharacter>();
        attackPower = Mathf.FloorToInt((peast.Health )- 1);
        transform.rotation = Quaternion.Euler(0, 0, rotation);
        transform.Translate(Vector2.up * speed * 14 * Time.deltaTime);

        base.Update();
    }

    public override void OnHitEntity()
    {
        DestoryBullet();
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        //float delay = 2f;
        if (col.isTrigger == true && col.CompareTag("Player"))
        {
            Destroy(this.gameObject);
        }
    }
}
*/
#endregion

#region VIPER CODE
/*Added on 20/04/2018
 * 
 * 
public class e_viper : EnemyClass {

    enum states
    {
        back_away,
        attack
    }
    states state = states.attack;

    enum attack {
        attack,
        viper_shot
    }
    attack attk;
    AudioClip laugh;

    new void Start() {
        dashDelayTimerStart = 0.12f;
        maxHealth = 7;
        initialSpeed = 86f;
        exp_amount = 5;

        dropItem = Resources.Load("Prefabs/Items/Healing Heart") as GameObject;
        laugh = Resources.Load("Sound/snanake attk") as AudioClip;
        base.Start();
    }
    
    new void Update () {
		base.Update();
        transform.Find("BiteAttk").rotation = Quaternion.Euler(0,0, charDirection +90);
        target = GameObject.FindGameObjectWithTag("Player");
        enemy = target.GetComponent<o_plcharacter>();
        GameObject enemyBullet = GameObject.FindGameObjectWithTag("Bullet");

        checkWalkAnims();
        switch (enemyStatemachine) {

            case enemyState.stand:
                Speed = initialSpeed;

                if (walkTimer > 0)
                {
                    walkTimer = walkTimer - Time.deltaTime;
                }
                else
                {
                    MoveCharacter(false);
                    horizontalDir = Random.Range(-1, 2);
                    verticalDir = Random.Range(-1, 2);
                    walkTimer = Random.Range(0.3f, 2);
                }
                
                if (280 > Mathf.Abs(target.transform.position.x - transform.position.x) && 280 > Mathf.Abs(target.transform.position.y - transform.position.y)) {
                    if (CanSeePlayer())
                    {
                        enemyStatemachine = enemyState.moving;
                    }
                }
                break;

            case enemyState.moving:


                if (DistanceToAttack(280f, target) || CanSeePlayer()) {

                    LookAtPlayer();
                    MoveCharacter(true);

                    charAnimator.SetBool("IsMoving", true);
                    switch (state)
                    {
                        case states.attack:

                            switch_Dir = false;
                            Speed = initialSpeed;
                            if (DistanceToAttack(170f, enemyBullet))
                            {
                                if (attackdelay == 0)
                                {
                                    attackdelay = 2.5f;
                                    state = states.back_away;
                                }
                            }
                            else if (attackdelay == 0) {
                                attk = attack.attack;
                                enemyStatemachine = enemyState.attack_init;
                                attackTimer = 0.01f;
                            }

                            if (DistanceToAttack(60f, target))
                            {
                                print("NearPlayer");
                                attackdelay = 2f;
                                state = states.back_away;
                            }

                            break;

                        case states.back_away:

                            switch_Dir = true;
                            Speed = 140f;

                            if (DistanceToAttack(170f, enemyBullet))
                            {
                                if (dashDelay == 0)
                                {
                                    attackTimer = 0.01f;
                                    attk = attack.viper_shot;
                                    enemyStatemachine = enemyState.attack_init;
                                    SoundManager.SFX.playSound(laugh);
                                }
                            }

                            if (attackdelay == 0)
                            {
                                switch_Dir = false;
                                print("Back to attack");
                                state = states.attack;
                            }
                            break;
                    }
                }
                else { enemyStatemachine = enemyState.stand;
                    charAnimator.SetBool("IsMoving", false);
                }
                
                break;

            case enemyState.dash:

                attackdelay = 0.4f;
                break;

            case enemyState.attack_init:
                switch (attk){

                    case attack.attack:

                        if (dashDelay == 0) {
                            StartCoroutine(beforeBite());
                            enemyStatemachine = enemyState.nothing;
                        }
                        break;
                    case attack.viper_shot:
                        state = states.attack;
                        enemyStatemachine = enemyState.nothing;
                        StartCoroutine(ShootVenom());
                        break;
                }

                break;

            case enemyState.attack:

                Invinciblity = true;
                if (ReadyToAttack(0.6f)) {
                    thisrender.color = Color.white;
                    charAnimator.SetBool("IsAttacking", false);
                    transform.Find("BiteAttk").GetComponent<BoxCollider2D>().enabled = false;
                    Invinciblity = false;
                    enemyStatemachine = enemyState.stand;

                }
                break;

        }
	}



    public IEnumerator ShootVenom() {
        thisrender.color = Color.magenta;
        charAnimator.SetBool("IsAttacking", true);

        rbody2d.velocity = Vector2.zero;
        yield return new WaitForSeconds(0.22f);

        ShootBullet("enemybullet",0, 2);
        enemyStatemachine = enemyState.stand;

        yield return null;
    }

    public IEnumerator beforeBite()
    {
        thisrender.color = Color.cyan;
        charAnimator.SetBool("IsAttacking", true);

        rbody2d.velocity = Vector2.zero;
        yield return new WaitForSeconds(0.22f);

        dashDelay = 0.08f;
        enemyStatemachine = enemyState.dash;
        Invinciblity = true;
        transform.Find("BiteAttk").GetComponent<BoxCollider2D>().enabled = true;

        yield return null;
    }

    public override void afterDashAction()
    {
        switch (state) {
            case states.back_away:
                state = states.attack;

                break;
        }

        attackTimer = 0.09f;
        enemyStatemachine = enemyState.attack;
    }
}
*/
#endregion

#region OLD HIT DETECTION CODE
/* Added on 20/04/2018
 * 
        Collider2D[] collided = Physics2D.OverlapBoxAll(transform.position, new Vector2(hit_box.size.x, hit_box.size.y), 0, 4608);
        List<Collider2D> collidedchar = new List<Collider2D>();

        for (int i = 0; i < collided.Length; i++) {

            if (collided[i].name == this.name) {
                continue;
            }
            if (collided[i].GetComponent<BulletClass>()) {
                if (collided[i].GetComponent<BulletClass>().parent == this) {
                    continue;
                }
            }
            collidedchar.Add(collided[i]);
        }

        if (collidedchar.Count > 0)
        {
            for (int i = 0; i < collidedchar.Count; i++)
            {
                if (FreindlyFire(collidedchar[i]))
                {
                    if (hurtdone)
                    {
                        //StartCoroutine(TakeDamage(1));
                        //collidedchar[i].gameObject.SetActive(false);
                        return collidedchar[i];
                    }
                }
            }
        }*/
#endregion

#region SNAKE HERO CODE
/* Added on 20/04/2018
 * This was another feature I added in quite early into the game's devleopment (around early June or late May).
 * The snake hero character was just a test playable character just to see how easy it would be to implement characters beyond Peast and Kazzi.
 * I never took this idea seriously, and hence it remained in the game's files for a good amount of the developmnet... 
 * 
 * ... until today, where it is forked and doccumented.
 * 
 * The snake did a "Slither" move, which doesn't do anything at all.
 * 
 * The notes I made at the time of adding in this character:
         * 
         * I dunno if he's gonna be a playable character but he's just an experiment for now
         * Maybe if there is a 3rd one, maybe...
         * Or in some spin off game

 * I did hint a 3rd one being made, as of now I'm not sure if this 2nd game will ever be completed
 * Therefore I'm unsure of a 3rd one.
 * If I complete this game, what I've said 2 lines above will be obselete.
 * And I'll probably look back at this and think
 * 
 * "Hamza you're an idiot"
 * 
         
new void Start () {
		maxHealth = 3;
		Speed = 35f;
		
        WeaponBase slither = new WeaponBase ();
		slither.name = "Slither";
		slither.levelAttackPow = new int[]{ 0, 2, 3, 6 };
		slither.ammoLimits = false;
		slither.expToNextLevel = new int[]{ 0, 13, 12 };

		weapons.Add (slither);
		selectedWeapon = weapons [0];
	}
	
	new void Update() {
		base.Update();
    }
 
 */
#endregion

#region EARL_BOSS CODE
/*
 * Added on 18/04/2018
 * This was a boss who was removed and replaced with the fat slug, 
 * his moves consisted of shooting a machine gun and shotgun, as well as having a 3
 * combo move.
 * Earl's role in the story was to stop the player from going into the train, and he also worked
 * for Milbert.
 * 
 * 
public class boss_Earl : EnemyClass {

    public enum threeComboMove { nan ,first, second, thrid, finish }
    public threeComboMove combo { get; set; }
    

    public enum earl_attks { dodge_back, machine_gun, triple_combo, shotgun, none }
    public earl_attks attks;

    public AudioClip hurtsnd;
    public AudioClip attk;

    int attack_number;
    public int comboPhase = 0;

    new void Start()
    {
        destroyOnDeath = false;
        dashDelayTimerStart = 0.04f;
        maxHealth = 95;
        initialSpeed = 95f;
        base.Start();
    }

    public new void Update()
    {
        charAnimator.SetInteger("ComboPhase", comboPhase);
        charAnimator.SetInteger("Attack", attack_number);
        PlayerGUI.GUIObj.activateBossHP();
        base.Update();
        switch (enemyStatemachine)
        {
            case enemyState.stand:
                Speed = initialSpeed;
                comboPhase = 0;
                attack_number = 0;
                combo = threeComboMove.nan;
                if (ReadyToAttack(1)) {
                    enemyStatemachine = enemyState.moving;
                
                }
                break;

            case enemyState.moving:
                
                LookAtPlayer();
                MoveCharacter(true);

                if (attackdelay == 0)
                {
                    enemyStatemachine = toAttack();
                }
                break;

            case enemyState.attack_init:
                switch (attks)
                {
                    case earl_attks.machine_gun:
                        StartCoroutine(MachineGun());
                        break;

                    case earl_attks.shotgun:
                        shotgun(charDirection + Random.Range(-20, 20));
                        break;
                }

                enemyStatemachine = enemyState.attack;
                break;

            case enemyState.attack:

                switch (attks) {

                    case earl_attks.triple_combo:
                        attack_number = 1;
                        ComboAttk();
                        break;

                    case earl_attks.shotgun:
                        if (ReadyToAttack(1.4f))
                        {
                            thisrender.color = Color.white;
                            enemyStatemachine = enemyState.stand;
                        }
                        break;

                    case earl_attks.dodge_back:

                        horizontalDir = -horizontalDir + Random.Range(-1, 2);
                        verticalDir = -verticalDir + Random.Range(-1, 2);

                        dashDelayTimerStart = 0.12f;
                        enemyStatemachine = enemyState.dash;
                        break;

                    case earl_attks.machine_gun:
                        Speed = 40;
                        LookAtPlayer();
                        MoveCharacter(true);
                        break;
                }
                break;

            case enemyState.dead:
                PlayerGUI.GUIObj.deActivateBossHP();
                TriggerCutscene();
                break;
        }
    }

    public IEnumerator MachineGun() {

        for (int i = 0; i < 20; i++) {

            ShootBullet("enemybullet", Random.Range(-5, 5),2.3f);
            yield return new WaitForSeconds(0.1f);
        }
        if (ReadyToAttack(3.4f))
        {
            thisrender.color = Color.white;
            enemyStatemachine = enemyState.stand;
        }
        yield return null;
    }

    void ComboAttk() {

        Speed = 387f;

        switch (combo) {
            case threeComboMove.nan:
                break;

            case threeComboMove.first:
                comboPhase = 1;
                MoveCharacter(true);
                break;

            case threeComboMove.second:
                comboPhase = 2;
                MoveCharacter(true);
                break;

            case threeComboMove.thrid:
                comboPhase = 3;
                break;

            case threeComboMove.finish:
                comboPhase = 0;
                print("Ok");
                if (ReadyToAttack(1f)) {
                    SoundManager.SFX.playSound(attk);
                    thisrender.color = Color.white;
                    enemyStatemachine = enemyState.stand;
                }
                break;
        }
    }


    void shotgun(float direction) {
        ShootBullet("enemybullet", 0, 1.2f);
        ShootBullet("enemybullet", 12, 1.2f);
        ShootBullet("enemybullet", -12, 1.2f);
    }



    new public enemyState toAttack()
    {
        int num = 0;
        if (currentPhase == 0) {
            
            if (DistanceToAttack(300f, target))
            {
                
                if (num == 0) {
                    attks = earl_attks.triple_combo;
                }
                return enemyState.attack_init;
            }
        }        
        if (currentPhase == 1) {

            num = Random.Range(0, 2);
            if (DistanceToAttack(300f, target))
            {
                if (num == 0) {
                    LookAtPlayer();
                    attks = earl_attks.shotgun;
                }

                if (num == 1) {
                    attks = earl_attks.triple_combo;
                }
                if (num == 2)
                {
                    attks = earl_attks.machine_gun;
                }
                return enemyState.attack_init;
            }
        }
        if (currentPhase == 2) {

            if (DistanceToAttack(180f, enemyBullet)) {

                attks = earl_attks.dodge_back;
            }
            else if (DistanceToAttack(400f, target))
            {
                num = Random.Range(0, 3);
                if (num == 0) {
                    LookAtPlayer();
                    attks = earl_attks.shotgun;
                }

                if (num == 1) {
                    attks = earl_attks.triple_combo;
                }
                if (num == 2)
                {
                    attks = earl_attks.machine_gun;
                }
                return enemyState.attack_init;
            }
        }

        return enemyState.moving;
    }

    void CreateAttk() {
        if (bulletObj == null)
        {
            GameObject bitev = (Resources.Load("Prefabs/Bullet prefabs/GoblinBash") as GameObject);
            GameObject bite = Instantiate(bitev, transform.position, Quaternion.Euler(0, 0, 90 - charDirection)) as GameObject;
            bulletObj = bite;
            bite.transform.parent = this.transform;
        }
    }

    void TurnAround() {
        LookAtPlayer();
    }

    public override void afterDashAction()
    {
        
    }
}
*/
#endregion

#region ASSASIN CODE
/* Added 17/04/2018
 * 
public class e_assasin : EnemyClass
{
    public enum assasin_atttks {

        slash,
        second_slash,
        walk,
        shrukien,
        jump_back,
        walk_away

    }
    public assasin_atttks attks;

    
	new void Start () {
        maxHealth = 10;
        enemyStatemachine = enemyState.stand;
        //attks = (assasin_atttks)Random.Range(0, 2);
        base.Start();
	}

    new enemyState toAttack()
    {
        if (currentPhase == 0)
        {
            if (AILibary.AILibary.DistanceToAttack(170, target, this.gameObject))
            {
                switch (attks)
                {
                    case assasin_atttks.walk:
                        int attk = Random.Range(0, 2);
                        if (attk == 0)
                        {
                            LookAtPlayer();
                            attks = assasin_atttks.slash;
                        }
                        if (attk == 1)
                        {
                            LookAtPlayer();
                            currpos = Vector2.zero;
                            attks = assasin_atttks.shrukien;
                        }
                        break;
                }
                return enemyState.attack_init;
            }
        }

        if (currentPhase == 1)
        {
            if (AILibary.AILibary.DistanceToAttack(340, target, this.gameObject))
            {
                switch (attks)
                {
                    case assasin_atttks.walk:
                        int attk = 0;//Random.Range(0, 2);
                        LookAtPlayer();
                        print("3f");
                        attks = assasin_atttks.jump_back;
                        if (attk == 0)
                        {
                        }
                        break;
                }
                return enemyState.attack_init;
            }
        }
        return enemyState.moving;
    }

    public IEnumerator CreateMarker() {

        //play pre animation
        
        yield return new WaitForSeconds(0.1f);

        dashDelayTimerStart = 0.0006f;
        Speed = 125;
        enemyStatemachine = enemyState.dash;
    }



	new void Update () {
        base.Update();
        target = GameObject.FindGameObjectWithTag("Player");
        enemy = target.GetComponent<o_plcharacter>();
        GameObject enemyBullet = GameObject.FindGameObjectWithTag("Bullet");

        switch (enemyStatemachine) {

            case enemyState.stand:
                attks = assasin_atttks.walk;
                Speed = initialSpeed;
                enemyStatemachine = enemyState.moving;
                break;
                
            case enemyState.moving:
                switch (attks) {
                    case assasin_atttks.walk:

                        LookAtPlayer();
                        MoveCharacter(true);

                        if (AILibary.AILibary.DistanceToAttack(65, target, this.gameObject)) {
                            attackdelay = 1.5f;
                            attks = assasin_atttks.walk_away;
                        } else {
                            if (attackdelay == 0 && dashDelay == 0)
                            {
                                enemyStatemachine = toAttack();
                            }
                        }

                        break;

                    case assasin_atttks.walk_away:
                        switch_Dir = true;
                        LookAtPlayer();
                        MoveCharacter(true);

                        if (attackdelay == 0) {
                            switch_Dir = false;
                            attks = assasin_atttks.walk;
                        }

                        break;
                }
                break;

            case enemyState.attack_init:

                switch (attks) {
                    case assasin_atttks.slash:

                        StartCoroutine(CreateMarker());
                        MoveCharacter(true);
                        break;

                    case assasin_atttks.second_slash:

                        StartCoroutine(CreateMarker());
                        MoveCharacter(true);
                        break;

                    case assasin_atttks.jump_back:
                        switch_Dir = true;
                        StartCoroutine(CreateMarker());
                        MoveCharacter(true);
                        break;

                    case assasin_atttks.shrukien:
                        StartCoroutine(ThrowShruiken());
                        break;

                }
                break;
        }
    }

    public IEnumerator ThrowShruiken() {

        enemyStatemachine = enemyState.nothing;
        rbody2d.velocity = Vector2.zero;
        yield return new WaitForSeconds(0.2f);

        ShootBullet("enemybullet", 0, 1.6f, 60f);
        attackdelay = 0.6f;
        attks = assasin_atttks.walk;
        LookAtPlayer();
        enemyStatemachine = enemyState.stand;
        if (ReadyToAttack(2f))
        {
        }
    }


    public override void afterDashAction()
    {
        switch (attks)
        {
            case assasin_atttks.slash:
                if (ReadyToAttack(0.2f))
                {
                    if (dashDelay == 0)
                    {
                        LookAtPlayer();
                        attks = assasin_atttks.second_slash;
                        enemyStatemachine = enemyState.attack_init;
                    }
                }
                break;

            case assasin_atttks.second_slash:

                LookAtPlayer();
                attackdelay = 1f;
                attks = assasin_atttks.walk;
                enemyStatemachine = enemyState.stand;
                break;

            case assasin_atttks.jump_back:

                switch_Dir = false;

                attackdelay = 0.6f;
                attks = assasin_atttks.walk;
                LookAtPlayer();
                enemyStatemachine = enemyState.stand;
                ShootBullet("enemybullet", 0, 1.6f);
                break;
        }

    }
}
*/
#endregion

#region KNIFE CODE
/* Added on 11/04/2018
 * This was the thing that enemies were meant to shoot. I added this on 02/09/2017
 * The knife class' use ended when I started changing bits of the NPC code.
 * 
public class Knife : BulletClass
{
    public override void InitializeBullet()
    {
        throw new NotImplementedException();
    }

    new void Update ()
    {
        if (isStarted)
        {
            transform.rotation = Quaternion.Euler(0, 0, rotation);
            transform.Translate(Vector2.up * speed * 14 * Time.deltaTime);
            base.Update();
        }

    }
    public override void OnHitEntity()
    {
        DestoryBullet();
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        //float delay = 2f;
        if (col.isTrigger == true && col.CompareTag("Player") || col.CompareTag("Collision"))
        {
            this.gameObject.SetActive(false);
        }
    }
}
*/
#endregion

#region ENEMY CODE
/* Added on 11/04/2018
 * 
 * 
public abstract class EnemyClass : CharacterClass,IpoolObject {

    public enum enemyState { stand, moving, attack_init, attack, dash, dash_delay, dead, nothing };
    public enemyState enemyStatemachine;
    protected GameObject target;
    protected GameObject bulletObj;
    protected GameObject enemyBullet;

    public string cutscene_to_trigger;

    protected o_plcharacter enemy;
    protected int exp_amount;
    protected float walkTimer;
    protected Vector3 currpos;
    protected Collider2D hitbx;
    public float attack_power;

    public GameObject dropItem;
    protected float bonusTime;

    protected float dashDelayTimerStart;
    public string type;

    protected float attackdelay;
    protected float attackTimer = 0;
    EnemyWeapon currentweap;

    public string CallID() { return ""; }

    [System.Serializable]
    internal struct EnemyWeapon {
        public float attack_timer;
        public float attack_power;
    }

    [System.Serializable]
    public struct EnemyPhases {

        public EnemyClass host { set; get; }
        public float health_division;
        public bool health_requisites {
            get {
                return host.Health < host.maxHealth * health_division;
            }
        }
    }
    public List<EnemyPhases> enemy_phasemachine = new List<EnemyPhases>();
    protected int currentPhase { get; set; }

    void IpoolObject.SpawnStart() {

        GetFoes();
    } 

    private void Awake()
    {
        currentPhase = 0;
        for(int i = 0; i < enemy_phasemachine.Count; i++)
        {
            EnemyPhases tempphase = new EnemyPhases();
            tempphase.health_division = enemy_phasemachine[i].health_division;
            tempphase.host = this;
            enemy_phasemachine[i] = tempphase;
        }
        charAnimator = GetComponent<Animator>();
    }

    public void GetFoes()
    {
        //Debug.Log(AILibary.AILibary.GetLevelTargetChars(this, AILibary.AILibary.affilation.enemies)[0].name);
        //target_characters = s_levelmanager.alliance_in_level;
    }

    public void TempFunctionToBeDeleted() {
        
        target = GameObject.FindGameObjectWithTag("Player");
        enemy = target.GetComponent<o_plcharacter>();
    }

    protected void TriggerCutscene() {
        GameObject.Find(cutscene_to_trigger).GetComponent<CutsceneHandler>().enabled = true;
    }

    protected bool NearPlayer(float dist, GameObject obj) {
        if (DistanceToAttack(dist, obj)) {
            return true;
        } else { return false; }
    }

    new void Start()
    {
        base.Start();
        GetFoes();
        hurt_del = 0.4f;
    }
    
    
    

    public void dropExp()
    {
        GameObject.Find("Peast").GetComponent<o_plcharacter>().selectedWeapon.exp += exp_amount;
    }

    public void attkAnimationDone() {
        charAnimator.SetBool("IsAttacking", false);
        enemyStatemachine = enemyState.stand;
    }

    public void checkStates()
    {
        if (enemyStatemachine == enemyState.moving) {
            charAnimator.SetBool("IsMoving", true);
        } else {
            charAnimator.SetBool("IsMoving", false);
        }

        if (enemyStatemachine == enemyState.attack_init)
        {
            charAnimator.SetBool("IsAttacking", true);
        }
    }

    public void checkWalkAnims() {
        //verticalDir = Mathf.Clamp(verticalDir, -1, 1);
        //horizontalDir = Mathf.Clamp(horizontalDir, -1, 1);
        charAnimator.SetFloat("X", Mathf.RoundToInt(horizontalDir));
        charAnimator.SetFloat("Y", Mathf.RoundToInt(verticalDir));
    }

    new public void Update() {

        //attack_power = currentweap.attack_power;
        

        dashDelay = dashDelay > 0 ? dashDelay -= Time.deltaTime : 0;
        attackdelay = attackdelay > 0 ? attackdelay -= Time.deltaTime : 0;
        base.Update();

        if (enemy_phasemachine.Count > 0) {
            currentPhase += enemy_phasemachine[currentPhase].health_requisites && currentPhase != enemy_phasemachine.Count ? 1 : 0;
        }
        
        if (Physics2D.OverlapBoxAll(transform.position, new Vector2(500, 500), 90).Length > 0) {
            Vector2 distance_fromChar = AILibary.AILibary.RepelFromOthers(this.gameObject);
            print(distance_fromChar);
            distance_fromChar.Normalize();
            rbody2d.velocity -= distance_fromChar;
        }

        checkStates();
        checkWalkAnims();
        
        bonusTime = bonusTime > 0 ? bonusTime - Time.deltaTime : 0;

        if (Health <= 0)
        {
            enemyStatemachine = enemyState.dead;
        }

        switch (enemyStatemachine) {
            case enemyState.stand:

                currpos = new Vector3(0, 0);
                break;

            case enemyState.dash:
                dash(dashDelayTimerStart);
                Invinciblity = true;
                enemyStatemachine = enemyState.dash_delay;

                break;

            case enemyState.dash_delay:

                if (dashTime >= 0) { dashTime = dashTime - Time.deltaTime;
                }
                else {
                    Invinciblity = false;
                    afterDashAction();
                }

                break;
            case enemyState.dead:
                if (destroyOnDeath) {
                    dropExp();
                    if (bonusTime >= 2.1f)
                    {
                        target.GetComponent<o_plcharacter>().Health++;
                    }
                }
                break;
        }
    }

    public void LookAtPlayer()
    {
        target = GameObject.FindGameObjectWithTag("Player");
        enemy = target.GetComponent<o_plcharacter>();
        currpos = !switch_Dir ? new Vector3(target.transform.position.x - transform.position.x, target.transform.position.y - transform.position.y).normalized : new Vector3(-(target.transform.position.x - transform.position.x), -(target.transform.position.y - transform.position.y)).normalized;
        charDirection = 360 - Mathf.Atan2(currpos.x, currpos.y) * Mathf.Rad2Deg;
    }

    public void MoveCharacter(bool use_motor) {
        if (use_motor) { 
        horizontalDir = currpos.x;
        verticalDir = currpos.y;
        }

        rbody2d.velocity = new Vector2(horizontalDir, verticalDir) * Speed;
    }

    public bool CanSeePlayer()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, target.transform.position - transform.position, 89, 1024);
        Debug.DrawRay(transform.position, target.transform.position - transform.position, Color.green);
        return !hit;
    }

    public enemyState toAttack()
    {
        return enemyState.moving;
    }

    public new void EnemyDamage()
    {
        bonusTime += 0.5f;
    }
    
    public void DamageFunction(Collider2D hitbx)
    {
        if (hurt_sound.Length != 0) { SoundManager.SFX.playSound(hurt_sound[Random.Range(0, hurt_sound.Length)]); }
        StartCoroutine(TakeDamage(hitbx.GetComponent<BulletClass>().attackPower));
        //target = AquireShootTarget(hitbx.GetComponent<BulletClass>());
    }

    public bool ReadyToAttack(float number) {
        bool answer = false;

        while (attackTimer > 0f)
        {
            attackTimer = attackTimer - Time.deltaTime;
        }
        answer = true;
        attackdelay = number;
        return answer;
    }

    public Vector2 CalcDist(GameObject obj) {
        //print(obj.transform.position - this.transform.position + " the distance");
        return obj.transform.position - this.transform.position;
    }

    public bool DistanceToAttack(float dist, GameObject obj)
    {
        if (obj != null) {
            return dist > Mathf.Abs(CalcDist(obj).x) && dist > Mathf.Abs(CalcDist(obj).y);
        } else { return false; }
    }

    public bool AbleToAttackIn(float dist, GameObject obj) {
        print((dist < Mathf.Abs(CalcDist(obj).x) && dist < Mathf.Abs(CalcDist(obj).y)) + " is able to attack");
        return dist < Mathf.Abs(CalcDist(obj).x) && dist < Mathf.Abs(CalcDist(obj).y);
    }

    //if the character does nothing then just move their state to ---> enemyStatemachine = enemyState.stand;
    public abstract void afterDashAction();

    public void dash(float dashtime) {

        Invinciblity = true;
        rbody2d.velocity = new Vector2(horizontalDir * Speed * 11.5f, verticalDir * Speed * 11.5f);
        dashTime = dashtime;
    }
}

namespace AILibary
{

    [System.Serializable]
    public class AILibary
    {

        public enum affilation
        {
            alliance,
            enemies
        };

        public enum generic_aistates
        {
            idle,
            attack,
            searchfortar
        };

        public generic_aistates aistate;

        public static Vector3 GetPos(GameObject target)
        {
            return target.transform.position;
        }

        ///<summary>
        /// This is for the characters who have yet to have a target to attack
        ///</summary>
        public static CharacterClass AquireTarget(GameObject user, List<CharacterClass> target_characters, Vector3 currpos)
        {
            for (int i = 0; i < target_characters.Count; i++)
            {
                CharacterClass pot_target = target_characters[i];
                if (LineOfSight(user, pot_target.gameObject, currpos, user.GetComponent<CharacterClass>().view_distance))
                {
                    return pot_target;
                }
            }
            return null;
        }

        public static bool LineOfSight(GameObject user, GameObject target, Vector3 userview, float viewdist)
        {
            if (Vector3.Distance(target.transform.position, user.transform.position) < viewdist)
            {
                Vector3 dirTotarg = (target.transform.position - user.transform.position).normalized;
                float anglebetweenplyer = Vector2.Angle(userview, dirTotarg);
                if (anglebetweenplyer < 360 / 2f)
                {
                    Debug.DrawLine(user.transform.position, dirTotarg, Color.blue);
                    if (!Physics2D.Linecast(user.transform.position, target.transform.position, user.GetComponent<CharacterClass>().layer1))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Checks if target is not obstructed by a wall.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public static bool CanSeeTarget(GameObject target, GameObject user)
        {
            // LayerMask layer1 = ;
            if (!Physics2D.Linecast(user.transform.position, target.transform.position, user.GetComponent<CharacterClass>().layer1))
            {
                return true;
            }
            // Debug.DrawRay(user.transform.position, target.transform.position - user.transform.position, Color.green);
            return false;
        }
        public static bool CanSeeTarget(Vector3 target, GameObject user)
        {
            if (!Physics2D.Linecast(user.transform.position, target, user.GetComponent<CharacterClass>().layer1))
            {
                return true;
            }
            //Debug.DrawRay(user.transform.position, target - user.transform.position, Color.green);
            return false;
        }


        public static Vector2 CalcDist(GameObject obj, GameObject user_character)
        {
            return obj.transform.position - user_character.transform.position;
        }

        public static Vector2 CalcDist(Vector3 obj, GameObject user_character)
        {
            return obj - user_character.transform.position;
        }

        public static bool DistanceToAttack(float dist, Vector3 obj, GameObject user_character)
        {
            if (obj != null)
            {
                if (CanSeeTarget(obj, user_character))
                {
                    if (dist > Mathf.Abs(CalcDist(obj, user_character).x) && dist > Mathf.Abs(CalcDist(obj, user_character).y))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public static bool DistanceToAttack(float dist, GameObject obj, GameObject user_character)
        {
            if (obj != null)
            {
                if (CanSeeTarget(obj, user_character))
                {
                    if (dist > Mathf.Abs(CalcDist(obj, user_character).x) && dist > Mathf.Abs(CalcDist(obj, user_character).y))
                    {
                        return true;
                    }
                }
            }
            return false;
        }


        public static List<CharacterClass> GetLevelTargetChars(CharacterClass user, affilation e_aff)
        {
            List<CharacterClass> enemy = new List<CharacterClass>();

            //Enemies get hero/ally characters, allies get enemy characters
            switch (e_aff)
            {

                case affilation.alliance:
                    if (GameObject.Find("Area").transform.Find("Enemies"))
                    {
                        EnemyClass[] list_of_enemies = GameObject.Find("Area").transform.Find("Enemies").GetComponentsInChildren<EnemyClass>();
                        foreach (EnemyClass en in list_of_enemies)
                        {
                            enemy.Add(en);
                        }
                    }
                    break;

                case affilation.enemies:
                    if (GameObject.Find("Area").transform.Find("Characters"))
                    {
                        CharacterClass[] list_of_enemies = GameObject.Find("Area").transform.Find("Characters").GetComponentsInChildren<AllyClass>();
                        foreach (CharacterClass en in list_of_enemies)
                        {
                            enemy.Add(en);
                        }
                        enemy.Add(GameObject.Find("Players").GetComponent<PlayerManager>().Selected_character.GetComponent<o_plcharacter>());
                    }
                    break;
            }
            return enemy;
        }

        public static void MoveCharacter(CharacterClass user, Vector3 currpos)
        {

            user.horizontalDir = currpos.x;
            user.verticalDir = currpos.y;
            user.rbody2d.velocity = new Vector2(user.horizontalDir, user.verticalDir) * user.Speed;
        }

        public static void MoveCharacter(CharacterClass user)
        {
            user.rbody2d.velocity = new Vector2(user.horizontalDir, user.verticalDir) * user.Speed;
        }


        public static CharacterClass AquireTarget(GameObject user, List<CharacterClass> target_characters)
        {
            CharacterClass new_target = null;
            for (int i = 0; i < target_characters.Count; i++)
            {
                CharacterClass pot_target = target_characters[i];
                if (DistanceToAttack(150, pot_target.gameObject, user))
                {
                    new_target = pot_target;
                    return new_target;
                }
            }
            return null;
        }

        public static Vector2 RepelFromOthers(GameObject user)
        {
            Collider2D[] coll = Physics2D.OverlapBoxAll(user.transform.position, new Vector2(500, 500), 0);

            foreach (Collider2D col in coll)
            {
                if (col.IsTouching(user.GetComponent<BoxCollider2D>()))
                {
                    return user.transform.position - col.gameObject.transform.position;
                }
            }
            return Vector2.zero;
        }

        public static bool CheckIfCornered(GameObject user, Vector3 direction)
        {
            Debug.DrawLine(user.transform.position, user.transform.position - (direction * 12), Color.blue);
            if (Physics2D.Linecast(user.transform.position, user.transform.position - (direction * 12), user.GetComponent<CharacterClass>().layer1))
            {
                return true;
            }
            return false;
        }

        public static float SetCharDirection(Vector3 currpos)
        {
            return 360 - Mathf.Atan2(currpos.x, currpos.y) * Mathf.Rad2Deg;
        }

        public static Vector3 LookAtTarget(GameObject user, Vector3 target)
        {
            user.GetComponent<CharacterClass>().charDirection = SetCharDirection(new Vector3(target.x - user.transform.position.x, target.y - user.transform.position.y).normalized);
            return new Vector3(target.x - user.transform.position.x, target.y - user.transform.position.y).normalized;
        }

        public static Vector3 LookAtTarget(GameObject user, GameObject target)
        {
            user.GetComponent<CharacterClass>().charDirection = SetCharDirection(new Vector3(target.transform.position.x - user.transform.position.x, target.transform.position.y - user.transform.position.y).normalized);
            return new Vector3(target.transform.position.x - user.transform.position.x, target.transform.position.y - user.transform.position.y).normalized;
        }
    }
}

*/
#endregion

#region ALLY CODE
/* Added on 11/04/2018
 * This was created in 04/02/‎2018, it was meant to for AI that was on the player's side,
 * however, I was soon to realise that I had to bore myself with essentialy copying the code from
 * the Enemy Class.
 * 
 * The solution to this was me merging the two together to just be an "NPC" class.
 * I then created a faction system for all characters (including the playable ones) which dictates
 * what character could attack who.
 * 
 * I think this is one of the best things that has happened to this project, because I can make enemies have their own
 * goals rather than just trying to kill the player, some enemies attack others.
 * This can also lead into some pretty fun to mess with gameplay.
 * 

 public class AllyClass : CharacterClass {
    

    public enum allyState { stand, moving, attack_init, attack, dash, dash_delay, dead, nothing };
    public allyState state;

    protected float attackdelay;
    public enum artificial_intelligence { follow, attacking, standing, heal_player, walk_back }
    public artificial_intelligence ai_states;

    public EnemyClass target;
    public AudioClip dashsound;
    protected GameObject player_leader;
    protected Vector3 currpos;
    protected o_plcharacter leader;

    public int current_weapon_Pow;

    new void Start () {
        base.Start();
        SetTargChars();
    }
    

    public List<EnemyClass> GetLevelTargetChars() {
        List<EnemyClass> enemy = new List<EnemyClass>();
        if (GameObject.Find("Area").transform.Find("Enemies")) {

            EnemyClass[] list_of_enemies = GameObject.Find("Area").transform.Find("Enemies").GetComponentsInChildren<EnemyClass>();
            foreach (EnemyClass en in list_of_enemies) {
                enemy.Add(en);
            }
        }
        return enemy;
    }

    public void checkWalkAnims()
    {
        //verticalDir = Mathf.Clamp(verticalDir, -1, 1);
        //horizontalDir = Mathf.Clamp(horizontalDir, -1, 1);
        charAnimator.SetFloat("X", Mathf.RoundToInt(horizontalDir));
        charAnimator.SetFloat("Y", Mathf.RoundToInt(verticalDir));
    }

   


    public void MoveCharacter()
    {
        if (target == null) {
            if (!AILibary.AILibary.DistanceToAttack(45, player_leader, this.gameObject)) {
                rbody2d.velocity = new Vector2(currpos.x, currpos.y).normalized * Speed;
                charAnimator.SetBool("IsMoving", true);
            } else {
                rbody2d.velocity = new Vector2(0, 0);
                charAnimator.SetBool("IsMoving", false);
            }
        } else {
            if (!AILibary.AILibary.DistanceToAttack(150, target.gameObject, this.gameObject)) {
                rbody2d.velocity = new Vector2(currpos.x, currpos.y).normalized * Speed;
                charAnimator.SetBool("IsMoving", true);
            } else {
                rbody2d.velocity = new Vector2(0, 0);
                if (attackdelay <= 0) {
                    ShootBullet("bullet", 14, 0.3f);
                    attackdelay = 0.4f;
                }
                charAnimator.SetBool("IsMoving", false);
            }
        }


        horizontalDir = currpos.x;
        verticalDir = currpos.y;
    }

    public void SetTargChars()
    {
       target_characters = AILibary.AILibary.GetLevelTargetChars(this, AILibary.AILibary.affilation.alliance);
    }

    new public void Update () {
        base.Update();
        attackdelay = attackdelay > 0 ? attackdelay -= Time.deltaTime : 0;
        dashDelay = dashDelay > 0 ? dashDelay -= Time.deltaTime : 0;

        switch (state) {
            case allyState.dash:

                dash(0.003f);
                dashDelay = 1.2f;
                Invinciblity = true;
                SoundManager.SFX.playSound(dashsound);
                state = allyState.dash_delay;

                break;

            case allyState.dash_delay:

                if (dashTime >= 0) {
                    dashTime = dashTime - Time.deltaTime;
                } else {
                    Invinciblity = false;
                    state = allyState.moving;
                }
                break;
        }
    
    }

    public void dash(float dashtime)
    {
        Invinciblity = true;
        rbody2d.velocity = new Vector2(horizontalDir * Speed * 11.5f, verticalDir * Speed * 11.5f);
        dashTime = dashtime;
    }
}
*/
#endregion

#region BOMB CODE
/* Added on 11/04/2018
 * This was added in quite early on into the project (5th july) along with the forward attack, in fact it was a usable weapon in
 * the 2017 pre-alpha (not the public alpha I relesed in december of that year).
 * 
 * The purpose of this bomb was to hurt anything, including the player within a big radius
 * it also dealt collosal damage to enemies like the snakes.

public class Bomb : BulletClass {
    public bool isExploded;
    
    new void Start ()
    {

        playerBullet = true;
        col.enabled = false;
        isExploded = false;
        speed = 0f;
        parent = GameObject.Find("Peast");
        attackPower = parent.GetComponent<p_peast>().selectedWeapon.attackPow;
    }
	
	new void Update () {
        base.Update();
        if (delay <= 0.01f) { isExploded = true; col.enabled = true; cameraStuff.cameraShake(10,10,1); }
    }
}
*/
#endregion

#region NEW WEAPON CODE
/* Added on 11/04/2018
public class NewWeapon : MonoBehaviour {

    public WeaponBase weapon;
    List<WeaponBase> weapontypes = new List<WeaponBase>();
    public int weaponID;
    public AudioClip ammosound;

    void OnTriggerStay2D(Collider2D col)
    {
        if (col.name == "Peast")
        {
            o_plcharacter playerdat;
            playerdat = col.GetComponent<o_plcharacter>();

            if (playerdat.name == "Peast") {
                if (!CheckIfWeaponsExists(weapon.name)) {
                    playerdat.weapons.Add(weapon);
                    SoundManager.SFX.playSound(ammosound);
                    this.gameObject.SetActive(false);
                }
            }
        }
    }

    bool CheckIfWeaponsExists(string w_name) {
        for (int i = 0; i < weapontypes.Count; i++) {
            if (weapontypes[i].name == w_name) {
                return true;
            }
        }
        return false;
    }
}
*/
#endregion

#region MILBERT CODE
/* Added on 11/04/2018
 * This is Milbert's old code
 * You may see that he was called GoblinX or Goblin before I came up with the name Milbert
 * Fun fact: He was based on a drawing of mine I did back a year ago (April 2017), it was meant to be
 * a placeholder.
 * 
    new void Start ()
    {
        hurtGob = Resources.Load("Sound/goblinHurt") as AudioClip;
        dodgeGob = Resources.Load("Sound/goblinDodge") as AudioClip;
        dieGob = Resources.Load("Sound/goblinDie") as AudioClip;
        attkGob = Resources.Load("Sound/goblinAttk") as AudioClip;
        destroyOnDeath = false;
        dashDelayTimerStart = 0.2f;
        maxHealth = 80;
		initialSpeed = 83f;
        attackdelay = 0.3f;
        enemyStatemachine = enemyState.moving;
        thisrender = this.gameObject.GetComponent<SpriteRenderer>();
        base.Start();
    }

    void OnTriggerEnter2D(Collider2D hitbx)
    {
        if (hitbx.isTrigger == true && hitbx.CompareTag("Bullet"))
        {
            if (!Invinciblity) {

                StartCoroutine(TakeDamage(hitbx.GetComponent<BulletClass>().attackPower));
                if (SoundManager.SFX != null)
                {
                    SoundManager.SFX.playSound(hurtGob);
                }
                EnemyDamage();
                cameraStuff.cameraShake(5, 5, 0.2f);
            }
        }
    }


    new void Update() {
        base.Update();

        PlayerGUI.GUIObj.activateBossHP();
        attackdelay = attackdelay > 0 ? attackdelay -= Time.deltaTime : 0;

        enemyBullet = GameObject.FindGameObjectWithTag("Bullet");

        switch (enemyStatemachine)
        {
            case enemyState.stand:

                enemyStatemachine = enemyState.moving;
                break;

            case enemyState.moving:

                thisrender.color = Color.white;

                if (dashDelay <= 0){
                    if (attackdelay == 0) {
                        enemyStatemachine = toAttack();
                    }
                }
                LookAtPlayer();
                MoveCharacter(true);
                break;

            case enemyState.attack_init:

                switch (AI)
                {
                    case GOBLIN_AI.DASH_ATTACK:

                        if (dashDelay == 0)
                        {
                            horizontalDir += Random.Range(-0.3f, 0.3f);
                            verticalDir += Random.Range(-0.3f, 0.3f);
                            dashDelayTimerStart = 0.12f;
                            StartCoroutine(anticipationForAttk());
                            rbody2d.velocity = Vector2.zero;
                            enemyStatemachine = enemyState.nothing;
                        }

                        break;

                    case GOBLIN_AI.DASH_COUNTER:

                        dashDelay = 0.3f;
                        SoundManager.SFX.playSound(dodgeGob);
                        dashDelayTimerStart = 0.007f;
                        enemyStatemachine = enemyState.dash;

                        break;

                    case GOBLIN_AI.BACK_UP:

                        print("back up");
                        horizontalDir = -horizontalDir + Random.Range(-1, 2);
                        verticalDir = -verticalDir + Random.Range(-1, 2);

                        dashDelayTimerStart = 0.12f;
                        enemyStatemachine = enemyState.dash;

                        break;
                }
                break;

            case enemyState.attack:

                if (ReadyToAttack(0.6f)) {
                    charAnimator.SetBool("IsAttacking", false);
                    AI = GOBLIN_AI.NONE; print("Milbert attack done");
                    enemyStatemachine = enemyState.stand;
                }

                break;
            case enemyState.dead:
                PlayerGUI.GUIObj.deActivateBossHP();
                TriggerCutscene();
                break;
        }
    }

    new public enemyState toAttack()
{
    if (enemyBullet != null)
    {
        if (enemyBullet.GetComponent<BulletClass>().playerBullet)
        {

            if (currentPhase == 0)
            {
                if (DistanceToAttack(60f, enemyBullet) && attackdelay == 0)
                {
                    AI = GOBLIN_AI.DASH_COUNTER;
                    return enemyState.attack_init;
                }
            }

            //weak attack
            if (currentPhase > 0)
            {
                if (DistanceToAttack(100f, enemyBullet) && attackdelay == 0)
                {
                    AI = GOBLIN_AI.BACK_UP;
                    SoundManager.SFX.playSound(dodgeGob);
                    print("BackUP!");
                    return enemyState.attack_init;
                }
            }
        }
    }
    else
    {

        if (DistanceToAttack(250f, target) && attackdelay == 0)
        {
            AI = GOBLIN_AI.DASH_ATTACK;
            return enemyState.attack_init;
        }
    }
    return enemyState.moving;
}

public IEnumerator anticipationForAttk()
{
    thisrender.color = Color.cyan;
    charAnimator.SetBool("IsAttacking", true);

    yield return new WaitForSeconds(0.36f);
    SoundManager.SFX.playSound(attkGob);
    dashDelay = 0.09f;
    enemyStatemachine = enemyState.dash;
    print(dashTime);
    attack();

    yield return null;
}

public void attack()
{

    if (bulletObj == null)
    {
        GameObject bitev = (Resources.Load("Prefabs/Bullet prefabs/GoblinBash") as GameObject);
        GameObject bite = Instantiate(bitev, transform.position, Quaternion.Euler(0, 0, 90 - charDirection)) as GameObject;
        bulletObj = bite;
        bite.transform.parent = this.transform;
    }
}

public override void afterDashAction()
{
    switch (AI)
    {
        case GOBLIN_AI.NONE:

            dashDelay = 0.12f;
            enemyStatemachine = enemyState.moving;
            break;

        case GOBLIN_AI.DASH_ATTACK:

            attacktimer = 0.28f;
            enemyStatemachine = enemyState.attack;

            break;

        case GOBLIN_AI.DASH_COUNTER:

            attacktimer = 0.03f;
            enemyStatemachine = enemyState.attack;

            break;

        case GOBLIN_AI.BACK_UP:

            print("back up");
            dashDelay = 0.12f;
            attackdelay = 0.6f;
            enemyStatemachine = enemyState.dash;
            AI = GOBLIN_AI.NONE;
            break;
    }
}

*/
#endregion

#region PLAYERMANAGER CODE
/* Added on 02/04/2018
 * This is the code that was basicaly the core of how the players were selected and managed.
 * I removed this because I decided to have the level itself select the playable characters rather than using this
 * 
public List<GameObject> PlayableCharacters = new List<GameObject>();
public GameObject Selected_character;
public int Selected_player;
public GameObject gameover;

bool gameoverinitilaized = false;

void checkforGamover()
{
    if (Selected_character.GetComponent<o_plcharacter>().characterstates == CharacterClass.CHARACTER_STATEMACHINE.DEAD)
    {
        if (!gameoverinitilaized)
        {
            gameOver();
            gameoverinitilaized = true;
        }
    }
}

public void gameOver()
{
    Instantiate(gameover, Selected_character.transform.position, Quaternion.identity);
}

public void Update()
{
    checkforGamover();
}

public void SwitchCharacter()
{

    Selected_player = Mathf.Clamp(Selected_player, 0, PlayableCharacters.Count - 1);

    for (int i = 0; i < PlayableCharacters.Count; i++)
    {
        if (i == Selected_player)
        {
            Selected_character = (GameObject)PlayableCharacters[Selected_player];
            Selected_character.SetActive(true);
            continue;
        }
        else
        {
            PlayableCharacters[i].SetActive(false);
        }
    }
}
*/
#endregion

#region PLAYER/BOSS GUI CODE
/* Put here on 23/03/2018
    //For drawing and enabling the boss' health so the player can see it
    //Any sort of programmer would probably figure out that the boss was going to have a long bar
    //Those wondering what colour it was, it was purple

    //This also drew the Player HP and Stats like what weapon they were currently holding
    //I felt like these functions were too specific and so hard-coded that I decided to have a function that draws objects in a line
    //Rather than having some specific function that draws the player's health (like you see below)
      
        public void activateBossHP() {

            BossInfo = GameObject.FindGameObjectWithTag("Boss").GetComponent<EnemyClass>();
        }

        public void DrawBossStats()
        {
            BossHeart.enabled = true;
            BossHeartMax.enabled = true;
            float HP_CAC = BossInfo.Health / BossInfo.maxHealth;
            Rect BOSSHP = BossHeart.GetComponent<GUITexture>().pixelInset;
            BOSSHP = new Rect(BOSSHP.x, BOSSHP.y, HP_CAC * 300, BOSSHP.height);
            BossHeart.pixelInset = BOSSHP;
        }

        public void deActivateBossHP()
        {
            BossHeart.enabled = false;
            BossHeartMax.enabled = false;
            BossInfo = null;
        }

        if (BossInfo != null) {
                    guiMaxBossHp = BossInfo.GetComponent<EnemyClass> ().maxHealth;
                    guiBossHp = BossInfo.GetComponent<EnemyClass> ().Health;
                    DrawBossStats();
         }
         
    void OnGUI () {

        if (playerInfo.Selected_character != null) {
            guiMaxHP = playerInfo.Selected_character.GetComponent<o_plcharacter>().maxHealth;
            guiHP = playerInfo.Selected_character.GetComponent<o_plcharacter>().Health;
            DrawPlayerStats();

            WeaponBase slctWPN = playerInfo.Selected_character.GetComponent<o_plcharacter>().selectedWeapon;

            weapon.text = ("Weapon: " + slctWPN.name) + "\n" + "Level: "+ slctWPN.level + "\n" + slctWPN.exp + "/ " + slctWPN.expToNextLevel[slctWPN.level];

            if (slctWPN.ammoLimits == true) { ammoTxt.text = "Ammo:" + slctWPN.ammoCap; }
            else { ammoTxt.text = ""; }
            
		}
    }

    void DrawPlayerStats() {
        
        int filler_y = 0;
        int x_pos = 0;
        for (int i = 0; i < guiHP; i++)
        {
            if (x_pos == 15)
            {
                filler_y += 10;
                x_pos = 0;
            }
            x_pos++;
            GUI.DrawTexture(new Rect((x_pos * 2.5f - 1) * Screen.width / 100, (6 + filler_y) * Screen.height / 100, 32, 32), Heart);
        }
    }*/
#endregion

#region LEVEL LOADER/EDITOR CODE
/* Put here on 18/03/2018
 * This is the old levelmanager. It was used to load levels.
 * The collisions, enemies and items were generated from a small image which would be read from this.
 * This was used in the December 2017 Public Alpha.
 * Boy... has a lot changed since then.
 * I eventually got sick of this thing, making one level required you to constantly expand loads of lists to have the map, player spawn point, camera bounds-
 * Jesus! I'm glad I made the new editor, it has a lot of better things
 * 
 * By the time the 2nd level manager was made, I only used this to create the base level (found in the "GenerateObjects" function)
 * but even then, that was becoming useless.
 * 
 * 
public class LevelManager : MonoBehaviour
{

    public int level;
    public List<Texture2D> levelMaps = new List<Texture2D>();
    public List<string> levels = new List<string>();

    public GameObject Player;
    public GameObject Level;

    Vector3 currentPlayerPos;

    public List<Vector3> newPlayerPos;
    public List<Vector3> mincameraBounds;
    public List<Vector3> maxcameraBounds;

    public GameObject levelObj;
    public GameObject loadObj;

    int CharIndex;
    int tileWidth = 25;

    string levelName;
    bool inEditor = true;

    public Texture2D mapBase;
    public PixelColour[] colourMatch;

    private List<GameObject> tilesCreated;
    public s_levelmanager host_manager;

    public List<AudioClip> music = new List<AudioClip>();


    void Start()
    {
        inEditor = false;
        deleteMap(); ;
        Player = GameObject.Find("Players");
        if (!MainMenu.isLoadgame)
        {
            level = 0;
            LegacyLoad();
        }
        else
        {
            Instantiate(loadObj, transform.position, Quaternion.identity);

        }
    }

    public void GenerateObjects()
    {
        GameObject area = new GameObject("Area");
        GameObject enemies = new GameObject("Enemies");
        GameObject boundary = new GameObject("Boundaries");
        GameObject scenery = new GameObject("Scenery");
        GameObject characters = new GameObject("Characters");
        GameObject cutscenes = new GameObject("Cutscenes");
        GameObject itemsObj = new GameObject("Items");
        GameObject Objects = new GameObject("Objects");
        GameObject goal = Instantiate(Resources.Load("Goal") as GameObject);

        characters.name = "Characters";
        enemies.name = "Enemies";
        boundary.name = "Boundaries";
        scenery.name = "Scenery";
        itemsObj.name = "Items";
        cutscenes.name = "Cutscenes";
        Objects.name = "Objects";
        goal.name = "Goal";

        Level = area;
        GameObject levsav = Instantiate(Resources.Load("Levels/" + host_manager.levels[host_manager.current_level]) as GameObject);
        levsav.name = host_manager.levels[host_manager.current_level].name;

        if (levsav.GetComponent<LevelData>().mapBase != null)
        {
            mapBase = levsav.GetComponent<LevelData>().mapBase;
        }

        goal.transform.SetParent(area.transform);
        Objects.transform.SetParent(area.transform);
        itemsObj.transform.SetParent(area.transform);
        cutscenes.transform.SetParent(area.transform);
        scenery.transform.SetParent(area.transform);
        boundary.transform.SetParent(area.transform);
        characters.transform.SetParent(area.transform);
        enemies.transform.SetParent(area.transform);
    }



    public void LegacyLoad()
    {

        print("Obeselete");
        levelName = levels[level];
        if (Level != null) { deleteMap(); }
        mapBase = levelMaps[level];

        GameObject characters = Instantiate((Resources.Load("Levels/" + levelName + "/" + "Characters", typeof(GameObject))), transform.position, Quaternion.identity) as GameObject;
        GameObject enemies = Instantiate((Resources.Load("Levels/" + levelName + "/" + "Enemies", typeof(GameObject))), transform.position, Quaternion.identity) as GameObject;
        GameObject boundaries = Instantiate((Resources.Load("Levels/" + levelName + "/" + "Boundaries", typeof(GameObject))), transform.position, Quaternion.identity) as GameObject;
        GameObject scenery = Instantiate((Resources.Load("Levels/" + levelName + "/" + "Scenery", typeof(GameObject))), transform.position, Quaternion.identity) as GameObject;
        GameObject itemsandgoodies = Instantiate((Resources.Load("Levels/" + levelName + "/" + "Items and goodies", typeof(GameObject))), transform.position, Quaternion.identity) as GameObject;
        GameObject cutscenes = Instantiate((Resources.Load("Levels/" + levelName + "/" + "Cutscenes", typeof(GameObject))), transform.position, Quaternion.identity) as GameObject;
        GameObject goal = Instantiate((Resources.Load("Levels/" + levelName + "/" + "Goal", typeof(GameObject))), transform.position, Quaternion.identity) as GameObject;
        GameObject objects = Instantiate((Resources.Load("Levels/" + levelName + "/" + "Objects", typeof(GameObject))), transform.position, Quaternion.identity) as GameObject;

        characters.name = "Characters";
        enemies.name = "Enemies";
        boundaries.name = "Boundaries";
        scenery.name = "Scenery";
        itemsandgoodies.name = "Items and goodies";
        cutscenes.name = "Cutscenes";
        goal.name = "Goal";
        objects.name = "Objects";

        if (music[level] != null && SoundManager.SFX != null)
        {
            SoundManager.SFX.playMusic(music[level], true);
        }

        Level = Instantiate(levelObj, transform.position, Quaternion.identity);
        Level.name = "Area";


        goal.transform.parent = Level.transform;
        characters.transform.parent = Level.transform;
        enemies.transform.parent = Level.transform;
        boundaries.transform.parent = Level.transform;
        scenery.transform.parent = Level.transform;
        itemsandgoodies.transform.parent = Level.transform;
        cutscenes.transform.parent = Level.transform;
        objects.transform.parent = Level.transform;

        if (!inEditor)
        {
            playerUpdatePosition(newPlayerPos[level]);
        }

        cameraScript cam = GameObject.Find("Main Camera").GetComponent<cameraScript>();
        cam.GetCameraBounds(mincameraBounds[level], maxcameraBounds[level]);


    }

    public void deleteMap()
    {
        if (inEditor)
        {
            DestroyImmediate(Level);
        }
        else
        {
            Destroy(Level);

        }

    }

    public void loadMap()
    {
        CharIndex = 0;
        Color32[] pixels = mapBase.GetPixels32();
        int width = mapBase.width;
        int height = mapBase.height;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                addTileAtPos(x, y);
            }
        }
        print("LoadedMap");
    }

    public void deleteTiles()
    {
        if (inEditor)
        {
            foreach (GameObject pix in tilesCreated)
            {
                print("Deleting map");
                DestroyImmediate(pix);
            }

            tilesCreated.Clear();
        }
        else
        {
            foreach (GameObject pix in tilesCreated)
            {
                Destroy(pix);
            }
        }
    }

    public void playerUpdatePosition(Vector3 vec)
    {

        Player.transform.position = vec;

        PlayerManager pManager = Player.GetComponent<PlayerManager>();

        for (int i = 0; i < pManager.PlayableCharacters.Count; i++)
        {
            pManager.PlayableCharacters[i].transform.position = vec;
        }
    }

    void addTileAtPos(float x, float y)
    {
        Color pixelColour = mapBase.GetPixel(Mathf.FloorToInt(x), Mathf.FloorToInt(y));
        foreach (PixelColour pix in colourMatch)
        {
            if (pix.colour == pixelColour)
            {
                Vector3 pos = new Vector3(x * tileWidth, y * tileWidth, 2);
                GameObject go = null;
                if (pix.colour != colourMatch[2].colour)
                {
                    go = Instantiate(pix.prefab, pos, Quaternion.identity) as GameObject;
                    go.name = pix.prefab.name;

                    go.transform.parent = Level.transform.Find("Scenery").transform;
                }

                if (pix.colour == colourMatch[1].colour || pix.colour == colourMatch[6].colour)
                {
                    CharIndex++;
                    go.transform.parent = Level.transform.Find("Enemies").transform;
                    go.transform.position = new Vector3(go.transform.position.x, go.transform.position.y, 3);
                    if (pix.colour == colourMatch[1].colour) { go.name = colourMatch[1].prefab.name + "_" + CharIndex; }
                    if (pix.colour == colourMatch[6].colour) { go.name = colourMatch[6].prefab.name + "_" + CharIndex; }
                }
                if (pix.colour == colourMatch[2].colour)
                {
                    newPlayerPos.Add(pos);
                    print(newPlayerPos);
                    playerUpdatePosition(newPlayerPos[level]);
                }
                if (pix.colour == colourMatch[3].colour || pix.colour == colourMatch[4].colour || pix.colour == colourMatch[5].colour)
                {
                    go.transform.parent = Level.transform.Find("Items").transform;
                }
            }
        }
    }


}


[System.Serializable]
public struct PixelColour
{
    public Color colour;
    public GameObject prefab;
}


*/
#endregion

#region SNANAKE CODE
/*
 * Added on 18/03/2018
 * 
    int dirx;
    int diry;

    enum states {
        back_away,
        attack
    }
    states state = states.attack;

    public AILibary.AILibary.generic_aistates aistat = AILibary.AILibary.generic_aistates.idle;

    AudioClip laugh;
    bool near_player;

    void Awake()
    {
        type = "Snanake";
    }

    new void Start () {
        dashDelayTimerStart = 0.06f;
        maxHealth = 4;
		initialSpeed = 75f;
        exp_amount = 2;
        base.Start();
        GetFoes();
        //TempFunctionToBeDeleted();

        view_distance = 390f;

        dropItem = Resources.Load("Prefabs/Items/Healing Heart") as GameObject;
        laugh = Resources.Load("Sound/snanake attk") as AudioClip;
	}

	void OnTriggerEnter2D(Collider2D attackcol){

		if (attackcol.CompareTag("Player")) {
			enemy.TakeDamage(1);
		}
	}

void OnTriggerEnter2D(Collider2D hitbx)
{
    if (hitbx.isTrigger == true && hitbx.CompareTag("Bullet"))
    {
        DamageFunction(hitbx);
        EnemyDamage();
    }
}

new void Update()
{
    base.Update();
    transform.Find("BiteAttk").rotation = Quaternion.Euler(0, 0, charDirection + 90);
    GameObject enemyBullet = GameObject.FindGameObjectWithTag("Bullet");
    // Debug.DrawLine(transform.position , new Vector3(horizontalDir,verticalDir), Color.blue);
    checkWalkAnims();
           
            
                enemy = (o_plcharacter)AILibary.AILibary.AquireTarget(this.gameObject, target_characters);
            }

    if (target == null)
    {
        if (AILibary.AILibary.AquireTarget(this.gameObject, s_levelmanager.alliance_in_level) != null)
        {
            target = AILibary.AILibary.AquireTarget(this.gameObject, s_levelmanager.alliance_in_level).gameObject;
        }
    }

    switch (enemyStatemachine)
    {

        case enemyState.stand:

            Speed = initialSpeed;

            horizontalDir = 0;
            verticalDir = 0;
            MoveCharacter(false);

            if (AILibary.AILibary.DistanceToAttack(view_distance, target, this.gameObject))
            {
                aistat = AILibary.AILibary.generic_aistates.attack;
                enemyStatemachine = enemyState.moving;
            }

            break;

        case enemyState.moving:
            switch (aistat)
            {
                case AILibary.AILibary.generic_aistates.idle:
                    if (AILibary.AILibary.DistanceToAttack(view_distance, target, this.gameObject))
                    {
                        aistat = AILibary.AILibary.generic_aistates.attack;
                        enemyStatemachine = enemyState.moving;
                        print("see");
                    }

                    if (walkTimer > 0)
                    {
                        walkTimer = walkTimer - Time.deltaTime;
                    }
                    else
                    {
                        MoveCharacter(false);
                        horizontalDir = (int)Random.Range(-1, 1);
                        verticalDir = (int)Random.Range(-1, 1);
                        if (horizontalDir == 0 && verticalDir == 0)
                        {
                            enemyStatemachine = enemyState.stand;
                        }
                        walkTimer = Random.Range(0.3f, 2);
                    }
                    break;


                case AILibary.AILibary.generic_aistates.searchfortar:

                    if (!AILibary.AILibary.DistanceToAttack(34, tar_last_pos, this.gameObject))
                    {
                        currpos = AILibary.AILibary.LookAtTarget(this.gameObject, tar_last_pos);
                        MoveCharacter(true);

                        if (AILibary.AILibary.DistanceToAttack(view_distance, tar_last_pos, this.gameObject))
                        {
                            aistat = AILibary.AILibary.generic_aistates.attack;
                        }
                    }
                    else
                    {
                        aistat = AILibary.AILibary.generic_aistates.idle;
                    }
                    break;

                case AILibary.AILibary.generic_aistates.attack:


                    if (AILibary.AILibary.CanSeeTarget(target, this.gameObject))
                    {
                        switch (state)
                        {
                            case states.attack:


                                tar_last_pos = AILibary.AILibary.GetPos(target);
                                currpos = AILibary.AILibary.LookAtTarget(this.gameObject, target);
                                MoveCharacter(true);

                                dashDelayTimerStart = 0.12f;
                                switch_Dir = false;
                                Speed = initialSpeed;

                                if (AILibary.AILibary.DistanceToAttack(100, target, this.gameObject))
                                {

                                    if (attackdelay == 0)
                                    {
                                        print("see");
                                        attackdelay = 2.7f;
                                        state = states.back_away;
                                    }
                                }
                                if (AILibary.AILibary.DistanceToAttack(270, target, this.gameObject))
                                {
                                    if (attackdelay == 0)
                                    {
                                        enemyStatemachine = enemyState.attack_init;
                                        attackTimer = 0.01f;
                                    }
                                }

                                break;

                            case states.back_away:

                                switch_Dir = true;
                                Speed = 140f;
                                currpos = -AILibary.AILibary.LookAtTarget(this.gameObject, target);
                                AILibary.AILibary.MoveCharacter(this, currpos);

                                if (AILibary.AILibary.DistanceToAttack(105, enemyBullet, this.gameObject) && enemyBullet != null)
                                {
                                    if (dashDelay == 0)
                                    {
                                        dashDelayTimerStart = 0.03f;
                                        attackTimer = 0.01f;
                                        enemyStatemachine = enemyState.attack_init;
                                        SoundManager.SFX.playSound(laugh);
                                    }
                                }
                                if (AILibary.AILibary.CheckIfCornered(this.gameObject, -currpos * 2))
                                {
                                    print("I'm blocked");
                                    switch_Dir = false;
                                    attackdelay = 0.5f;
                                    state = states.attack;
                                }

                                if (attackdelay == 0)
                                {
                                    switch_Dir = false;
                                    print("Back to attack");
                                    //aistat = AILibary.AILibary.generic_aistates.attack;
                                    state = states.attack;
                                }

                                break;
                        }

                    }
                    else
                    {
                        walkTimer = 0;
                        aistat = AILibary.AILibary.generic_aistates.searchfortar;
                    }
                    break;
            }

            break;

        case enemyState.dash:

            attackdelay = 0.4f;
            break;

        case enemyState.attack_init:
            if (dashDelay == 0)
            {
                StartCoroutine(beforeBite());
                enemyStatemachine = enemyState.nothing;
            }

            break;

        case enemyState.attack:

            Invinciblity = true;
            if (ReadyToAttack(0.6f))
            {
                thisrender.color = Color.white;
                charAnimator.SetBool("IsAttacking", false);
                transform.Find("BiteAttk").GetComponent<BoxCollider2D>().enabled = false;
                Invinciblity = false;
                aistat = AILibary.AILibary.generic_aistates.idle;
                enemyStatemachine = enemyState.moving;

            }
            break;

    }
}

public override void afterDashAction()
{
    switch (state)
    {
        case states.back_away:
            state = states.attack;

            break;
    }

    attackTimer = 0.09f;
    enemyStatemachine = enemyState.attack;
}

public IEnumerator beforeBite()
{
    enemyStatemachine = enemyState.nothing;
    thisrender.color = Color.cyan;
    charAnimator.SetBool("IsAttacking", true);

    rbody2d.velocity = Vector2.zero;
    yield return new WaitForSeconds(0.22f);

    dashDelay = 0.08f;
    enemyStatemachine = enemyState.dash;
    Invinciblity = true;
    transform.Find("BiteAttk").GetComponent<BoxCollider2D>().enabled = true;

    yield return null;
}
*/
#endregion

#region FROG-GUNNER CODE
/* Put here on 18/03/2018

enum FROG_AI { Normal, SeePlayer, walk_back };
    FROG_AI frogStates;
    AudioClip shoot;
    string thistype;

    void IpoolObject.SpawnStart() {

    }

    void Awake()
    {
        type = "FrogShooter";
    }

    new void Start () {
        dashDelayTimerStart = 0.04f;
        maxHealth = 5;
        initialSpeed = 67f;
        exp_amount = 5;

        dropItem = Resources.Load("Prefabs/Items/Healing Heart") as GameObject;
        shoot = Resources.Load("Sound/shoot") as AudioClip;
        Invinciblity = false;
        base.Start();
    }

    void OnTriggerEnter2D(Collider2D hitbx)
    {
        if (hitbx.isTrigger == true && hitbx.CompareTag("Bullet"))
        {
            StartCoroutine(TakeDamage(hitbx.GetComponent<BulletClass>().attackPower));
            EnemyDamage();
        }
    }

    new void Update()
    {
        base.Update();

        target = GameObject.FindGameObjectWithTag("Player");
        enemy = target.GetComponent<o_plcharacter>();
        
        
        checkWalkAnims();

        switch (enemyStatemachine)
        {

            case enemyState.stand:

                enemyStatemachine = enemyState.moving;
                break;

            case enemyState.moving:
                if (DistanceToAttack(300f, target))
                {
                    LookAtPlayer();

                    switch (frogStates)
                    {
                        case FROG_AI.Normal:
                            //placeholder
                            frogStates = FROG_AI.SeePlayer;
                            break;

                        case FROG_AI.SeePlayer:
                            switch_Dir = false;
                            Speed = initialSpeed;

                            MoveCharacter(true);
                            if (DistanceToAttack(200f, target)) {
                                attackdelay = 1.3f;
                                frogStates = FROG_AI.walk_back;
                            }

                            if (DistanceToAttack(260f, target)) {
                                if (attackdelay == 0) {
                                    enemyStatemachine = enemyState.attack_init;
                                    attackTimer = 0.4f;
                                }
                            }

                            break;

                        case FROG_AI.walk_back:

                            switch_Dir = true;
                            Speed = 140f;

                            MoveCharacter(true);
                            if (attackdelay == 0) {
                                switch_Dir = false;
                                frogStates = FROG_AI.SeePlayer;
                            }

                            break;
                    }
                }

                
                break;

            case enemyState.attack_init:
                rbody2d.velocity = Vector2.zero;
                ShootBullet("enemybullet", 0, 0.7f);
                enemyStatemachine = enemyState.attack;

                break;

            case enemyState.attack:

                if (ReadyToAttack(1f))
                {
                    thisrender.color = Color.white;
                    charAnimator.SetBool("IsAttacking", false);
                    enemyStatemachine = enemyState.stand;
                }

                break;

        }

    }

    void fireAtEnemy(float direction) {
        SoundManager.SFX.playSound(shoot);
        GameObject bullet = Instantiate((Resources.Load("Prefabs/Bullet prefabs/Knife", typeof(GameObject))), transform.position, Quaternion.identity) as GameObject;
        BulletClass bulletdata = bullet.GetComponent<BulletClass>();
        bulletdata.rotation = charDirection;
    }

    public override void afterDashAction()
    {
        
    }
     */
#endregion

#region PLAYERCODE
/* Put here on 18/03/2018
 * 
 * !!REMOVED PLAYER CODE!!
 * This is the old code for the player which was used for the update function
 * I relaized this was very inefficent as well as the fact that I had the characterstates
 * Thus rendering the playerstates moot.
 * 
 * I also think the whole Update states function looked kind of messy.
 * /

/* 
    public new void Update() {
        base.Update();
        bulletSwitch();

        if (Health == 0) { states = Playerstates.DEAD; }
        if ((firedelay > 0.01f) && (states != Playerstates.ATTACK_ANIMATION)) { firedelay = firedelay - Time.deltaTime; }
        if (attkdelay >= 0.1) { attkdelay = attkdelay - Time.deltaTime; }

}

        horizontalDir = Input.GetAxisRaw("Horizontal");
        verticalDir = Input.GetAxisRaw("Vertical");

        bullets = GameObject.FindGameObjectsWithTag("Bullet");
        checkStates();
        UpdateStates(horizontalDir,verticalDir);
    }

    public void UpdateStates(float h, float v)
    {
        Vector3 dir;
        if (h != 0 || v != 0) {
            last_h = h;
            last_v = v;
            dir = new Vector3(h, v, 0).normalized;
            charDirection = 360 - Mathf.Atan2(dir.x, dir.y) * Mathf.Rad2Deg;
        }
        switch (states)
        {
            case Playerstates.STAND:

                rbody2d.velocity = Vector2.zero;
                if ((h != 0 || v != 0))
                {
                    states = Playerstates.MOVING;
                }

                //go into attack state
                if (Input.GetKeyDown(KeyCode.Z) && (attkdelay < 0.1) && !temp_invinciblity) {
                    states = Playerstates.ATTACKING;
                }

                if (selectedWeapon.auto && Input.GetKey(KeyCode.Z) && (attkdelay < 0.1)) {
                    Speed = 20f;
                    states = Playerstates.ATTACKING;
                }

                break;

            case Playerstates.MOVING:

                if (dashDelay >= 0)
                {
                    dashDelay = dashDelay - Time.deltaTime;
                }

                if (h == 0 && v == 0) {
                    Speed = initialSpeed;
                    states = Playerstates.STAND;
                } else if (h != 0 || v != 0) {
                    charAnimator.SetFloat("X", h);
                    charAnimator.SetFloat("Y", v);
                }

                if (selectedWeapon.auto && Input.GetKey(KeyCode.Z) && (attkdelay < 0.1)) {
                    Speed = 20f;
                    states = Playerstates.ATTACKING;
                } else {
                    Speed = initialSpeed;
                }

                if (Input.GetKeyDown(KeyCode.Z) && (attkdelay < 0.1) && !temp_invinciblity) {
                    states = Playerstates.ATTACKING;
                }

                rbody2d.velocity = new Vector2(h, v) * Speed;

                if (Input.GetKeyDown(KeyCode.X) && dashDelay <= 0 && !temp_invinciblity)
                {
                    if (!Input.GetKeyDown(KeyCode.LeftArrow) && !Input.GetKeyDown(KeyCode.RightArrow))
                    {
                        if (SoundManager.SFX != null)
                        {
                            SoundManager.SFX.playSound(dash);
                        }
                        dashDelay = 0.13f;
                        states = Playerstates.DASHING;
                    }
                }

                break;

            case Playerstates.DASHING:

                Invinciblity = true;

                rbody2d.velocity = new Vector2(h, v).normalized * Speed * 5;
                dashTime = 0.26f;
                states = Playerstates.DASH_DELAY;

                break;

            case Playerstates.ATTACKING:
                charAnimator.SetBool("IsDashing", false);
                charAnimator.SetBool("IsMoving", false);
                if (selectedWeapon.ammoCap > 0 || !selectedWeapon.ammoLimits) {
                    FireGun();
                } else {
                    Speed = initialSpeed; states = Playerstates.MOVING; }
                states = Playerstates.ATTACK_ANIMATION;
                break;

            case Playerstates.ATTACK_ANIMATION:
                rbody2d.velocity = Vector2.zero; 
                break;

            case Playerstates.DASH_DELAY:

                if (dashTime >= 0)  {
                    if (dashTime <= 0.18) {
                        if (!Input.GetKey(KeyCode.X)) {
                            dashTime = 0;
                            Invinciblity = false;
                            states = Playerstates.MOVING;
                        }
                    }
                    dashTime = dashTime - Time.deltaTime;
                } else {
                    Invinciblity = false; Speed = initialSpeed;
                    states = Playerstates.MOVING;
                }
                break;

            case Playerstates.DEAD:
                Invinciblity = true;
                rbody2d.velocity = Vector2.zero;
                break;
        }
    }
    */
#endregion

#region CLONE SLIME SPAWN
/*
/* Put here on 02/11/2018
 * This was meant to spawn small slimes once the bigger slime was low on health
 * But I encountered a lot of bugs associated with spawning smaller slimes i.e. the slime doing nothing due to not having enough small slimes in the object pooler
 * The clone slimes were actually intended to damage the player but I came to realise that they can be used as a way of sheilding its parent from attacks
 * So I thought it would make the game too hard for them to attack the player.
 * The small slimes would also get despawned when their parent was defeated.
 * 
                    if (smol_slimes.Peek() != null)
                    {
                        GameObject slimetospawn = smol_slimes.Dequeue();
                        slimetospawn.transform.position = transform.position;
                        slimetospawn.GetComponent<npc_cloneslime>().parent_slime = this;
                        slimetospawn.GetComponent<IpoolObject>().SpawnStart();
                        slimetospawn.SetActive(true);
                        slimetospawn.transform.SetParent(GameObject.Find("Entities").transform);
                    }

                    

        if (smol_slimes.Count == 0)
        {
            for (int i = 0; i < 2; i++)
            {
                GameObject objtospawn = ObjectPooler.instance.SpawnObject("Small Slime", transform.position,Quaternion.identity,true);
                objtospawn.GetComponent<npc_cloneslime>().parent_slime = this;
                objtospawn.transform.SetParent(transform);
                objtospawn.SetActive(false);
                childeren_slimes.Add(objtospawn);
            }
        }
        
        for (int i = 0; i < 2; i++)
        {
            smol_slimes.Enqueue(childeren_slimes[i]);
        }
        
                //Recall all the little slimes
                for (int i = 0; i < 2; i++)
                {
                    if (!childeren_slimes[i].activeSelf)
                    {
                        continue;
                    }
                    else
                    {
                        childeren_slimes[i].SetActive(false);
                        smol_slimes.Enqueue(childeren_slimes[i]);
                    }
                }

 */
#endregion