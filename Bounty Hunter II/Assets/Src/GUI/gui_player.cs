using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class gui_player : s_gui {

    public Texture2D player_health;
    public Texture2D enemy_health;
    public Texture2D empty_health;
    //List<Image> listofstuff = new List<Image>();
    public static o_plcharacter character;
    public static o_npcharacter[] othercharacter = new o_npcharacter[4];
    string tex;
    public bool guihide = false;
    public bool bossguishow = false;

    public void ToggleBossGUI(bool setup)
    {
        bossguishow = setup;
    }


    private new void OnGUI()
    {
        if (!guihide)
        {
            if (character != null)
            {
                if (!character.selectedWeapon.noLevel)
                {
                    if (character.selectedWeapon.level != character.selectedWeapon.expToNextLevel.Length)
                    {
                        if (character.selectedWeapon.ammoLimits)
                        {
                            tex = character.selectedWeapon.name + " Level: " + character.selectedWeapon.level
                            + "\n" + character.selectedWeapon.exp + "/ " +
                            (int)character.selectedWeapon.expToNextLevel[character.selectedWeapon.level] + "\n" +
                            "Ammo: " + character.selectedWeapon.ammoCap + "/ " + character.selectedWeapon.ammo_max;
                        }
                        else
                        {
                            tex = character.selectedWeapon.name + " Level: " + character.selectedWeapon.level
                            + "\n" + character.selectedWeapon.exp + "/ " +
                            (int)character.selectedWeapon.expToNextLevel[character.selectedWeapon.level];
                        }

                    }
                    else
                    {
                        if (!character.selectedWeapon.ammoLimits)
                        { tex = character.selectedWeapon.name + " MAX"; }
                        else
                        {
                            tex = character.selectedWeapon.name + " MAX" + "\n" +
                              "Ammo: " + character.selectedWeapon.ammoCap + "/ " + character.selectedWeapon.ammo_max;
                        }
                    }


                }
                else
                    tex = character.selectedWeapon.name;


                DrawText(tex, new Rect(0, 90, 50 / Screen.width, 500 / Screen.height));

                if (character.name == "Peast")
                {
                    DrawCharacterHP(new Color(1f, 0.68f, 0, 1));
                }
                if (character.name == "Kazzi")
                {
                    DrawCharacterHP(new Color(0.05f, 0.86f, 1, 1));
                }
            }
        }

        if (bossguishow)
        {
            if (othercharacter != null)
            {
                int totalhealth = 0;
                int totalmaxhealth = 0;
                for (int i = 0; i < othercharacter.Length; i++)
                    if (othercharacter[i] != null)
                    {
                        totalhealth += (int)othercharacter[i].Health;
                        totalmaxhealth += (int)othercharacter[i].maxHealth;
                    }

                //health / 94
                float calc = 3;
                if (totalmaxhealth > 0 && totalmaxhealth < 95)
                {
                    calc = (94 / totalmaxhealth) * 10;
                    calc = Mathf.Clamp(calc, 20, 75);
                }
                PositionIconsRow(new Vector2(calc * 6, 380), enemy_health, totalmaxhealth, 7, 95, Color.black);
                PositionIconsRow(new Vector2(calc * 6, 380), enemy_health, totalhealth, 7, 95, Color.white);

            }
        }
    }

    public void DrawCharacterHP(Color colour)
    {
        PositionIconsRow(new Vector2(3, 30), empty_health, (int)character.maxHealth, 12, 15, Color.white);
        if (character.Health < character.maxHealth / 3)
            PositionIconsRow(new Vector2(3, 30), player_health, (int)character.Health, 12, 15, Color.red, new Vector2(3.5f, 3.5f));
        else
            PositionIconsRow(new Vector2(3, 30), player_health, (int)character.Health, 12, 15, Color.white);
    }

}
