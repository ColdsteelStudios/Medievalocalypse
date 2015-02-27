// ---------------------------------------------------------------------------
// FootstepSounds.cs
// 
// Class description goes here
// 
// Original Author: Harley Laurie
// ---------------------------------------------------------------------------

using UnityEngine;
using System.Collections;

public class FootstepSounds : MonoBehaviour
{
    public AudioClip m_footstepSound;
    public AudioSource m_audioSource;

	void OnTriggerEnter(Collider other)
    {
        if (other.transform.name == "Dungeon_Floor(Clone)")
            m_audioSource.PlayOneShot(m_footstepSound);
    }
}