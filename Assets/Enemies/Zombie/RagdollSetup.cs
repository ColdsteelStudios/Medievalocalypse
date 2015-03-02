// ---------------------------------------------------------------------------
// RagdollSetup.cs
// 
// Used by the EnemyHealthBreakdown script to remove the un-needed body parts
// from the ragdoll before we set it up for use.
// 
// Original Author: Harley Laurie
// ---------------------------------------------------------------------------

using UnityEngine;
using System.Collections;

public class RagdollSetup : MonoBehaviour
{
    public GameObject m_headRenderer;
    public GameObject m_leftArmRenderer;
    public GameObject m_rightArmRenderer;

    public GameObject m_headBone;
    public GameObject m_leftArmBone;
    public GameObject m_rightArmBone;

	private void RemoveHead()
    {
        //Disable the head renderer and delete its bones / ragdoll components
        m_headRenderer.SetActive(false);
        m_headBone.SetActive(false);
    }

    private void RemoveLeftArm()
    {
        m_leftArmRenderer.SetActive(false);
        m_leftArmBone.SetActive(false);
    }

    private void RemoveRightArm()
    {
        m_rightArmRenderer.SetActive(false);
        m_rightArmBone.SetActive(false);
    }
}