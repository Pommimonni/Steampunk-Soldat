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

    public IWeaponSpec weaponSpec; //for future use

    protected bool onCooldown = false;

    protected AudioSource shootingSound;

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
        shootingSound = GetComponent<AudioSource>();
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

    [ClientRpc]
    public void RpcShootingSound()
    {
        shootingSound.Play(); //bang
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
            Debug.Log("Combat Bang");
            Debug.Log("requesting to shoot");
            Vector3 pPos = weaponOwner.transform.position;
            Vector3 mPos = Input.mousePosition;
            //Debug.Log("Mouse: " + mPos);
            Vector3 mWorldPos = Camera.main.ScreenToWorldPoint(new Vector3(mPos.x, mPos.y, -Camera.main.transform.position.z));
            Vector3 shootDirection = (mWorldPos - pPos);
            shootDirection.z = 0; // just to be sure
            shootDirection.Normalize();
            CmdShoot(barrelEnd.position, shootDirection + getInaccurasy()); //server fires the gun
            if(ammoLeftInClip < 1)
            {
                SetReload(true);
            } else
            {
                SetCooldown(true); //setting cooldown also here on client so there wont be too many failed shooting requests.
            }
            
        }
    }

    public Vector3 getInaccurasy()
    {
        return (new Vector3(Random.value, Random.value, 0))*inaccurasy;
    }

    [Command]
    public void CmdShoot(Vector3 from, Vector3 towards)
    {
        if (!onCooldown)
        {
            Debug.Log("Revolver bang!");
            ShootBullet(from, towards);
            ammoLeftInClip--;
            if (ammoLeftInClip < 1)
            {
                SetReload(true);
            }
            else
            {
                SetCooldown(true); //setting cooldown also here on client so there wont be too many failed shooting requests.
            }
            RpcShootSound(); //called on all clients
        }

    }

    [Server]
    public void ShootBullet(Vector3 from, Vector3 towards)
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
        NetworkServer.Spawn(bullet);
    }

    //Server asks everyone to play sound
    [ClientRpc]
    public void RpcShootSound()
    {
        GetComponent<AudioSource>().Play();
    }

    void ClearCooldown()
    {
        onCooldown = false;
    }
}
