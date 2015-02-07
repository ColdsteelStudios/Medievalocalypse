// ---------------------------------------------------------------------------
// PlayerControl.cs
// 
// Controls for player character
// 
// Original Author: Harley Laurie
// ---------------------------------------------------------------------------

using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(CharacterController))]
public class PlayerControl : MonoBehaviour 
{
    private Animator anim; //a reference to the animator of the character

    //Combat variables
    private float attackOneLength = 0.77f;
    private float attackTwoLength = 0.73f;
    private float currentAttackRemaining = 0.0f;
    private float attackComboOverflow = 0.0f; //If the player attacks soon again after the last attack they perform a combo
    private bool currentlyAttacking = false;
    public BoxCollider swordCollider;

    //The speed when walking
    public float walkSpeed = 2.0f;
    //when pressing "Fire3" button (cmd) we start running
    public float runSpeed = 6.0f;
    public float inAirControlAcceleration = 3.0f;
    //The gravity for the character
    public float gravity = 20.0f;
    //The gravity in controlled descent mode
    public float speedSmoothing = 10.0f;
    public float rotateSpeed = 500.0f;
    //The current move direction in x-z
    private Vector3 moveDirection = Vector3.zero;
    //The current vertical speed
    private float verticalSpeed = 0.0f;
    //The current x-z move speed
    private float moveSpeed = 0.0f;

    //The last collision flags returned from controller.Move
    private CollisionFlags collisionFlags;

    private float lockCameraTimer = 0.0f;

    //Are we moving backwards (this locks the camera to not do a 180 degree spin)
    private bool movingBack = false;
    //Is the user pressing any keys?
    private bool isMoving = false;
    //When did the user start walking (Used for going into trot after a while)
    private float walkTimeStart = 0.0f;

    private Vector3 inAirVelocity = Vector3.zero;

    private float lastGroundedTime = 0.0f;

    private bool isControllable = true;

    void Start()
    {
        //Initialising reference variables
        anim = GetComponent<Animator>();
        moveDirection = transform.TransformDirection(Vector3.forward);
    }

    private void UpdateSmoothedMovementDirection()
    {
        Transform cameraTransform = Camera.main.transform;
        bool grounded = IsGrounded();

        //Forward vector relative to the camera along the x-z place
        Vector3 forward = cameraTransform.TransformDirection(Vector3.forward);
        forward.y = 0;
        forward = forward.normalized;

        //Right vector relative to the camera
        //Always orthogonal to the forward vector
        Vector3 right = new Vector3(forward.z, 0, -forward.x);

        float v = Input.GetAxisRaw("Vertical");
        float h = Input.GetAxisRaw("Horizontal");

        //Are we moving backwards or looking backwards
        movingBack = (v < -0.2f) ? true : false;

        bool wasMoving = isMoving;
        isMoving = Mathf.Abs(h) > 0.1 || Mathf.Abs(v) > 0.1;

        //Target direction relative to the camera
        Vector3 targetDirection = h * right + v * forward;


        //Grounded controls
        if (grounded)
        {
            //Lock camera for short period when transitioning moving & standing still
            lockCameraTimer += Time.deltaTime;
            if (isMoving != wasMoving)
                lockCameraTimer = 0.0f;

            //We store speed and direction seperately,
            //so that when the character stands still we still have a valid forward direction
            //moveDirection is always normalized, and we only update it if there is user input.
            if (targetDirection != Vector3.zero)
            {
                //If we are really slow, just snap to the target direction
                if (moveSpeed < walkSpeed * 0.9f)
                    moveDirection = targetDirection.normalized;
                //Otherwise smoothly turn towards it
                else
                {
                    moveDirection = Vector3.RotateTowards(moveDirection, targetDirection, rotateSpeed * Mathf.Deg2Rad * Time.deltaTime, 1000);

                    moveDirection = moveDirection.normalized;
                }
            }

            //Smooth the speed based on the current target direction
            float curSmooth = speedSmoothing * Time.deltaTime;

            //Choose target speed
            //* We want to support analog input but make sure you cant walk faster diagonally than just forward or sidways
            float targetSpeed = Mathf.Min(targetDirection.magnitude, 1.0f);

            //Pick speed modifier
            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift) || Input.GetButton("Fire4"))
            {
                targetSpeed *= runSpeed;
            }
            else
            {
                targetSpeed *= walkSpeed;
            }

            moveSpeed = Mathf.Lerp(moveSpeed, targetSpeed, curSmooth);

            //pass movement speed to animator controller
            anim.SetFloat("Speed", moveSpeed);

            //Reset walk time start when we slow down
            if (moveSpeed < walkSpeed * 0.3f)
                walkTimeStart = Time.time;

            //Allow player to attack
            if(!currentlyAttacking)
            {
                if(Input.GetButton("Fire3") || Input.GetMouseButton(0))
                {
                    //Perform combo attack if possible
                    if(attackComboOverflow>0.0f)
                    {
                        currentlyAttacking = true;
                        anim.SetBool("AttackingTwo", true);
                        currentAttackRemaining = attackTwoLength;
                        attackComboOverflow = 0.0f;
                        swordCollider.enabled = true;
                    }
                    //Otherwise, perform first attack
                    else
                    {
                        currentlyAttacking = true;
                        anim.SetBool("AttackingOne", true);
                        currentAttackRemaining = attackOneLength;
                        attackComboOverflow = attackOneLength + 0.25f;
                        swordCollider.enabled = true;
                    }
                    
                }
            }
            else
            {
                //Count down timer to see how long we have left for our current attack
                currentAttackRemaining -= Time.deltaTime;
                attackComboOverflow -= Time.deltaTime;
                if (currentAttackRemaining <= 0.0f)
                {
                    swordCollider.enabled = false;
                    anim.SetBool("AttackingOne", false);
                    anim.SetBool("AttackingTwo", false);
                    currentlyAttacking = false;
                }
            }
        }
        //In air controls
        else
        {
            if (isMoving)
                inAirVelocity += targetDirection.normalized * Time.deltaTime * inAirControlAcceleration;
        }
    }

    private void ApplyGravity()
    {
        if(isControllable)  //don't move player at all if not controllable.
        {
            if (IsGrounded())
                verticalSpeed = 0.0f;
            else
                verticalSpeed -= gravity * Time.deltaTime;
        }
    }

    void Update()
    {
        if (!isControllable)
        {
            //kill all inputs if not controllable.
            Input.ResetInputAxes();
        }

        UpdateSmoothedMovementDirection();

        //Apply gravity
        //-extra power jump modifies gravity
        //-controlledDescent mode modifies gravity
        ApplyGravity();

        //Calculate actual motion
        Vector3 movement = moveDirection * moveSpeed + new Vector3(0, verticalSpeed, 0) + inAirVelocity;
        movement *= Time.deltaTime;

        //Move the controller
        CharacterController controller = GetComponent<CharacterController>();
        collisionFlags = controller.Move(movement);

        //Set rotation to the move direction
        if (IsGrounded())
        {
            transform.rotation = Quaternion.LookRotation(moveDirection);
        }
        else
        {
            Vector3 xzMove = movement;
            xzMove.y = 0;
            if (xzMove.sqrMagnitude > 0.001f)
                transform.rotation = Quaternion.LookRotation(xzMove);
        }

        if(IsGrounded())
        {
            lastGroundedTime = Time.time;
            inAirVelocity = Vector3.zero;
        }
    }

    public bool IsGrounded()
    {
        return (collisionFlags & CollisionFlags.CollidedBelow) != 0;
    }

    public bool IsMoving()
    {
        return Mathf.Abs(Input.GetAxisRaw("Vertical")) + Mathf.Abs(Input.GetAxisRaw("Horizontal")) > 0.5f;
    }
}