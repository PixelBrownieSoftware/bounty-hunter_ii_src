using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public struct gui_screenres
{
    public gui_screenres(float x, float y)
    {
        this.x = x;
        this.y = y;
    }

    public float x, y;
}

public class gui_options : s_gui
{

    public bool accept;
    public gui_main_menu mainmenu;
    public Texture2D image;

    public List<gui_screenres> screenresolutions = new List<gui_screenres>();
    public static gui_screenres curscreenreslutn;

    public enum MENUOPT
    {
        NONE,
        VOLUME,
        SCREEN_RES,
        CONFIMATION
    }
    MENUOPT MENU;
    public int optch = 0;
    public static float orthgraSize = 0;
    public static float proport = 0;

    private void Start()
    {
        screenresolutions.Add(new gui_screenres(768, 432));
        screenresolutions.Add(new gui_screenres(1280, 720));

        DISPLAY_MODE = GUI_DISP.MENU;
        opt.Add(new s_options(0, Option1, "Set Volume"));
        opt.Add(new s_options(1, Option2, "Screen Resolution"));
        opt.Add(new s_options(2, EraseDataOption, "Erase Save"));
        opt.Add(new s_options(3, Back, "Back"));
    }
    new void Update()
    {
        base.Update();
        switch (MENU)
        {
            case MENUOPT.NONE:

                break;

            case MENUOPT.VOLUME:

                optch += (int)Input.GetAxisRaw("Vertical");
                optch = Mathf.Clamp(optch, -1, 0);

                switch (optch)
                {
                    case 0:

                        SoundManager.volume += Input.GetAxisRaw("Horizontal") / 10;
                        break;

                    case -1:

                        SoundManager.mus_volume += Input.GetAxisRaw("Horizontal") / 10;
                        break;
                }

                SoundManager.mus_volume = Mathf.Clamp(SoundManager.mus_volume, 0, 1);
                SoundManager.volume = Mathf.Clamp(SoundManager.volume, 0, 1);

                if (Input.GetKeyDown(KeyCode.X))
                {
                    menuchoice = 0;
                    stabalize_cursor = false;
                    MENU = MENUOPT.NONE;
                }
                break;

            case MENUOPT.SCREEN_RES:

                menuchoice = Mathf.Clamp(menuchoice, 0, 1);

                if (delaybetweenmenus == 0)
                {
                    if (Input.GetKeyDown(KeyCode.Z))
                    {
                        switch (menuchoice)
                        {
                            case 0:
                                Screen1(); break;

                            case 1:
                                Screen3(); break;

                            case 2:
                                Screen2(); break;

                        }
                    }
                    if (Input.GetKeyDown(KeyCode.X))
                    {
                        menuchoice = 1;
                        stabalize_cursor = false;
                        MENU = MENUOPT.NONE;
                    }
                }
                break;

            case MENUOPT.CONFIMATION:

                optch += (int)Input.GetAxisRaw("Vertical");
                optch = Mathf.Clamp(optch, 0, 1);
                if (delaybetweenmenus == 0)
                {
                    switch (optch)
                    {
                        case 0:
                            if (Input.GetKeyDown(KeyCode.Z))
                            {
                                print("Deleted!");
                                menuchoice = 2;
                                stabalize_cursor = false;
                                MENU = MENUOPT.NONE;
                            }
                            break;

                        case 1:
                            if (Input.GetKeyDown(KeyCode.Z))
                            {
                                print("Deleted!");
                                EraseData();
                                menuchoice = 2;
                                stabalize_cursor = false;
                                MENU = MENUOPT.NONE;
                            }
                            break;
                    }

                }

                break;
        }

    }
    new void OnGUI()
    {
        base.OnGUI(); switch (MENU)
        {
            case MENUOPT.NONE:

                break;

            case MENUOPT.VOLUME:

                for (int i = 0; i < SoundManager.volume * 10; i++)
                {
                    if (optch == 0)
                    {
                        GUI.color = Color.blue;
                    }
                    else { GUI.color = Color.white; }
                    GUI.DrawTexture(new Rect(20 + (35 * i), 30, 30, 40), image, ScaleMode.ScaleToFit);

                }

                for (int i = 0; i < SoundManager.mus_volume * 10; i++)
                {
                    if (optch == -1)
                    {
                        GUI.color = Color.blue;
                    }
                    else { GUI.color = Color.white; }
                    GUI.DrawTexture(new Rect(20 + (35 * i), 90, 30, 40), image, ScaleMode.ScaleToFit);

                }
                break;

            case MENUOPT.SCREEN_RES:

                GUI.color = Color.white;
                switch (menuchoice)
                {
                    case 0:
                        DrawText("Small", new Rect(new Vector2(65, 140), new Vector2(250, 300)));
                        break;

                   /* case 1:
                        DrawText("Large", new Rect(new Vector2(0, 120), new Vector2(250, 300)));
                        break;*/

                    case 1:
                        DrawText("FullScreen", new Rect(new Vector2(65, 140), new Vector2(250, 300)));
                        break;

                }
                break;

            case MENUOPT.CONFIMATION:

                switch (optch)
                {
                    case 0:
                        GUI.color = Color.blue;
                        break;

                    case 1:
                        GUI.color = Color.white;
                        break;
                }
                DrawText("Yes", new Rect(new Vector2(0, 120), new Vector2(250, 300)));

                switch (optch)
                {
                    case 0:
                        GUI.color = Color.white;
                        break;

                    case 1:
                        GUI.color = Color.blue;
                        break;
                }
                DrawText("No", new Rect(new Vector2(0, 170), new Vector2(250, 300)));

                break;
        }
    }

    public void Screen1()
    {
        Screen.SetResolution(768, 432, false);
        curscreenreslutn = screenresolutions[0];
        proport = Camera.main.orthographicSize / Mathf.Min(Screen.width, Screen.height);
        orthgraSize = proport * Mathf.Min(Screen.width, Screen.height);
    }
    public void Screen2()
    {
        Screen.SetResolution(1280, 720, false);
        curscreenreslutn = screenresolutions[1];
        proport = Camera.main.orthographicSize / Mathf.Min(Screen.width, Screen.height);
        orthgraSize = proport * Mathf.Min(Screen.width, Screen.height);
    }
    public void Screen3()
    {
        Screen.SetResolution(768, 432, FullScreenMode.FullScreenWindow);
        
        curscreenreslutn = screenresolutions[0];
        proport = Camera.main.orthographicSize / Mathf.Min(Screen.width, Screen.height);
        orthgraSize = proport * Mathf.Min(Screen.width, Screen.height);
    }

    public void Option1()
    {
        stabalize_cursor = true;
        MENU = MENUOPT.VOLUME;
    }
    public void Option2()
    {
        stabalize_cursor = true;
        SetMenus(this);
        MENU = MENUOPT.SCREEN_RES;
    }
    public void EraseDataOption()
    {
        stabalize_cursor = true;
        SetMenus(this);
        MENU = MENUOPT.CONFIMATION;
    }
    public void EraseData()
    {
        if (File.Exists("save.BH2"))
        {
            print("Deleted!");
            File.Delete("save.BH2");
        }
    }

    public void Back()
    {
        print("This is ok3");
        SetMenus(mainmenu);
    }

}