using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Value<float> Health = new Value<float>(100f);


    public Value<Vector2> LookInput	= new Value<Vector2>(Vector2.zero);
    public Value<bool> ViewLocked=new Value<bool>(false);
    public Activity Aim = new Activity();
}
