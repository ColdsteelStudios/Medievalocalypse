// ---------------------------------------------------------------------------
// CameraFollow.cs
// 
// Class description goes here
// 
// Original Author: Harley Laurie
// ---------------------------------------------------------------------------

using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour
{
    Vector3 m_offset;
    GameObject m_followTarget;

    public void SetFollowTarget(GameObject a_followTarget, Vector3 a_offset, Quaternion a_rotation)
    {
        m_followTarget = a_followTarget;
        m_offset = a_offset;
        transform.position = m_followTarget.transform.position - m_offset;
        transform.rotation = a_rotation;
    }

    void Update()
    {
        if(m_followTarget != null && m_offset != null)
        {
            transform.position = m_followTarget.transform.position - m_offset;
        }
    }
}