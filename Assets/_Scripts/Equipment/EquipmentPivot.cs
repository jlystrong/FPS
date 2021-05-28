using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentPivot : MonoBehaviour
{
    private void OnDrawGizmos() {
        Gizmos.color=Color.red;
        Gizmos.DrawSphere(transform.position,0.02f);
    }
}
