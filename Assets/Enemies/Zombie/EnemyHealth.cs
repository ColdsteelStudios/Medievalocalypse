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

    //Spawns blood splatte when damage is taken
    public GameObject m_bloodSplatterPrefab;

    //Stop enemies taking damage multiple times from one weapon swing
    private float damageCooldown = 0.25f;
    private float damageCooldownRemaining = 0.0f;
    private bool canTakeDamage = true;

    //We need to send our parent room a message when we die so it knows when
    //all the zombies in this room are dead
    private GameObject m_parentRoom;

    //Sound effect to play when the zombie is hit
    public AudioClip m_hurtSound;

    private bool canDie = false;

    void Update()
    {
        if(!canTakeDamage)
        {
            damageCooldownRemaining -= Time.deltaTime;
            if(damageCooldownRemaining <= 0.0f)
            {
                canTakeDamage = true;
            }
        }
    }

    public void CanDie()
    {
        canDie = true;
    }

    void OnTriggerEnter(Collider other)
    {
        if ((other.transform.tag == "Weapon") && canDie && canTakeDamage)
        {
            if (m_bloodSplatterPrefab == null)
                Debug.Log(transform.name + " is missing blood splatter prefab.");
            else
                GameObject.Instantiate(m_bloodSplatterPrefab, other.transform.position, Quaternion.identity);
            TakeDamage();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if ((other.transform.tag == "Weapon") && canDie && canTakeDamage)
        {
            GameObject.Instantiate(m_bloodSplatterPrefab, other.transform.position, Quaternion.identity);
            TakeDamage();
        }
    }

    private void TakeDamage()
    {
        //Play hurt sound
        GetComponent<AudioSource>().PlayOneShot(m_hurtSound);
        health--;
        SendMessage("Blocked");
        damageCooldownRemaining = damageCooldown;
        canTakeDamage = false;
        if (health <= 0)
            Death();
    }

    private void Death()
    {
        if(m_parentRoom != null)
            m_parentRoom.SendMessage("DecrementSpawnCountRemainder");
        transform.GetComponent<CharacterController>().enabled = false;
        GameObject RD = transform.GetComponent<ReplaceRagdoll>().ReplaceWithRagdoll();
        Vector3 PC = GameObject.FindGameObjectWithTag("Player").transform.position;
        Vector3 D = transform.position - PC;
        RD.transform.FindChild("Character1_Reference").GetComponent<Rigidbody>().AddForce(D * ForceMultiplier);
        GameObject.Destroy(this.gameObject);
    }

    private void SetParentRoom(GameObject parentRoom)
    {
        m_parentRoom = parentRoom;
    }
}