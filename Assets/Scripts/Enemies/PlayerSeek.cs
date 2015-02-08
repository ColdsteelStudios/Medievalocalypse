// ---------------------------------------------------------------------------
// PlayerSeekl.cs
// 
// Seeks players location through the navmesh if they are close enough
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

    private GameObject PC; //Reference to player character
    private NavMeshAgent NMA; //Reference to our NavMeshAgent component
    private Animator Anim; //Reference to our animator

    void Start()
    {
        //Initialize references
        PC = GameObject.FindGameObjectWithTag("Player");
        NMA = transform.GetComponent<NavMeshAgent>();
        Anim = transform.GetComponent<Animator>();
    }

    void Update()
    {
        //Calculate distance to the PC
        float D = Vector3.Distance(transform.position, PC.transform.position);
        //Update animation controller with our velocity
        Anim.SetFloat("Speed", Vector3.Magnitude(NMA.velocity));

        if (!isFollowing)
        {//Check if we are close enough to start following
            if(D<=followStartDistance)
            {
                isFollowing = true;
                NMA.SetDestination(PC.transform.position);
                UpdateDestination = 0.25f;
            }
        }
        else
        {//Already following
            //Check if we are too far to continue following
            if(D>=followStopDistance)
            {
                isFollowing = false;
                NMA.Stop();
            }
            else
            {//Keep following
                UpdateDestination -= Time.deltaTime;
                if(UpdateDestination <= 0.0f)
                {
                    NMA.SetDestination(PC.transform.position);
                    UpdateDestination = 0.25f;
                }
            }
        }
    }
}