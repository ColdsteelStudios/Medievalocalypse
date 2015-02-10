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
public class PlayerControllerNew : MonoBehaviour
{
    //Animation
    private Animator m_animator; //The players animator component

    //Combat
    private BoxCollider m_weaponCollider;       //Collider that is attatched to the player weapon
    private bool m_isBlocking = false;   //Is the character currently blocking?
    private bool m_isBashing = false;    //Is the character currently shield bashing?
    private bool m_isAttackingOne = false;
    private bool m_isAttackingTwo = false;
    private bool m_isAttacking = false;

    //Movement
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

    //Collision
    private CollisionFlags m_collisionFlags;

    void Start()
    {
        //Initialize reference variables
        m_animator = GetComponent<Animator>();
        m_controller = GetComponent<CharacterController>();
        m_moveDirection = transform.TransformDirection(Vector3.forward);
    }

    void Update()
    {
        PlayerMovement();
        BlockAndBash();
        Attack();
        ApplyGravity();
        ApplyMovement();
    }

    private void PlayerMovement()
    {
        //Allow movement only if the player is on the ground
        if (!IsGrounded())
            return;

        //Movement is relative to camera position
        Transform l_cameraTransform = Camera.main.transform;

        //Get forward vector which is relative to the camera along the x-z plane
        Vector3 l_forward = l_cameraTransform.TransformDirection(Vector3.forward);
        l_forward.y = 0;
        l_forward = l_forward.normalized;

        //Get right vector relative to the camera
        //Always orthogonal to the forward vector
        Vector3 l_right = new Vector3(l_forward.z, 0, -l_forward.x);

        //Get wasd / left thumbstick input
        float l_v = Input.GetAxisRaw("Vertical");
        float l_h = Input.GetAxisRaw("Horizontal");

        //Target direction relative to the camera
        Vector3 l_targetDirection = l_h * l_right + l_v * l_forward;

        //We store speed and direction seperately,
        //so that when the character stands still we still have a valid forward direction
        //m_moveDirection is always normalized, and we only update it if there is user input
        if (l_targetDirection != Vector3.zero)
        {
            //Smoothly turn towards our target direction
            m_moveDirection = Vector3.RotateTowards(m_moveDirection, l_targetDirection, m_rotateSpeed * Mathf.Deg2Rad * Time.deltaTime, 1000.0f);
            m_moveDirection = m_moveDirection.normalized;
        }

        //Smooth the speed based on the current target direction
        float l_currentSmooth = m_speedSmoothing * Time.deltaTime;

        //Choose target speed
        //* We want to support analog input but make sure you cant walk faster diagonally than just forward or sideways
        float l_targetSpeed = Mathf.Min(l_targetDirection.magnitude, 1.0f);

        //Modify the speed based on the player holding the sprint button
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetButton("Fire4"))
            l_targetSpeed *= m_runSpeed;
        else
            l_targetSpeed *= m_walkSpeed;

        m_movementVelocity = Mathf.Lerp(m_movementVelocity, l_targetSpeed, l_currentSmooth);

        //Pass movement speed to animator controller
        m_animator.SetFloat("Speed", m_movementVelocity);
    }

    private void BlockAndBash()
    {
        //Blocking / Shield Bashing
        if (Input.GetKey(KeyCode.LeftControl))
        {
            m_isBlocking = true;
            m_animator.SetBool("Blocking", true);
            //If player is blocking and presses attack, they perform a shield bash
            if (Input.GetButton("Fire3") || Input.GetMouseButton(0))
            {
                m_isBashing = true;
                m_animator.SetBool("Bashing", true);
            }
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
        if (!m_animator.GetCurrentAnimatorStateInfo(2).IsName("Block") &&
            !m_animator.GetCurrentAnimatorStateInfo(2).IsName("ShieldBash"))
        {
            if (Input.GetButton("Fire3") || Input.GetMouseButton(0))
            {
                m_isAttacking = true;
                m_animator.SetBool("Attacking", true);

                if (!m_isAttackingOne && !m_animator.GetCurrentAnimatorStateInfo(1).IsName("Attacking2"))
                {
                    m_isAttackingOne = true;
                    m_isAttackingTwo = false;
                }
                else if (!m_isAttackingTwo && !m_animator.GetCurrentAnimatorStateInfo(1).IsName("Attacking1"))
                {
                    m_isAttackingTwo = true;
                    m_isAttackingTwo = false;
                }
            }
            else
            {
                if(m_isAttackingOne)
                {
                    if (!m_animator.GetCurrentAnimatorStateInfo(1).IsName("Attack1"))
                    {
                        m_isAttackingOne = false;
                        m_animator.SetBool("Attacking", false);
                        m_isAttacking = false;
                    }
                    else if (!m_animator.GetCurrentAnimatorStateInfo(1).IsName("Attack2") && m_isAttackingTwo)
                    {
                        m_isAttackingTwo = false;
                        m_animator.SetBool("Attacking", false);
                        m_isAttacking = false;
                    }
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
            transform.rotation = Quaternion.LookRotation(m_moveDirection);
        {
            Vector3 l_xzMove = l_movement;
            l_xzMove.y = 0;
            if (l_xzMove.sqrMagnitude > 0.001f)
                transform.rotation = Quaternion.LookRotation(l_xzMove);
        }
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