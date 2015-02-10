// ---------------------------------------------------------------------------
// CamFollowX.cs
// 
// Class description goes here
// 
// Original Author: Harley Laurie
// ---------------------------------------------------------------------------

using UnityEngine;
using System.Collections;

public class CamFollowX : MonoBehaviour 
{
    private GameObject PC; //Player character reference

    private float ZDistance;
    private float YDistance;

    void Start()
    {
        //Initialise references
        PC = GameObject.FindGameObjectWithTag("Player");
        ZDistance = PC.transform.position.z - transform.position.z;
        YDistance = PC.transform.position.y - transform.position.y;
    }

    void Update()
    {
        //Store position locally
        Vector3 P = transform.position;
        //Change X pos to match player
        P.x = PC.transform.position.x;
        P.z = PC.transform.position.z - ZDistance;
        P.y = PC.transform.position.y - YDistance;
        //Update pos
        transform.position = Vector3.Lerp(transform.position, P, Time.deltaTime);
    }
}