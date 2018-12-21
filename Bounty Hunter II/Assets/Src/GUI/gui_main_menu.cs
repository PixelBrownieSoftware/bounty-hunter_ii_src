using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

public class gui_main_menu : s_gui
{
    public gui_options opts;
    public static bool isLoadgame = false;
    public GameObject fadeIn;
    bool show_info = false;
    public GameObject instructions;
    public List<gui_screenres> screenresolutions = new List<gui_screenres>();
    public static gui_screenres curscreenreslutn;
    public static float orthgraSize = 0;
    public static float proport = 0;
    

    enum MENU_STATES
    {
        MAIN,
        INSTRUCTIONS,
        OPTIONS,
        ERASE_DAT,
        SCREEN_RES,
        LOADING
    }
    MENU_STATES menu;


    private void Start()
    {
        instructions = GameObject.Find("Instructions");
        current_menu_gui = this;
        DISPLAY_MODE = GUI_DISP.MENU;
        opt.Add(new s_options(0, StartGame, "Start"));
        opt.Add(new s_options(1, Load, "Load"));
        opt.Add(new s_options(2, ShowInstructions, "Instructions"));
        opt.Add(new s_options(3, test2, "Options"));
        opt.Add(new s_options(4, Quit, "Exit"));
        instructions.SetActive(false);
    }

    public new void Update()
    {
        base.Update();
    }



    new void OnGUI ()
    {
        base.OnGUI();

        if(menu != MENU_STATES.LOADING)
            DrawText("NOTE: This game autosaves everytime you complete a level.\nClick on the buttons to navigate through the menu.\nPress F4 to toggle between fullscreen and windowed mode.", new Rect(65, 90, 90, 90));

        switch (menu)
        {
            case MENU_STATES.MAIN:

                if (Button(new Rect(20, 40 , 90, 20), "Start"))
                {
                    StartGame();
                }
                if (File.Exists("save.BH2"))
                {
                    if (Button(new Rect(20, 40, 90, 20), "Load"))
                    {
                        Load();
                    }
                }
                /*
                if (Button(new Rect(20, 40, 90, 20), "Options"))
                {
                    menu = MENU_STATES.OPTIONS;
                }*/
                if (Button(new Rect(20, 40, 90, 20), "Instructions"))
                {
                    instructions.SetActive(true);
                    menu = MENU_STATES.INSTRUCTIONS;
                }
                if (Button(new Rect(20, 40, 90, 20), "Quit"))
                {
                    Quit();
                }

                break;

            case MENU_STATES.INSTRUCTIONS:
                if (Button(new Rect(20, 40, 90, 20), "Back"))
                {
                    instructions.SetActive(false);
                    menu = MENU_STATES.MAIN;
                }
                break;

            case MENU_STATES.OPTIONS:

                if (Button(new Rect(20, 40, 90, 20), "Screen Resolution"))
                {
                    menu = MENU_STATES.SCREEN_RES;
                }
                if (Button(new Rect(20, 40, 90, 20), "Back"))
                {
                    menu = MENU_STATES.MAIN;
                }
                break;

            case MENU_STATES.SCREEN_RES:
                if (!Screen.fullScreen)
                {
                    if (Button(new Rect(20, 40, 90, 20), "Full"))
                    {
                        FullScreen();
                    }
                }
                else
                {
                    if (Button(new Rect(20, 40 , 90, 20), "Windowed"))
                    {
                        SmallScreen();
                    }
                }
                if (Button(new Rect(20, 40, 90, 20), "Back"))
                {
                    menu = MENU_STATES.OPTIONS;
                }
                break;
        }

        if (Screen.fullScreen)
        {
            if (Input.GetKeyDown(KeyCode.F4))
            {
                Screen.fullScreen = false;
            }
        }
        else
        {

            if (Input.GetKeyDown(KeyCode.F4))
            {
                Screen.fullScreen = true;
            }
        }

        if (show_info)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                HideInstructions();
            }
        }
        ///TODO HAVE GUI BUTTONS WITH OPTIONS
        //if(GUI.Button())

    }

    public void StartGame()
    {
        menu = MENU_STATES.LOADING;
        isLoadgame = false;
        stabalize_cursor = true;
        fadeIn.GetComponent<Animator>().SetBool("Fadeout", true);
        StartCoroutine(fade());
    }

    public void SmallScreen()
    {
        Screen.SetResolution(768, 432, false);
    }

    public void FullScreen()
    {
        Screen.SetResolution(768, 432, FullScreenMode.FullScreenWindow);
    }

    public void test2()
    {
        SetMenus(opts);
    }

    public void HideInstructions()
    {
        instructions.SetActive(false);
        show_info = false;
        stabalize_cursor = false;
    }

    public void ShowInstructions()
    {
        stabalize_cursor = true;
        show_info = true;
        instructions.SetActive(true);
    }

    public void Load()
    {
        if (File.Exists("save.BH2"))
        {
            menu = MENU_STATES.LOADING;
            stabalize_cursor = true;
            isLoadgame = true;
            fadeIn.GetComponent<Animator>().SetBool("Fadeout", true);
            StartCoroutine(fadeLoad());
        }
    }
    

    IEnumerator fadeLoad()
    {
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene("MainGame");
    }
    IEnumerator fade()
    {
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene("Opening");
    }

    public void Quit()
    {
        Application.Quit();
    }

}
