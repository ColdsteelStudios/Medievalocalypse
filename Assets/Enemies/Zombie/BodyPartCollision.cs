// ---------------------------------------------------------------------------
// BodyPartCollision.cs
// 
// Detects collisions for player attacks hitting various body parts of the enemy
// Used for zombies so we know where the player hit, then we can remove the
// right parts of the body
// 
// Original Author: Harley Laurie
// ---------------------------------------------------------------------------

using UnityEngine;
using System.Collections;

public class BodyPartCollision : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        transform.root.SendMessage("BodyPartHit", transform.name);
    }
}