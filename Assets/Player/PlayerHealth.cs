// ---------------------------------------------------------------------------
// PlayerHealth.cs
// 
// Recieves hurt messages from enemies, disaplys current health on the GUI
// 
// Original Author: Harley Laurie
// ---------------------------------------------------------------------------

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    private Text textDisplay;//Reference to gui object displaying players current health
    public int currentHealth;

    //We want a small cooldown between when the player can take damage
    private bool canBeHurt = true;
    private float lastTimeHurt;
    private float hurtCooldown = 0.25f;

    void Start()
    {
        //Initialise references and update display message to show current health
        textDisplay = GameObject.Find("DisplayMessage").GetComponent<Text>();
        UpdateDisplay();
    }

    void Update()
    {
        if(!canBeHurt)
        {
            lastTimeHurt -= Time.deltaTime;
            if (lastTimeHurt <= 0.0f)
                canBeHurt = true;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "EnemyWeapon")
            Hurt();
    }

    private void Hurt()
    {
        //Check if we are blocking
        if (transform.GetComponent<PlayerControllerNew>().IsBlocking())
            return; //Break out if we are, instead of taking damage
        if(canBeHurt)
        {
            //Decrement health amount
            currentHealth--;
            //Start taking damage cooldown
            canBeHurt = false;
            lastTimeHurt = hurtCooldown;
            if (currentHealth > 0)
                //If the player is still alive, update the display message to show how much HP is remaining
                UpdateDisplay();
            else
            //Otherwise display a message letting the player know they are dead
            {
                textDisplay.text = "Dead!";
            }
        }
    }

    private void UpdateDisplay()
    {
        string l_displayMessage = currentHealth + " HP";
        textDisplay.text = l_displayMessage;
    }
}