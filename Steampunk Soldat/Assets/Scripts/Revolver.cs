using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System;

public class Revolver : NetworkBehaviour, IWeapon {

    public float damage = 50;
    public float bulletSpeed = 35f;
    public float cooldown = 1f;
    public GameObject bulletPrefab;

    bool onCooldown = false;

    // Use this for initialization
    void Start()
    {
        //if(!isLocalPlayer)
            //NetworkServer.Spawn(this.gameObject);
    }

    public float GetDamage()
    {
        return damage;
    }
    
    //Call only on server
    public void Shoot(Vector3 from, Vector3 towards, Collider col)
    {
        if (!onCooldown)
        {
            //Debug.Log("Revolver bang!");
            GameObject bullet = (GameObject)Instantiate(bulletPrefab, from, Quaternion.LookRotation(towards));
            Physics.IgnoreCollision(col, bullet.GetComponent<Collider>());
            Bullet bulletControl = bullet.GetComponent<Bullet>();
            bulletControl.setDamage(damage);
            Rigidbody bulletRB = bullet.GetComponent<Rigidbody>();
            bulletRB.velocity = towards * bulletSpeed;
            Destroy(bullet, 3.0f);
            NetworkServer.Spawn(bullet);
            onCooldown = true;
            Invoke("ClearCooldown", cooldown);
            RpcShootSound();
        }
        
    }
    
    [ClientRpc]
    public void RpcShootSound()
    {
        GetComponent<AudioSource>().Play();
    }

    void ClearCooldown()
    {
        onCooldown = false;
    }
    
	
	
	// Update is called once per frame
	void Update () {
	
	}
}
