using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.AI;

public class PlayerMovement : PlayerComponent
{
    public NavMeshAgent agent;
    [Serializable]
    private class GeneralSettings{
        public float maxSpeedForward=2;
        public float maxSpeedSide=1.3f;
        public float maxSpeedBackward=1;
        public float lerpAcc=10.0f;
        public float aimMul=0.5f;
        
        [Space]
        public float walkStepLength=0.6f;
        public float runStepLength=1.0f;
    }
    [SerializeField]
    [Group]
    private GeneralSettings m_GeneralSettings = null;

    private static float SPEED_WALK=0.1f;
    private static float SPEED_RUN=1.8f;

    private float m_DistMovedSinceLastCycleEnded;
    private float m_CurrentStepLength;

    private void Start(){
        agent.velocity=Vector3.zero;
    }

    void Update(){
        UpdateKeys();
        UpdateMove();
        EvaluatePlayerState();
        UpdateMoveCycle();
    }

    private void UpdateKeys(){
        if(Input.GetKey(KeyCode.W)){
            Player.moveInput.y=1;
        }else if(Input.GetKey(KeyCode.S)){
            Player.moveInput.y=-1;
        }
        if(Input.GetKey(KeyCode.D)){
            Player.moveInput.x=1;
        }else if(Input.GetKey(KeyCode.A)){
            Player.moveInput.x=-1;
        }
    }

    private void UpdateMove(){
        Vector2 desireVelocity=Vector2.zero;
        if(Player.moveInput.y<0){
            desireVelocity.y=m_GeneralSettings.maxSpeedBackward*Player.moveInput.y;
        }else{
            desireVelocity.y=m_GeneralSettings.maxSpeedForward*Player.moveInput.y;
        }
        desireVelocity.x=m_GeneralSettings.maxSpeedSide*Player.moveInput.x;
        Player.velocity=Vector2.Lerp(Player.velocity,desireVelocity,Mathf.Min(m_GeneralSettings.lerpAcc*Time.deltaTime,1));

        Transform agentTrans=agent.transform;
        Vector3 forwardMove=agentTrans.forward*Player.velocity.y;
        Vector3 sideMove=agentTrans.right*Player.velocity.x;
        Vector3 totalMove=(forwardMove+sideMove)*Time.deltaTime;
        agent.Move(totalMove);
    }

    private void UpdateMoveCycle(){
        if(Player.moveState<=0){
            m_DistMovedSinceLastCycleEnded=0;
            m_CurrentStepLength=0;
            return;
        }
        
        m_DistMovedSinceLastCycleEnded += Player.velocity.magnitude * Time.deltaTime;
        float targetStepLength = m_GeneralSettings.walkStepLength;
        if(Player.moveState==2){
            targetStepLength=m_GeneralSettings.runStepLength;
        }
        if (m_DistMovedSinceLastCycleEnded > targetStepLength){
            m_DistMovedSinceLastCycleEnded -= targetStepLength;
            if(Player.moveCycleEnded!=null){
                Player.moveCycleEnded();
            }
        }
        Player.moveCycle=Mathf.Clamp01(m_DistMovedSinceLastCycleEnded/targetStepLength);
    }

    private void EvaluatePlayerState(){
        float speed=Player.velocity.magnitude;
        if(speed<SPEED_WALK){
            Player.moveState=0;
        }else if(speed<SPEED_RUN){
            Player.moveState=1;
        }else{
            Player.moveState=2;
        }
    }
}
