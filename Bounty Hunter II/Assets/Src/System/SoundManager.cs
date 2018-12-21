using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour {
    
    public static SoundManager SFX;
    public AudioSource audioPlay;
    public static float volume = 0.5f;
    public static float mus_volume = 0.5f;
    public AudioSource music;


    private void Start()
    {
        if (SFX == null)
        {
            SFX = this;
        }
    }

    private void Update()
    {
        audioPlay.volume = volume;
        music.volume = mus_volume;
    }

    public AudioClip LoadAudio(string s_name)
    {
        if (SFX != null && Resources.Load("Sound" + "/" + s_name) as AudioClip != null)
        {
            AudioClip clip = Resources.Load("Sound" + "/" + s_name) as AudioClip;
            return clip;
        }
        else
            return null;
    }

    public void playSound(AudioClip audio) {
        if (audio == null) { return; }
        List<AudioClip> audioClips = new List<AudioClip>();
        audioClips.Add(audio);
        audioPlay.volume = volume;
        foreach (AudioClip clip in audioClips)
        { 
            audioPlay.PlayOneShot(clip);
        }

    }
    public void stopMusic()
    {
        if (SFX != null)
        {
            music.Stop();
        }
    }

    public void playMusic(AudioClip audio, bool isLoop)
    {
        stopMusic();
        music.clip = audio;
        music.loop = true;
        music.Play();
    }
}
