using UnityEngine;
using System.Collections;

public class RevolverSpec : WeaponBase {

    public new void ShootingRequest()
    {
        if (!Cooldown())
        {
            Debug.Log("Revolver Spec Bang");
            Debug.Log("requesting to shoot");
            Vector3 pPos = weaponOwner.transform.position;
            Vector3 mPos = Input.mousePosition;
            //Debug.Log("Mouse: " + mPos);
            Vector3 mWorldPos = Camera.main.ScreenToWorldPoint(new Vector3(mPos.x, mPos.y, -Camera.main.transform.position.z));
            Vector3 shootDirection = (mWorldPos - pPos);
            shootDirection.z = 0; // just to be sure
            shootDirection.Normalize();
            CmdShoot(pPos + 0.5f * shootDirection, shootDirection); //server fires the gun
            SetCooldown(true); //setting cooldown also here on client so there wont be too many failed shooting requests.
        }
    }

}
