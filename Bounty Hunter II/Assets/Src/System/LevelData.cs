using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelData : MonoBehaviour {

    public List<Boundaries> boundaries = new List<Boundaries>();
    public List<s_characterdat> entities = new List<s_characterdat>();
    public List<TileObject> tiles = new List<TileObject>();
    public List<GenericObjects> objects = new List<GenericObjects>();
    public List<CutsceneObj> cutscenes = new List<CutsceneObj>();
    public List<ItemSave> items = new List<ItemSave>();
    public List<data_trap> traps = new List<data_trap>();

    public string PLAYERCHARACTER = "Peast";

    [HideInInspector]
    public Vector2 goal_size;
    public Vector3 goal_position;

    public Vector3 min_boundaries;
    public Vector3 max_boundaries;

    public AudioClip music;
    public Vector3 player_position;
    public string object_file_directory;

    public Texture2D mapBase;

    public void SaveThing()
    {
        object_file_directory = this.name;
        cutscenes.Clear();
        entities.Clear();
        boundaries.Clear();
        tiles.Clear();
        objects.Clear();
        items.Clear();
        traps.Clear();

        SaveGoal(GameObject.Find("Goal").gameObject.GetComponent<GoalCass>());
        GameObject area = GameObject.Find("Area");

        GameObject enemiesObj = area.transform.Find("Entities").gameObject;
        o_npcharacter[] enemieslist = enemiesObj.GetComponentsInChildren<o_npcharacter>();

        foreach (o_npcharacter enemy in enemieslist)
        {
            s_characterdat enemydat = FindEnemyData(enemy);
            entities.Add(enemydat);
        }
        GameObject boundariesObj = area.transform.Find("Boundaries").gameObject;
        o_boundaries[] boundarys = boundariesObj.GetComponentsInChildren<o_boundaries>();

        foreach (o_boundaries boundary in boundarys) {
            Boundaries thing = new Boundaries();

            thing.is_visible = boundary.transform.Find("Hitbox").GetComponent<SpriteRenderer>().enabled;
            thing.posiiton = boundary.transform.position;
            thing.size_x = boundary.transform.localScale.x;
            thing.size_y = boundary.transform.localScale.y;
            thing.is_disabled = boundary.transform.Find("Hitbox").GetComponent<BoxCollider2D>().enabled ? false : true;
            Vector2 hitbx = boundary.gameObject.transform.Find("Hitbox").transform.localScale;
            thing.inner_size = new Vector2(hitbx.x, hitbx.y);
            thing.name = boundary.name;

            thing.batch = FindBatch(boundary);
            boundaries.Add(thing);
        }
        
        GameObject tilesObj = area.transform.Find("Scenery").gameObject;
        for (int i = 0; i < tilesObj.transform.childCount; i++) {
            SaveTiles(tilesObj.transform.GetChild(i).gameObject);
        }

        GameObject Objects = area.transform.Find("Objects").gameObject;
        for (int i = 0; i < Objects.transform.childCount; i++)
        {
            if (Objects.transform.GetChild(i).gameObject.GetComponent<o_trap>())
            {
                o_trap selectedtrap = Objects.transform.GetChild(i).gameObject.GetComponent<o_trap>();
                
                //All the little traps within the parent object
                List<GameObject> objst = new List<GameObject>();
                for(int c = 0; c < Objects.transform.GetChild(i).gameObject.transform.childCount; c++) {
                    objst.Add(Objects.transform.GetChild(i).transform.GetChild(c).gameObject);
                }
                traps.Add(new data_trap(objst, Objects.transform.GetChild(i).gameObject.name, selectedtrap.teleport_position, selectedtrap.TELEPORT_TRAP, selectedtrap.group_immunity));
            } else
                SaveObjects(Objects.transform.GetChild(i).gameObject);
        }

        GameObject itemsObj = area.transform.Find("Items").gameObject;
        for (int i = 0; i < itemsObj.transform.childCount; i++)
        {
            SaveItems(itemsObj.transform.GetChild(i).gameObject);
        }

        GameObject cutscenesObj = area.transform.Find("Cutscenes").gameObject;
        for (int i = 0; i < cutscenesObj.transform.childCount; i++)
        {
            SaveCutscenes(cutscenesObj.transform.GetChild(i).gameObject.GetComponent<CutsceneHandler>());
        }
    }

    void SaveItems(GameObject item) {
        ItemSave item_save = new ItemSave();
        item_save.file_location = "Prefabs/Items/" + item.name;
        item_save.name = item.name;
        item_save.position = item.transform.position;
        items.Add(item_save);
    }

    void SaveGoal(GoalCass goal) {
        goal_position = goal.transform.position;
        GameObject g = goal.gameObject;
        goal_size = new Vector2(g.GetComponent<BoxCollider2D>().size.x, g.GetComponent<BoxCollider2D>().size.y);
    }

    public void SaveCutscenes(CutsceneHandler cutscene) {
        CutsceneObj obj = new CutsceneObj();
        obj.position = cutscene.transform.position;
        obj.name = cutscene.name;
        obj.size_x = cutscene.gameObject.GetComponent<BoxCollider2D>().size.x;
        obj.size_y = cutscene.gameObject.GetComponent<BoxCollider2D>().size.y;

        obj.is_skippable = cutscene.is_skippable;
        obj.start_on_trigger = cutscene.triggerOnStartUp;
        obj.is_automatic = cutscene.is_automatic;

        obj.character_names = new string[cutscene.characters.Count];
        for (int i = 0; i < cutscene.characters.Count; i++) {
            obj.character_names[i] = cutscene.characters[i].ToString();
        }
        obj.boundary_names = new string[cutscene.boundaries.Count];
        for (int i = 0; i < cutscene.boundaries.Count; i++) {
            obj.boundary_names[i] = cutscene.boundaries[i].ToString();
        }

        foreach (CutsceneElement cut in cutscene.cutsceneElements) {
            obj.elements.Add(cut);
        }
        cutscenes.Add(obj);
    }


    public void SaveObjects(GameObject obj) {
        GenericObjects objec = new GenericObjects();
        objec.position = obj.transform.position;
        objec.obj_name = obj.name;
        objects.Add(objec);
    }

    /*
    public void SaveCharacters(GameObject obj)
    {
        GenericObjects objec = new GenericObjects();
        objec.filename = "Prefabs/Characters/NPC/" + object_file_directory + "/" + obj.name;
        objec.position = obj.transform.position;
        objec.obj_name = obj.name;
        characters.Add(objec);
    }*/

    public Batch FindBatch(o_boundaries ar) {
        Batch thing = new Batch();
        thing.nameofbatch = ar.enemyB;

        if (thing.nameofbatch != "")
        {
            GameObject mainBatch = GameObject.Find(thing.nameofbatch);
            if (mainBatch.GetComponentsInChildren<o_npcharacter>() == null)
                return null;
            o_npcharacter[] enemylist = mainBatch.GetComponentsInChildren<o_npcharacter>();

            thing.enemies = new s_characterdat[enemylist.Length];
            for (int i = 0; i < enemylist.Length; i++)
            {
                thing.enemies[i] = FindEnemyData(enemylist[i]);
                thing.enemies[i].batch = thing.nameofbatch;
            }
        }
        return thing;
    }

    public s_characterdat FindEnemyData(o_npcharacter enemy)
    {
        s_characterdat thing = new s_characterdat();
        thing.file_location = "Prefabs/Characters/Entities/" + enemy.type;
        thing.name = enemy.name;
        thing.is_disabled = enemy.Active ? false: true;
        thing.position = enemy.transform.position;

        return thing;
    }

    public void SaveTiles(GameObject tile) {
        if (tile.name != "PlayerSpawn(Clone)")
        {
            TileObject tilething = new TileObject();
            tilething.size = tile.transform.localScale;
            tilething.position = tile.transform.position;
            tiles.Add(tilething);
        }
    }





}
