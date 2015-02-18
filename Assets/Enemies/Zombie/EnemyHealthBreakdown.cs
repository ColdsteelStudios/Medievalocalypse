// ---------------------------------------------------------------------------
// EnemyHealthBreakdown.cs
// 
// Keeps track of health, replaces with body parts with ragdolls as health
// approaches zero
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

    //Bones used to spawn the ragdoll parts in correct position
    public GameObject HeadBone;
    public GameObject LeftArmBone;
    public GameObject RightArmBone;

    public int health = 1;
    public int ForceMultiplier = 8000;

    //Spawns blood splatte when damage is taken
    public GameObject m_bloodSplatterPrefab;

    //Stop enemies taking damage multiple times from one weapon swing
    private float damageCooldown = 0.25f;
    private float damageCooldownRemaining = 0.0f;
    private bool canTakeDamage = true;

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

    public void CanDie()
    {
        canDie = true;
    }

    private void TakeDamage()
    {
        health--;
        SendMessage("Blocked");
        damageCooldownRemaining = damageCooldown;
        canTakeDamage = false;
        if (health <= 0)
            Death();
    }

    void OnTriggerEnter(Collider other)
    {
        if ((other.tag == "Weapon") && canTakeDamage && canDie)
            BodyPartHit("");
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

    private void BodyPartHit(string bodyPartName)
    {
        if(canTakeDamage)
        {
            TakeDamage();
            if (health == 8)
                RemoveLeftArm();
            if (health == 6)
                RemoveRightArm();
            if (health == 4)
                RemoveHead();
        }
    }

    private void RemoveHead()
    {
        //Disable my head and spawn a ragdoll version in its place
        GameObject MyHead = transform.FindChild("Zombie_Head").gameObject;
        GameObject headPart = GameObject.Instantiate(HeadRagdoll, HeadBone.transform.position, HeadBone.transform.rotation) as GameObject;
        MyHead.SetActive(false);
        //Apply a force relative to the players position

    }

    private void RemoveLeftArm()
    {
        //Disable left arm and spawn ragdoll version
        GameObject MyLArm = transform.FindChild("Zombie_Leftarm").gameObject;
        GameObject LArmPart = GameObject.Instantiate(LeftArmRagdoll, MyLArm.transform.position, MyLArm.transform.rotation) as GameObject;
        MyLArm.SetActive(false);
    }

    private void RemoveRightArm()
    {
        GameObject MyRArm = transform.FindChild("Zombie_Rightarm").gameObject;
        GameObject RArmPart = GameObject.Instantiate(RightArmRagdoll, MyRArm.transform.position, MyRArm.transform.rotation) as GameObject;
        MyRArm.SetActive(false);
    }
}