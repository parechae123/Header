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
            }
            return bgm;
        } 
    }
}
