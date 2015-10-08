using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class Bullet : NetworkBehaviour {

    float bulletDamage = 50;
    IWeapon daddy;

    public void setDamage(float damage)
    {
        bulletDamage = damage;
    }

    public void setDaddy(IWeapon newDad) //which gun shot this bullet
    {
        this.daddy = newDad;
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
