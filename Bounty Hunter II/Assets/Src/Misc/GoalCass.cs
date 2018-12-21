using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GoalCass : MonoBehaviour {

    public GameObject save;
    s_levelmanager lvl;
    Image fad;
    
    void Start()
    {
        lvl = GameObject.Find("LevelManager").GetComponent<s_levelmanager>();
        fad = GameObject.Find("Fade").GetComponent<Image>();
    }
    

    public void saveUponLevelComplete(GameObject player) {
        lvl.current_level++;
        Instantiate((Resources.Load("Prefabs/Misc/SaveObj", typeof(GameObject))), transform.position, Quaternion.identity);
        lvl.LoadLevel();

    }

    void OnTriggerEnter2D(Collider2D col) {
        if (col.name == "Peast")
        {
            StartCoroutine(FadeAndSave(col.gameObject));
        }
    }

    IEnumerator FadeAndSave(GameObject co)
    {

        cameraScript cam = GameObject.Find("Main Camera").GetComponent<cameraScript>();
        cam.FadeAdd(Color.black);

        o_character c = co.gameObject.GetComponent<o_character>();
        c.Active = false;
        c.characterstates = o_character.CHARACTER_STATEMACHINE.STAND;
        c.rbody2d.velocity = Vector2.zero;

        yield return new WaitForSeconds(1.15f);

        lvl.current_level++;
        Instantiate(save, transform.position, Quaternion.identity);
        lvl.LoadLevel();

    }

    public IEnumerator Fade(Color colour)
    {
        Color former = fad.color;
        float time = 0;
        while (fad.color != colour)
        {
            print(time);
            time += 0.1f;
            fad.color = Color.Lerp(former, colour, time);
            yield return new WaitForSeconds(Time.deltaTime);
        }
    }


    IEnumerator FadeO()
    {
        float time = 1;

        while (time > 0)
        {
            time -= 0.1f;
            print(time >= 0);
            fad.color = Color.Lerp(Color.black, Color.clear, time);
            yield return new WaitForSeconds(Time.deltaTime);
        }
    }

}
