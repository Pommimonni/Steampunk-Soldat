using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {

    float bulletDamage = 50;
    //IWeapon shotBy;
    GameObject owner;
    //bool serverBullet = false;

    public GameObject hitWallFX;
    public GameObject hitPlayerFX;
    
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
        Transform otherParentTrans = otherCollider.transform.parent;
        GameObject otherParent = null;
        if(otherParentTrans != null)
        {
            otherParent = otherParentTrans.gameObject;
        }
        if (alreadyTriggered || (owner == otherParent))
        {
            return;
        }
        //Debug.Log("Bullet collided");
        GameObject other = otherCollider.transform.gameObject;
        if (other.layer == LayerMask.NameToLayer("Bullet"))
        {
            //Debug.Log("Bullet hit bullet");
            return;
        }
        alreadyTriggered = true; //avoid possible double triggers
        if (other.layer == LayerMask.NameToLayer("Ground"))
        {
            //Debug.Log("Bullet hit ground");
            GetComponent<RandomAudio>().Play();
            GameObject fx = (GameObject)GameObject.Instantiate(hitWallFX, gameObject.transform.position, Quaternion.Euler(GetComponent<Rigidbody>().velocity));
            Destroy(gameObject, 1f);
        }
        if (otherParent != null && otherParent.layer == LayerMask.NameToLayer("Player"))
        {
            //Debug.Log("Bullet hit player");
            if (LocalData.isServerPlayer)
            {
                //Debug.Log("Bullet hit player on SErver, applying dmg!");
                otherParent.GetComponentInParent<CombatControl>().TakeDamage(bulletDamage, owner);
            }
            GameObject fx = (GameObject)GameObject.Instantiate(hitPlayerFX, gameObject.transform.position, Quaternion.Euler(GetComponent<Rigidbody>().velocity));
            Destroy(gameObject);
            //play sound
        }
        
    }

}
