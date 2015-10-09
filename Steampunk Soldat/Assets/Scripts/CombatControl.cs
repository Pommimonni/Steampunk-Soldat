using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections.Generic;

public class CombatControl : NetworkBehaviour {

    public List<GameObject> weaponList;
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
        if (isLocalPlayer)
        {
            
            Debug.Log("Player " + playerControllerId + " will now request weapon spawn");
            CmdSpawnWeapon(0); //Server will spawn the default weapon
        }
        
        healthBar.value = maxHealth;
        health = maxHealth;
        selfCollider = GetComponentInChildren<Collider>();
	}

    //Server spawns a weapon for the player running this script
    [Command]
    void CmdSpawnWeapon(int choise)
    {
        Debug.Log("switching weapon to: " + choise);
        if(weaponList.Count < 1)
        {
            Debug.LogError("Weapon list is empty!");
            return;
        }
        if(weaponList[choise] == null)
        {
            Debug.LogError("Invalid weapon index: "+choise);
            return;
        }
        Debug.Log("cmd spawning weapon");
        if(weapon != null)
        {
            Destroy(weapon);
        }
        weapon = (GameObject)Instantiate(weaponList[choise], transform.position, transform.rotation);
        weapon.transform.parent = transform;
        weaponScript = weapon.GetComponent<IWeapon>();
        Debug.Log("Setting net ID to weapon: " + this.netId);
        Debug.Log("For: " + weapon +" Script: "+weaponScript);
        weaponScript.SetParent(this.netId);
        NetworkServer.SpawnWithClientAuthority(weapon, connectionToClient); //on clients this spawns on root
    }

    //on the weapon side, when the weapon spawns on the client, the parenting is set and SetWeapon is called to set the weaponScript here

    //when the weapon spawns on client it will call this
    [Client]
    public void SetWeapon(IWeapon newWeapon)
    {
        weaponScript = newWeapon;
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
            weaponScript.ShootingRequest(); //sends a request to server to shoot
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
    
}
