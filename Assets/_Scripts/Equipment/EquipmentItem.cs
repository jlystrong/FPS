using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentItem : EquipmentComponent
{
    public Animator m_Animator;

    public virtual void Equip(){}
    public virtual void Unequip(){}
    public virtual void FireDown(){}
    public virtual void FireUp(){}
    public virtual void Reload(){}
    public virtual void Aim(){}


    // Firing
	protected int continuouslyUsedTimes = 0;
    public int ContinuouslyUsedTimes { get => continuouslyUsedTimes; }

    public int TryGetMagazineSize(){
        return 100;
    }
}
