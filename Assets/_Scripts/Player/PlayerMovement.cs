using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerMovement : PlayerComponent
{
    public NavMeshAgent agent;
    public float maxSpeedForward=2;
    public float maxSpeedSide=1;
    public float maxSpeedBackward=1;
    public float lerpAcc=0.2f;

    private void Start(){
        agent.velocity=Vector3.zero;
    }

    void Update(){
        Vector2 desireVelocity=Vector2.zero;
        if(Player.moveInput.y<0){
            desireVelocity.y=maxSpeedBackward*Player.moveInput.y;
        }else{
            desireVelocity.y=maxSpeedForward*Player.moveInput.y;
        }
        desireVelocity.x=maxSpeedSide*Player.moveInput.x;
        Player.Velocity.Set(Vector2.Lerp(Player.Velocity.Val,desireVelocity,lerpAcc*Time.deltaTime));

        Transform agentTrans=agent.transform;
        Vector3 forwardMove=agentTrans.forward*desireVelocity.y;
        Vector3 sideMove=agentTrans.right*desireVelocity.x;
        Vector3 totalMove=(forwardMove+sideMove)*Time.deltaTime;
        agent.Move(totalMove);
    }
}
