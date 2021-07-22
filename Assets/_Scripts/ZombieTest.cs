using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ZombieTest : MonoBehaviour
{
    private NavMeshAgent agent;
    private Animator animator;

    public float repathInterval=2f;
    private float lastRepathTime=0f;

    private Transform playerTrans;

    void Awake(){
        GameObject playerObj=GameObject.Find("Player");
        playerTrans=playerObj.transform;

        agent=GetComponent<NavMeshAgent>();
        animator=GetComponent<Animator>();
        
    }

    void Update(){
        if(Time.time>=(lastRepathTime+repathInterval)){
            lastRepathTime=Time.time;
            agent.ResetPath();
            agent.destination=playerTrans.position;
            agent.updatePosition=false;
        }
    }

    private void OnAnimatorMove() {
        Vector3 position=animator.rootPosition;
        agent.nextPosition=new Vector3(position.x,agent.nextPosition.y,position.z);
        position.y=agent.nextPosition.y;
        transform.position=position;    
    }
}
