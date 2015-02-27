// ---------------------------------------------------------------------------
// PlayerControllerNew.cs
// 
// Movement and Combat controls for player character
// 
// Original Author: Harley Laurie
// ---------------------------------------------------------------------------

using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    //Animation
    private Animator m_animator; //The players animator component

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
    private Vector3 m_moveDirection = Vector3.zero;
    private float m_verticalVelocity = 0.0f;
    private float m_movementVelocity = 0.0f;

    //Rolling
    private bool m_isRolling = false;
    private float m_rollCooldown = 1.167f;
    private float m_rollCooldownRemainder = 0.0f;
    //Sprint Delay - sprinting wont start until button has been held down for a short amount of time
    private float m_sprintDelay = 0.2f;
    private float m_sprintDelayRemainder;
    private float m_sprintEndDelay;
    private bool m_isSprinting = false;
    private bool m_preSprinting = false;

    //Rotation
    private Quaternion m_previousRotation;

    //Collision
    private CollisionFlags m_collisionFlags;

    void Start()
    {
        //Initialize reference variables
        m_animator = GetComponent<Animator>();
        m_controller = GetComponent<CharacterController>();
        m_moveDirection = transform.TransformDirection(Vector3.forward);
        m_weaponCollider = transform.FindChild("Hunter_Animated/Character1_Reference/Character1_Hips/Character1_Spine/Character1_Spine1/Character1_Spine2/Character1_RightShoulder/Character1_RightArm/Character1_RightForeArm/Character1_RightHand/Character1_RightHandMiddle1/sword").GetComponent<BoxCollider>();
        m_previousRotation = transform.rotation;
    }

    void Update()
    {
        PlayerMovement();
        BlockAndBash();
        Attack();
        ApplyGravity();
        ApplyMovement();
        ResetCooldowns();
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

    private void PlayerMovement()
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

        //Get wasd / left thumbstick input
        float xInput = Input.GetAxis("GamePad Move X");
        if (Input.GetKey(KeyCode.D))
            xInput = 1.0f;
        if (Input.GetKey(KeyCode.A))
            xInput = -1.0f;

        float yInput = Input.GetAxis("GamePad Move Y");
        if (Input.GetKey(KeyCode.W))
            yInput = 1.0f;
        if (Input.GetKey(KeyCode.S))
            yInput = -1.0f;
        Vector3 l_movementVector = new Vector3(xInput, 0.0f, yInput);

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
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetButton("Roll Sprint"))
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
        if (Input.GetButtonUp("Roll Sprint") || Input.GetKeyUp(KeyCode.LeftShift))
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
        if (Input.GetKey(KeyCode.LeftControl) || Input.GetButton("Block"))
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
            if (Input.GetButton("Light Attack") || Input.GetMouseButton(0))
            {
                if(!m_isAttacking)
                {
                    m_isAttacking = true;

                    //Which attack are we going to perform?
                    m_currentAttack = Random.Range(1, 3);

                    if (m_currentAttack == 1)
                    {
                        m_animator.SetBool("Attacking1", true);
                        m_weaponCollider.enabled = true;
                    }
                    else
                    {
                        m_animator.SetBool("Attacking2", true);
                        m_weaponCollider.enabled = true;
                    }
                }
            }
            else if (m_isAttacking)
            {
                //Check for end of attack one
                if(m_currentAttack == 1 &&
                    (!m_animator.GetCurrentAnimatorStateInfo(1).IsName("Attack1")))
                {
                    m_isAttacking = false;
                    m_weaponCollider.enabled = false;
                    m_animator.SetBool("Attacking1", false);
                }
                else if (m_currentAttack == 2 &&
                    (!m_animator.GetCurrentAnimatorStateInfo(1).IsName("Attack2")))
                {
                    m_isAttacking = false;
                    m_weaponCollider.enabled = false;
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
        //Set rotation to the movement direction
        if (IsGrounded())
        {
            transform.rotation = Quaternion.LookRotation(m_moveDirection);
            if(m_canMove)
            {
                Vector3 l_xzMove = l_movement;
                l_xzMove.y = 0;
                if (l_xzMove.sqrMagnitude > 0.001f)
                    transform.rotation = Quaternion.LookRotation(l_xzMove);
            }
        }
        //Store previous rotation so we can tell if the player is rotating to the left or the right
        m_previousRotation = transform.rotation;
    }

    private void ApplyGravity()
    {
        m_verticalVelocity -= m_gravity * Time.deltaTime;
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