// ---------------------------------------------------------------------------
// PlayerControllerNew.cs
// 
// Movement and Combat controls for player character
// 
// Original Author: Harley Laurie
// ---------------------------------------------------------------------------

using UnityEngine;
using System.Collections;
using UnitySampleAssets.CrossPlatformInput; //Used to get virtual joystick axis input

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    //Build type influences player controls
    private BuildType._BuildType m_buildType;

    //Animation
    private Animator m_animator; //The players animator component

    //Controls disabled when health reaches 0
    private bool m_controlsDisabled = false;

    //Combat
    private BoxCollider m_weaponCollider;       //Collider that is attatched to the player weapon
    private bool m_isBlocking = false;   //Is the character currently blocking?
    private bool m_isBashing = false;    //Is the character currently shield bashing?
    private bool m_isAttacking = false;
    private int m_currentAttack = 0;

    //Movement
    private GameObject m_playerCamera = null;
    private bool m_canMove = true;
    private CharacterController m_controller;
    public float m_walkSpeed = 2.0f;     //Players speed when walking
    public float m_runSpeed = 6.0f;      //Players speed when running
    public float m_inAirControlAcceleration = 3.0f;
    public float m_gravity = 20.0f;     //Strength of gravity that effects the player character
    public float m_speedSmoothing = 10.0f;
    public float m_rotateSpeed = 500.0f;
    public float m_rotationDampening = 6.0f;
    private Vector3 m_moveDirection = Vector3.zero;
    private float m_verticalVelocity = 0.0f;
    private float m_movementVelocity = 0.0f;

    //Rolling / Sprinting
    private bool m_isRolling = false;
    private float m_rollCooldown = 1.167f;
    private float m_rollCooldownRemainder = 0.0f;
    private float m_sprintDelay = 0.2f;//Sprinting wont start until button has been held down for a short amount of time
    private float m_sprintDelayRemainder;
    private float m_sprintEndDelay;
    private bool m_isSprinting = false;
    private bool m_preSprinting = false;

    //Collision
    private CollisionFlags m_collisionFlags;

    //Mobile controls
    private bool m_mobileIsSprinting = false;
    private bool m_mobileIsAttacking = false;
    private bool m_disallowMobileAttackEnd = false;

    void Start()
    {
        //Get current build type
        m_buildType = GameObject.Find("System").GetComponent<BuildType>().m_buildType;

        //If we are in mobile build type, we need to tell the run and attack buttons to set up their
        //event triggers and reference them to this player character
        if(m_buildType == BuildType._BuildType.Android)
        {
            GameObject.Find("RunButton").SendMessage("SetUp", this.gameObject);
            GameObject.Find("AttackButton").SendMessage("SetUp", this.gameObject);
            GameObject.Find("BlockButton").SendMessage("SetUp", this.gameObject);
        }

        //Initialize reference variables
        m_animator = GetComponent<Animator>();
        m_controller = GetComponent<CharacterController>();
        m_moveDirection = transform.TransformDirection(Vector3.forward);
        m_weaponCollider = transform.FindChild("Hunter_Animated/Character1_Reference/Character1_Hips/Character1_Spine/Character1_Spine1/Character1_Spine2/Character1_RightShoulder/Character1_RightArm/Character1_RightForeArm/Character1_RightHand/Character1_RightHandMiddle1/sword").GetComponent<BoxCollider>();
    }

    void Update()
    {
        //Do nothing once controls have been disabled
        if (m_controlsDisabled)
            return;

        switch(m_buildType)
        {
            case(BuildType._BuildType.Console):
                ConsoleMovement();
                BlockAndBash();
                Attack();
                ApplyMovement();
                ResetCooldowns();
                break;
            case(BuildType._BuildType.PC):
                PCMovement();
                BlockAndBash();
                Attack();
                ApplyMovement();
                ResetCooldowns();
                break;
            case(BuildType._BuildType.Android):
                MobileMovement();
                MobileAttack();
                MobileEndAttack();
                ApplyMovement();
                ResetCooldowns();
                break;
        }
        
    }

    //Called by our player health function when our health reaches 0
    private void DisableControls()
    {
        m_controlsDisabled = true;
    }

    private void EnableWeaponCollider()
    {
        m_weaponCollider.enabled = true;
        GameObject.Find("System").GetComponent<CallBack>().CreateCallback(this.gameObject, "DisableWeaponCollider", 0.5f);
    }

    private void DisableWeaponCollider()
    {
        m_weaponCollider.enabled = false;
    }

    private void SetPlayerCamera(GameObject l_playerCamera)
    {
        m_playerCamera = l_playerCamera;
    }

    private void ResetCooldowns()
    {
        //Rolling needs cooldown to prevent attacking and rolling inside a roll
        if(m_isRolling)
        {
            m_rollCooldownRemainder -= Time.deltaTime;
            if(m_rollCooldownRemainder <= 0.0f)
            {
                m_isRolling = false;
                m_animator.SetBool("Rolling", false);
            }
        }
    }

    //Called by GUI buttons to turn sprinting on and off during mobile gameplay
    private void SetMobileSprinting(bool a_sprintValue)
    {
        m_mobileIsSprinting = a_sprintValue;
    }

    private void MobileMovement()
    {
        //Get right virtual joystick input for player look/aim direction
        float l_xLookInput = CrossPlatformInputManager.GetAxis("Joystick Look X");
        float l_yLookInput = CrossPlatformInputManager.GetAxis("Joystick Look Y");
        Vector3 l_lookVector = new Vector3(l_xLookInput, 0.0f, l_yLookInput);
        //Get the world position where the player should be looking, based on this input
        l_lookVector = transform.position + l_lookVector;
        transform.LookAt(l_lookVector);

        //Get left virtual joystick input for player movement
        float l_xInput = CrossPlatformInputManager.GetAxis("Joystick Move X");
        float l_yInput = CrossPlatformInputManager.GetAxis("Joystick Move Y");
        Vector3 l_movementVector = new Vector3(l_xInput, 0.0f, l_yInput);

        m_canMove = l_movementVector != Vector3.zero ? false : true;
        if (l_movementVector != Vector3.zero)
        {
            m_moveDirection = l_movementVector;
        }

        //Smooth the speed based on the current target direction
        float l_currentSmooth = m_speedSmoothing * Time.deltaTime;

        //Choose target speed
        //* We want to support analog input but make sure you cant walk faster diagonally than just forward or sideways
        float l_targetSpeed = Mathf.Min(l_movementVector.magnitude, 1.0f);

        if (m_mobileIsSprinting)
            l_targetSpeed *= m_runSpeed;
        else
            l_targetSpeed *= m_walkSpeed;

        m_movementVelocity = Mathf.Lerp(m_movementVelocity, l_targetSpeed, l_currentSmooth);

        //Pass movement speed to animator controller
        m_animator.SetFloat("Speed", m_movementVelocity);
    }

    //Callback to allow mobile attack stops
    private void AllowMobileAttackStop()
    {
        m_disallowMobileAttackEnd = false;
    }

    private void MobileAttackOff()
    {
        m_mobileIsAttacking = false;
    }

    private void MobileAttackOn()
    {
        m_mobileIsAttacking = true;
    }

    //Called by attack button on the UI during mobile gameplay
    private void MobileAttack()
    {
        //Cant attack while rolling
        if (m_isRolling)
            return;

        //Player cant perform normal attacks while blocking or shield bashing
        if (!m_animator.GetCurrentAnimatorStateInfo(2).IsName("Block") &&
            !m_animator.GetCurrentAnimatorStateInfo(2).IsName("ShieldBash"))
        {
            if (m_mobileIsAttacking && !m_isAttacking)
            {
                m_isAttacking = true;
                m_disallowMobileAttackEnd = true;
                GameObject.Find("System").GetComponent<CallBack>().CreateCallback(this.gameObject, "AllowMobileAttackStop", 0.1f);

                //Which attack are we going to perform?
                m_currentAttack = Random.Range(1, 3);

                if (m_currentAttack == 1)
                {
                    m_animator.SetBool("Attacking1", true);
                    GameObject.Find("System").GetComponent<CallBack>().CreateCallback(this.gameObject, "EnableWeaponCollider", 0.25f);
                }
                else
                {
                    m_animator.SetBool("Attacking2", true);
                    GameObject.Find("System").GetComponent<CallBack>().CreateCallback(this.gameObject, "EnableWeaponCollider", 0.25f);
                }
            }
        }
    }

    private void MobileEndAttack()
    {
        if (m_disallowMobileAttackEnd)
            return;

        if (m_isAttacking)
        {
            //Check for end of attack one
            if (m_currentAttack == 1 &&
                (!m_animator.GetCurrentAnimatorStateInfo(1).IsName("Attack1")))
            {
                m_isAttacking = false;
                m_animator.SetBool("Attacking1", false);
            }
            else if (m_currentAttack == 2 &&
                (!m_animator.GetCurrentAnimatorStateInfo(1).IsName("Attack2")))
            {
                m_isAttacking = false;
                m_animator.SetBool("Attacking2", false);
            }
        }
    }

    private void MobileShieldOn()
    {
        //Cant block or bash while rolling
        if (m_isRolling)
            return;

        m_isBlocking = true;
        m_animator.SetBool("Blocking", true);
    }

    private void MobileShieldOff()
    {
        m_isBlocking = false;
        m_animator.SetBool("Blocking", false);
    }

    private void PCMovement()
    {
        //Player should be facing mouse position, find that with a raycast
        Vector3 l_mousePos;
        Ray ray = m_playerCamera.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 1000.0f))
        {
            l_mousePos = hit.point;
            //Elevate the hit point to the same elevation as the player
            l_mousePos.y = 0.0f;
            //Face the player towards this point
            transform.LookAt(l_mousePos);
        }

        //Get WASD input for player movement
        float l_xInput = Input.GetKey(KeyCode.A) ? -1.0f : Input.GetKey(KeyCode.D) ? 1.0f : 0.0f;
        float l_yInput = Input.GetKey(KeyCode.S) ? -1.0f : Input.GetKey(KeyCode.W) ? 1.0f : 0.0f;
        Vector3 l_movementVector = new Vector3(l_xInput, 0.0f, l_yInput);

        m_canMove = l_movementVector != Vector3.zero ? false : true;
        if(l_movementVector != Vector3.zero)
        {
            m_moveDirection = l_movementVector;
        }

        //Smooth the speed based on the current target direction
        float l_currentSmooth = m_speedSmoothing * Time.deltaTime;

        //Choose target speed
        //* We want to support analog input but make sure you cant walk faster diagonally than just forward or sideways
        float l_targetSpeed = Mathf.Min(l_movementVector.magnitude, 1.0f);

        if (Input.GetKey(KeyCode.LeftShift))
        {
            if (!m_preSprinting)
            {
                m_preSprinting = true;
                m_sprintDelayRemainder = m_sprintDelay;
            }
            else
            {
                m_sprintDelayRemainder -= Time.deltaTime;
                if (m_sprintDelayRemainder <= 0.0f)
                {
                    m_isSprinting = true;
                    l_targetSpeed *= m_runSpeed;
                }
            }
        }
        else
        {
            if (m_isSprinting)
            {
                m_preSprinting = false;
                m_sprintEndDelay = m_sprintDelay;
                m_isSprinting = false;
            }
            m_sprintEndDelay -= Time.deltaTime;
            l_targetSpeed *= m_walkSpeed;
        }

        //Allow player to roll if they tap roll/sprint button
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            if (!m_isSprinting && (m_sprintEndDelay <= 0.0f))
            {
                m_animator.SetBool("Rolling", true);
                m_isRolling = true;
                m_rollCooldownRemainder = m_rollCooldown;
            }
        }

        m_movementVelocity = Mathf.Lerp(m_movementVelocity, l_targetSpeed, l_currentSmooth);

        //Pass movement speed to animator controller
        m_animator.SetFloat("Speed", m_movementVelocity);
    }

    private void ConsoleMovement()
    {
        //Allow movement only if the player is on the ground
        if (!IsGrounded())
            return;

        //Movement is relative to camera position
        Transform l_cameraTransform = m_playerCamera.transform;

        //Get forward vector which is relative to the camera along the x-z plane
        Vector3 l_forward = l_cameraTransform.TransformDirection(Vector3.forward);
        l_forward.y = 0;
        l_forward = l_forward.normalized;

        //Get right vector relative to the camera
        //Always orthogonal to the forward vector
        Vector3 l_right = new Vector3(l_forward.z, 0, -l_forward.x);

        //Get left thumbstick input
        float xInput = Input.GetAxis("GamePad Move X");
        float yInput = Input.GetAxis("GamePad Move Y");
        Vector3 l_movementVector = new Vector3(xInput, 0.0f, yInput);

        //Movement is relative to camera
        l_movementVector = m_playerCamera.transform.TransformDirection(l_movementVector);
        l_movementVector.y = 0.0f;

        //We store speed and direction seperately,
        //so that when the character stands still we still have a valid forward direction
        //m_moveDirection is always normalized, and we only update it if there is user input
        //Dont allow the player to move when they are turning
        if (l_movementVector != Vector3.zero)
        {
            m_canMove = false;
            //Smoothly turn towards our target direction
            m_moveDirection = Vector3.RotateTowards(m_moveDirection, l_movementVector, m_rotateSpeed * Mathf.Deg2Rad * Time.deltaTime, 100.0f);
            m_moveDirection = m_moveDirection.normalized;
        }
        else
            m_canMove = true;

        //Smooth the speed based on the current target direction
        float l_currentSmooth = m_speedSmoothing * Time.deltaTime;

        //Choose target speed
        //* We want to support analog input but make sure you cant walk faster diagonally than just forward or sideways
        float l_targetSpeed = Mathf.Min(l_movementVector.magnitude, 1.0f);

        //Modify the speed based on the player holding the sprint button
        if (Input.GetButton("Roll Sprint"))
        {
            if(!m_preSprinting)
            {
                m_preSprinting = true;
                m_sprintDelayRemainder = m_sprintDelay;
            }
            else
            {
                m_sprintDelayRemainder -= Time.deltaTime;
                if(m_sprintDelayRemainder <= 0.0f)
                {
                    m_isSprinting = true;
                    l_targetSpeed *= m_runSpeed;
                }
            }
        }
        else
        {
            if(m_isSprinting)
            {
                m_preSprinting = false;
                m_sprintEndDelay = m_sprintDelay;
                m_isSprinting = false;
            }
            m_sprintEndDelay -= Time.deltaTime;
            l_targetSpeed *= m_walkSpeed;
        }

        //Allow player to roll if they tap roll/sprint button
        if (Input.GetButtonUp("Roll Sprint"))
        {
            if (!m_isSprinting && (m_sprintEndDelay <= 0.0f))
            {
                m_animator.SetBool("Rolling", true);
                m_isRolling = true;
                m_rollCooldownRemainder = m_rollCooldown;
            }
        }

        m_movementVelocity = Mathf.Lerp(m_movementVelocity, l_targetSpeed, l_currentSmooth);

        //Pass movement speed to animator controller
        m_animator.SetFloat("Speed", m_movementVelocity);
    }

    private void BlockAndBash()
    {
        //Cant block or bash while rolling
        if (m_isRolling)
            return;

        //Blocking / Shield Bashing
        //if (Input.GetKey(KeyCode.LeftControl) || Input.GetButton("Block"))
        if(Input.GetMouseButton(1))
        {
            m_isBlocking = true;
            m_animator.SetBool("Blocking", true);
        }
        else
        {
            m_isBlocking = false;
            m_animator.SetBool("Blocking", false);
        }

        //Check if the bash animation has finished playing
        if (!m_animator.GetCurrentAnimatorStateInfo(2).IsName("ShieldBash"))
            m_animator.SetBool("Bashing", false);
    }

    private void Attack()
    {
        //Cant attack while rolling
        if (m_isRolling)
            return;

        //Player cant perform normal attacks while blocking or shield bashing
        if (!m_animator.GetCurrentAnimatorStateInfo(2).IsName("Block") &&
            !m_animator.GetCurrentAnimatorStateInfo(2).IsName("ShieldBash"))
        {
            //if (Input.GetButton("Light Attack") || Input.GetMouseButton(0))
            if (Input.GetMouseButton(0) && !m_isAttacking)
            {
                m_isAttacking = true;

                //Which attack are we going to perform?
                m_currentAttack = Random.Range(1, 3);

                if (m_currentAttack == 1)
                {
                    m_animator.SetBool("Attacking1", true);
                    GameObject.Find("System").GetComponent<CallBack>().CreateCallback(this.gameObject, "EnableWeaponCollider", 0.25f);
                }
                else
                {
                    m_animator.SetBool("Attacking2", true);
                    GameObject.Find("System").GetComponent<CallBack>().CreateCallback(this.gameObject, "EnableWeaponCollider", 0.25f);
                }
            }
            else if (m_isAttacking)
            {
                //Check for end of attack one
                if(m_currentAttack == 1 &&
                    (!m_animator.GetCurrentAnimatorStateInfo(1).IsName("Attack1")))
                {
                    m_isAttacking = false;
                    m_animator.SetBool("Attacking1", false);
                }
                else if (m_currentAttack == 2 &&
                    (!m_animator.GetCurrentAnimatorStateInfo(1).IsName("Attack2")))
                {
                    m_isAttacking = false;
                    m_animator.SetBool("Attacking2", false);
                }
            }
        }
    }

    private void ApplyMovement()
    {
        //Calculate movement vector
        Vector3 l_movement = m_moveDirection * m_movementVelocity + new Vector3(0, m_verticalVelocity, 0);
        l_movement *= Time.deltaTime;
        //Move the character controller
        m_collisionFlags = m_controller.Move(l_movement);
    }

    private bool IsGrounded()
    {
        return (m_collisionFlags & CollisionFlags.CollidedBelow) != 0;
    }

    public bool IsBlocking()
    {
        return m_isBlocking;
    }
}