using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentComponent : PlayerComponent
{
    protected EquipmentHandler m_EHandler;

    public virtual void Initialize(EquipmentHandler equipmentHandler)
    {
        m_EHandler = equipmentHandler;
    }
}
