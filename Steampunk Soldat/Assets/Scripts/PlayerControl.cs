using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System;

public class PlayerControl : NetworkBehaviour {


    public float maxSpeed = 1f;
    public float accel = 1f;
    public float airAccel = 1f;
    public float jumpForce = 1f;
    public float jumpTakeoffTime = 0.4f;
    public float jumpCooldown = 0.2f;
    public float sideStepForce = 1f;
    public float sideStepUpForce = 1f;

    float jumpCharge = 0f;

    bool jumping = false; //common state for both jumps
    bool upJumping = false;
    bool sideStepping = false;
    bool sideStepLeft = false;
    bool grounded = false;
    public GameObject[] groundCheckObjects;

    Vector3 respawnTarget;

    // Use this for initialization
    void Start () {
        //Debug.Log("is local"+isLocalPlayer);
        if (isLocalPlayer)
            gameObject.AddComponent<AudioListener>();
        respawnTarget = new Vector3(0, 35, 0);
    }
	
	// Update is called once per frame
	void Update () {
        if (!isLocalPlayer)
            return;
        
        CheckJump();
        if(!GetComponent<CombatControl>().dead)
            MoveCamera();
	}

    
    
    

    void FixedUpdate()
    {
        Rigidbody rigidBody = GetComponent<Rigidbody>();
        Vector3 zToZero = new Vector3(rigidBody.position.x, rigidBody.position.y, 0);
        rigidBody.MovePosition(zToZero);
        if (GetComponent<CombatControl>().respawning)
        {
            rigidBody.MovePosition(respawnTarget);
        }
        if (!isLocalPlayer || GetComponent<CombatControl>().dead)
            return;

        grounded = GroundCheck();
        ForceMovement();
        if (sideStepping)
        {
            SideStep();
        }
        if (upJumping)
        {
            JumpUp();
        }
    }

    void SideStep()
    {
        sideStepping = false;
        Debug.Log("side stepping");
        float xForce = sideStepLeft ? -sideStepForce : sideStepForce;
        float jumpCoef = GetJumpEffort(jumpCharge, jumpTakeoffTime);
        Vector3 sideStepVector = new Vector3(xForce* jumpCoef, sideStepUpForce* jumpCoef, 0);
        GetComponent<Rigidbody>().AddForce(sideStepVector);
        Invoke("JumpCooldown", jumpCooldown);
    }

    void JumpUp()
    {
        upJumping = false;
        float jumpCoef = GetJumpEffort(jumpCharge, jumpTakeoffTime);
        //Debug.Log("Jump final charge = " + jumpCoef);
        GetComponent<Rigidbody>().AddForce(new Vector3(0, jumpCoef * jumpForce, 0));
        Invoke("JumpCooldown", jumpCooldown);
    }

    void CheckJump()
    {
        if (jumping) //on cooldown
            return;

        bool jumpRelease = Input.GetKeyUp(KeyCode.Space) || Input.GetKeyUp(KeyCode.W);
        bool jumpHold = Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.W);

        bool rightInput = Input.GetButton("Right");
        bool leftInput = Input.GetButton("Left");

        bool jumpFullyCharged = (jumpCharge >= jumpTakeoffTime);
        bool jumpNow = (jumpRelease || jumpFullyCharged);
        
        if (grounded && jumpNow)
        {
            if(rightInput || leftInput)
            {
                sideStepping = true;
                sideStepLeft = leftInput;
            }
            else
            {
                upJumping = true; //upwards jump
            }
            jumping = true;
        }
        else if (grounded && jumpHold) //cannot charge if already jumping
        {
            jumpCharge += Time.deltaTime;
        }
        else
        {
            jumpCharge = 0;
        }
    }


    public void SetRespawnTarget(Vector3 newTar)
    {
        Debug.Log("setting spawn target: " + newTar);
        respawnTarget = newTar;
    }
   

    //how long has the jump key been held down and how should it affect the jump
    float GetJumpEffort(float jumpCharge, float maxCharge)
    {
        float jumpCoef = jumpCharge / maxCharge; // 0-1
        //Debug.Log("Jump initial charge = " + jumpCoef);
        if (jumpCoef < 0.4f) //Minimum percentage of jumpTakeOffTime that needs to be held down for the player to jump at all
        {
            jumpCoef = 0;
        }
        else if (jumpCoef > 1)
        {
            jumpCoef = 1;
        }
        else
        {
            jumpCoef = jumpCoef * jumpCoef;
            //doesn't jump during the first half of holding and then start scaling the jump up to full hold
        }
        return jumpCoef;
    }

    void JumpCooldown()
    {
        Debug.Log("Clearing jump cooldown now ");
        jumping = false;
        jumpCharge = 0;
    }

    //unused
    void VelocityMovement()
    {
        Vector2 targetVel = new Vector2(0, 0);
        if (Input.GetAxisRaw("Horizontal") < 0)
        {
            targetVel.x = -maxSpeed;

        }
        if (Input.GetAxisRaw("Horizontal") > 0)
        {
            targetVel.x = maxSpeed;

        }
        this.GetComponent<Rigidbody>().velocity = new Vector3(targetVel.x, GetComponent<Rigidbody>().velocity.y, 0);
    }

    //force based movement
    void ForceMovement()
    {
        //float horInput = Input.GetAxisRaw("Horizontal");

        bool wantsToMoveRight = Input.GetButton("Right");
        bool wantsToMoveLeft = Input.GetButton("Left");

        Rigidbody rigidBody = GetComponent<Rigidbody>();
        Vector3 currentVel = rigidBody.velocity;
        float xVel = currentVel.x;

        if (!wantsToMoveLeft && !wantsToMoveRight)
        {
            if(grounded && xVel != 0) //no input and on the ground
            {
                rigidBody.velocity = new Vector3(0, currentVel.y, 0); // stop x movement if no input and grounded
            }
            return;
        }

        float xSpeed = Mathf.Abs(xVel);
        float acceleration = (grounded ? accel : airAccel); //different acceleration in the air
        
        if (wantsToMoveLeft && xVel > -maxSpeed)
        {
            rigidBody.AddForce(new Vector3(-acceleration / (1+xSpeed), 0, 0));
        }
        else if (wantsToMoveRight && xVel < maxSpeed) // character not moving to the right on max vel
        {
            rigidBody.AddForce(new Vector3(acceleration / (1+xSpeed), 0, 0));
        }
        
    }

    //the center of the screen should be between the player's character and mouse pointer
    void MoveCamera()
    {
        Vector3 pPos = this.transform.position;
        Vector3 mPos = Input.mousePosition;
        Vector3 cPos = Camera.main.transform.position;
        Rect screenRect = new Rect(1, 1, Screen.width-2, Screen.height-2);
        //Debug.Log("Mouse: " + mPos);
        if (screenRect.Contains(mPos))
        {
            Vector3 mWorldPos = Camera.main.ScreenToWorldPoint(new Vector3(mPos.x, mPos.y, -cPos.z)); //mouse position in the world on the xy plane when z = 0
            float camY = (mWorldPos.y + pPos.y) / 2;
            float camX = (mWorldPos.x + pPos.x) / 2; 
            Camera.main.transform.position = new Vector3(camX, camY, cPos.z); //middle of the camera is set to between the cursor and the character
        } else
        {
            Camera.main.transform.position = new Vector3(pPos.x, pPos.y, cPos.z); //if the pointer is out of the screen, focus camera on the character
        }
    }

    bool GroundCheck()
    {
        //check if the player is on the ground
        int gcMask = 1 << LayerMask.NameToLayer("Ground");
        foreach(GameObject gco in groundCheckObjects)
        {
            Vector3 gcOrig = new Vector3(transform.position.x, transform.position.y, 0);
            Vector3 gcEnd = new Vector3(gco.transform.position.x, gco.transform.position.y, 0);
            if(Physics.Linecast(gcOrig, gcEnd, gcMask)) //raycast from player's character downwards to the groundcheck object
            {
                return true;
            }
        }
        return false;
    }
}
