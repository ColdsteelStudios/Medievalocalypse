// ---------------------------------------------------------------------------
// EnemyHealth.cs
// 
// Keeps track of health, replaces with ragdoll on death
// 
// Original Author: Harley Laurie
// ---------------------------------------------------------------------------

using UnityEngine;
using System.Collections;

public class EnemyHealth : MonoBehaviour 
{
    public int health = 1;
    public int ForceMultiplier = 8000;

    void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "Weapon")
        {
            health--;
            if (health <= 0)
                Death();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if(other.transform.tag == "Weapon")
        {
            health--;
            if (health <= 0)
                Death();
        }
    }

    private void Death()
    {
        transform.GetComponent<CharacterController>().enabled = false;
        GameObject RD = transform.GetComponent<ReplaceRagdoll>().ReplaceWithRagdoll();
        Vector3 ForceDirection = GameObject.FindGameObjectWithTag("Player").transform.position + RD.transform.position;
        RD.transform.FindChild("Character1_Reference").GetComponent<Rigidbody>().AddForce(-RD.transform.forward * ForceMultiplier);
        GameObject.Destroy(this.gameObject);
    }
}