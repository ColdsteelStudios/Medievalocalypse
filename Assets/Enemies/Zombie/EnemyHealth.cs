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

    private bool canDie = false;

    public void CanDie()
    {
        canDie = true;
    }

    void OnTriggerEnter(Collider other)
    {
        if ((other.transform.tag == "Weapon") && canDie)
        {
            health--;
            if (health <= 0)
                Death();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if ((other.transform.tag == "Weapon") && canDie)
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
        Vector3 PC = GameObject.FindGameObjectWithTag("Player").transform.position;
        Vector3 D = transform.position - PC;
        RD.transform.FindChild("Character1_Reference").GetComponent<Rigidbody>().AddForce(D * ForceMultiplier);
        GameObject.Destroy(this.gameObject);
    }
}