using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public delegate void ButtonFunction();

[System.Serializable]
public struct s_options
{
    public int i;
    public ButtonFunction func;
    public string text;
    public s_options(int i, ButtonFunction func, string text)
    {
        this.func = func;
        this.i = i;
        this.text = text;
    }
}

public class s_gui : MonoBehaviour {
    public Font font;
    public GUIStyle style;
    public GUISkin Box;
    const int padding = 90;
    const int offset = 50;
    
    const float MinimumImageSize_X = 768, MinimumImageSize_Y = 432;
    internal int buttonindex = 0;

    protected bool stabalize_cursor = false;
    protected ButtonFunction selected_function;

    internal static float delaybetweenmenus;    //This is a workaround for menus instantly going back
    public static s_gui current_menu_gui;   //So the menuchoices can only apply to one instance

    public enum GUI_DISP {
        INFO,
        MENU
    };
    protected GUI_DISP DISPLAY_MODE;
    public int menuchoice;
    public List<s_options> opt = new List<s_options>();

    public void SelectOption(ButtonFunction function)
    {
        function();
    }
    public GUIStyle DrawText(string txt, Rect rect) {
        style = new GUIStyle();
        style.font = font;
        style.fontSize = 17;
        style.normal.textColor = Color.white;
        GUI.Label(rect,txt, style);
        return style;
    }

    public void PositionIconsRow(Vector2 pos, Texture2D imag_list, int row_num, int spacing, int maxnum, Color colour, Vector2 shake)
    {
        PositionIconsRow(pos + new Vector2(Random.Range(-shake.x, shake.x), Random.Range(-shake.y, shake.y)), imag_list, row_num, spacing, maxnum, colour);
    }
    public void PositionIconsRow(Vector2 pos, Texture2D imag_list, int row_num, int spacing, int maxnum, Color colour)
    {
        int y = 0;
        int x = 0;
        for (int i = 0; i < row_num; i++)
        {
            GUI.color = colour;


            GUI.DrawTexture(new Rect(pos.x + (spacing * x), pos.y + (imag_list.height * y+1),  imag_list.width , imag_list.height ), imag_list);
            x++;
            if (LineIndent(i, maxnum)) {
                x = 0;
                y++;
            }
        }
    }

    public bool Button(Rect rect, string words, bool buttonind)
    {
        buttonindex++;
        if (buttonind)
        {
            if (GUI.Button(new Rect(rect.x, (rect.y  * buttonindex) + padding, rect.width, rect.height), words, Box.GetStyle("button")))
                return true;
            else
                return false;
        }
        else {
            if (GUI.Button(new Rect(rect.x, rect.y + padding , rect.width, rect.height), words, Box.GetStyle("button")))
                return true;
            else
                return false;
        }
    }

    public bool Button(Rect rect, string words)
    {
        buttonindex++;
        if (GUI.Button(new Rect(rect.x, (rect.y * buttonindex) + padding , rect.width, rect.height), words, Box.GetStyle("button")))
            return true;
        else
            return false;
    }
    bool LineIndent(int i, int maxnum) {
        if (i % maxnum == 0 && i != 0)
        {
            return true;
        }
        return false;
    }
    public void DrawBox() {

    }
    public void Update()
    {
        switch (DISPLAY_MODE)
        {
            case GUI_DISP.INFO:
                //draw whatever the heck you want here
                break;

            case GUI_DISP.MENU:

                if (current_menu_gui == this && delaybetweenmenus == 0)
                {
                    if (Input.GetKeyDown(KeyCode.UpArrow))
                        menuchoice -= 1;

                    if (Input.GetKeyDown(KeyCode.DownArrow))
                        menuchoice += 1;

                    if (!stabalize_cursor)
                    {
                        menuchoice = Mathf.Clamp(menuchoice, 0, opt.Count - 1);

                        selected_function = opt[menuchoice].func;

                        if (Input.GetKeyDown(KeyCode.Return))
                        {
                            SelectOption(selected_function);
                        }
                    }
                    //have a menuchoice or something
                    
                }
                break;

        }
        delaybetweenmenus = delaybetweenmenus > 0 ? delaybetweenmenus - Time.deltaTime : 0;
    }
    protected void SetMenus(s_gui menu)
    {
        //This is so that the menus don't swich back constantly
        current_menu_gui = menu;
        delaybetweenmenus = 0.25f;
    }

    protected void OnGUI()
    {
        buttonindex = 0;
        /*
        if (current_menu_gui == this)
        {
            for (int i = 0; i < opt.Count; i++)
            {
                if (menuchoice == i && !stabalize_cursor)
                {
                    GUI.color = new Color(0.2f, 0.6f, 0.2f);
                    DrawText(opt[i].text, new Rect(new Vector2(65, 180 + 25 * i), new Vector2(250, 300)));
                }
                else
                {
                    GUI.color = Color.white;
                    DrawText(opt[i].text, new Rect(new Vector2(65, 180 + 25 * i), new Vector2(250, 300)));
                }

            }
        }
        */
    }

}
