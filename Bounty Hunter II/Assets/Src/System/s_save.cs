using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using System.Runtime.Serialization.Formatters.Binary;

public class s_save : MonoBehaviour
{
    public PlayerData savedPlayer;
    public enum saveState { save, load }
    public saveState savestat;

    public void Start()
    {
        BinaryFormatter bf = new BinaryFormatter();

        switch (savestat)
        {
            case saveState.save:
                FileStream file = File.Create("save.BH2");
                if (GameObject.Find("Peast").GetComponent<o_plcharacter>())
                {
                    o_plcharacter playerChar = GameObject.Find("Peast").GetComponent<o_plcharacter>();
                    try
                    {
                        savedPlayer = new PlayerData();
                        savedPlayer.savedMaxHealth = playerChar.maxHealth;
                        savedPlayer.savedHealth = playerChar.Health;
                        savedPlayer.savedWeapons = playerChar.weapons;
                        savedPlayer.level = GameObject.Find("LevelManager").GetComponent<s_levelmanager>().current_level;
                    }
                    catch (Exception e)
                    {
                        print("Error Message: " + e.Message + "/n" + "Error source: "+ e.Source);
                    }
                }

                bf.Serialize(file, savedPlayer);
                file.Close();
            break;

            case saveState.load:
                if (File.Exists("save.BH2"))
                {
                    file = File.Open("save.BH2", FileMode.Open);
                    PlayerData playerdatastuff = (PlayerData)bf.Deserialize(file);

                    s_levelmanager lman = GameObject.Find("LevelManager").GetComponent<s_levelmanager>();

                    try {
                        lman.current_level = playerdatastuff.level;
                        lman.LoadLevel();
                        o_plcharacter playerdat = GameObject.Find("Peast").GetComponent<o_plcharacter>();
                        playerdat.maxHealth = playerdatastuff.savedMaxHealth;
                        playerdat.Health = playerdatastuff.savedHealth;
                        playerdat.weapons = playerdatastuff.savedWeapons;

                    } catch (Exception e) {

                        print("Error Message:" + e.Message + "/n" + "Error source: " + e.Source);
                        lman.LoadLevel();
                    }

                    file.Close();
                }
             break;
        }

        Destroy(this.gameObject);
    }

    
}

[System.Serializable]
public class PlayerData {

    public float savedMaxHealth;
    public float savedHealth;
    public List<WeaponBase> savedWeapons;
    public int level;

}
