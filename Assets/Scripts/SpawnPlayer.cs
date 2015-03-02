// ---------------------------------------------------------------------------
// SpawnPlayer.cs
// 
// Spawns player and camera into the scene and sets them up correctly
// Mainly used for the test chamber
// 
// Original Author: Harley Laurie
// ---------------------------------------------------------------------------

using UnityEngine;
using System.Collections;

public class SpawnPlayer : MonoBehaviour
{
    public GameObject m_playerPrefab;
    public GameObject m_cameraPrefab;

    void Start()
    {
        Vector3 SpawnPos = new Vector3(0, 3, 0);
        GameObject Player = GameObject.Instantiate(m_playerPrefab, SpawnPos, Quaternion.identity) as GameObject;
        GameObject l_playerCamera = GameObject.Instantiate(m_cameraPrefab, SpawnPos, Quaternion.identity) as GameObject;
        Player.SendMessage("SetPlayerCamera", l_playerCamera);
        l_playerCamera.GetComponent<CameraFollow>().SetFollowTarget(Player, new Vector3(0.0f, -7.1f, 3.3f), new Quaternion(0.5f, 0.0f, 0.0f, 0.9f));
    }
}