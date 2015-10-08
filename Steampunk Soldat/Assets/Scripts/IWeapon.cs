using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public interface IWeapon {
    float GetDamage();
    bool Cooldown();
    void SetCooldown(bool onCD);
    void Shoot(Vector3 from, Vector3 towards, Collider col);
}
