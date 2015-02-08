// ---------------------------------------------------------------------------
// ReplaceRagdoll.cs
// 
// Places a ragdoll in the same position and pose as the parent model
// 
// Original Author: Harley Laurie
// ---------------------------------------------------------------------------

using UnityEngine;
using System.Collections;

public class ReplaceRagdoll : MonoBehaviour 
{
    public GameObject RagdollPrefab;

    public GameObject ReplaceWithRagdoll()
    {
        GameObject RD = GameObject.Instantiate(RagdollPrefab, transform.position, transform.rotation) as GameObject;

        Transform[] ragdollJoints = RD.GetComponentsInChildren<Transform>();
        Transform[] currentJoints = transform.GetComponentsInChildren<Transform>();

        for(int i = 0; i < ragdollJoints.Length; i++)
        {
            for(int j = 0; j < currentJoints.Length; j++)
            {
                if(currentJoints[j].name.CompareTo(ragdollJoints[i].name) == 0)
                {
                    ragdollJoints[i].position = currentJoints[j].position;
                    ragdollJoints[i].rotation = currentJoints[j].rotation;
                    break;
                }
            }
        }

        return RD;
    }
}