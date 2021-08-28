using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSoundPackFlatPlayer : MonoBehaviour
{
    public GameSoundPackFlat packFlat;

    void Start(){
        AudioSource audioSource=packFlat.Play();
        if(audioSource==null){
            return ;
        }
        audioSource.transform.position=transform.position;
    }
}
