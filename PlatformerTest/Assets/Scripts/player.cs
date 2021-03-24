using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private Vector2 velocity;
    private Vector2 delayedPosition;
    private Rigidbody2D body;
    private BoxCollider2D collider;
    private int horizontal;
    private int direction;
    private int jumpFrames;
    private int wallJumpFrames;
    private int dashFrames;
    private bool isGrounded;
    private bool isWalledL;
    private bool isWalledR;
    private int wallJumpDir;
    private bool jumpInputUsed;
    private int framesSinceGround;
    private bool jumpButtonActive;
    private bool doubleJumpActive;
    private bool dashButtonActive;
    private bool dashFromGround;
    private float checkBoxOffset = 0.05f;

    private static bool downPressed;
    private static bool upPressed;
    private static bool leftPressed;
    private static bool rightPressed;
    private static bool dashPressed;
    private static bool jumpPressed;

    [SerializeField]
    private LayerMask platformLayer;

    [Header("Horizontal movement")]
    [SerializeField]
    private float maxSpeed;
    [SerializeField]
    private int accFrames;
    [SerializeField]
    private int decFrames;

    [Header("Vertical movement")]
    [SerializeField]
    private float maxFallSpeed;
    [SerializeField]
    private float maxQuickFallSpeed;
    [SerializeField]
    private float decVert;
    [SerializeField]
    private float accVert;

    [Header("Jump movement")]
    [SerializeField]
    private float maxRizeSpeed;
    [SerializeField]
    private int maxJumpFrames;
    [SerializeField]
    private int maxFloatJumpFrames;

    [Header("Double Jump movement")]
    [SerializeField]
    private float doubleJumpSpeed;

    [Header("Wall Jump movement")]
    [SerializeField]
    private float maxWallJumpFrames;
    [SerializeField]
    private float wallJumpFirstFrames;
    [SerializeField]
    private bool wallRestoresDoubleJump;
    [SerializeField]
    private bool wallRestoresDash;

    [Header("Dash movement")]
    [SerializeField]
    private float maxDashSpeed;
    [SerializeField]
    private int maxDashFrames;
    [SerializeField]
    private int dashCooldownFrames;

    [Header("Unussual movement")]
    [SerializeField]
    private bool midJumpDash;
    [SerializeField]
    private bool groundDashJump;


    private float eps;


    void Start()
    {
        body = gameObject.GetComponent<Rigidbody2D>();
        collider = gameObject.GetComponent<BoxCollider2D>();
        body.fixedAngle = true;
        body.position = delayedPosition;
        velocity = new Vector2(0.0f, 0.0f);
        horizontal = 0;
        direction = 1;
        jumpPressed = false;
        dashPressed = false;
        jumpFrames = maxJumpFrames;
        dashFrames = maxDashFrames;
        jumpButtonActive = false;
        doubleJumpActive = false;
        dashButtonActive = false;
        dashFromGround = false;
        eps = 0.001f * maxSpeed / (accFrames + decFrames);
        Input.ResetInputAxes();
    }

    private void Update()
    {
        if (Input.GetKeyDown("'"))
            rightPressed = true;
        if (Input.GetKeyUp("'"))
            rightPressed = false;

        if (Input.GetKeyDown("l"))
            leftPressed = true;
        if (Input.GetKeyUp("l"))
            leftPressed = false;

        if (Input.GetKeyDown("p"))
            upPressed = true;
        if (Input.GetKeyUp("p"))
            upPressed = false;

        if (Input.GetKeyDown(";"))
            downPressed = true;
        if (Input.GetKeyUp(";"))
            downPressed = false;

        if (Input.GetKeyDown("z"))
            jumpPressed = true;
        if (Input.GetKeyUp("z"))
            jumpPressed = false;

        if (Input.GetKeyDown("c"))
            dashPressed = true;
        if (Input.GetKeyUp("c"))
            dashPressed = false;
    }

    private void FixedUpdate()
    { 
        if (isGrounded)
            velocity.y = Mathf.Max(velocity.y, 0.0f);
        else if (Mathf.Abs(velocity.y - body.velocity.y) > eps && jumpFrames > 0)
            jumpFrames = maxJumpFrames;
        velocity = body.velocity;

        isGroundedUpdate();
        isWalledRUpdate();
        isWalledLUpdate();

        horizontal = 0;
        if (Input.GetKey("'"))
            ++horizontal;
        if (Input.GetKey("l"))
            --horizontal;
        jumpPressed = Input.GetKey("z");
        downPressed = Input.GetKey(";");
        dashPressed = Input.GetKey("c");

        if (!jumpPressed)
            jumpInputUsed = false;
       
        if (horizontal != 0 && (dashFrames <= 0 || dashFrames >= maxDashFrames))
            direction = horizontal;

        if (horizontal == 0)
        {
            if (velocity.x < -eps)
                velocity.x = Mathf.Max(velocity.x - maxSpeed / decFrames, 0);
            else
                velocity.x = Mathf.Min(velocity.x + maxSpeed / accFrames, 0);
        }
        else
        {
            if (velocity.x * horizontal < 0.0f)
                velocity.x += maxSpeed * horizontal / decFrames;
            else
                velocity.x += maxSpeed * horizontal / accFrames;

            if (velocity.x > maxSpeed)
                velocity.x = maxSpeed;
            if (velocity.x < -maxSpeed)
                velocity.x = -maxSpeed;
        }

        if (wallRestoresDoubleJump && (isWalledL || isWalledR))
            doubleJumpActive = true;
        if (isGrounded)
        {
            framesSinceGround = 0;
            doubleJumpActive = true;
        }
        else
            ++framesSinceGround;

        if (!jumpPressed)
            jumpButtonActive = true;

        if (isGrounded && jumpFrames == maxJumpFrames)
            jumpFrames = 0;


        if (jumpPressed && jumpFrames < maxJumpFrames && (dashFrames <= 0 || dashFrames >= maxDashFrames) && (jumpButtonActive || jumpFrames > 0))
        {
            velocity.y = maxRizeSpeed;
            ++jumpFrames;
            jumpButtonActive = false;
            jumpInputUsed = true;
        }
        else if (dashFrames <= 0 || dashFrames >= maxDashFrames)
        {
            velocity.y = Mathf.Max(velocity.y - ((velocity.y > 0) ? decVert : accVert), -((downPressed) ? maxQuickFallSpeed : maxFallSpeed));

            if (jumpButtonActive && doubleJumpActive && jumpPressed && !isWalledR && !isWalledL && !jumpInputUsed)
            {
                velocity.y = doubleJumpSpeed;
                doubleJumpActive = false;
                jumpButtonActive = false;
                jumpInputUsed = true;
            }

            if (isGrounded)
                jumpFrames = 0;
            else if ((jumpFrames != 0 || framesSinceGround > maxFloatJumpFrames) && (dashFrames <= 0 || dashFrames >= maxDashFrames))
            {
                jumpFrames = maxJumpFrames;
            }
        }
        else 
        { 
            if (jumpFrames > 0 && !midJumpDash)
                jumpFrames = maxJumpFrames;
            if (!isGrounded && jumpFrames == 0 && !groundDashJump)
                jumpFrames = maxJumpFrames;
        }

        if (!jumpInputUsed && jumpPressed)
        {
            if (isWalledR || isWalledL)
            {
                wallJumpDir = (isWalledL? -1 : 0) + (isWalledR? 1 : 0);
                wallJumpFrames = 1;
                jumpInputUsed = true;
                velocity.y = maxRizeSpeed;
                velocity.x = -wallJumpDir * maxSpeed;
            }                
        }
        else if (wallJumpFrames < maxWallJumpFrames && wallJumpFrames > 0)
        {
            if (wallJumpFrames < wallJumpFirstFrames)
            {
                ++wallJumpFrames;
                jumpInputUsed = true;
                velocity.y = maxRizeSpeed;
                velocity.x = -wallJumpDir * maxSpeed;
            }
            else if (jumpPressed)
            {
                ++wallJumpFrames;
                jumpInputUsed = true;
                velocity.y = maxRizeSpeed;
            }
            else
                wallJumpFrames = 0;
        }

        if (wallJumpFrames >= maxWallJumpFrames)
            wallJumpFrames = 0;


        if (((dashPressed && dashButtonActive) || dashFrames > 0) && dashFrames < maxDashFrames && dashFrames >= 0)
        {
            velocity.x = maxDashSpeed * direction;
            velocity.y = 0;
            ++dashFrames;

            dashButtonActive = false;

            if (isGrounded)
                dashFromGround = true;
        }
        else
        {
            if (dashFrames < 0)
                ++dashFrames;

            if (((isGrounded || dashFromGround) || wallRestoresDash && (isWalledL || isWalledR)) && dashFrames > 0)
            {
                dashFrames = -dashCooldownFrames;
                dashFromGround = false;
            }
        }

        if (!dashPressed)
            dashButtonActive = true;

        body.velocity = velocity;
    }

    private void isGroundedUpdate()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(collider.bounds.center, collider.bounds.size, 0.0f, Vector2.down, checkBoxOffset, platformLayer);

        Color rayColor;
        if (raycastHit.collider != null)
            rayColor = Color.green;
        else
            rayColor = Color.red;

        Debug.DrawRay(collider.bounds.center - new Vector3(collider.bounds.extents.x, collider.bounds.extents.y + checkBoxOffset), Vector2.right * collider.bounds.extents.x * 2, rayColor);

        isGrounded = raycastHit.collider != null;
    }

    private void isWalledRUpdate()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(collider.bounds.center, collider.bounds.size, 0.0f, Vector2.right, checkBoxOffset, platformLayer);

        Color rayColor;
        if (raycastHit.collider != null)
            rayColor = Color.green;
        else
            rayColor = Color.red;

        Debug.DrawRay(collider.bounds.center + new Vector3(collider.bounds.extents.x + checkBoxOffset, collider.bounds.extents.y), Vector2.down * collider.bounds.extents.y * 2, rayColor);

        isWalledR = raycastHit.collider != null;
    }

    private void isWalledLUpdate()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(collider.bounds.center, collider.bounds.size, 0.0f, Vector2.left, checkBoxOffset, platformLayer);

        Color rayColor;
        if (raycastHit.collider != null)
            rayColor = Color.green;
        else
            rayColor = Color.red;

        Debug.DrawRay(collider.bounds.center + new Vector3(-collider.bounds.extents.x - checkBoxOffset, collider.bounds.extents.y), Vector2.down * collider.bounds.extents.y * 2, rayColor);

        isWalledL = raycastHit.collider != null;
    }

    public void SetPosition(Vector2 pos)
    {
        if (body)
            body.position = pos;
        delayedPosition = pos;
    }
}
