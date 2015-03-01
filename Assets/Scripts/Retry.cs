// ---------------------------------------------------------------------------
// Retry.cs
// 
// Allows players to retry and quit when they die
// 
// Original Author: Harley Laurie
// ---------------------------------------------------------------------------

using UnityEngine;
using System.Collections;

public class Retry : MonoBehaviour
{
	public void TryAgain()
    {
        Application.LoadLevel(0);
    }

    public void Quit()
    {
        Application.Quit();
    }
}