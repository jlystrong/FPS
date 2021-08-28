using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSound
{
    private static float minPitch=0.05f;

    public static AudioSource PlayClipFlat(AudioClip clip,float volume=1,float pitch=1,float delay=0){    
        GameObject obj=new GameObject("Audio:"+clip.name);
        AudioSource audioSource=obj.AddComponent<AudioSource>();
        audioSource.clip=clip;
        audioSource.volume=volume;
        pitch=Mathf.Max(minPitch,pitch);
        audioSource.pitch=pitch;
        if(delay>0){
            audioSource.PlayDelayed(delay);
        }else{
            audioSource.Play();
        }
        GameObject.Destroy(obj,clip.length/pitch+delay+1);
        return audioSource;
    }
}
