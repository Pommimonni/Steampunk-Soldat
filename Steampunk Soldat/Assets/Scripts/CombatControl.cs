using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

public class CombatControl : NetworkBehaviour {

    public static bool serverPlayer = false;

    public List<GameObject> weaponList;
    public GameObject weapon;
    IWeapon weaponScript;

    [SyncVar(hook = "HealthChanged")]
    public float health = 100;

    public float maxHealth = 100;
    public Slider healthBar;

    public float respawnImmuneTime = 1;
    public float respawnTime = 2;
    public Collider selfCollider;

    public bool dead = false;
    bool weaponChangeCooldown = false;
    public RandomAudio dyingScream;
    public AudioSource bulletHit;
    public CharacterModelControl cmc;
    // Use this for initialization
    void Start () {
        if (isLocalPlayer)
        {
            Debug.Log("Player " + playerControllerId + " will now request weapon spawn");
            CmdSpawnWeapon((int)Weapon.MachineGun); //Server will spawn the default weapon
        }
        healthBar.value = maxHealth;
        health = maxHealth;
        selfCollider = GetComponentInChildren<Collider>();
	}

    public override void OnStartServer()
    {
        Debug.Log("Server bullet");
        LocalData.isServerPlayer = true;
    }

    //Server spawns a weapon for the player running this script
    [Command]
    void CmdSpawnWeapon(int choise)
    {
        if (weaponChangeCooldown)
            return;
        ChangedWeapon();
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
        weapon = (GameObject)Instantiate(weaponList[choise], Vector3.zero, Quaternion.identity);
        weaponScript = weapon.GetComponent<IWeapon>();
        Debug.Log("Setting net ID to weapon: " + this.netId);
        Debug.Log("For: " + weapon +" Script: "+weaponScript);
        weaponScript.SetParent(this.netId);
        NetworkServer.SpawnWithClientAuthority(weapon, connectionToClient); //on clients this spawns on root
    }

    //on the weapon side, when the weapon spawns on the client, the parenting is set and SetWeapon is called to set the weaponScript here

    //when the weapon spawns on client it will call this
    public void SetWeapon(IWeapon newWeapon)
    {
        weaponScript = newWeapon;
        cmc.TriggerWeaponChange();
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
        if (isServer)
        {
            if(transform.position.y < -10)
            {
                this.Die();
            }
        }
        if (!isLocalPlayer)
            return;

        if (Input.GetKeyDown(KeyCode.Keypad1) || Input.GetKeyDown(KeyCode.Alpha1))
        {
            CmdSpawnWeapon((int)Weapon.Pistol);
        }
        if (Input.GetKeyDown(KeyCode.Keypad2) || Input.GetKeyDown(KeyCode.Alpha2))
        {
            CmdSpawnWeapon((int)Weapon.MachineGun);
        }
        if (Input.GetKeyDown(KeyCode.Keypad3) || Input.GetKeyDown(KeyCode.Alpha3))
        {
            CmdSpawnWeapon((int)Weapon.Minigun);
        }
    }

    void ChangedWeapon()
    {
        weaponChangeCooldown = true;
        Invoke("WeaponChangeCooldownOff", 1.5f);
    }

    void WeaponChangeCooldownOff()
    {
        weaponChangeCooldown = false;
    }
    
    [Server]
    public void TakeDamage(float dmg, GameObject shooter)
    {
        if (!isServer || dead)
            return;
        Debug.Log("server applied damage to a player");
        health -= dmg;
        if(health <= 0)
        {
            Debug.Log("server noticed player death");
            Die();
            shooter.GetComponent<CombatControl>().GotAKill();
        }
    }

    //this is called on the player object who got the kill
    [Server]
    private void GotAKill()
    {
        Debug.Log("player " + GetComponent<PlayerScore>().playerID + " got a kill!");
        GetComponent<PlayerScore>().kills++;
        //add score 
    }

    //this is called on the player object who died
    [Server]
    private void Die()
    {
        if (dead)
            return;
        dead = true;
        health = maxHealth;
        RpcDie();
        //Respawn();
        //Invoke("EndImmunity", respawnTime + respawnImmuneTime);
        Debug.Log("Player " + GetComponent<PlayerScore>().playerID + " died");
        GetComponent<PlayerScore>().deaths++;
        //add a death
    }

    private void EndImmunity()
    {
        dead = false;
        respawning = false;
        healthBar.gameObject.SetActive(true);
    }

    [ClientRpc]
    public void RpcDie()
    {
        dyingScream.Play();
        healthBar.gameObject.SetActive(false);
        dead = true;
        Invoke("EndImmunity", respawnTime + respawnImmuneTime);
        if (isLocalPlayer || (isServer && GetComponent<EnemyAI>() != null))
        {
            //if(GetComponent<PlayerControl>() != null)
                //GetComponent<PlayerControl>().enabled = false;
            //RigidbodyConstraints rbc = RigidbodyConstraints.None;
            //GetComponent<Rigidbody>().constraints = rbc;
            Invoke("Respawn", respawnTime);
        }
    }

    public bool respawning = false;
    
    public void Respawn()
    {
        respawning = true;
        GameObject spawnPoint = SpawnPoint.FindNearest(this.transform.position);
        if(spawnPoint != null)
        {
            if(this.GetComponent<PlayerControl>() != null)
            {
                Debug.Log("cc setting spawn target: " + spawnPoint.transform.position);
                this.GetComponent<PlayerControl>().SetRespawnTarget(spawnPoint.transform.position);
            }
                
            this.GetComponent<Rigidbody>().MovePosition(spawnPoint.transform.position);
        }
        //dead = false;
        //GetComponent<Rigidbody>().constraints = RigidbodyConstraints.
        //RigidbodyConstraints rbc = RigidbodyConstraints.;
        //if (GetComponent<PlayerControl>() != null)
            //GetComponent<PlayerControl>().enabled = true;

    }

    private void HealthChanged(float h)
    {
        health = h;
        if(health != maxHealth)
            bulletHit.Play();
    }

}
