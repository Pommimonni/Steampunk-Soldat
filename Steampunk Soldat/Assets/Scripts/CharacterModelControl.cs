using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class CharacterModelControl : NetworkBehaviour {

    
    public bool localPlay = false;
    public float roll = 0;
    public GameObject weaponHand;

    [SyncVar]
    public float armDirection;
    float localArmDirection;

    [SyncVar(hook ="JumpChanged")]
    bool jumpOn = false;
    bool localJumpOn = false;

    NetworkIdentity netID;

    public Animator anim;

    public GameObject ragdollPrefab;
    public GameObject controlledCharacter;


    // Use this for initialization
    void Start () {
        Debug.Log("start in cmc");
        netID = GetComponent<NetworkIdentity>();
    }
	
	// Update is called once per frame
	void Update () {
        SetXSpeed(GetComponent<Rigidbody>().velocity.x);
        
        if (netID.isLocalPlayer)
        {
            AimAtCursor();
            Turn();
        }
            
        
    }

    public void SwitchRagdoll(bool toRag)
    {
        if (toRag)
        {
            EnableCharacterModel(false);
            SpawnRagdoll();
        } else
        {
            EnableCharacterModel(true);
        }
        

    }

    void EnableCharacterModel(bool enableChar)
    {
        controlledCharacter.SetActive(enableChar);
    }

    void SpawnRagdoll()
    {
        Vector3 dyingVelocity = this.GetComponent<Rigidbody>().velocity;
        Debug.Log("Spawning ragdoll with force: " + dyingVelocity);
        Quaternion spawnRotation = this.transform.rotation;
        spawnRotation *= Quaternion.Euler(0,-90,0);
        GameObject newRag = (GameObject)GameObject.Instantiate(ragdollPrefab, this.transform.position, spawnRotation);
        //newRag.transform.parent = this.gameObject.transform;
        foreach(Rigidbody rdrb in newRag.GetComponent<RagdollParts>().ragParts)
        {
            rdrb.velocity = dyingVelocity*2;
        }
        //newRag.GetComponent<Rigidbody>().velocity = dyingVelocity*2;
        Destroy(newRag, 5f);
    }

    float faceDir = 1;
    void Turn()
    {
        Vector3 cPos = Camera.main.transform.position;
        Vector3 mPos = Input.mousePosition;
        Vector3 mWorldPos = Camera.main.ScreenToWorldPoint(new Vector3(mPos.x, mPos.y, -cPos.z));
        if (mWorldPos.x < gameObject.transform.position.x)
        {
            gameObject.transform.rotation = Quaternion.Euler(0, 0, 0);
            faceDir = -1;
        }
        else
        {
            gameObject.transform.rotation = Quaternion.Euler(0, 180, 0);
            faceDir = 1;
        }
    }

    void FixedUpdate()
    {
        NetSyncAimDirection();
        
    }

    int count = 0;
    void NetSyncAimDirection()
    {
        if (netID.isLocalPlayer)
        {
            count++;
            if (count > 6)
            {
                CmdSetArmDirection(localArmDirection);
                count = 0;
            }
        }
        else
        {
            SetArmDirection(armDirection);
        }
    }

    [Command]
    void CmdSetArmDirection(float newArmDir)
    {
        this.armDirection = newArmDir;
    }

    void AimAtCursor()
    {
        //Debug.Log("LOCAL AIMING");
        Vector3 pPos = transform.position;
        Vector3 mPos = Input.mousePosition;
        //Debug.Log("Mouse: " + mPos);
        Vector3 mWorldPos = Camera.main.ScreenToWorldPoint(new Vector3(mPos.x, mPos.y, -Camera.main.transform.position.z));

        Vector3 shootDirection = (mWorldPos - pPos);
        shootDirection.z = 0; // just to be sure
        shootDirection.Normalize();

        SetArmDirection(shootDirection.y);
        localArmDirection = shootDirection.y;
    }
    
    public void SetArmDirection(Quaternion q)
    {
        
        Vector3 norm = q * Vector3.forward;
        armDirection = norm.y;
        if (armDirection > 0)
        {
            //float animSet = animControl ;
            //animControl = animSet*animSet;
            armDirection *= armDirection;
            armDirection *= 0.5f;
        }
        anim.SetFloat("aimDirection", armDirection);
    }
    public void SetArmDirection(float direction)
    {
        float adjustedDirection = direction;
        if (adjustedDirection > 0)
        {
            adjustedDirection *= adjustedDirection;
            adjustedDirection *= 0.5f;
        }
        anim.SetFloat("aimDirection", direction);
    }
    public void SetXSpeed(float speed)
    {
        
        float a = (speed / 6)*faceDir;
        //Debug.Log("x speed: " + a);
        anim.SetFloat("rolling", a);
    }

    [Command]
    void CmdSetJump(bool jump)
    {
        jumpOn = jump;
        anim.SetBool("jumping", jump);
    }

    void JumpChanged(bool newJump)
    {
        jumpOn = newJump;
        if (isLocalPlayer || isServer)
            return;
        anim.SetBool("jumping", newJump);
        
    }

    public void SetJumpAnimation(bool jump)
    {
        if (isLocalPlayer)
        {
            anim.SetBool("jumping", jump);
            CmdSetJump(jump);
        }
    }

    public bool IsJumpOn()
    {
        return jumpOn;
    }

    public void TriggerShoot()
    {
        anim.SetTrigger("shooting");
    }

    public void TriggerReload()
    {
        anim.SetTrigger("changing");
    }

    public void TriggerWeaponChange()
    {
        anim.SetTrigger("changing");
    }

}
