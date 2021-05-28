using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentItem : EquipmentComponent
{
    public Animator m_Animator;

    public virtual void FireDown(){}
    public virtual void FireUp(){}
    public virtual void Reload(){}
    public virtual void Aim(){}
}
