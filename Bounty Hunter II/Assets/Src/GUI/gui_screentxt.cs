using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public struct st_commands
{
    public float duration;
    public Rect square;
    public string text;
    public st_commands(float dur, Rect sq, string tx)
    {
        duration = dur;
        square = sq;
        text = tx;
    }
}

public class gui_screentxt : s_gui {

    public static Stack<st_commands> drawtxt = new Stack<st_commands>();
    float duration = 0;
    

    public static void TextDraw(string txt, float duration, Rect position) {
        st_commands com = new st_commands(duration, position, txt);
        drawtxt.Push(com);
    }

    public void AppearOnScrn(st_commands com) {
        GUIStyle thing = DrawText(com.text, com.square);
        thing.normal.textColor = new Color(thing.normal.textColor.r, thing.normal.textColor.g, thing.normal.textColor.b, duration);
        if (duration <= 0.2)
        {
            drawtxt.Pop();
        }
    }

    private new void OnGUI()
    {
        if (duration > 0)
        {
            duration = duration - Time.deltaTime;
        }
        
        if (drawtxt.Count > 0)
        {
            if (duration <= 0)
            {
                duration = drawtxt.Peek().duration;
            }
            AppearOnScrn(drawtxt.Peek());
        }
    }
}
