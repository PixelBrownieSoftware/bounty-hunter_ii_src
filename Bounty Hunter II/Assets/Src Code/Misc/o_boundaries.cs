using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class o_boundaries : MonoBehaviour {

    public GameObject enemy;
    public o_npcharacter[] enemies;
    public BoxCollider2D collision;

    public string enemyB;

    /*
    void Start() {
        
        if (enemyB != "") { enemy = GameObject.Find(enemyB); }
        display = GameObject.Find("EnemyLimit").GetComponent<GUIText>();
        collision = transform.Find("Hitbox").GetComponent<BoxCollider2D>();
    }*/

    void Update ()
    { if (enemyB != "") { enemy = GameObject.Find(enemyB); if (enemy != null) {
                if (enemy.GetComponent< o_npcharacter>() == null) { enemies = enemy.gameObject.GetComponentsInChildren<o_npcharacter>();
                    if (enemies.Length <= 0) { Destroy(this.gameObject); } }
                if (enemy.GetComponent<o_npcharacter>()) {
                    if (enemy.GetComponent<o_npcharacter>().Health == 0) {
                        this.gameObject.SetActive(false); } } } } }

    /*
    void OnTriggerStay2D(Collider2D col) {

        if (col.CompareTag("Player")) {
            if (enemyB != "") {
                if (enemy.GetComponent<o_npcharacter>() == null) { display.text = "Enemies remaining: " + enemies.Length; } }
        }
    }

    void OnTriggerExit2D(Collider2D col) {
        if (enemy != null) { if (enemy.GetComponent<o_npcharacter>() == null) { display.text = ""; }  }
    }
    */
}
