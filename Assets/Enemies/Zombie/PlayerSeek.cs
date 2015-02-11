// ---------------------------------------------------------------------------
// PlayerSeekl.cs
// 
// Seeks players location through the navmesh if they are close enough
// Attacks them when in range
// 
// Original Author: Harley Laurie
// ---------------------------------------------------------------------------

using UnityEngine;
using System.Collections;

public class PlayerSeek : MonoBehaviour 
{
    public float followStartDistance = 10.0f; //Will start following player if they are within this distance
    public float followStopDistance = 15.0f; //Will stop following player if they get this far away
    private bool isFollowing = false; //Are we following the PC right now?
    private float UpdateDestination; //When following, we want to update our destination every 0.25 seconds

    private bool BirthingComplete = false;
    private float BirthingDuration = 5.875f / 3.0f;

    //Combat
    public float attackRange = 2.0f;//How close we must be to the player before we try to attack
    private bool isAttacking = false;
    private float attackDuration = 1.208f;//Length of our attack animation
    private float attackCooldown = 0.0f;
    private BoxCollider LeftHandCollider;//Colliders for our hands which detect when we hit the player with an attack
    private BoxCollider RightHandCollider;

    //Stumbling back when player blocks
    private float stumbleAnimationTime = 0.67f;
    private float stumbleAnimationRemaining = 0.0f;
    private bool isStumbling = false;

    private GameObject PC; //Reference to player character
    private NavMeshAgent NMA; //Reference to our NavMeshAgent component
    private Animator Anim; //Reference to our animator

    void Start()
    {
        //Initialize references
        PC = GameObject.FindGameObjectWithTag("Player");//Player character
        NMA = transform.GetComponent<NavMeshAgent>();//Our nav mesh agent
        Anim = transform.GetComponent<Animator>();//Our animator controller
        LeftHandCollider = transform.Find("Character1_Reference/Character1_Hips/Character1_Spine/Character1_Spine1/Character1_Spine2/Character1_LeftShoulder/Character1_LeftArm/Character1_LeftForeArm/Character1_LeftHand/Character1_LeftHandMiddle1").GetComponent<BoxCollider>();
        RightHandCollider = transform.Find("Character1_Reference/Character1_Hips/Character1_Spine/Character1_Spine1/Character1_Spine2/Character1_RightShoulder/Character1_RightArm/Character1_RightForeArm/Character1_RightHand/Character1_RightHandMiddle1").GetComponent<BoxCollider>();

    }

    void Update()
    {
        //Reduce attack cooldown
        if (attackCooldown > 0.0f)
            attackCooldown -= Time.deltaTime;

        //Time the ending of the stumble animation if an attack was blocked by the player
        if (isStumbling)
        {
            stumbleAnimationRemaining -= Time.deltaTime;
            if(stumbleAnimationRemaining<=0.0f)
            {
                isStumbling = false;
                GetComponent<Animator>().SetBool("Blocked", false);
            }
        }

        //Zombie spawn underground then crawl out
        //If the crawling out(birthing) animation is not complete
        //then we keep playing that
        if(!BirthingComplete)
        {
            BirthingDuration -= Time.deltaTime;
            if (BirthingDuration <= 0.0f)
            {
                //Once we have completed birthing then allow the zombie to be killed
                BirthingComplete = true;
                transform.SendMessage("CanDie");
            }
        }
        else
        {
            //Calculate distance to the PC
            float D = Vector3.Distance(transform.position, PC.transform.position);

            //Update animation controller with our velocity
            Anim.SetFloat("Speed", Vector3.Magnitude(NMA.velocity));

            TryAttack(D);

            //Time how long the zombie is attacking for
            if(isAttacking)
            {
                attackDuration -= Time.deltaTime;
                //Once the attacking animation has finished the attack is over
                if(attackDuration<=0.0f)
                {
                    attackDuration = 1.208f;
                    isAttacking = false;
                    //Turn off our attacking animation
                    Anim.SetBool("Attacking", false);
                    //Turn off our hands box colliders
                    LeftHandCollider.enabled = false;
                    RightHandCollider.enabled = false;
                }
            }

            if ((!isFollowing) && (!isAttacking))
            {//Check if we are close enough to start following
                if (D <= followStartDistance)
                {
                    isFollowing = true;
                    NMA.SetDestination(PC.transform.position);
                    UpdateDestination = 0.25f;
                }
            }
            else
            {//Already following
             //Check if we are too far to continue following
                if (D >= followStopDistance)
                {
                    isFollowing = false;
                    NMA.Stop();
                }
                else
                {//Keep following
                    UpdateDestination -= Time.deltaTime;
                    if (UpdateDestination <= 0.0f)
                    {
                        NMA.SetDestination(PC.transform.position);
                        UpdateDestination = 0.25f;
                    }
                }
            }
        }
    }

    private void TryAttack(float a_fPlayerDistance)
    {
        //Break out of attack if stumbling back
        if(isStumbling)
            return;

        //If we arent already attacking and we are in range then we attack
        if (!isAttacking && 
            a_fPlayerDistance <= attackRange && 
            attackCooldown <= 0.0f)
        {
            attackCooldown = 1.5f;
            isAttacking = true;
            //Start playing the attacking animation
            Anim.SetBool("Attacking", true);
            //Stop moving through the navmesh
            //NMA.Stop();
            //Turn on our hands box colliders
            LeftHandCollider.enabled = true;
            RightHandCollider.enabled = true;
        }
    }

    //Sent message from the player when they block our attack
    private void Blocked()
    {
        GetComponent<Animator>().SetBool("Blocked", true);
        isStumbling = true;
        stumbleAnimationRemaining = stumbleAnimationTime;
    }
}