using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class MinigunSpec : WeaponBase
{
    
    public float kickback = 1;
    
    
    public override void HandleShoot(Vector3 from, Vector3 towards)
    {
        if (weaponOwner.GetComponent<NetworkIdentity>().isLocalPlayer)
        {
            this.weaponOwner.GetComponent<Rigidbody>().AddForce(-towards * kickback);
        }
        ShootBullet(from, towards);
    }


}
