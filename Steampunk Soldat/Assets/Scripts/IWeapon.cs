using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

enum Weapon : int { Pistol = 0, MachineGun, Minigun };

public interface IWeapon {
    
    float GetDamage();
    bool Cooldown();
    void SetCooldown(bool onCD);
    void SetParent(NetworkInstanceId netID);
    [Command]
    void CmdShoot(Vector3 from, Vector3 towards);
    void ShootingRequest();
}
