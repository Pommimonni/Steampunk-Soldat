using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class WeaponBase : NetworkBehaviour, IWeapon {

    public float damage = 50;
    public float bulletSpeed = 35f;
    public float cooldown = 1f;
    public float reloadTime = 5f;
    public float inaccurasy = 0;
    public int ammoPerClip = 40;
    public GameObject bulletPrefab;
    
    public Transform barrelEnd;
    public GameObject shellPrefab;

    public RandomAudio shootSound;
    public AudioSource reloadSound;

    public IWeaponSpec weaponSpec; //for future use

    protected bool onCooldown = false;

    [SyncVar]
    public NetworkInstanceId weaponOwnerNetID;
    protected GameObject weaponOwner;
    protected Collider weaponOwnerCollider;

    [SyncVar]
    protected int ammoLeftInClip = 40;
    


    public override void OnStartClient()
    {
        // When we are spawned on the client,
        // find the parent object using its ID,
        // and set it to be our transform's parent.

        base.OnStartClient();
        //Debug.Log("revolver started on client");
        // Debug.Log("For: " + gameObject + " Script: " + this);
        //Debug.Log("Trying to find: " + parentNetId);
        weaponOwner = ClientScene.FindLocalObject(weaponOwnerNetID);
        //Debug.Log("Parent object: " + parentObject);
        transform.parent = weaponOwner.transform;
        weaponOwner.GetComponent<CombatControl>().SetWeapon(this);
        // Debug.Log("Parent set");
        weaponOwnerCollider = weaponOwner.GetComponentInChildren<Collider>();
        if (!weaponOwner.GetComponent<NetworkIdentity>().isLocalPlayer)
        {
            GetComponent<PointToCursor>().enabled = false;
        } else
        {
            Animator an = weaponOwner.GetComponentInChildren<Animator>();
            GetComponent<PointToCursor>().SetAnimator(an);
        }
    }

    public void SetParent(NetworkInstanceId newParent)
    {
        Debug.Log("Setting revolver's parent");
        this.weaponOwnerNetID = newParent;
    }
    // Use this for initialization
    void Start()
    {
        ammoLeftInClip = ammoPerClip;
    }

    void Update()
    {
        if (!weaponOwner.GetComponent<NetworkIdentity>().isLocalPlayer)
            return;
        if (Input.GetMouseButton(0))
        {
            
            ShootingRequest(); //sends a request to server to shoot
        }
    }

    public bool Cooldown()
    {
        return onCooldown;
    }

    public void SetCooldown(bool onCD)
    {
        onCooldown = onCD;
        Invoke("ClearCooldown", cooldown);
    }

    public void SetReload(bool onCD)
    {
        Debug.Log("reloading!");
        onCooldown = onCD;
        Invoke("ClearReload", reloadTime);
        reloadSound.Play();
    }

    public void ClearReload()
    {
        onCooldown = false;
        ammoLeftInClip = ammoPerClip;
    }

    public float GetDamage()
    {
        return damage;
    }

    //Runs on client
    public void ShootingRequest()
    {
        if (!Cooldown())
        {
            Debug.Log("Gun not on cooldown");
            Debug.Log("requesting to shoot");
            Vector3 pPos = weaponOwner.transform.position;
            Vector3 mPos = Input.mousePosition;
            //Debug.Log("Mouse: " + mPos);
            Vector3 mWorldPos = Camera.main.ScreenToWorldPoint(new Vector3(mPos.x, mPos.y, -Camera.main.transform.position.z));
            Vector3 shootDirection = (mWorldPos - pPos);
            shootDirection.z = 0; // just to be sure
            shootDirection.Normalize();
            Vector3 inacc = getInaccurasy();
            CmdShoot(barrelEnd.position, shootDirection + inacc); //server fires the real bullet
            Debug.Log("Calling local sim shoot");
            //StartCoroutine(LocalShootSim(barrelEnd.position, shootDirection + inacc)); //local player simulates it
            ammoLeftInClip--;
            if (ammoLeftInClip < 1)
            {
                SetReload(true);
            }
            else
            {
                SetCooldown(true); //setting cooldown also here on client so there wont be too many failed shooting requests.
            }
        }
    }

    public Vector3 getInaccurasy()
    {
        return (new Vector3(Random.value - 0.5f, Random.value - 0.5f, 0))*inaccurasy;
    }

    [Command]
    public virtual void CmdShoot(Vector3 from, Vector3 towards) //the real bullet that deals dmg
    {
        Debug.Log("cmd bang!");
        //StartCoroutine(DelayedShoot(from, towards));
        HandleShoot(from, towards);
        RpcShootSim(from, towards);
    }

    IEnumerator DelayedShoot(Vector3 from, Vector3 towards)
    {
        Debug.Log("Before delay");
        yield return new WaitForSeconds(0.005f);
        Debug.Log("After delay");
        HandleShoot(from, towards);
        Debug.Log("After delayed shoot");
    }

    [ClientRpc]
    public void RpcShootSim(Vector3 from, Vector3 towards) //non local simulation
    {
        if (weaponOwner.GetComponent<NetworkIdentity>().isServer)  
            return;
        Debug.Log("Rpc sim bang!");
        HandleShoot(from, towards);//non local clients

    }

    public virtual IEnumerator LocalShootSim(Vector3 from, Vector3 towards) //local simulation
    {
        Debug.Log("local sim bang 1!");
        if (!weaponOwner.GetComponent<NetworkIdentity>().isLocalPlayer || weaponOwner.GetComponent<NetworkIdentity>().isServer)
            yield return null;

        
        if (!onCooldown)
        {
            Debug.Log("local sim bang 2 before delay!");
            yield return new WaitForSeconds(Network.GetLastPing(Network.player));
            Debug.Log("local sim bang 2 after delay!");
            HandleShoot(from, towards);
        }
    }

    public virtual void HandleShoot(Vector3 from, Vector3 towards)
    {
        ShootBullet(from, towards);
    }
    
    public void ShootBullet(Vector3 from, Vector3 towards) //both server and sim
    {
        Debug.Log("Spawning a bullet");
        GameObject bullet = (GameObject)Instantiate(bulletPrefab, from, Quaternion.LookRotation(towards));
        Physics.IgnoreCollision(weaponOwnerCollider, bullet.GetComponent<Collider>()); //dont collide to local player
        Bullet bulletControl = bullet.GetComponent<Bullet>();
        bulletControl.setDamage(damage);
        bulletControl.ShotBy(weaponOwner);
        bulletControl.ShotBy(this);
        Rigidbody bulletRB = bullet.GetComponent<Rigidbody>();
        bulletRB.velocity = towards * bulletSpeed;
        Destroy(bullet, 3.0f);
        DropShell((this.transform.position + from) / 2);
        ShootSound();
        GetComponent<NetworkSyncLocalRotation>().cmc.TriggerShoot();
    }

    void DropShell(Vector3 from)
    {
        GameObject shell = (GameObject)Instantiate(shellPrefab, from, Quaternion.identity);
        Rigidbody shellRB = shell.GetComponent<Rigidbody>();
        Vector3 shellRandomDir = new Vector3(0.5f*(Random.value - 0.5f), 0, Random.value - 0.5f) * 35f;
        Vector3 shellDir = Vector3.up * 20 + shellRandomDir;
        shellRB.AddForce(shellDir * 10);
        shellRB.AddTorque(new Vector3(0.5f * (Random.value - 0.5f), 0, Random.value - 0.5f) * 35f);
        Destroy(shell, 5.0f);
    }

    public void ShootSound()
    {
        if(shootSound != null)
        {
            shootSound.Play();
        }   
    }

    void ClearCooldown()
    {
        onCooldown = false;
    }
}
