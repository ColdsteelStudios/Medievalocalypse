// ---------------------------------------------------------------------------
// HealthDisplayControl.cs
// 
// Functions in here called by player health display, changes the sprites
// which are display to indicate how much health the player has left
// 
// Original Author: Harley Laurie
// ---------------------------------------------------------------------------

using UnityEngine;
using System.Collections;

public class HealthDisplayControl : MonoBehaviour
{
    public GameObject[] m_heartImages;

    public void SetHealthDisplay(int a_health)
    {
        int iter = (int)((float)a_health / 2.0f);
        if (((float)a_health / (float)iter) > 2.0f)
            m_heartImages[iter].SendMessage("SetHalfHeart");
        else
            m_heartImages[iter].SendMessage("SetEmptyHeart");
    }
}