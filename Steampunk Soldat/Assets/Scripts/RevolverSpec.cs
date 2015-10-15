using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class RevolverSpec : WeaponBase {

    public override void HandleShoot(Vector3 from, Vector3 towards)
    {
        ShootBullet(from + new Vector3(0,0.07f,0), towards);
        ShootBullet(from + new Vector3(0, -0.07f, 0), towards);
    }


}
