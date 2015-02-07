// ---------------------------------------------------------------------------
// ZombieControl.cs
// 
// This is by far the worst code I have ever written
// 
// Original Author: Harley Laurie
// ---------------------------------------------------------------------------

using UnityEngine;
using System.Collections;

public class ZombieControl : MonoBehaviour 
{
    public GameObject ragdollPrefab;

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Weapon")
            rag();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Weapon")
            rag();
    }

    private void rag()
    {
        GameObject RD = GameObject.Instantiate(ragdollPrefab, transform.position, transform.rotation) as GameObject;
        transform.GetComponent<CapsuleCollider>().enabled = false;
        transform.GetComponent<Rigidbody>().active = false;

        RD.transform.FindChild("Root").transform.position = transform.FindChild("Root").transform.position;
        RD.transform.FindChild("Root").transform.rotation = transform.FindChild("Root").transform.rotation;

        RD.transform.FindChild("Root/Hips").transform.position = transform.FindChild("Root/Hips").transform.position;
        RD.transform.FindChild("Root/Hips").transform.position = transform.FindChild("Root/Hips").transform.position;

        RD.transform.FindChild("Root/Hips/LeftHips").transform.position = transform.FindChild("Root/Hips/LeftHips").transform.position;
        RD.transform.FindChild("Root/Hips/LeftHips").transform.position = transform.FindChild("Root/Hips/LeftHips").transform.position;

        RD.transform.FindChild("Root/Hips/LeftHips/LeftKnee").transform.position = transform.FindChild("Root/Hips/LeftHips/LeftKnee").transform.position;
        RD.transform.FindChild("Root/Hips/LeftHips/LeftKnee").transform.position = transform.FindChild("Root/Hips/LeftHips/LeftKnee").transform.position;

        RD.transform.FindChild("Root/Hips/LeftHips/LeftKnee/LeftAnkle").transform.position = transform.FindChild("Root/Hips/LeftHips/LeftKnee/LeftAnkle").transform.position;
        RD.transform.FindChild("Root/Hips/LeftHips/LeftKnee/LeftAnkle").transform.position = transform.FindChild("Root/Hips/LeftHips/LeftKnee/LeftAnkle").transform.position;

        RD.transform.FindChild("Root/Hips/LeftHips/LeftKnee/LeftAnkle/LeftFoot").transform.position = transform.FindChild("Root/Hips/LeftHips/LeftKnee/LeftAnkle/LeftFoot").transform.position;
        RD.transform.FindChild("Root/Hips/LeftHips/LeftKnee/LeftAnkle/LeftFoot").transform.position = transform.FindChild("Root/Hips/LeftHips/LeftKnee/LeftAnkle/LeftFoot").transform.position;

        RD.transform.FindChild("Root/Hips/RightHips").transform.position = transform.FindChild("Root/Hips/RightHips").transform.position;
        RD.transform.FindChild("Root/Hips/RightHips").transform.position = transform.FindChild("Root/Hips/RightHips").transform.position;

        RD.transform.FindChild("Root/Hips/RightHips/RightKnee").transform.position = transform.FindChild("Root/Hips/RightHips/RightKnee").transform.position;
        RD.transform.FindChild("Root/Hips/RightHips/RightKnee").transform.position = transform.FindChild("Root/Hips/RightHips/RightKnee").transform.position;

        RD.transform.FindChild("Root/Hips/RightHips/RightKnee/RightAnkle").transform.position = transform.FindChild("Root/Hips/RightHips/RightKnee/RightAnkle").transform.position;
        RD.transform.FindChild("Root/Hips/RightHips/RightKnee/RightAnkle").transform.position = transform.FindChild("Root/Hips/RightHips/RightKnee/RightAnkle").transform.position;

        RD.transform.FindChild("Root/Hips/RightHips/RightKnee/RightAnkle/RightFoot").transform.position = transform.FindChild("Root/Hips/RightHips/RightKnee/RightAnkle/RightFoot").transform.position;
        RD.transform.FindChild("Root/Hips/RightHips/RightKnee/RightAnkle/RightFoot").transform.position = transform.FindChild("Root/Hips/RightHips/RightKnee/RightAnkle/RightFoot").transform.position;

        RD.transform.FindChild("Root/Hips/LowerSpine").transform.position = transform.FindChild("Root/Hips/LowerSpine").transform.position;
        RD.transform.FindChild("Root/Hips/LowerSpine").transform.position = transform.FindChild("Root/Hips/LowerSpine").transform.position;

        RD.transform.FindChild("Root/Hips/LowerSpine/MiddleSpine").transform.position = transform.FindChild("Root/Hips/LowerSpine/MiddleSpine").transform.position;
        RD.transform.FindChild("Root/Hips/LowerSpine/MiddleSpine").transform.position = transform.FindChild("Root/Hips/LowerSpine/MiddleSpine").transform.position;

        RD.transform.FindChild("Root/Hips/LowerSpine/MiddleSpine/UpperSpine").transform.position = transform.FindChild("Root/Hips/LowerSpine/MiddleSpine/UpperSpine").transform.position;
        RD.transform.FindChild("Root/Hips/LowerSpine/MiddleSpine/UpperSpine").transform.position = transform.FindChild("Root/Hips/LowerSpine/MiddleSpine/UpperSpine").transform.position;

        RD.transform.FindChild("Root/Hips/LowerSpine/MiddleSpine/UpperSpine/LeftShoulder").transform.position = transform.FindChild("Root/Hips/LowerSpine/MiddleSpine/UpperSpine/LeftShoulder").transform.position;
        RD.transform.FindChild("Root/Hips/LowerSpine/MiddleSpine/UpperSpine/LeftShoulder").transform.position = transform.FindChild("Root/Hips/LowerSpine/MiddleSpine/UpperSpine/LeftShoulder").transform.position;

        RD.transform.FindChild("Root/Hips/LowerSpine/MiddleSpine/UpperSpine/LeftShoulder/LeftArm").transform.position = transform.FindChild("Root/Hips/LowerSpine/MiddleSpine/UpperSpine/LeftShoulder/LeftArm").transform.position;
        RD.transform.FindChild("Root/Hips/LowerSpine/MiddleSpine/UpperSpine/LeftShoulder/LeftArm").transform.position = transform.FindChild("Root/Hips/LowerSpine/MiddleSpine/UpperSpine/LeftShoulder/LeftArm").transform.position;

        RD.transform.FindChild("Root/Hips/LowerSpine/MiddleSpine/UpperSpine/LeftShoulder/LeftArm/LeftElbow").transform.position = transform.FindChild("Root/Hips/LowerSpine/MiddleSpine/UpperSpine/LeftShoulder/LeftArm/LeftElbow").transform.position;
        RD.transform.FindChild("Root/Hips/LowerSpine/MiddleSpine/UpperSpine/LeftShoulder/LeftArm/LeftElbow").transform.position = transform.FindChild("Root/Hips/LowerSpine/MiddleSpine/UpperSpine/LeftShoulder/LeftArm/LeftElbow").transform.position;

        RD.transform.FindChild("Root/Hips/LowerSpine/MiddleSpine/UpperSpine/LeftShoulder/LeftArm/LeftElbow/LeftWrist").transform.position = transform.FindChild("Root/Hips/LowerSpine/MiddleSpine/UpperSpine/LeftShoulder/LeftArm/LeftElbow/LeftWrist").transform.position;
        RD.transform.FindChild("Root/Hips/LowerSpine/MiddleSpine/UpperSpine/LeftShoulder/LeftArm/LeftElbow/LeftWrist").transform.position = transform.FindChild("Root/Hips/LowerSpine/MiddleSpine/UpperSpine/LeftShoulder/LeftArm/LeftElbow/LeftWrist").transform.position;

        RD.transform.FindChild("Root/Hips/LowerSpine/MiddleSpine/UpperSpine/LeftShoulder/LeftArm/LeftElbow/LeftWrist/LeftHand").transform.position = transform.FindChild("Root/Hips/LowerSpine/MiddleSpine/UpperSpine/LeftShoulder/LeftArm/LeftElbow/LeftWrist/LeftHand").transform.position;
        RD.transform.FindChild("Root/Hips/LowerSpine/MiddleSpine/UpperSpine/LeftShoulder/LeftArm/LeftElbow/LeftWrist/LeftHand").transform.position = transform.FindChild("Root/Hips/LowerSpine/MiddleSpine/UpperSpine/LeftShoulder/LeftArm/LeftElbow/LeftWrist/LeftHand").transform.position;

        RD.transform.FindChild("Root/Hips/LowerSpine/MiddleSpine/UpperSpine/Neck").transform.position = transform.FindChild("Root/Hips/LowerSpine/MiddleSpine/UpperSpine/Neck").transform.position;
        RD.transform.FindChild("Root/Hips/LowerSpine/MiddleSpine/UpperSpine/Neck").transform.position = transform.FindChild("Root/Hips/LowerSpine/MiddleSpine/UpperSpine/Neck").transform.position;

        RD.transform.FindChild("Root/Hips/LowerSpine/MiddleSpine/UpperSpine/Neck/Head").transform.position = transform.FindChild("Root/Hips/LowerSpine/MiddleSpine/UpperSpine/Neck/Head").transform.position;
        RD.transform.FindChild("Root/Hips/LowerSpine/MiddleSpine/UpperSpine/Neck/Head").transform.position = transform.FindChild("Root/Hips/LowerSpine/MiddleSpine/UpperSpine/Neck/Head").transform.position;

        RD.transform.FindChild("Root/Hips/LowerSpine/MiddleSpine/UpperSpine/RightShoulder").transform.position = transform.FindChild("Root/Hips/LowerSpine/MiddleSpine/UpperSpine/RightShoulder").transform.position;
        RD.transform.FindChild("Root/Hips/LowerSpine/MiddleSpine/UpperSpine/RightShoulder").transform.position = transform.FindChild("Root/Hips/LowerSpine/MiddleSpine/UpperSpine/RightShoulder").transform.position;

        RD.transform.FindChild("Root/Hips/LowerSpine/MiddleSpine/UpperSpine/RightShoulder/RightArm").transform.position = transform.FindChild("Root/Hips/LowerSpine/MiddleSpine/UpperSpine/RightShoulder/RightArm").transform.position;
        RD.transform.FindChild("Root/Hips/LowerSpine/MiddleSpine/UpperSpine/RightShoulder/RightArm").transform.position = transform.FindChild("Root/Hips/LowerSpine/MiddleSpine/UpperSpine/RightShoulder/RightArm").transform.position;

        RD.transform.FindChild("Root/Hips/LowerSpine/MiddleSpine/UpperSpine/RightShoulder/RightArm/RightElbow").transform.position = transform.FindChild("Root/Hips/LowerSpine/MiddleSpine/UpperSpine/RightShoulder/RightArm/RightElbow").transform.position;
        RD.transform.FindChild("Root/Hips/LowerSpine/MiddleSpine/UpperSpine/RightShoulder/RightArm/RightElbow").transform.position = transform.FindChild("Root/Hips/LowerSpine/MiddleSpine/UpperSpine/RightShoulder/RightArm/RightElbow").transform.position;

        RD.transform.FindChild("Root/Hips/LowerSpine/MiddleSpine/UpperSpine/RightShoulder/RightArm/RightElbow/RightWrist").transform.position = transform.FindChild("Root/Hips/LowerSpine/MiddleSpine/UpperSpine/RightShoulder/RightArm/RightElbow/RightWrist").transform.position;
        RD.transform.FindChild("Root/Hips/LowerSpine/MiddleSpine/UpperSpine/RightShoulder/RightArm/RightElbow/RightWrist").transform.position = transform.FindChild("Root/Hips/LowerSpine/MiddleSpine/UpperSpine/RightShoulder/RightArm/RightElbow/RightWrist").transform.position;

        RD.transform.FindChild("Root/Hips/LowerSpine/MiddleSpine/UpperSpine/RightShoulder/RightArm/RightElbow/RightWrist/RightHand").transform.position = transform.FindChild("Root/Hips/LowerSpine/MiddleSpine/UpperSpine/RightShoulder/RightArm/RightElbow/RightWrist/RightHand").transform.position;
        RD.transform.FindChild("Root/Hips/LowerSpine/MiddleSpine/UpperSpine/RightShoulder/RightArm/RightElbow/RightWrist/RightHand").transform.position = transform.FindChild("Root/Hips/LowerSpine/MiddleSpine/UpperSpine/RightShoulder/RightArm/RightElbow/RightWrist/RightHand").transform.position;

        //Apply force to push the zombie away from the player
        Vector3 dir = GameObject.FindGameObjectWithTag("Player").transform.position + transform.position;
        RD.transform.FindChild("Root").GetComponent<Rigidbody>().AddForce(dir.normalized * 5000.0f);

        GameObject.Destroy(this.gameObject);
    }
}