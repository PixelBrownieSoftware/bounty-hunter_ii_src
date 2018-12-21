using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class s_factionhandler : MonoBehaviour
{
    public static Dictionary<string, List<o_character>> faction = new Dictionary<string, List<o_character>>();
    public static List<string> factionKeys = new List<string>();

    public List<o_faction> factions = new List<o_faction>();

    [System.Serializable]
    public struct o_faction
    {
        public List<o_character> characters;
        public string faction_name;
    }
    public static string GetFaction(string id)
    {
        for (int i = 0; i < factionKeys.Count; i++)
        {
            for (int k = 0; k < faction[factionKeys[i]].Count; k++)
            {
                string nam = faction[factionKeys[i]][k].type;
                if (nam == id)
                {
                    return factionKeys[i];
                }
            }
        }
        return "N/A";
    }


    void Awake() {
        if (faction.Count == 0)
        {
            for (int i = 0; i < factions.Count; i++)
            {
                factionKeys.Add(factions[i].faction_name);
                faction.Add(factions[i].faction_name, factions[i].characters);
            }
        }
    }


}