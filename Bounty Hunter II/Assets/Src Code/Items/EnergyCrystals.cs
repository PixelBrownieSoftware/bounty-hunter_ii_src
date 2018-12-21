using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyCrystals : MonoBehaviour {

    public int expamount;

    void OnTriggerStay2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            o_plcharacter playerdat;
            playerdat = col.GetComponent<p_peast>();

            playerdat.selectedWeapon.exp += expamount;

        }
    }
}
