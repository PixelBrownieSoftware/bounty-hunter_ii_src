using UnityEngine;
using System.Collections;

public class HealthItem : MonoBehaviour {

    public int type;
    public AudioClip heal_sound;
    public AudioClip heal_inc_sound;

    void OnTriggerStay2D(Collider2D col)
    {

        if (col.name == "Peast")
        {
            o_plcharacter playerdat;
            playerdat = col.GetComponent<p_peast>();

            //heal player
            if (type == 1) {
                if (playerdat.Health < playerdat.maxHealth) {
                    playerdat.Health++;
                    SoundManager.SFX.playSound(heal_sound);
                    this.gameObject.SetActive(false);
                }
            }
            //increase maximum HP
            if (type == 2) {
                playerdat.maxHealth++;
                playerdat.Health++;
                SoundManager.SFX.playSound(heal_inc_sound);
                this.gameObject.SetActive(false);
            }

        }
    }
}
