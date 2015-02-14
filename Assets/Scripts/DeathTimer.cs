// ---------------------------------------------------------------------------
// DeathTimer.cs
// 
// Destroys the parent GameObject after a cset amount of time has passed
// 
// Original Author: Harley Laurie
// ---------------------------------------------------------------------------

using UnityEngine;
using System.Collections;

public class DeathTimer : MonoBehaviour
{
    public float m_deathTime;

    void Update()
    {
        m_deathTime -= Time.deltaTime;
        if (m_deathTime <= 0.0f)
            GameObject.Destroy(this.gameObject);
    }
}