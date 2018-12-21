using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Tilemaps;

public static class s_events {
    
    public static KeyCode KeyPress() {
        if (Event.current.type == EventType.KeyDown){
            Debug.Log("key pressed");
            return Event.current.keyCode;
        }
        return KeyCode.None;
    }
}

[System.Serializable]
public struct Boundaries
{
    public bool is_visible;
    public float size_x, size_y;
    public Vector3 posiiton;
    public Batch batch;
    public string name;
    public Vector2 inner_size;
    public bool is_disabled;
}

[System.Serializable]
public class CutsceneObj {

    public bool is_disabled;
    public bool is_automatic;
    public List<CutsceneElement> elements = new List<CutsceneElement>();
    public string[] character_names;
    public string[] boundary_names;
    public bool is_skippable;
    public bool start_on_trigger;

    public float size_x, size_y;
    public Vector3 position;
    public string name;
}

[System.Serializable]
public class Batch
{
    public string nameofbatch;
    public s_characterdat[] enemies;
}

[System.Serializable]
public struct s_characterdat
{
    public string batch;
    public bool is_disabled;
    public string name;
    public string file_location;
    public Vector3 position;
}

[System.Serializable]
public struct data_trap
{
    public List<trap_data> traps;
    public string obj_name;
    public Vector3 teleport_position;
    public o_trap.TRAP_TYPE traptype;
    public string group_immunity;

    public data_trap(List<GameObject> traps , string t_name, Vector3 position, o_trap.TRAP_TYPE trap_type, string group_immunity)
    {
        this.group_immunity = group_immunity;
        traptype = trap_type;
        teleport_position = position;
        obj_name = t_name;
        List<trap_data> temp_trap = new List<trap_data>();
        for (int i = 0; i < traps.Count; i++)
        {
            temp_trap.Add(new trap_data(traps[i].transform.lossyScale, traps[i].transform.position));
            Debug.Log(traps[i].transform.lossyScale);
        }
        this.traps = temp_trap;
    }
}

[System.Serializable]
public struct trap_data
{
    public Vector2 size;
    public Vector2 position;
    public trap_data(Vector2 size, Vector2 position)
    {
        this.position = position;
        this.size = size;
    }
}
[System.Serializable]
public struct ItemSave
{
    public string name;
    public string file_location;
    public Vector3 position;
}

[System.Serializable]
public struct TileObject {
    public Vector3 position;
    public Vector2 size;
}

[System.Serializable]
public struct GenericObjects {
    public string filename;
    public string obj_name;
    public Vector2 position;
}

public class s_levelmanager : MonoBehaviour {

    public GameObject TrapObj;
    public GameObject TrapObjParent;
    public GameObject EnemyBound;
    public GameObject Goal;
    public GameObject collisionObject;

    public List<o_npcharacter> enemy_objects = new List<o_npcharacter>();

    public List<GameObject> item_objects = new List<GameObject>();

    public static List<o_character> enemies_in_level = new List<o_character>();
    public bool CollsiionVisible;

    public List<LevelData> levels = new List<LevelData>();
    public int current_level { get; set; }
    public LevelData Level { get; set; }
    GameObject lev;
    bool inEditor = true;

    [HideInInspector]
    public GameObject loadObj;
    public static GameObject Player;
    [HideInInspector]
    public bool is_numbered;
    
    public List<GameObject> bullet_pooler = new List<GameObject>();

    int char_number = 0;
    [HideInInspector]
    public GameObject levsav;

    public void ResetNumber()
    {
        char_number = 0;
    }

    private void Awake()
    {
        inEditor = false;
    }

    private void Start()
    {
        if (!gui_main_menu.isLoadgame)
        {
            StartCoroutine(StartGame());
        }
        else
        {
            Instantiate(loadObj, transform.position, Quaternion.identity);
        }
    }

    IEnumerator StartGame()
    {
        cameraScript cam = GameObject.Find("Main Camera").GetComponent<cameraScript>();
        current_level = 0;
        LoadLevel();
        yield return new WaitForSeconds(1.1f);
        cam.FadeAdd(Color.clear);
    }

    public void DestroyLevel()
    {
        if (levsav != null) { 
        }
        if (lev != null) {
            if (inEditor) {
                DestroyImmediate(lev, true);
            } else {

                BulletClass[] bullets = GameObject.Find("Items").GetComponentsInChildren<BulletClass>();
                for (int i = 0; i < bullets.Length; i++)
                {
                    if (bullets[i].gameObject.activeSelf)
                    {
                        bullets[i].gameObject.SetActive(false);
                    }
                    bullets[i].transform.SetParent(GameObject.Find("OBj pool").transform);
                }
                o_item[] it = GameObject.Find("Items").GetComponentsInChildren<o_item>();
                for (int i = 0; i < it.Length; i++)
                {
                    if (it[i].gameObject.activeSelf)
                    {
                        it[i].gameObject.SetActive(false);
                    }
                    it[i].transform.SetParent(GameObject.Find("OBj pool").transform);
                }

                o_npcharacter[] characters = GameObject.Find("Entities").GetComponentsInChildren<o_npcharacter>();
                for (int i = 0; i < enemies_in_level.Count; i++)
                {
                    if (enemies_in_level[i].GetComponent<o_plcharacter>()) {
                        enemies_in_level[i].enabled = false;
                        continue;
                    }
                    if (enemies_in_level[i].gameObject.activeSelf) {
                        enemies_in_level[i].gameObject.SetActive(false);
                    }
                    enemies_in_level[i].transform.SetParent(GameObject.Find("OBj pool").transform);
                }
                Destroy(lev);
            }
        }
    }

    public void LoadLevel()
    {
        cameraScript cam = GameObject.Find("Main Camera").GetComponent<cameraScript>();

        DestroyLevel();
        enemies_in_level.Clear();

        Level = levels[current_level];
        cam.GetCameraBounds(Level.min_boundaries, Level.max_boundaries, Level.player_position);

        if (SoundManager.SFX != null)
        {
            if (SoundManager.SFX.music.clip != Level.music)
                SoundManager.SFX.playMusic(Level.music, true);
            else if (Level.music == null)
                SoundManager.SFX.stopMusic();
        }
        if (inEditor)
        {
            //levsav = Instantiate(Level.gameObject);
            levsav.name = Level.name;
        }
        
        Player = GameObject.Find(Level.PLAYERCHARACTER);
        cam.player = Player;
        /*
        GameObject players = GameObject.Find("Players");

        
        for (int i = 0; i < players.transform.childCount; i++) {
            GameObject pl = players.transform.GetChild(i).gameObject;

            if (pl != Player)
                pl.GetComponent<o_plcharacter>().enabled = false;
            else
                pl.GetComponent<o_plcharacter>().enabled = true;
        }
        */

        Player.GetComponent<o_plcharacter>().enabled = true;
        Player.GetComponent<o_plcharacter>().Active = true;
        Player.GetComponent<IpoolObject>().SpawnStart(); 
        gui_player.character = Player.GetComponent<o_plcharacter>();

        if (Level.music != null && SoundManager.SFX != null) {
            SoundManager.SFX.playMusic(Level.music, true);
        }
        LoadResources();
        cam.FadeAdd(Color.clear);
    }

    void LoadResources()
    {
        GameObject area = new GameObject("Area");
        lev = area;
        GameObject enemies = new GameObject("Entities");
        GameObject boundary = new GameObject("Boundaries");
        GameObject scenery = new GameObject("Scenery");
        GameObject cutscenes = new GameObject("Cutscenes");
        GameObject itemsObj = new GameObject("Items");
        GameObject Objects = new GameObject("Objects");

        playerUpdatePosition(Level.player_position);

        //So the level can be deleted relatively easier
        Objects.transform.SetParent(lev.transform);
        itemsObj.transform.SetParent(lev.transform);
        cutscenes.transform.SetParent(lev.transform);
        scenery.transform.SetParent(lev.transform);
        boundary.transform.SetParent(lev.transform);
        enemies.transform.SetParent(lev.transform);

        //When it's in edit mode, I can easily access the save object and edit the level as I please
        if (inEditor) {
            levsav = levels[current_level].gameObject;
            print(levsav.name);
            levsav.name = levels[current_level].name;
        }

        LoadGoal().transform.SetParent(lev.transform);

        foreach (data_trap trap in Level.traps) {
            GameObject object_i = Instantiate(TrapObjParent, new Vector3(0,0), Quaternion.identity) as GameObject;
            object_i.name = trap.obj_name;

            o_trap tra = object_i.GetComponent<o_trap>();
            tra.group_immunity = trap.group_immunity;
            tra.teleport_position = trap.teleport_position;
            tra.TELEPORT_TRAP = trap.traptype;
            for (int i = 0; i < trap.traps.Count; i++)
            {
                GameObject trp = Instantiate(TrapObj, trap.traps[i].position, Quaternion.identity) as GameObject;
                trp.transform.localScale = trap.traps[i].size;
                trp.transform.SetParent(object_i.transform);
                if (inEditor)
                    trp.GetComponent<SpriteRenderer>().enabled = true;
                else
                    trp.GetComponent<SpriteRenderer>().enabled = false;
                }
            object_i.transform.SetParent(lev.transform.Find("Objects").transform);
        }

        foreach (s_characterdat enemy in Level.entities) {
            enemies_in_level.Add(AddEnemy(enemy));
        }
        enemies_in_level.Add(GameObject.Find(Level.PLAYERCHARACTER).GetComponent<o_character>());

        foreach (Boundaries bound in Level.boundaries) {
            AddBoundaries(bound);
        }

        foreach (TileObject tile in Level.tiles) {
            LoadTiles(tile);
        }

        foreach (GenericObjects tile in Level.objects) {
            LoadObjects(tile);
        }

        foreach (CutsceneObj tile in Level.cutscenes) {
            LoadCutscenes(tile);
        }

        foreach (ItemSave tile in Level.items) {
            LoadItems(tile);
        }
        

        for (int i = 0; i < enemies_in_level.Count; i++)
        {
            enemies_in_level[i].cur_faction = s_factionhandler.GetFaction(enemies_in_level[i].type);
            enemies_in_level[i].AddTargets(enemies_in_level);
        }
    }

    public void playerUpdatePosition(Vector3 vec) {

        Player.transform.position = vec;
    }

    GameObject LoadGoal() {
        GameObject goal = Instantiate(Goal, Level.goal_position, Quaternion.identity);
        goal.GetComponent<BoxCollider2D>().size = new Vector2(Level.goal_size.x, Level.goal_size.y);
        goal.name = "Goal";
        return goal;
    }

    GameObject LoadObjects(GenericObjects obj) {
        GameObject object_i = Instantiate(Resources.Load("Prefabs/Objects/"+ obj.obj_name), obj.position, Quaternion.identity) as GameObject;
        object_i.name = obj.obj_name;
        object_i.transform.SetParent(lev.transform.Find("Objects").transform);
        return object_i;
    }

    void LoadCharacters(GenericObjects obj) {
        GameObject object_i = Instantiate(Resources.Load("Prefabs/Characters/NPC/" + obj.obj_name), obj.position, Quaternion.identity) as GameObject;
        object_i.name = obj.obj_name;
        object_i.transform.SetParent(lev.transform.Find("Characters").transform);
    }

    void LoadItems(ItemSave obj) {
        //GameObject object_i = ObjectPooler.Instance.SpawnObject(obj.name, obj.position);
        GameObject object_i = Instantiate(item_objects.Find(x => x.name == obj.name), obj.position, Quaternion.identity) as GameObject;
        object_i.name = obj.name;
        object_i.GetComponent<o_item>().Initialize();
        object_i.transform.SetParent(lev.transform.Find("Items").transform);
    }

    void LoadCutscenes(CutsceneObj cut)
    {
        GameObject cutsc = Instantiate(Resources.Load("Prefabs/Objects/Generic/Cutscene_Base") as GameObject, cut.position, Quaternion.identity);

        cutsc.GetComponent<SpriteRenderer>().enabled = inEditor;
        
        cutsc.name = cut.name;
        foreach (CutsceneElement cutel in cut.elements) {
            cutsc.GetComponent<CutsceneHandler>().cutsceneElements.Add(cutel);
        }

        if (cut.character_names.Length > 0) {
            foreach (string chara in cut.character_names) {
                cutsc.GetComponent<CutsceneHandler>().characters.Add(chara);
            }
        }
        foreach (string bound in cut.boundary_names) {
            cutsc.GetComponent<CutsceneHandler>().boundaries.Add(bound);
        }
        //if (cut.boundary_names.Length > 0) { }
        cutsc.GetComponent<CutsceneHandler>().is_automatic = cut.is_automatic;
        cutsc.GetComponent<CutsceneHandler>().is_skippable = cut.is_skippable;
        cutsc.GetComponent<CutsceneHandler>().triggerOnStartUp = cut.start_on_trigger;
        cutsc.GetComponent<BoxCollider2D>().size = new Vector2(cut.size_x, cut.size_y);
        cutsc.GetComponent<CutsceneHandler>().enabled = (cut.is_disabled) ? false : true;
        cutsc.transform.SetParent(lev.transform.Find("Cutscenes").transform);
    }

    void AddBoundaries(Boundaries boundary)
    {
        GameObject bound = Instantiate(EnemyBound, boundary.posiiton, Quaternion.identity);
        bound.name = boundary.name;
       
        o_boundaries boundaryScript = bound.GetComponent<o_boundaries>();
        bound.transform.localScale = new Vector2(boundary.size_x, boundary.size_y);
        boundaryScript.collision.gameObject.transform.localScale = new Vector2(boundary.inner_size.x, boundary.inner_size.y);
        
        GameObject hitbx_sprite = boundaryScript.collision.gameObject;
        hitbx_sprite.GetComponent<SpriteRenderer>().color = new Color(1,1,1,0.2f);
        bound.transform.Find("Hitbox").GetComponent<Collider2D>().enabled = boundary.is_disabled ? false : true;
        bound.GetComponent<o_boundaries>().collision.GetComponent<SpriteRenderer>().enabled = boundary.is_visible ? true : false;

        if (boundary.batch.nameofbatch != "") {
            if (!GameObject.Find(boundary.batch.nameofbatch)) {
                boundaryScript.enemy = AddBatch(boundary.batch, bound);
            } else { boundaryScript.enemy = GameObject.Find(boundary.batch.nameofbatch); }

            boundaryScript.enemyB = boundary.batch.nameofbatch;
        }
        bound.transform.SetParent(lev.transform.Find("Boundaries").transform);
    }

    public GameObject AddBatch(Batch batch, GameObject origin) {
        GameObject bat = new GameObject(batch.nameofbatch);
        bat.name = batch.nameofbatch;
        //print(bat.name);

        foreach (s_characterdat enemy in batch.enemies) {
            GameObject curenemy = GameObject.Find(enemy.name);
            //print(enemy.name);
            curenemy.transform.SetParent(bat.transform);
        }
        bat.transform.SetParent(lev.transform.Find("Entities").transform);
        return bat;
    }

    public void LoadTiles(TileObject tile)
    {
        GameObject thing = Instantiate( collisionObject, new Vector3(tile.position.x, tile.position.y, tile.position.z), Quaternion.identity) as GameObject;
        thing.name = "tileObj";
        if (!CollsiionVisible) {
        thing.GetComponent<SpriteRenderer>().enabled = inEditor ? true : false;
        }
        thing.transform.localScale = tile.size;
        thing.transform.SetParent(lev.transform.Find("Scenery").transform);
    }

    public o_npcharacter AddEnemy(s_characterdat enemy) {
        GameObject thingObj;
        string objectthing = enemy.file_location;
        string[] str = new string[] { "Prefabs/Characters/Entities/" };
        string[] res;
        res = objectthing.Split(str, StringSplitOptions.RemoveEmptyEntries);
        if (inEditor) {
            thingObj = Instantiate(enemy_objects.Find(x => x.name == res[0]).gameObject, enemy.position, Quaternion.identity);
        } else {

            if(!enemy.is_disabled)
                thingObj = ObjectPooler.instance.SpawnObject(res[0], enemy.position, Quaternion.identity, true);
            else
                thingObj = ObjectPooler.instance.SpawnObject(res[0], enemy.position, Quaternion.identity, false);

        }

        o_npcharacter thing =  thingObj.GetComponent<o_npcharacter>();
        thing.Active = (enemy.is_disabled) ? false : true;
        thing.name = enemy.name;
        thingObj.transform.SetParent(lev.transform.Find("Entities").transform);
        
        return thing;
    }

    public void CreateObject(Vector2 spawnpos)
    {
        GameObject thing = Instantiate(collisionObject, new Vector3(spawnpos.x, spawnpos.y), Quaternion.identity) as GameObject;
        thing.name = "tileObj";
        if (!CollsiionVisible)
        {
            thing.GetComponent<SpriteRenderer>().enabled = inEditor ? true : false;
        }
        thing.transform.SetParent(GameObject.Find("Area").transform.Find("Scenery").transform);
    }

    public void CreateObject(string filepath, string name, string area_part, Vector2 spawnpos)
    {
        char_number++;
        GameObject go = null;

        if(area_part == "Entities")
            go = Instantiate(enemy_objects.Find(x => x.name == name).gameObject);
        else if(area_part == "Items")
            go = Instantiate(item_objects.Find(x => x.name == name).gameObject);
        else if (area_part == "Boundaries")
            go = Instantiate(EnemyBound);

        go.transform.position = spawnpos;
        go.transform.parent = GameObject.Find("Area").transform.Find(area_part);
        go.name = is_numbered ? name + "_" + char_number : name;
    }
    public void CreateObject(string filepath, string name, GameObject area_part, Vector2 spawnpos)
    {
        char_number++;
        GameObject go = Instantiate(Resources.Load(filepath + name) as GameObject);
        go.transform.position = spawnpos;
        go.transform.parent = area_part.transform;
        go.name = is_numbered ? name + "_" + char_number : name;
    }

}

