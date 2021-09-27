using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Player : MonoBehaviour
{
    public Vector2 lookInput = Vector2.zero;
    public bool viewLocked = false;

    public Vector2 moveInput = Vector2.zero;
    public Vector2 velocity = Vector2.zero;
    public int moveState=0; //0:idle 1:walk 2:run
    public float moveCycle = 0;
    public System.Action moveCycleEnded = null;

    public EquipmentItem m_CurrentItem;
    public int equipmentIndex=0;

    public bool isAiming=false;
    public bool isReloading=false;
    public bool isFiring=false;

    public System.Action startFireAction = null;
    public System.Action endFireAction = null;
    public System.Action<bool> fireAction = null;

    public FirstPersonCamera Camera { get => m_Camera; }
    [SerializeField]
    private FirstPersonCamera m_Camera = null;
}
