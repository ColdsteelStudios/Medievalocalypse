// ---------------------------------------------------------------------------
// EnemyHealthBreakdown.cs
// 
// When this zombie is hit, it has a chance to remove a random body part.
// If the legs are removed, the zombie becomes a crawler.
// If the head is removed, the zombie instantly dies.
// When the zombies health reaches 0, whatever body parts left become a ragdoll.
// 
// Original Author: Harley Laurie
// ---------------------------------------------------------------------------

using UnityEngine;
using System.Collections;

public class EnemyHealthBreakdown : MonoBehaviour
{
    //Ragdoll prefab objects
    public GameObject HeadRagdoll;
    public GameObject LeftArmRagdoll;
    public GameObject RightArmRagdoll;
    public GameObject m_completeRagdollPrefab;

    //Bones used to spawn the ragdoll parts in correct position
    public GameObject HeadBone;
    public GameObject LeftArmBone;
    public GameObject RightArmBone;

    //Which bodyparts still remain on the character?
    private bool m_headRemains = true;
    private bool m_leftArmRemains = true;
    private bool m_rightArmRemains = true;
    private int m_remainingBodyParts = 3;

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

    private bool canDie = false;

    void Update()
    {
        if (!canTakeDamage)
        {
            damageCooldownRemaining -= Time.deltaTime;
            if (damageCooldownRemaining <= 0.0f)
                canTakeDamage = true;
        }
    }

    private void SetParentRoom(GameObject parentRoom)
    {
        m_parentRoom = parentRoom;
    }

    public void CanDie()
    {
        canDie = true;
    }

    private void TakeDamage()
    {
        //25% chance to decapitate a body part from the zombie when hitting it
        int l_decapitateChance = Random.Range(1, 101);
        if (l_decapitateChance <= 25)
            RemoveRandomPart();

        health--;
        SendMessage("Blocked");
        damageCooldownRemaining = damageCooldown;
        canTakeDamage = false;
        if (health <= 0)
            Death();
    }

    private void RemoveRandomPart()
    {
        //If there are no body parts left to remove, just leave
        if (m_remainingBodyParts <= 0)
            return;
        //Otherwise, choose a random part to remove
        int l_randomPart = Random.Range(1, 4);
        switch(l_randomPart)
        {
            case(1):
                //If the head is already removed select a different part to remove
                if(!m_headRemains)
                {
                    RemoveRandomPart();
                    return;
                }
                //Remove head, instakill
                RemoveHead();
                Death();
                break;
            case(2):
                //If the left arm is already removed, select a different part to remove
                if (!m_leftArmRemains)
                {
                    RemoveRandomPart();
                    return;
                }
                //Remove left arm
                RemoveLeftArm();
                break;
            case(3):
                //If the right arm is already removed, select a different part to remove
                if(!m_rightArmRemains)
                {
                    RemoveRandomPart();
                    return;
                }
                //Remove right arm
                RemoveRightArm();
                break;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if ((other.tag == "Weapon") && canTakeDamage && canDie)
            TakeDamage();
    }

    private void Death()
    {
        //Send a message to our parent room, letting it know we are dead
        if (m_parentRoom != null)
            m_parentRoom.SendMessage("DecrementSpawnCountRemainder");
        transform.GetComponent<CharacterController>().enabled = false;
        //We need to create a ragdoll which doesn't have the body parts which have already been removed during the zombies lifetime
        GameObject l_ragdoll = transform.GetComponent<ReplaceRagdoll>().ReplaceWithRagdoll();
        //Remove the body parts from the ragdoll which have been decapitated during the zombies lifetime
        if (!m_headRemains)
            l_ragdoll.SendMessage("RemoveHead");
        if (!m_leftArmRemains)
            l_ragdoll.SendMessage("RemoveLeftArm");
        if (!m_rightArmRemains)
            l_ragdoll.SendMessage("RemoveRightArm");
        //Calculate a force relative to the player position to apply to the ragdoll so it gets knocked away from the player
        Vector3 l_playerLocation = GameObject.FindGameObjectWithTag("Player").transform.position;
        Vector3 l_forceDirection = transform.position - l_playerLocation;
        //Apply the force
        l_ragdoll.transform.FindChild("Character1_Reference").GetComponent<Rigidbody>().AddForce(l_forceDirection * ForceMultiplier);
        //Now destroy this object as we have finished creating the ragdoll which replaces us
        GameObject.Destroy(this.gameObject);
    }

    private void RemoveHead()
    {
        //Disable my head and spawn a ragdoll version in its place
        m_headRemains = false;
        m_remainingBodyParts--;
        GameObject MyHead = transform.FindChild("Zombie_Head").gameObject;
        GameObject headPart = GameObject.Instantiate(HeadRagdoll, HeadBone.transform.position, HeadBone.transform.rotation) as GameObject;
        MyHead.SetActive(false);
        //Apply a force relative to the players position

    }

    private void RemoveLeftArm()
    {
        //Disable left arm and spawn ragdoll version
        m_leftArmRemains = false;
        m_remainingBodyParts--;
        GameObject MyLArm = transform.FindChild("Zombie_Leftarm").gameObject;
        GameObject LArmPart = GameObject.Instantiate(LeftArmRagdoll, MyLArm.transform.position, MyLArm.transform.rotation) as GameObject;
        MyLArm.SetActive(false);
        //Apply a force relative to the players position
    }

    private void RemoveRightArm()
    {
        m_rightArmRemains = false;
        m_remainingBodyParts--;
        GameObject MyRArm = transform.FindChild("Zombie_Rightarm").gameObject;
        GameObject RArmPart = GameObject.Instantiate(RightArmRagdoll, MyRArm.transform.position, MyRArm.transform.rotation) as GameObject;
        MyRArm.SetActive(false);
        //Apply a force relative to the players position
    }
}