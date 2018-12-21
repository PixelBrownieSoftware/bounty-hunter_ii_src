using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DebugClass : MonoBehaviour {

    string scenename;

    private void Awake()
    {
        scenename = SceneManager.GetActiveScene().name;
    }

    void SwitchToMenu()
    {
        Screen.SetResolution(768, 432, false);
        SceneManager.LoadScene("MainMenu");
    }

    void SwitchToGame()
    {
        SceneManager.LoadScene("MainGame");
    }

    void DestorySelf()
    {
        Destroy(this.gameObject);
    }

    void AddToGui()
    {
        transform.SetParent(GameObject.Find("General Crap").transform.Find("GUIs"));
    }

    private void Update()
    {
        if (scenename == "Opening")
        {
            if (Input.GetKeyDown(KeyCode.Z))
                SwitchToGame();
        }
    }


    void SetCollisionsActive() {
        
    }
}
