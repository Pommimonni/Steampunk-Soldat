using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class PlayerControl : NetworkBehaviour {

    public float maxSpeed = 1f;
    public float accel = 1f;
    public float airAccel = 1f;
    public float jumpForce = 1f;
    public static float camDistance = -16;
    bool jump = false;
    bool grounded = false;
    public GameObject groundCheckObject;
    // Use this for initialization
    void Start () {
        //Debug.Log("is local"+isLocalPlayer);
    }
	
	// Update is called once per frame
	void Update () {
        if (!isLocalPlayer)
            return;

        if (grounded && Input.GetKeyDown(KeyCode.Space))
        {
            jump = true;
        }
        MoveCamera();
	}

    void FixedUpdate()
    {
        if (!isLocalPlayer)
            return;

        grounded = GroundCheck();
        ForceMovement();
        
        if (jump)
        {
            GetComponent<Rigidbody>().AddForce(new Vector3(0, jumpForce, 0));
            jump = false;
        }

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
        Rect screenRect = new Rect(1, 1, Screen.width-2, Screen.height-2);
        //Debug.Log("Mouse: " + mPos);
        if (screenRect.Contains(mPos))
        {
            Vector3 mWorldPos = Camera.main.ScreenToWorldPoint(new Vector3(mPos.x, mPos.y, -camDistance)); //mouse position in the world on the xy plane when z = 0
            float camY = (mWorldPos.y + pPos.y) / 2;
            float camX = (mWorldPos.x + pPos.x) / 2; 
            Camera.main.transform.position = new Vector3(camX, camY, camDistance); //middle of the camera is set to between the cursor and the character
        } else
        {
            Camera.main.transform.position = new Vector3(pPos.x, pPos.y, camDistance); //if the pointer is out of the screen, focus camera on the character
        }
    }

    bool GroundCheck()
    {
        //check if the player is on the ground
        int gcMask = 1 << LayerMask.NameToLayer("Ground");
        Vector3 gcOrig = new Vector3(transform.position.x, transform.position.y, 0);
        Vector3 gcEnd = new Vector3(groundCheckObject.transform.position.x, groundCheckObject.transform.position.y, 0);
        return Physics.Linecast(gcOrig, gcEnd, gcMask); //raycast from player's character downwards to the groundcheck object
    }
}
