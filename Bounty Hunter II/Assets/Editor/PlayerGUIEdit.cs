using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(o_character))]
[CanEditMultipleObjects]
public class PlayerGUIEdit : EditorWindow
{
    AnimationClip source;
    int ind = 0;
    int statenum;
    s_anim_clip.dir animclip;
    Vector2 scrollpos;

    [MenuItem("Brownie/CATS")]
    static void init()
    {
        GetWindow<PlayerGUIEdit>("Character ability tinkering system");
    }
    private void OnGUI()
    {
        if (Selection.activeGameObject != null)
        {
            o_character lvl = Selection.activeGameObject.GetComponent<o_character>();
            if (lvl != null)
            {
                scrollpos = EditorGUILayout.BeginScrollView(scrollpos, GUILayout.Width(650), GUILayout.Height(300));

                int i = 0;
                foreach (s_anim_clip an in lvl.animationclips)
                {
                    an.animation_number = EditorGUILayout.IntField(i + ". State: " + (o_character.CHARACTER_STATEMACHINE)an.animation_number, an.animation_number);
                    an.DIR = (s_anim_clip.dir)EditorGUILayout.EnumPopup(an.DIR);
                    an.animation = (AnimationClip)EditorGUILayout.ObjectField(an.animation, typeof(AnimationClip), false);

                    EditorGUILayout.Space();
                    i++;
                }
                EditorGUILayout.EndScrollView();

                EditorGUILayout.Space();
                if (GUILayout.Button("Delete at"))
                {
                    lvl.animationclips.Remove(lvl.animationclips[ind]);
                }
                ind = EditorGUILayout.IntField(ind);

                EditorGUILayout.Space();
                source = (AnimationClip)EditorGUILayout.ObjectField(source, typeof(AnimationClip), false);
                animclip = (s_anim_clip.dir)EditorGUILayout.EnumPopup(animclip);
                statenum = EditorGUILayout.IntField(statenum);

                if (GUILayout.Button("Add State"))
                {
                    lvl.animationclips.Add(new s_anim_clip(animclip, statenum, source));
                }

            }
        }
    }
}
