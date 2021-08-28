using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName="GameAssets/Create GameSoundPackFlat")]
public class GameSoundPackFlat : ScriptableObject
{
    [SerializeField]
    private AudioClip[] clips;

    [Space]
    [SerializeField]
    private float baseVolume=1.0f;
    [SerializeField]
    private float randomVolume=0.1f;

    [Space]
    [SerializeField]
    private float basePitch=1.0f;
    [SerializeField]
    private float randomPitch=0.1f;

    [Space]
    [SerializeField]
    private float baseDelay=0.0f;
    [SerializeField]
    private float randomDelay=0.0f;

    [Space]
    [SerializeField]
    private float base3D=0.0f;
    [SerializeField]
    private float random3D=0.0f;

    public AudioSource Play(){
        AudioClip clip=clips[Random.Range(0,clips.Length)];
        if(clip==null){
            return null;
        }
        float volume=baseVolume+Random.Range(-randomVolume,randomVolume);
        float pitch=basePitch+Random.Range(-randomPitch,randomPitch);
        float delay=baseDelay+Random.Range(-randomDelay,randomDelay);
        AudioSource audioSource=GameSound.PlayClipFlat(clip,volume,pitch,delay);

        if(base3D>0){
            float sound3D=base3D+Random.Range(-random3D,random3D);
            audioSource.spatialBlend=sound3D;
        }
        
        return audioSource;
    }
}
