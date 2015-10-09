using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class Bullet : NetworkBehaviour {

    float bulletDamage = 50;
    IWeapon shotBy;
    GameObject owner;

    public void setDamage(float damage)
    {
        bulletDamage = damage;
    }

    public void ShotBy(IWeapon newWeapon) //which gun shot this bullet
    {
        this.shotBy = newWeapon;
    }

    public void ShotBy(GameObject shooter) //which player shot this bullet
    {
        this.owner = shooter;
    }

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void FixedUpdate()
    {
        this.transform.rotation = Quaternion.LookRotation(this.GetComponent<Rigidbody>().velocity);
    }

    void OnTriggerEnter(Collider otherCollider)
    {
        if (!isServer) //server/host handles the bullets
            return;
        GameObject other = otherCollider.gameObject;
        if (other.layer == LayerMask.NameToLayer("Ground"))
        {
            Destroy(gameObject);
        }
        if (other.layer == LayerMask.NameToLayer("Player"))
        {
            //Debug.Log("hit");
            other.GetComponentInParent<CombatControl>().TakeDamage(bulletDamage);
            Destroy(gameObject);
            //play sound
        }
    }
}
