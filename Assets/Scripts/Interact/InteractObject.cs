// ---------------------------------------------------------------------------
// InteractObject.cs
// 
// Allows player to interact with this object, displays a message to screen
// giving them instructions on what this object will do
// 
// Original Author: Harley Laurie
// ---------------------------------------------------------------------------

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class InteractObject : MonoBehaviour 
{
    public string displayMessage; //Message to display on screen when player is in range of interact object
    public KeyCode keyboardInputTrigger; //Key name which will trigger this object when pressed
    public string buttonInputTrigger; //Controller button name which will trigger this object when pressed
    public float triggerRange = 1.0f; //How close the player needs to be in order to interact with this object

    public GameObject triggerTarget; //The gameobject to interact with when the trigger is activated
    public string triggerName; //The name of the function to call on the triggerTarget when this object is interacted with

    private GameObject pc; //player character reference
    private Text displayTest; //UI Text used for displaying the displayMessage

    void Start()
    {
        //Initialize references
        pc = GameObject.FindGameObjectWithTag("Player");
        displayTest = GameObject.Find("InteractMessage").GetComponent<Text>();
    }

    void Update()
    {
        //Calculate range from trigger to player character
        float D = Vector3.Distance(transform.position, pc.transform.position);

        if (D < triggerRange)
        {//If the player is in range, we will allow them to interact with this object
            //Display the message
            displayTest.text = displayMessage;

            //Activate the object when the player interacts with it
            if (Input.GetKeyDown(keyboardInputTrigger))
                triggerTarget.SendMessage(triggerName);
        }
        //Remove display message if players out of range
        else
            displayTest.text = "";

    }
}