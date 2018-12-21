using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class o_trap : MonoBehaviour {

    public Vector2 teleport_position;
    public string group_immunity;


    public enum TRAP_TYPE
    {
        TELEPORT,
        TELEPORT_NON_HURT,
        TELEPORT_IF_NO_DAHSH,
        NON_TELEPORT
    };
    public TRAP_TYPE TELEPORT_TRAP;

    public void TeleportCharacter(o_character character, Vector2 pos)
    {
        switch (TELEPORT_TRAP)
        {
            case TRAP_TYPE.NON_TELEPORT:

                character.StartCoroutine(character.TakeDamage(1));
                break;

            case TRAP_TYPE.TELEPORT:

                character.StartCoroutine(character.TakeDamage(1));
                character.gameObject.transform.position = pos;
                character.characterstates = o_character.CHARACTER_STATEMACHINE.STAND;
                break;

            case TRAP_TYPE.TELEPORT_IF_NO_DAHSH:
                if (character.characterstates != o_character.CHARACTER_STATEMACHINE.DASHING
                    && character.characterstates != o_character.CHARACTER_STATEMACHINE.DASH_DELAY)
                {
                    StartCoroutine(Fall(character, pos));
                }
                break;

            case TRAP_TYPE.TELEPORT_NON_HURT:

                character.gameObject.transform.position = pos;
                character.characterstates = o_character.CHARACTER_STATEMACHINE.STAND;
                break;
        }
    }
    IEnumerator Fall(o_character character, Vector2 pos)
    {
        SoundManager.SFX.playSound(Resources.Load("Sound/trap_fall") as AudioClip);
        character.GetComponent<SpriteRenderer>().enabled = false;

        yield return new WaitForSeconds(0.5f);
        if (character.Health > 0)
        {
            character.gameObject.transform.position = pos;
            character.isfalling_trap = false;
            character.characterstates = o_character.CHARACTER_STATEMACHINE.STAND;
            character.GetComponent<SpriteRenderer>().enabled = true;
            character.Health--;
        }
        if (character.Health == 0)
        {
            character.GetComponent<SpriteRenderer>().enabled = false;
        }
    }

}
