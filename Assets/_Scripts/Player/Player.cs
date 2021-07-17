using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Value<float> Health = new Value<float>(100f);

    public Value<Vector2> LookInput	= new Value<Vector2>(Vector2.zero);
    public Value<bool> ViewLocked=new Value<bool>(false);

    public Value<float> MoveCycle = new Value<float>();
	public System.Action MoveCycleEnded = null;
    
    public Vector2 moveInput=Vector2.zero; 
    public Value<Vector2> Velocity=new Value<Vector2>(Vector2.zero);

    public EquipmentItem m_CurrentItem;

    public Activity Walk = new Activity();
	public Activity Run = new Activity();
    public Activity Aim = new Activity();
    public Activity Reload = new Activity();

    public System.Action startFireAction = null;
    public System.Action endFireAction = null;
    public System.Action<bool> fireAction = null;

    public FirstPersonCamera Camera { get => m_Camera; }
    [SerializeField]
	private FirstPersonCamera m_Camera = null;
}
