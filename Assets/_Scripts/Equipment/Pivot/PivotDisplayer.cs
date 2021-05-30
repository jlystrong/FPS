using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PivotDisplayer : MonoBehaviour
{
    [SerializeField]
    private Color m_Color = Color.red;

    [SerializeField]
    private float m_Radius = 0.06f;

    [SerializeField]
    private bool m_AlwaysDraw = true;


    private void OnDrawGizmos()
    {
        if (m_AlwaysDraw)
            DrawSphere();
    }

    private void OnDrawGizmosSelected()
    {
        if (!m_AlwaysDraw)
            DrawSphere();
    }

    private void DrawSphere()
    {
        Gizmos.color = m_Color;
        Gizmos.DrawSphere(transform.position, m_Radius);
    }
}
