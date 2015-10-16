using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {

    float bulletDamage = 50;
    //IWeapon shotBy;
    GameObject owner;
    //bool serverBullet = false;

    

    public void setDamage(float damage)
    {
        bulletDamage = damage;
    }

    public void ShotBy(IWeapon newWeapon) //which gun shot this bullet
    {
        //this.shotBy = newWeapon;
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

    bool alreadyTriggered = false;

    void OnTriggerEnter(Collider otherCollider) //local sim just destroys the bullet, server applies dmg
    {
        if (alreadyTriggered)
        {
            return;
        }
        Debug.Log("Bullet collided");
        GameObject other = otherCollider.gameObject;
        if (other.layer == LayerMask.NameToLayer("Bullet"))
        {
            return;
        }
        alreadyTriggered = true; //avoid possible double triggers
        if (other.layer == LayerMask.NameToLayer("Ground"))
        {
            Debug.Log("Bullet hit ground");
            GetComponent<AudioSource>().Play();
            Destroy(gameObject, 1f);
        }
        if (other.layer == LayerMask.NameToLayer("Player"))
        {
            Debug.Log("Bullet hit player");
            if (LocalData.isServerPlayer)
            {
                Debug.Log("Bullet hit player on SErver, applying dmg!");
                other.GetComponentInParent<CombatControl>().TakeDamage(bulletDamage, owner);
            }
            Destroy(gameObject);
            //play sound
        }
        
    }

}
