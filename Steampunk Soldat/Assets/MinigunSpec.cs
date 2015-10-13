using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class MinigunSpec : WeaponBase
{

    public WeaponBase weaponBase;
    public float kickback = 1;

    void Update()
    {
        if (!weaponOwner.GetComponent<NetworkIdentity>().isLocalPlayer)
            return;
        if (Input.GetMouseButton(0))
        {
            ShootingRequest(); //sends a request to server to shoot
        }
    }

    public new void ShootingRequest()
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
            CmdShoot(barrelEnd.position, shootDirection + getInaccurasy());
            this.weaponOwner.GetComponent<Rigidbody>().AddForce(-shootDirection*kickback);
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
    

}
