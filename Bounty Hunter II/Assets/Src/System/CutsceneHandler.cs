using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

#region cutscene element
[System.Serializable]
public class CutsceneElement {
    //base
    public enum cutsceneType{ dialogue, movmenet, moveCamera, none, hideCharacter, showCharacter ,createObject, fade, wait, changeScene, music, loadLevel,destroy, recoverHP, play_sound, play_animation, instruction, boss_hp };
    public cutsceneType typeOfCutscene;

    //speaking
    public string[] dialouge;
    public float[] dialogueSpd;

    //moving
    public Animator anim;
    public Vector3 movePosition;
    public string characterToMove;
    public float charMoveSpd;
    public bool doneMoving;
    public float directionObj;

    //music
    public AudioClip music;
    public bool isLoop;

    public Rect rectan;

    public void SetPos(Vector3 pos) {
        movePosition = pos;
    }

    public void PlaySound(AudioClip sound) {
        SoundManager.SFX.playSound(sound);
    }

    public void musicChange(AudioClip audio) {
        SoundManager.SFX.playMusic(audio,isLoop);
    }

    public float x;
    public float y;
    public bool is_teleport;
    public void moveChar(Vector2 initialcharpos,Vector3 position)
    {
        doneMoving = false;
        GameObject chartoMov = GameObject.Find(characterToMove);

        if (!is_teleport)
        {
            Rigidbody2D charRbody2d = chartoMov.GetComponent<Rigidbody2D>();
            o_character character = chartoMov.GetComponent<o_character>();
            Animator anim = chartoMov.GetComponent<Animator>();
            

            charRbody2d.velocity = initialcharpos * charMoveSpd * 60;
            if (charMoveSpd < Vector2.Distance(chartoMov.transform.position, movePosition) && !doneMoving)
            {
                if (character != null)
                {
                    if(character.charAnimator == null)
                        character.charAnimator = anim;
                    int x_po = Mathf.RoundToInt(charRbody2d.velocity.normalized.x);
                    int y_po = Mathf.RoundToInt(charRbody2d.velocity.normalized.y);

                    character.direction_vec.x = x_po;
                    character.direction_vec.y = y_po;
                    character.horizontalDir = x_po;
                    character.verticalDir = y_po;
                    character.SetAnimation(1);
                }
                doneMoving = false;
            }
            else
            {
                if (!doneMoving)
                {
                    charRbody2d.velocity = Vector2.zero;
                    if (character != null && anim != null)
                    {
                        character.SetAnimation(0);
                    }
                }
                doneMoving = true;
            }
        }
        else
        {
            chartoMov.transform.position = position;

            doneMoving = true;
            Debug.Log("DONE");
        }
        
    }

    public void OnScreenText()
    {
        //gui_screentxt.TextDraw(characterToMove, fade_timer_start, rectan);
    }

    //disable character
    public void disableChar() {

        GameObject chartoMov = GameObject.Find(characterToMove);
        chartoMov.SetActive(false);
 
    }

    //change scene
    public void changeScene(string scene) {
        SceneManager.LoadScene(scene);
    }

    //enable character
    public void enableChar()
    {
        GameObject chartoMov = GameObject.Find(characterToMove);
        chartoMov.SetActive(true);
    }


    //moving camera
    public bool isChar;
    public void cameraMove() {

        if (isChar) {
            
            //GameObject character = GameObject.Find(characterToMove);
            //cameraScript.camOptions.player = character;
            //Debug.Log(character);
        }
        else {
            //cameraScript.camOptions.player = null;
            cameraScript.camOptions.destination = movePosition;
            Debug.Log(movePosition);
        }
    }

    //create object
    public GameObject createObject() {
        GameObject obj;
        if (ObjectPooler.instance.SpawnObject(characterToMove, true) != null) {
            obj = ObjectPooler.instance.SpawnObject(characterToMove, true);
        } else {
            obj = Resources.Load("" + characterToMove) as GameObject;
        }

        return obj;
    }

    //fadeOut
    public Image blackScrn;
    public float fade_timer;
    public bool is_fade_in;
    public Color fade_colour;
    public bool fading;

    /*
    public bool fading() {
        if (!is_fade_in )
        {
            if (fade_timer < 0.1f) {
                GameObject.Find("Fade").GetComponent<Image>().color = Color.clear;
                Debug.Log("isfading");
                return false;
            }
        } // fade_timer > 0.9f
        else if(is_fade_in)
        {
            if (fade_timer > 0.9f) {
                GameObject.Find("Fade").GetComponent<Image>().color = fade_colour;
                return false;
            }
        }
        return true;
    }

    public void fade()
    {
        GameObject.Find("Fade").GetComponent<Image>().color = Color.Lerp(Color.clear, fade_colour, fade_timer);
        fade_timer = is_fade_in ? fade_timer += Time.deltaTime : fade_timer -= Time.deltaTime;
        Debug.Log(fade_timer);
        fade_timer = Mathf.Clamp(fade_timer, 0, 1f);
    }

    */

    public float returnFadeTime()
    {
        return fade_timer;
    }

    public IEnumerator Fading(Image fade, Color former, Color after)
    {
        fading = false;
        fade_timer = 0f;
        while (fade_timer < 1.0f)
        {
            fade_timer += 0.1f;
            fade.color = Color.Lerp(former, after, fade_timer);
            yield return new WaitForSeconds(Time.deltaTime);
            //Debug.Log(fade_timer);
        }
        fading = true;
    }

    //camershake
    public float screenshake_x, screenshake_y;
    public bool is_shk_until_end, is_scrnshake;

    public void HideHealth() {
        gui_player gui = GameObject.Find("GUI").GetComponent<gui_player>();
        gui.guihide = true;
    }


    //delay
    public int animation_state_int;
    public bool aniamtion_bool;
    public Vector2 animation_vector;
    public int animation_integer;
    public bool end_animation_state;

    public enum animation_things {
        boolean,
        integer,
        float_value
    };
    public animation_things animation_type;
    
    
    //For the animations that don't have a character direction
    //Like facing up or down
    public void PlayAnimation(int custom_anim)
    {
        o_character character = GameObject.Find(characterToMove).GetComponent<o_character>();
        character.SetAnimation(custom_anim);
    }

    //For the animations that are custom and with direction
    public void PlayAnimation(int custom_anim, Vector2 pos)
    {
        o_character character = GameObject.Find(characterToMove).GetComponent<o_character>();
        character.SetAnimation(custom_anim);
        
        character.direction_vec.x = pos.x;
        character.direction_vec.y = pos.y;
        character.horizontalDir = pos.x;
        character.verticalDir = pos.y;
    }

    //Just reusing non-custom animations i.e. Running, dashing, attacking
    public void PlayNonCustomAnimation()
    {
    }





    public float waitSeconds;
}
#endregion

public class CutsceneHandler : MonoBehaviour {

    #region variables
    public GameObject save;
    public List<CutsceneElement> cutsceneElements;
    public static bool cutsceneplaying = false;
    bool skip_cutscene = false;

    Vector2 initialcharpos;
    public bool is_automatic = false;
    public bool is_skippable = true;

    private Text thistext;
    private Image dialogueBox;
    private Image fade;
    private bool istyping;
    private int currentLine;
	private int lineEnd;
    private bool buttonDown;
    public CutsceneElement.cutsceneType cutsceneKind;
    CutsceneElement currentPart;
    private int currentScene;
    bool shake_tillend;
    public bool done;
    public bool triggerOnStartUp;
    
    public List<string> characters = new List<string>();
    private bool enableCharacters;
    public List<string> boundaries = new List<string>();

	public bool cutsceneTriggered = false;
    #endregion

    #region INITIALIZATION
    void Start()
    {if (GameObject.Find("General Crap"))
        {
            GameObject fad = GameObject.Find("Fade");
            if (fad != null)
                fade = fad.GetComponent<Image>();
            if (GameObject.Find("General Crap").transform.Find("GUIs").transform.Find("Dialogue").GetComponent<Text>())
            { thistext = GameObject.Find("General Crap").transform.Find("GUIs").transform.Find("Dialogue").GetComponent<Text>(); }
            if (GameObject.Find("General Crap").transform.Find("GUIs").transform.Find("TextBox").GetComponent<Image>())
            { dialogueBox = GameObject.Find("General Crap").transform.Find("GUIs").transform.Find("TextBox").GetComponent<Image>(); }
        }

        if (triggerOnStartUp) {
            cutsceneStart();
        }
    }

    void cutsceneStart()
    {
        cutsceneTriggered = true;
        if (characters.Count > 0)
        {
            foreach (string chara in characters)
            {
                GameObject character = GameObject.Find(chara);
            }
        }
        enableCharacters = false;
        enableChars();
    }

    void OnTriggerEnter2D(Collider2D col){
        if (!cutsceneplaying)
        {
            if (col.GetComponent<o_plcharacter>() && !triggerOnStartUp)
            {
                cutsceneplaying = true;
                cutsceneStart();
            }
        }
    }

    private void OnDestroy()
    {
        cutsceneplaying = false;
    }

    public void ScreenShake(float x_pos,float y_pos,float timer)
    {
        cameraScript.camOptions.cameraShake(x_pos, y_pos, timer);
    }

    public IEnumerator waitSecs(float time) {
        done = false;
        yield return new WaitForSeconds(time);
        done = true;
    }

#endregion

    #region enabling and stuff
    public void enableChars(){

        if (cutsceneTriggered)
        {
            if(cameraScript.camOptions)
            cameraScript.camOptions.camState = cameraScript.cameraStates.cutscene;

            if (!enableCharacters)
            {
				for(int i = 0; i < characters.Count; i++)
                {
                    GameObject selectedChar = GameObject.Find(characters[i]);
                    if (selectedChar.GetComponent<o_character>() != null)
                    {
                        selectedChar.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                        o_plcharacter puppet_player = selectedChar.GetComponent<o_plcharacter>();
                        if (puppet_player != null) {
                            puppet_player.characterstates = o_character.CHARACTER_STATEMACHINE.STAND;
                            puppet_player.characterstates = o_character.CHARACTER_STATEMACHINE.NOTHING;
                            puppet_player.SetAnimation(0);
                            puppet_player.Invinciblity = true;
                        }

                        selectedChar.GetComponent<o_character>().Active = false;
                    } else {
                        o_npcharacter[] enemies = selectedChar.GetComponentsInChildren<o_npcharacter>();
                        foreach (o_npcharacter enemy in enemies)
                        {
                            enemy.gameObject.GetComponent<o_character>().enabled = true;
                            enemy.Active = false;
                        }
                    }
				}
                for (int i = 0; i < boundaries.Count; i++){

                    GameObject selectedBoundary = GameObject.Find(boundaries[i]);
                    selectedBoundary.transform.Find("Hitbox").GetComponent<BoxCollider2D>().enabled = false;
                    selectedBoundary.transform.Find("Hitbox").GetComponent<SpriteRenderer>().enabled = false;
                }

                nextCutscenePart();

            } else {
				for(int i = 0; i < characters.Count; i++) {

                    GameObject selectedChar = null;

                    if (GameObject.Find(characters[i]) != null)
                        selectedChar = GameObject.Find(characters[i]);
                    else
                        continue;

                    if (selectedChar.GetComponent<o_character>() != null)
                    {
                        if (selectedChar.GetComponent<o_plcharacter>() != null)
                        {
                            selectedChar.GetComponent<o_plcharacter>().characterstates = o_character.CHARACTER_STATEMACHINE.STAND;
                            selectedChar.GetComponent<o_plcharacter>().Invinciblity = false;
                        }
                        selectedChar.gameObject.GetComponent<o_character>().Active = true;
                        if (!selectedChar.GetComponent<o_character>().initialized)
                        {
                            selectedChar.gameObject.GetComponent<o_character>().GetComponent<IpoolObject>().SpawnStart();
                            selectedChar.gameObject.GetComponent<o_character>().enabled = true;
                        }
                    }
                    else
                    {
                        o_npcharacter[] enemies = selectedChar.GetComponentsInChildren<o_npcharacter>();
                        foreach (o_npcharacter enemy in enemies)
                        {
                            if (!enemy.Active)
                            {
                                enemy.GetComponent<o_character>().GetComponent<IpoolObject>().SpawnStart();
                                enemy.GetComponent<o_character>().enabled = true;
                                enemy.Active = true;
                            }
                        }
                    }

                }
                for (int i = 0; i < boundaries.Count; i++)
                {
                    GameObject selecteBoundary = GameObject.Find(boundaries[i]);
                    selecteBoundary.transform.Find("Hitbox").GetComponent<BoxCollider2D>().enabled = true;
                    selecteBoundary.transform.Find("Hitbox").GetComponent<SpriteRenderer>().enabled = true;
                }

                if (cameraScript.camOptions)
                    cameraScript.camOptions.camState = cameraScript.cameraStates.player;

                cutsceneTriggered = false;
            }
		}
	}

    public IEnumerator readText(string TextLines, CutsceneElement cutscene)
    {
        int letter = 0;
        thistext.text = "";
        int textLength = TextLines.Length;
        istyping = true;
        lineEnd = cutscene.dialouge.Length;


        while (istyping && (letter < textLength))
        {
            wrapText(currentPart.dialouge[currentLine].Substring(0, (int)letter));
            thistext.text += TextLines[letter];
            letter++;
            if (!skip_cutscene)
            {
                yield return new WaitForSeconds(cutscene.dialogueSpd[currentLine]);
            }
            else
            {
                currentLine++;
                istyping = !istyping;
            }
        }
        if (!is_automatic)
        {
            if (is_skippable)
                thistext.text += " (-Space: Contine- -X: Skip-)";
            else
                thistext.text += " (-Space: Contine-)";
        }
        if (is_automatic)
            yield return new WaitForSeconds(letter * 0.01f);

        if (!skip_cutscene)
        {
            currentLine++;
            istyping = !istyping;
        }
    }

    void wrapText(string Textlines)
    {

        if (Textlines.Length > 0)
        {
            string[] words = Textlines.Split(' ');
            string result = "";

            for (int i = 0; i < words.Length; i++)
            {

                thistext.text = (result + words[i] + " ");
                result = thistext.text;
            }

            thistext.text = result;
        }
        else
            thistext.text = "";

    }
    
    #endregion

    #region nextCutscenePart

    void nextCutscenePart() {

        if (currentScene < cutsceneElements.Count) {

            currentPart = cutsceneElements[currentScene];
            cutsceneKind = currentPart.typeOfCutscene;

            if (cutsceneKind != CutsceneElement.cutsceneType.dialogue) {
                currentLine = 0;
            }

            switch (cutsceneKind)
            {
                case CutsceneElement.cutsceneType.fade:
                    StartCoroutine(currentPart.Fading(fade, fade.color, currentPart.fade_colour));
                    break;

                //should the cutscene be the dialogue
                case CutsceneElement.cutsceneType.dialogue:
                    lineEnd = currentPart.dialouge.Length;

                    if (currentLine < lineEnd) {
                        dialogueBox.enabled = true;
                        StartCoroutine(readText(currentPart.dialouge[currentLine], currentPart));
                    } else if (currentLine == lineEnd) {
                        dialogueBox.enabled = false;
                        thistext.text = "";
                        currentScene++;
                        nextCutscenePart();
                    }

                    break;
                
                //cutscene being the movement
                case CutsceneElement.cutsceneType.movmenet:
                    /* TODO - enable animation */
                    GameObject cha = GameObject.Find(currentPart.characterToMove);
                    initialcharpos = new Vector3(currentPart.movePosition.x - cha.transform.position.x, currentPart.movePosition.y - cha.transform.position.y).normalized;
                    break;

                case CutsceneElement.cutsceneType.loadLevel:
                    s_levelmanager lvlManager;
                    lvlManager = GameObject.Find("LevelManager").GetComponent<s_levelmanager>();
                    lvlManager.current_level++;
                    Instantiate(save, transform.position, Quaternion.identity);
                    lvlManager.LoadLevel();
                    break;

                case CutsceneElement.cutsceneType.hideCharacter:
                    currentPart.disableChar();
                    currentScene++;
                    nextCutscenePart();
                    break;

                case CutsceneElement.cutsceneType.showCharacter:
                    currentPart.enableChar();
                    currentScene++;
                    nextCutscenePart();
                    break;

                case CutsceneElement.cutsceneType.createObject:
                    Instantiate(currentPart.createObject(), currentPart.movePosition, Quaternion.Euler(0,0,currentPart.directionObj));
                    currentScene++;
                    nextCutscenePart();
                    print(currentScene);
                    break;

                case CutsceneElement.cutsceneType.moveCamera:
                    if (currentPart.characterToMove != "")
                    {
                        cameraScript.camOptions.player = GameObject.Find(currentPart.characterToMove);
                    }

                    if (!currentPart.is_scrnshake) {
                        currentPart.cameraMove();

                    } else {
                        if (currentPart.is_shk_until_end) {
                            cameraScript.camerastaticshake = new Vector2(currentPart.screenshake_x, currentPart.screenshake_y);
                            cameraScript.camOptions.constant_shake = true;
                        } else {
                            cameraScript.camOptions.constant_shake = false;
                        }

                        ScreenShake(currentPart.screenshake_x, currentPart.screenshake_y, currentPart.fade_timer);
                    }
                    currentScene++;
                    nextCutscenePart();
                    print(currentScene);
                    break;
                case CutsceneElement.cutsceneType.wait:
                    StartCoroutine(waitSecs(currentPart.waitSeconds));

                    break;
                case CutsceneElement.cutsceneType.changeScene:
                    currentPart.changeScene(currentPart.characterToMove);
                    break;
                case CutsceneElement.cutsceneType.music:
                    currentPart.musicChange(currentPart.music); currentScene++;
                    nextCutscenePart();
                    break;
                case CutsceneElement.cutsceneType.recoverHP:
                    currentPart.HideHealth();
                    GameObject.Find("EndingBar").GetComponent<Image>().enabled = true;
                    GameObject.Find("EndingBar2").GetComponent<Image>().enabled = true;
                    currentScene++;
                    nextCutscenePart();
                    break;

                case CutsceneElement.cutsceneType.play_sound:
                    if (currentPart.music == null)
                        currentPart.musicChange(null);

                    currentPart.PlaySound(currentPart.music); currentScene++;
                    nextCutscenePart();
                    break;

                case CutsceneElement.cutsceneType.play_animation:

                    if (currentPart.animation_vector == null)
                        currentPart.PlayAnimation(currentPart.animation_state_int);
                    else
                        currentPart.PlayAnimation(currentPart.animation_state_int, currentPart.animation_vector);
                    currentScene++;
                    nextCutscenePart();
                    break;

                case CutsceneElement.cutsceneType.instruction:
                    currentPart.OnScreenText(); currentScene++;
                    nextCutscenePart();
                    break;

                case CutsceneElement.cutsceneType.boss_hp:

                    gui_player gui = GameObject.Find("GUI").GetComponent<gui_player>();
                    gui.ToggleBossGUI(currentPart.aniamtion_bool);
                    currentScene++;
                    nextCutscenePart();
                    break;
                    
            }
        } else {
            Time.timeScale = 1;
            cameraScript.camOptions.constant_shake = false;
            cameraScript.camOptions.player = GameObject.Find(s_levelmanager.Player.name);
            enableCharacters = true;
            enableChars();
            cutsceneplaying = false;
            cutsceneKind = CutsceneElement.cutsceneType.none;
            this.gameObject.SetActive(false);
        }
    }
    #endregion
    private void FixedUpdate()
    {

        if (Input.GetKey(KeyCode.Space))
        {
            buttonDown = true;
        }
        else
        {
            buttonDown = false;
        }
        if (cutsceneTriggered)
        {

            Time.timeScale = skip_cutscene ? 10 : 1;

            if (Input.GetKeyDown(KeyCode.X) && is_skippable)
            {
                skip_cutscene = true;
            }

            switch (cutsceneKind)
            {
                case CutsceneElement.cutsceneType.dialogue:

                    if (!istyping)
                    {
                        if (currentLine <= lineEnd)
                        {
                            if (!skip_cutscene)
                            {
                                if (is_automatic)
                                {
                                    nextCutscenePart();
                                }
                                else if (buttonDown)
                                {
                                    nextCutscenePart();
                                }
                            }
                            if (skip_cutscene)
                            {
                                nextCutscenePart();
                            }
                        }
                    }
                    break;

                case CutsceneElement.cutsceneType.movmenet:
                    if (currentPart.is_teleport)
                    {
                        GameObject.Find(currentPart.characterToMove).transform.position = currentPart.movePosition;

                        currentScene++;
                        currentPart.doneMoving = false;
                        nextCutscenePart();
                    }

                    if (!currentPart.doneMoving)
                    {
                        currentPart.moveChar(initialcharpos, currentPart.movePosition);
                    }
                    else
                    {
                        currentScene++;
                        currentPart.doneMoving = false;
                        nextCutscenePart();
                    }
                    break;

                case CutsceneElement.cutsceneType.fade:
                    if (currentPart.fading)
                    {
                        Debug.Log("Fade complete");
                        currentScene++;
                        nextCutscenePart();
                    }
                    break;

                case CutsceneElement.cutsceneType.wait:

                    if (done)
                    {
                        currentScene++;
                        nextCutscenePart();
                    }
                    break;

                case CutsceneElement.cutsceneType.play_animation:
                    if (done)
                    {
                        print("YES");
                        currentScene++;
                        nextCutscenePart();
                    }
                    break;
            }
        }
    }

}
