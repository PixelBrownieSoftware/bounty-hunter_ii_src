using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(s_levelmanager))]

public class LevelEditor2 : Editor {

    public enum object_type
    {
        collisions,
        items,
        enemies,
        trap_parent,
        traps,
        playerspawn,
        cutscene,
        enemy_boundary
    }
    Vector2 pos = new Vector2(0, 0);
    object_type objects;
    GameObject trappar;
    int generic_number = 0;
    string file_loc = "", area_part = "", character_name = "";

    private void OnSceneGUI()
    {
        Vector2 spawnpos = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition).origin;
        s_levelmanager lvl = (s_levelmanager)target;
        if (s_events.KeyPress() == KeyCode.S) {
            lvl.is_numbered = (objects == object_type.collisions || objects == object_type.items) ? false : true;

            switch (objects)
            {
                case object_type.playerspawn:
                //Set the player spawn
                    lvl.levsav.GetComponent<LevelData>().player_position = spawnpos;
                    Debug.Log("New Spawn: " + lvl.levsav.GetComponent<LevelData>().player_position);
                    break;

                //Create the object and set the transform parent
                case object_type.cutscene:
                case object_type.collisions:
                    lvl.CreateObject( spawnpos);
                    break;
                case object_type.enemies:
                case object_type.enemy_boundary:
                case object_type.items:
                case object_type.trap_parent:
                    lvl.CreateObject(file_loc, character_name, area_part, spawnpos);
                    break;

                case object_type.traps:
                    lvl.CreateObject(file_loc, character_name, trappar, spawnpos);
                    break;
            }
            
        }

        if (s_events.KeyPress() == KeyCode.X) {
            lvl.levsav.GetComponent<LevelData>().max_boundaries = spawnpos;
            Debug.Log("Max camera: " + lvl.levsav.GetComponent<LevelData>().max_boundaries);
        }
        if (s_events.KeyPress() == KeyCode.Z) {
            lvl.levsav.GetComponent<LevelData>().min_boundaries = spawnpos;
            Debug.Log("Min camera: " + lvl.levsav.GetComponent<LevelData>().min_boundaries);
        }

        if (s_events.KeyPress() == KeyCode.R)
        {
            lvl.ResetNumber();
            Debug.Log("yes?");
        }
    }

    public override void OnInspectorGUI() {
        base.OnInspectorGUI();

        EditorGUILayout.LabelField("");
        EditorGUILayout.LabelField("Help:");
        EditorGUILayout.LabelField("X and Z to set max and min camera view respectively.");
        EditorGUILayout.LabelField("S to spawn an object or set player Spawn");
        EditorGUILayout.LabelField("R to reset object number");
        objects = (object_type)EditorGUILayout.EnumPopup(objects);


        s_levelmanager lvl = (s_levelmanager)target;

        if (GUILayout.Button("Remove"))
        {
            lvl.DestroyLevel();
        }

        switch (objects)
        { 
            case object_type.enemies:
                file_loc = "Prefabs/Characters/Entities/"; area_part = "Entities";

                DrawList(EnemyList(lvl.enemy_objects));
                break;

            case object_type.items:
                file_loc = "Prefabs/Items/"; area_part = "Items";
                DrawList(ObjList(lvl.item_objects));
                break;

            case object_type.collisions:
                file_loc = "Prefabs/Objects/Generic/";
                character_name = "tileObj";
                area_part = "Scenery";
                break;

            case object_type.cutscene:

                file_loc = "Prefabs/Objects/Generic/";
                character_name = "Cutscene_Base";
                area_part = "Cutscenes";
                break;

            case object_type.traps:
                trappar = (GameObject)EditorGUILayout.ObjectField(trappar, typeof(GameObject), true);
                file_loc = "Prefabs/Misc/";
                character_name = "Trap";
                break;

            case object_type.trap_parent:
                file_loc = "Prefabs/Misc/";
                character_name = "Traph";
                area_part = "Objects";
                break;

            case object_type.enemy_boundary:

                file_loc = "Prefabs/Misc/";
                character_name = "Enemy Boundaries";
                area_part = "Boundaries";
                break;
        }

        s_levelmanager lman = (s_levelmanager)target;
        lman.current_level = EditorGUILayout.IntSlider(lman.current_level, 0, lman.levels.Count - 1);

        if (GUILayout.Button("Load Level"))
        {
            lman.LoadLevel();
        }

    }

    List<string> EnemyList(List<o_npcharacter> enemies) {
        List<string> enemy_words = new List<string>();
        foreach (o_npcharacter en in enemies) {
            enemy_words.Add(en.name);
        }
        return enemy_words;
    }

    List<string> ObjList(List<GameObject> enemies) {
        List<string> obj_words = new List<string>();
        foreach (GameObject en in enemies) {
            obj_words.Add(en.name);
        }
        return obj_words;
    }

    public void DrawList(List<string> list) {
        EditorGUILayout.BeginVertical();
        generic_number = EditorGUILayout.IntSlider(generic_number, 0, list.Count - 1);
        pos = EditorGUILayout.BeginScrollView(pos, GUILayout.Width(200), GUILayout.Height(200));

        for (int i = 0; i < list.Count; i++)
        {
            if (generic_number == i)
            {
                character_name = list[i];
                EditorGUILayout.LabelField(list[i], EditorStyles.boldLabel);
            }
            else
                EditorGUILayout.LabelField(list[i]);
        }

        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();
    }

}
