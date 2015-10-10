using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class EnemyAI : NetworkBehaviour {


    public float maxSpeed = 1f;
    public float accel = 1f;
    public float airAccel = 1f;
    public float jumpForce = 1f;
    bool jump = false;
    bool grounded = false;
    public GameObject groundCheckObject;

    bool aiLeft = false;
    bool aiRight = false;
    bool aiJump = false;

    // Use this for initialization
    void Start () {
        enabled = isServer;
        RandomizeAction();
	}
	
	// Update is called once per frame
	void Update () {
	}

    //randomizes some movement every 2 sec
    void RandomizeAction()
    {
        float randF = Random.Range(0, 100);
        resetAiRand();
        if (randF < 40)
        {
            aiLeft = true;
        }
        else if( randF < 80)
        {
            aiRight = true;
        }
        randF = Random.Range(0, 100);
        if (randF < 30)
        {
            aiJump = true;
        }
        Invoke("RandomizeAction", 1f);
    }

    void resetAiRand()
    {
        aiLeft = false;
        aiRight = false;
        aiJump = false;
    }


    void FixedUpdate()
    {
        if (!isServer)
            return;
        grounded = GroundCheck();
        ForceMovement(aiRight, aiLeft);
        if (aiJump && grounded)
        {
            GetComponent<Rigidbody>().AddForce(new Vector3(0, jumpForce, 0));
            aiJump = false;
        }
    }
    void ForceMovement(bool wantsToMoveRight, bool wantsToMoveLeft)
    {
        //float horInput = Input.GetAxisRaw("Horizontal");

        Rigidbody rigidBody = GetComponent<Rigidbody>();
        Vector3 currentVel = rigidBody.velocity;
        float xVel = currentVel.x;

        if (!wantsToMoveLeft && !wantsToMoveRight)
        {
            if (grounded && xVel != 0) //no input and on the ground
            {
                rigidBody.velocity = new Vector3(0, currentVel.y, 0); // stop x movement if no input and grounded
            }
            return;
        }

        float xSpeed = Mathf.Abs(xVel);
        float acceleration = (grounded ? accel : airAccel); //different acceleration in the air

        if (wantsToMoveLeft && xVel > -maxSpeed)
        {
            rigidBody.AddForce(new Vector3(-acceleration / (1 + xSpeed), 0, 0));
        }
        else if (wantsToMoveRight && xVel < maxSpeed) // character not moving to the right on max vel
        {
            rigidBody.AddForce(new Vector3(acceleration / (1 + xSpeed), 0, 0));
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
