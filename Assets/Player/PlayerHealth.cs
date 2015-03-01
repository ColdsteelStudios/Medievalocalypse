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
    private float hurtCooldown = 1.0f;

    //Sound effects
    public AudioClip m_hurtSound;//Sound to play when player is hurt
    public AudioClip m_blockSound;//when player blocks an enemy attack

    //Retry screen, disable this immediatly when game starts, then reactivate
    //it when the player dies
    private GameObject m_retryScreen;

    void Start()
    {
        m_retryScreen = GameObject.Find("RetryScreen");
        m_retryScreen.SetActive(false);
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
            Hurt(other);
    }

    private void Hurt(Collider other)
    {
        if (currentHealth == 0)
            return;
        //Check if we are blocking
        if (transform.GetComponent<PlayerController>().IsBlocking())
        {
            //Play block sound
            GetComponent<AudioSource>().PlayOneShot(m_blockSound);
            //Send a message to the enemy saying we blocked the attack
            other.transform.root.SendMessage("Blocked");
            return; //Break out if we blocked, instead of taking damage
        }

        if (canBeHurt)
        {
            //Play the hurt sound
            GetComponent<AudioSource>().PlayOneShot(m_hurtSound);
            //Decrement health amount
            currentHealth--;
            //Start taking damage cooldown
            canBeHurt = false;
            lastTimeHurt = hurtCooldown;
            GameObject.Find("HealthDisplay").SendMessage("SetHealthDisplay", currentHealth);
            //If health has reached zero activate the retry screen and disable controls
            if(currentHealth == 0)
            {
                m_retryScreen.SetActive(true);
                transform.SendMessage("DisableControls");
            }
        }
    }
}