using UnityEngine;
using System.Collections;

public class o_item : MonoBehaviour {

    public enum itemtype
    {
        exp,
        heart,
        maxhp,
        newammo,
        ammo
    }
    public itemtype ITEM_TYPE;
    public WeaponBase weapon;
    public int QUANT;       //The number of ammo/health/exp to add

    SpriteRenderer rend;
    public AudioClip pickup_sound;
    float t;
    bool isactive =true;

    public void Initialize()
    {
        isactive = true;
    }

    private void Awake()
    {
        rend = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (!isactive)
            rend.enabled = false;
        else
            rend.enabled = true;

        if (rend != null)
            rend.sortingOrder = (int)transform.position.y * -1;
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        rend.color = new Color(1, 1, 1, 1f);
    }


    public void OnTriggerEnter2D(Collider2D col) {
        if (col.GetComponent<o_plcharacter>())
        {
            if (!isactive)
                return;

            o_plcharacter playerdat;
            playerdat = col.GetComponent<p_peast>();

            switch (ITEM_TYPE) {

                case itemtype.heart:
                    if (playerdat.Health < playerdat.maxHealth)
                    {
                        playerdat.Health += QUANT;
                        gameObject.SetActive(false);
                        SoundManager.SFX.playSound(pickup_sound);
                    }
                    else {
                        rend.color = new Color(1, 1, 1, 0.5f);
                    }
                    break;

                case itemtype.maxhp:

                    playerdat.maxHealth++;
                    playerdat.Health++;
                    SoundManager.SFX.playSound(pickup_sound);
                    isactive = false;

                    break;

                case itemtype.ammo:
                    if (playerdat.selectedWeapon.ammoLimits && playerdat.selectedWeapon.ammo_max > playerdat.selectedWeapon.ammoCap) {
                        QUANT = playerdat.selectedWeapon.ammoitemquant_max;
                        int min = playerdat.selectedWeapon.ammoitemquant_min;

                        SoundManager.SFX.playSound(pickup_sound);
                        playerdat.selectedWeapon.ammoCap += Random.Range(min,QUANT);
                        isactive = false;
                    }

                    break;

                case itemtype.newammo:

                    playerdat.weapons.Add(weapon);
                    SoundManager.SFX.playSound(pickup_sound);
                    isactive = false;

                    break;

                case itemtype.exp:
                    if (playerdat.selectedWeapon.level < playerdat.selectedWeapon.expToNextLevel.Length)
                    {
                        playerdat.selectedWeapon.exp += QUANT;
                        SoundManager.SFX.playSound(pickup_sound);
                        isactive = false;
                    }
                    break;

            }

        }
    }
}
