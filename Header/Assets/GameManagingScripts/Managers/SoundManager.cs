using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager
{
    private AudioMixer mixer;
    public AudioMixer Mixer
    {
        get 
        { 
            if (mixer == null)
            {
                mixer = Resources.Load<AudioMixer>("audioMixer");
            }
            return mixer;
        }
    }
    private AudioSource sfx;
    public AudioSource SFX 
    { 
        get 
        { 
            if (sfx == null)
            {
                sfx = Managers.instance.gameObject.AddComponent<AudioSource>();

                
                AudioMixerGroup[] tempGroup = Mixer.FindMatchingGroups("Master/SoundEffect");
                sfx.outputAudioMixerGroup = tempGroup[0];
                sfx.volume = 0.5f;
            }
            return sfx;
        } 
    }
    private AudioSource bgm;
    public AudioSource BGM 
    { 
        get 
        {
            if (bgm == null)
            {
                bgm = Managers.instance.gameObject.AddComponent<AudioSource>();
                AudioMixerGroup[] tempGroup = Mixer.FindMatchingGroups("Master/BGM");
                bgm.outputAudioMixerGroup = tempGroup[0];
                bgm.volume = 0.5f;
            }
            return bgm;
        } 
    }
    public bool SFXLoopCheck
    {
        set
        {
            if (!value)
            {
                SFX.clip = null;
            }

            SFX.loop = value;
        }
    }
    public void SFXLoop(AudioClip sound)
    {
        SFXLoopCheck = true;
        SFX.pitch = 1;
        SFX.clip = sound;
        SFX.Play();

    }
    public void SFXPlayOneshot(AudioClip sound,bool ballSound = false,bool resetPitch = false)
    {
        SFXLoopCheck = false;
        if (ballSound)
        {
            //TODO : 추후 사운드 피치별로 실행시켜야함
            Debug.LogError(SFX.pitch);
            SFX.pitch = sfx.pitch+0.3f;
        }
        else if(!ballSound)
        {
            SFX.pitch = 1;
        }
        if (resetPitch)
        {
            SFX.pitch = 1;
        }
        
        SFX.PlayOneShot(sound);
    }
    public void SetSoundValue(bool isBGM,float value)
    {
        if (isBGM)
        {
            BGM.volume = value;
        }
        else
        {
            SFX.volume = value;
        }
    }
}
