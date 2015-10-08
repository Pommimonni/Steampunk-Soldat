using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;

public class CombatControl : NetworkBehaviour {

    public GameObject weapon;
    IWeapon weaponScript;

    [SyncVar]
    public float health = 100;

    public float maxHealth = 100;
    public Slider healthBar;

    public Collider selfCollider;

    bool dead = false;
	// Use this for initialization
	void Start () {
        if (isLocalPlayer && weapon)
        {
            SpawnWeapon();
        }
        if(weapon)
            weaponScript = weapon.GetComponent<IWeapon>();
        healthBar.value = maxHealth;
        health = maxHealth;
        selfCollider = GetComponentInChildren<Collider>();
	}

    //test
    
    void SpawnWeapon()
    {
        weapon = (GameObject)Instantiate(weapon, transform.position, transform.rotation);
        NetworkServer.SpawnWithClientAuthority(weapon, connectionToClient);
        //RpcSetWeapon(weapon);
    }

    //test
    [ClientRpc]
    void RpcSetWeapon(GameObject newWeapon)
    {
        weapon = newWeapon;
        weapon.transform.parent = transform;
    }
	
	// Update is called once per frame
	void Update () {
        healthBar.value = health;
        if (!isLocalPlayer)
            return;
        if (Input.GetMouseButton(0))
        {
            //Debug.Log("Combat Bang");
            if (!weaponScript.Cooldown())
            {
                Debug.Log("requesting to shoot");
                Vector3 pPos = this.transform.position;
                Vector3 mPos = Input.mousePosition;
                //Debug.Log("Mouse: " + mPos);
                Vector3 mWorldPos = Camera.main.ScreenToWorldPoint(new Vector3(mPos.x, mPos.y, -Camera.main.transform.position.z));
                Vector3 shootDirection = (mWorldPos - pPos);
                shootDirection.z = 0; // just to be sure
                shootDirection.Normalize();
                //weaponScript.CmdShoot(pPos+0.5f*shootDirection, shootDirection);
                CmdShoot(pPos, shootDirection);
                weaponScript.SetCooldown(true);
            }
        }
	}

    public void TakeDamage(float dmg)
    {
        if (!isServer)
            return;
        health -= dmg;
        if(health <= 0)
        {
            dead = true;
            health = 100;
            RpcRespawn();
        }
    }

    [ClientRpc]
    public void RpcRespawn()
    {
        if(isLocalPlayer)
            this.transform.position = new Vector3(0, 15, 0);
    }

    [Command]
    void CmdShoot(Vector3 from, Vector3 towards)
    {
        if (!weaponScript.Cooldown())
        {
            weaponScript.Shoot(from, towards, selfCollider);
            GetComponent<PlayerAudio>().RpcShootingSound();
        }
        
    }
}
