using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

[System.Serializable]
public class Dialogue {
    public string text{ get; set; }
    public float time;
    public string new_text { get; set; }
}


public class CutsceneEditor : EditorWindow {

    #region INITIALIZIATION
    public enum cutscene_mode {
        handler_options,
        element_options
    }
    enum dialogue_mode {
        load,
        edit
    };

    enum animation_mode {
        custom_anim,
        attack_anim,
        moving_anim,
        dashing_anim
    }
    animation_mode anim_mode;
    // These are temporary variables
    // I know they look messy but this is my first time making a cutscene handler

    Color fadecol;
    bool focus_on_character, animbools_open;

    bool is_done = false, is_show, anim_bool;
    Vector2 scroll;
    string textThing = "";
    float time = 0, temp_float = 0f;

    dialogue_mode dialougeMod = dialogue_mode.load;
    public cutscene_mode pop;
    int cutscene_part_number = 0;
    string text = "";
    int x = 0, y = 0, z = 0;


    Vector3 pos = new Vector3(0,0,0);
    CutsceneHandler cutsceneHand;
    public Object source;
    Dialogue[] dia;
    Vector2 shake_screen_pos = new Vector2(0,0);
    public List<Dialogue> dialist = new List<Dialogue>();
    SerializedProperty src;
    CutsceneElement.cutsceneType cutenum;
    GameObject marker;

    int x_pos, y_pos, animation__number;
    Vector2 animation_endpos;

    bool anim_end = false;
    bool fadein = false;
    bool swap_activated = false;
    bool trigger_float, trigger_bool, trigger_integer;
    bool animationpos = false;

    bool is_screenshake, is_screensake_const;

    bool trigger_start = false, is_skippable = false, is_automatic = false;

    Vector2 scrollpos;

    [MenuItem("Brownie/POUCH")]
    static void init()
    {
        GetWindow<CutsceneEditor>("POUCH");
    }
#endregion

    #region MAIN
    private void OnGUI() {

        pop = (cutscene_mode)EditorGUILayout.EnumPopup(pop);

        if (Selection.activeGameObject != null)
        {
            if (Selection.activeGameObject.GetComponent<CutsceneHandler>())
            {
                switch (pop) {

                    case cutscene_mode.element_options:

                                cutsceneHand = Selection.activeGameObject.GetComponent<CutsceneHandler>();
                                cutscene_part_number = EditorGUILayout.IntSlider(cutscene_part_number, 0, cutsceneHand.cutsceneElements.Count - 1);

                                for (int i = 0; i < cutsceneHand.cutsceneElements.Count; i++) {
                                    if (cutscene_part_number == i) {
                                        //it's just better to get this into one function rather than enghty lines of code
                                        DrawCutsceneGUI(cutsceneHand.cutsceneElements[i]);
                                    }
                                }
                        break;

                    case cutscene_mode.handler_options:

                        //Buttons in the main handler menu, this is where things like arranging/adding elemnts occur, it also has booleans which trigger how a cutscene is run i.e. it being skippable.

                        cutsceneHand = Selection.activeGameObject.GetComponent<CutsceneHandler>();
                        if (GUILayout.Button("Add Cutscene Element")) {
                            CutsceneElement thing = new CutsceneElement();
                            thing.typeOfCutscene = CutsceneElement.cutsceneType.wait;

                            cutsceneHand = Selection.activeGameObject.GetComponent<CutsceneHandler>();
                            //So I don't have to go to the unity editor if I want to initialize the length
                            thing.dialouge = new string[1];
                            thing.dialogueSpd = new float[1];
                            cutsceneHand.cutsceneElements.Add(thing);
                        }
                        if (GUILayout.Button("Remove Cutscene Element"))
                        {
                            cutsceneHand = Selection.activeGameObject.GetComponent<CutsceneHandler>();
                            cutsceneHand.cutsceneElements.Remove(cutsceneHand.cutsceneElements[cutsceneHand.cutsceneElements.Count-1]);
                        }
                        if (GUILayout.Button("Push latest to back"))
                        {
                            cutsceneHand = Selection.activeGameObject.GetComponent<CutsceneHandler>();
                            CutsceneElement push = cutsceneHand.cutsceneElements[cutsceneHand.cutsceneElements.Count - 1];
                            cutsceneHand.cutsceneElements.Remove(push);
                            cutsceneHand.cutsceneElements.Insert(0,push);
                        }

                        //These are all the buttons that allow you to set a cutscene's type
                        EditorGUILayout.LabelField("Automatic");
                        is_automatic = EditorGUILayout.Toggle(is_automatic);
                        EditorGUILayout.LabelField("Skippable");
                        is_skippable = EditorGUILayout.Toggle(is_skippable);
                        EditorGUILayout.LabelField("Trigger on initialize");
                        trigger_start = EditorGUILayout.Toggle(trigger_start);
                        
                        //SET EM ALL!
                        if (GUILayout.Button("Set booleans"))
                        {
                            cutsceneHand = Selection.activeGameObject.GetComponent<CutsceneHandler>();
                            cutsceneHand.is_automatic = is_automatic;
                            cutsceneHand.is_skippable = is_skippable;
                            cutsceneHand.triggerOnStartUp = trigger_start;
                        }
                        
                        //If you want to swap the elements, you can do so here!
                        swap_activated = EditorGUILayout.Toggle(swap_activated);

                        if (swap_activated) {

                            x = EditorGUILayout.IntSlider(x, 0, cutsceneHand.cutsceneElements.Count - 1);
                            y = EditorGUILayout.IntSlider(y, 0, cutsceneHand.cutsceneElements.Count - 1);

                            for (int i = 0; i < cutsceneHand.cutsceneElements.Count; i++) {
                                CutsceneElement push = cutsceneHand.cutsceneElements[i];
                                if (y == i || x == i) {
                                    EditorGUILayout.LabelField(push.typeOfCutscene.ToString(), EditorStyles.boldLabel);
                                }
                            }
                            if (GUILayout.Button("Push Elements")) {
                                cutsceneHand = Selection.activeGameObject.GetComponent<CutsceneHandler>();
                                CutsceneElement push1 = CopyCutsceneData(cutsceneHand.cutsceneElements[x]);
                                CutsceneElement push2 = CopyCutsceneData(cutsceneHand.cutsceneElements[y]);

                                cutsceneHand.cutsceneElements.Remove(push1);
                                cutsceneHand.cutsceneElements.Insert(y,push1);
                            }
                            if (GUILayout.Button("Swap Elements")) {
                                cutsceneHand = Selection.activeGameObject.GetComponent<CutsceneHandler>();
                                CutsceneElement push1 = CopyCutsceneData(cutsceneHand.cutsceneElements[x]);
                                CutsceneElement push2 = CopyCutsceneData(cutsceneHand.cutsceneElements[y]);

                                cutsceneHand.cutsceneElements[y] = push1;
                                cutsceneHand.cutsceneElements[x] = push2;
                            }
                        }
                        
                        
                        cutenum = (CutsceneElement.cutsceneType)EditorGUILayout.EnumPopup(cutenum);
                        scroll = EditorGUILayout.BeginScrollView(scroll, GUILayout.Width(650), GUILayout.Height(300));
                        
                        for (int i = 0; i < cutsceneHand.cutsceneElements.Count; i++) {
                            CutsceneElement push = cutsceneHand.cutsceneElements[i];
                            EditorGUILayout.LabelField(push.typeOfCutscene.ToString());
                            if (GUILayout.Button("Set Type")) {
                                CutsceneElement replacement = new CutsceneElement();
                                replacement = push;
                                replacement.typeOfCutscene = cutenum;
                                cutsceneHand.cutsceneElements[i] = replacement;
                            }
                        }
                        EditorGUILayout.EndScrollView();

                        break;

                }
            }
        }
    }
    #endregion

    #region DRAWING
    void DrawCutsceneGUI (CutsceneElement cuthand)
    {
        // If i put this on the update function, it would have looked pretty dang messy.
        // Well, it looks messy already, what I was meant to say is that it would look even MORE dang messy!

        CutsceneElement.cutsceneType cutscene_Type = cuthand.typeOfCutscene;
        cuthand.typeOfCutscene = (CutsceneElement.cutsceneType)EditorGUILayout.EnumPopup(cuthand.typeOfCutscene);
        switch (cutscene_Type)
        {
            
            #region MOVEMENT      
            case CutsceneElement.cutsceneType.movmenet:

                //EditorGUILayout.LabelField("Marker");
                /*
                pos = EditorGUI.Vector3Field(new Rect(0, 200, 200, 100), "Character Postion:", pos);*/
                x = EditorGUI.IntField(new Rect(23, 100, 32, 32), x);
                text = EditorGUI.TextField(new Rect(200, 200, 400, 50), "Character to move:" , text);

                EditorGUILayout.LabelField("Character to move: " + cuthand.characterToMove); 
                EditorGUILayout.LabelField("Position: " + cuthand.movePosition);

                animbools_open = EditorGUILayout.Toggle(animbools_open);

                if (GUI.Button(new Rect (330,63,130,23), "Set Postion")) {
                    CutsceneElement new_move_pos = new CutsceneElement();
                    if (marker == null)
                        marker = GameObject.Find("Marker");

                    new_move_pos.movePosition = new Vector3(marker.transform.position.x, marker.transform.position.y);
                    Debug.Log(cuthand.movePosition);
                    new_move_pos.characterToMove = text;
                    new_move_pos.is_teleport = animbools_open;
                    new_move_pos.charMoveSpd = (float)x;
                    if(trigger_integer)
                        new_move_pos.animation_integer = y;
                    else
                        new_move_pos.animation_integer = 1;
                    new_move_pos.typeOfCutscene = CutsceneElement.cutsceneType.movmenet;

                    cutsceneHand.cutsceneElements[cutscene_part_number] = new_move_pos;
                }
                /*TODO

                GET RID OF ALL THAT CRAP BELOW ONCE I'VE REDONE THE CURRENT ANIMS.
                HAVE A TELEPORT BOOLEAN
                */


                if (animbools_open) {
                    trigger_integer = EditorGUILayout.Toggle(trigger_integer, "Int"); textThing = EditorGUILayout.TextField(textThing, "Set animation to");
                    if (trigger_integer) { y = EditorGUILayout.IntField(y, "Set animation to"); }

                    if (GUILayout.Button("Set Anims")) { }
                }
                break;
#endregion

            #region DIALOGUE
            case CutsceneElement.cutsceneType.dialogue:
                
                if (GUILayout.Button("Load this cutscene"))
                {
                    dialougeMod = dialougeMod == dialogue_mode.edit ? dialogue_mode.load : dialogue_mode.edit;
                }

                switch (dialougeMod) {
                    case dialogue_mode.load:
                        
                        x = cuthand.dialouge.Length;

                        dialist.Clear();
                        for (int i = 0; i < x; i++)
                        {
                            //Copying this one by one so the text can present what kind of dialouge i'm editing.
                            Dialogue dialect = ShowDialogueText(cuthand, i);
                            dialect.text = EditorGUILayout.TextField(ShowDialogueText(cuthand, i).text);
                            dialect.time = ShowDialogueText(cuthand, i).time;
                            dialist.Add(dialect);
                            Debug.Log(dialect.new_text);
                        }
                        dialougeMod = dialogue_mode.edit;
                        break;

                    case dialogue_mode.edit:
                        y = EditorGUILayout.IntSlider(y, 0, x - 1);
                        //TODO

                        //HAVE TEXT FILES LOADED WITH A BUTTON

                        EditorGUILayout.LabelField("Enter the text you want to add in here:");
                        source = EditorGUILayout.ObjectField(source, typeof(TextAsset), false);
                        if (source.GetType() == typeof(TextAsset))
                        {
                            string[] dialog_bits = source.ToString().Split(new string[] { "***" }, System.StringSplitOptions.None);
                            EditorGUILayout.LabelField("Which part of the text");
                            z = EditorGUILayout.IntSlider(z, 0, dialog_bits.Length - 1);


                            if (GUILayout.Button("Load text file"))
                            {
                                //Call some function that loads the text incl setting the length of the dialogue
                                //This does not overwrite any new delay data as it will copy them
                                string[] textlist = GetStringArray(dialog_bits[z]);

                                List<Dialogue> newdialist = new List<Dialogue>();
                                for (int i = 0; i < textlist.Length; i++)
                                {

                                    Dialogue dialug = new Dialogue();   //Jesus, I'm running out of names...
                                    dialug.text = textlist[i];


                                    newdialist.Add(dialug);
                                }
                                dialist = newdialist;
                                x = dialist.Count;
                            }

                            //text = EditorGUILayout.TextField(text);
                            EditorGUILayout.LabelField("Set the time you want the thing to last:");
                            time = EditorGUILayout.Slider(time, 0, 0.1f);

                            EditorGUILayout.LabelField("");
                            EditorGUILayout.LabelField("");

                            EditorGUILayout.LabelField("Text to replace");
                            textThing = EditorGUILayout.TextField(dialist[y].text);
                            EditorGUILayout.LabelField("Delay Between letters");
                            temp_float = EditorGUILayout.FloatField(dialist[y].time);

                            if (GUILayout.Button("SetTime"))
                            {
                                SetDialogueText(y, time);
                                y++;
                            }
                            if (GUILayout.Button("Burn dialogue to Level File"))
                            {
                                SetEntireDialogue(cuthand);
                            }
                            EditorGUILayout.LabelField("Replacement Dialouge");

                            scrollpos = EditorGUILayout.BeginScrollView(scrollpos, GUILayout.Width(650), GUILayout.Height(300));

                            if (dialist != null)
                            {
                                for (int i = 0; i < dialist.Count; i++)
                                {
                                    Dialogue dialect = dialist[i];
                                    dialect.text = EditorGUILayout.TextField(dialist[i].text);
                                    dialect.time = EditorGUILayout.FloatField(dialist[i].time);
                                }
                            }

                            EditorGUILayout.EndScrollView();

                        }

                        break;
                }
                break;
            #endregion

            #region SHOWCHARACTER
            case CutsceneElement.cutsceneType.showCharacter:
                text = EditorGUILayout.TextField(text);
                if (GUILayout.Button("Show Character"))
                {
                    cuthand.characterToMove = text;
                }
                break;
#endregion 

            #region WAIT
            case CutsceneElement.cutsceneType.wait:
                x = EditorGUILayout.IntField(x);
                EditorGUILayout.LabelField("Current wait time: " + cuthand.waitSeconds);
                if (GUILayout.Button("Set wait time"))
                {
                    CutsceneElement new_move_pos = new CutsceneElement();
                    new_move_pos.typeOfCutscene = CutsceneElement.cutsceneType.wait;
                    new_move_pos.waitSeconds = x;
                    cutsceneHand.cutsceneElements[cutscene_part_number] = new_move_pos;

                }
                break;
#endregion

            #region HIDECHARACTER
            case CutsceneElement.cutsceneType.hideCharacter:
                text = EditorGUILayout.TextField(text);
                if (GUILayout.Button("Hide Character"))
                {
                    CutsceneElement new_move_pos = new CutsceneElement();
                    new_move_pos.typeOfCutscene = CutsceneElement.cutsceneType.hideCharacter;
                    new_move_pos.characterToMove = text;
                    cutsceneHand.cutsceneElements[cutscene_part_number] = new_move_pos;
                }
                break;
#endregion

            #region PLAYSOUND
            case CutsceneElement.cutsceneType.play_sound:

                source = EditorGUILayout.ObjectField(source, typeof(AudioClip), false);
                if (GUILayout.Button("Set to Sound"))
                {
                    CutsceneElement new_move_pos = new CutsceneElement();
                    new_move_pos.typeOfCutscene = CutsceneElement.cutsceneType.play_sound;
                    new_move_pos.music = (AudioClip)source;
                    cutsceneHand.cutsceneElements[cutscene_part_number] = new_move_pos;
                }
                break;
#endregion

            #region FADE
            case CutsceneElement.cutsceneType.fade:

                fadecol = EditorGUILayout.ColorField("Fade Colour", fadecol);
                fadein = EditorGUILayout.Toggle(fadein);
                EditorGUILayout.LabelField("Colour");
                EditorGUILayout.ColorField(cutsceneHand.cutsceneElements[cutscene_part_number].fade_colour);
                if (GUILayout.Button("Set Fade"))
                {
                    CutsceneElement new_move_pos = new CutsceneElement();
                    if (fadein) { new_move_pos.is_fade_in = true;  new_move_pos.fade_timer = 0; } else
                    { new_move_pos.is_fade_in = false; new_move_pos.fade_timer = 1; }
                    new_move_pos.typeOfCutscene = CutsceneElement.cutsceneType.fade;
                    new_move_pos.fade_colour = fadecol;
                    cutsceneHand.cutsceneElements[cutscene_part_number] = new_move_pos;
                }
                break;
#endregion

            #region MUSIC
            case CutsceneElement.cutsceneType.music:

                source = EditorGUILayout.ObjectField(source, typeof(AudioClip), false);
                if (GUILayout.Button("Set to Music"))
                {
                    CutsceneElement new_move_pos = new CutsceneElement();
                    new_move_pos.typeOfCutscene = CutsceneElement.cutsceneType.music;
                    new_move_pos.music = (AudioClip)source;
                    cutsceneHand.cutsceneElements[cutscene_part_number] = new_move_pos;

                }

                if (cuthand.music != null) {
                    EditorGUILayout.LabelField("Music to play: "+ cuthand.music.name);
                } else {
                    EditorGUILayout.LabelField("No music");
                }
                break;
#endregion

            #region MOVECAMERA
            case CutsceneElement.cutsceneType.moveCamera:
                x = EditorGUI.IntField(new Rect(23, 150, 32, 32), x);

                if (focus_on_character)
                    text = EditorGUI.TextField(new Rect(200, 200, 400, 50), "Character to focus on:", text);
                else
                    pos = EditorGUILayout.Vector3Field( "Camera Postion:", pos);

                focus_on_character = EditorGUILayout.Toggle("Focus on character?", focus_on_character);
                if (marker == null)
                    marker = GameObject.Find("Marker");

                if (is_screenshake) {
                    shake_screen_pos = EditorGUILayout.Vector3Field( "shake:", shake_screen_pos);
                    is_screensake_const= EditorGUILayout.Toggle("Constantly screenshake", is_screensake_const);
                    time = EditorGUILayout.FloatField("Float", time);
                }
                is_screenshake = EditorGUILayout.Toggle("Screen shake?", is_screenshake);

                EditorGUILayout.LabelField("Character to move: " + cuthand.characterToMove);
                EditorGUILayout.LabelField("Position: " + cuthand.movePosition);

                //if (GUI.Button(new Rect(330, 63, 130, 23), "Set Camera Postion"))
                if (GUILayout.Button ("Set Camera Postion"))
                {

                    CutsceneElement new_move_pos = new CutsceneElement();
                    new_move_pos.typeOfCutscene = CutsceneElement.cutsceneType.moveCamera;

                    if (is_screenshake)
                    {
                        new_move_pos.screenshake_x = shake_screen_pos.x;
                        new_move_pos.screenshake_y = shake_screen_pos.y;
                        new_move_pos.is_scrnshake = true;
                        new_move_pos.fade_timer = time;
                        new_move_pos.is_shk_until_end = is_screensake_const;
                    }
                    else {
                        new_move_pos.movePosition = new Vector2(marker.transform.position.x, marker.transform.position.y);
                        Debug.Log(cuthand.movePosition);
                        new_move_pos.characterToMove = text;
                        new_move_pos.x = x;
                      }

                    cutsceneHand.cutsceneElements[cutscene_part_number] = new_move_pos;
                }
                break;
#endregion

            #region ANIMATION
            case CutsceneElement.cutsceneType.play_animation:
                
                EditorGUILayout.LabelField("Duration");
                z = EditorGUILayout.IntField(z);

                EditorGUILayout.LabelField("");
                EditorGUILayout.LabelField("Animation enum: 1 - MOVING, 2 - ATTACKING, 3 - DASH_DELAY, 4 - DASHING");
                EditorGUILayout.LabelField("5 - ATTACK_ANIMATION, 7 - NOTHING");

                x = EditorGUILayout.IntField(x);
                EditorGUILayout.LabelField("");
                EditorGUILayout.LabelField("Direction?");
                animationpos = EditorGUILayout.Toggle(animationpos);
                if (animationpos) {
                    pos = EditorGUILayout.Vector2Field("Position",pos);
                }

                EditorGUILayout.LabelField("");
                EditorGUILayout.LabelField("Character to play animation on: " + cuthand.characterToMove);
                text = EditorGUILayout.TextArea(text);
                

                EditorGUILayout.LabelField("");
               // focus_on_character = EditorGUILayout.Toggle("", focus_on_character);

                if (GUILayout.Button("Set Anims")) {
                    CutsceneElement movement_cutscene = new CutsceneElement();
                    movement_cutscene.typeOfCutscene = CutsceneElement.cutsceneType.play_animation;
                    movement_cutscene.characterToMove = text;
                    movement_cutscene.animation_state_int = x;
                    movement_cutscene.animation_integer = y;
                    movement_cutscene.animation_vector = pos;
                    movement_cutscene.end_animation_state = anim_end;

                    movement_cutscene.waitSeconds = z;
                    cutsceneHand.cutsceneElements[cutscene_part_number] = movement_cutscene;
                }
                break;
#endregion

            #region CREATEOBJ
            case CutsceneElement.cutsceneType.createObject:

                text = EditorGUI.TextField(new Rect(200, 200, 400, 50), "Object name:", text);
                pos = EditorGUI.Vector3Field(new Rect(0, 200, 200, 100), "Object Postion:", pos);
                if (GUI.Button(new Rect(330, 63, 130, 23), "Set Object + Position"))
                {
                    CutsceneElement new_cut = new CutsceneElement();
                    new_cut.movePosition = pos;
                    new_cut.characterToMove = text;
                    new_cut.typeOfCutscene = CutsceneElement.cutsceneType.createObject;
                    cutsceneHand.cutsceneElements[cutscene_part_number] = new_cut;
                }
                break;
            #endregion

            #region DRAWTXT

            case CutsceneElement.cutsceneType.instruction:

                text = EditorGUILayout.TextArea(text);
                time = EditorGUILayout.FloatField(time);

                if (GUILayout.Button("Make Instruction"))
                {
                    CutsceneElement movement_cutscene = new CutsceneElement();
                    movement_cutscene.typeOfCutscene = CutsceneElement.cutsceneType.instruction;
                    movement_cutscene.characterToMove = text;
                    movement_cutscene.rectan = new Rect(new Vector2(330,200), new Vector2(250,320));

                    cutsceneHand.cutsceneElements[cutscene_part_number] = movement_cutscene;
                }
                break;

            #endregion

            #region BOSSGUI
            case CutsceneElement.cutsceneType.boss_hp:
                anim_bool = EditorGUILayout.Toggle(anim_bool);

                if (GUI.Button(new Rect(330, 63, 130, 23), "BossGUI"))
                {
                    CutsceneElement new_cut = new CutsceneElement();
                    new_cut.aniamtion_bool = anim_bool;
                    new_cut.typeOfCutscene = CutsceneElement.cutsceneType.boss_hp;
                    cutsceneHand.cutsceneElements[cutscene_part_number] = new_cut;
                }
                break;
                #endregion
        }
    }
#endregion

    #region FUNCTIONS
    CutsceneElement CopyCutsceneData(CutsceneElement cuthand) {
        return cuthand;
    }

    public Dialogue AddDialogue() {
        Dialogue dia = new Dialogue();
        dia.time = 0f;
        dia.text = "";
        return dia;

    }

    public string[] GetStringArray(string txt) {
        //Debug.Log(txt.text.Split('\n').Length);
        string[] dial = txt.Split('\n');

        string[] dials = new string[dial.Length-1];
        
        for(int i = 0; i < dials.Length; i++)
        {
            dials[i] = dial[i];
        }

        return dials;
    }

    public void SetDialogueText(int i, float time) {
        Dialogue dia = new Dialogue();
        dia.text = dialist[i].text;
        dia.time = time;
        dialist[i] = dia;
    }

    public void SetEntireDialogue(CutsceneElement hand)
    {
        CutsceneElement dia = new CutsceneElement();
        dia.dialouge = new string[dialist.Count];
        dia.dialogueSpd = new float[dialist.Count];
        dia.typeOfCutscene = CutsceneElement.cutsceneType.dialogue;

        for (int i = 0; i < x; i++) {
            dia.dialogueSpd[i] = dialist[i].time;
            dia.dialouge[i] = dialist[i].text;
        }
        cutsceneHand.cutsceneElements[cutscene_part_number] = dia;
    }

    public Dialogue ShowDialogueText(CutsceneElement cuthand, int i) {
        Dialogue dialogueComp = new Dialogue();
        // dialogueComp.text = cuthand.dialouge[i];
        dialogueComp.text = cuthand.dialouge[i];

        dialogueComp.time = cuthand.dialogueSpd[i];
        //cuthand.dialogueSpd[i] = time;
        return dialogueComp;
    }
#endregion

    void OnInspectorUpdate()
    {
        this.Repaint();
    }

}

