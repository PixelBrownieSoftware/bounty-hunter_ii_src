using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;

/*
public class MainMenu : MonoBehaviour {

    Text text;
    public int menuchoice;
    public enum mainMenuOptions { loadGame, options };
    public enum cursorMode { On, Off};
    cursorMode cursorMod;
    public static bool isLoadgame = false;
    public SpriteRenderer[] buttons;

    public GameObject fadeIn;


	void Start () {
        cursorMod = cursorMode.On;
    }
	
	void Update ()
    {
        switch (cursorMod) {
            case cursorMode.On:

                int prevmenchoice = menuchoice;

                buttons[prevmenchoice].color = Color.grey;
                float v;
                if (Input.GetButtonDown("Horizontal"))
                {

                    v = Input.GetAxisRaw("Horizontal");
                    menuchoice -= Mathf.FloorToInt(v);
                }
                menuchoice %= 2;
                if (menuchoice < 0)
                {
                    menuchoice =1;
                }

                buttons[menuchoice].color = new Color(0.6f, 0.8f,0,8f);
                break;
                
        }
        if (Input.GetKeyDown(KeyCode.Z)) {
            if (menuchoice == 0)
            {
                cursorMod = cursorMode.Off;
                isLoadgame = false;
                fadeIn.GetComponent<Animator>().SetBool("Fadeout", true);
            }

            if (menuchoice == 1)
            {
                if (File.Exists("save.BH2"))
                {
                    cursorMod = cursorMode.Off;
                    isLoadgame = true;
                    fadeIn.GetComponent<Animator>().SetBool("Fadeout", true);
                }
            }

        }

    }
}
*/
