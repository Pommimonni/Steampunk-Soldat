﻿using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class MinigunSpec : WeaponBase
{
    
    public float kickback = 1;
    public float chargeTime = 0.5f;
    public MultiPhaseAudio multiAudio;

    int shootingPhase = 1;

    bool buttonDown = false;
    float currentChargeTime = 0;
    

    void Update()
    {
        //to nullify base.Update();
    }

    void FixedUpdate()
    {
        if (!weaponOwner.GetComponent<NetworkIdentity>().isLocalPlayer)
            return;
        if (Input.GetMouseButton(0) && !reloading)
        {
            if (shootingPhase == 1 || shootingPhase == 0)
            {
                chargeShooting();
            }
            else
            {
                ShootingRequest(); //sends a request to server to shoot
            }
        }
        else
        {
            if (shootingPhase == 2)
            {
                CmdStopShootingSound();
                multiAudio.Play(3);
            }
            if (currentChargeTime > 0)
            {
                currentChargeTime -= Time.deltaTime;
            }
            else
            {
                currentChargeTime = 0;
                multiAudio.ResetToStart();
            }
            shootingPhase = 0;

        }
    }
    void chargeShooting()
    {
        if(shootingPhase == 0)
        {
            multiAudio.Play(1);
            shootingPhase = 1;
        } else
        {
            currentChargeTime += Time.deltaTime;
            if(currentChargeTime > chargeTime)
            {
                shootingPhase = 2;
                multiAudio.Play(2);
                CmdStartShootingSound();
                currentChargeTime = chargeTime;
            }
        }
    }


    public override void ShootSound()
    {
        //removes the basic shoot sound
    }

    public override void HandleShoot(Vector3 from, Vector3 towards)
    {
        if (weaponOwner.GetComponent<NetworkIdentity>().isLocalPlayer)
        {
            this.weaponOwner.GetComponent<Rigidbody>().AddForce(-towards * kickback);
        }
        ShootBullet(from, towards);
    }

    [Command]
    void CmdStartShootingSound()
    {
        RpcStartShootingSound();
    }
    [Command]
    void CmdStopShootingSound()
    {
        RpcStopShootingSound();
    }
    [ClientRpc]
    void RpcStartShootingSound()
    {
        if (isLocalPlayer)
            return;
        multiAudio.Play(2);
    }
    [ClientRpc]
    void RpcStopShootingSound()
    {
        if (isLocalPlayer)
            return;
        multiAudio.ResetToStart();
    }


    int count = 0;
    public override void DropShell(Vector3 from)
    {
        if(count < 3)
        {
            count++;
            return;
        }
        count = 0;
        GameObject shell = (GameObject)Instantiate(shellPrefab, from, Quaternion.identity);
        Rigidbody shellRB = shell.GetComponent<Rigidbody>();
        Vector3 shellRandomDir = new Vector3(0.5f * (Random.value - 0.5f), 0, Random.value - 0.5f) * 35f;
        Vector3 shellDir = Vector3.up * 20 + shellRandomDir;
        shellRB.AddForce(shellDir * 10);
        shellRB.AddTorque(new Vector3(0.5f * (Random.value - 0.5f), 0, Random.value - 0.5f) * 35f);
        Destroy(shell, 2.5f);
    }

}
