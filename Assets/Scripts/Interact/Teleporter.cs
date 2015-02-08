// ---------------------------------------------------------------------------
// Teleporter.cs
// 
// Teleporters the player character to various areas in the testing area
// 
// Original Author: Harley Laurie
// ---------------------------------------------------------------------------

using UnityEngine;
using System.Collections;

public class Teleporter : MonoBehaviour
{
    //A list of locations where we can teleport the player character
    public GameObject[] teleportLocations;

    //Teleports the teleportTarget to the location of targetLocation in teleportLocations array
    public void Teleport(GameObject teleportTarget, int targetLocation)
    {
        teleportTarget.transform.position = teleportLocations[targetLocation].transform.position;
    }

    //Teleports the player to the pathfinding area
    public void TeleportPathfinding()
    {
        GameObject.FindGameObjectWithTag("Player").transform.position = teleportLocations[1].transform.position;
    }
}