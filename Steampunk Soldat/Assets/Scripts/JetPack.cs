using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using UnityEngine.UI;

public class JetPack : NetworkBehaviour {

    public float maxCharge;
    public float power;
    public float rechargeRate;
    public float spendRate;
    public float maxYVelocity;
    public Slider chargeBar;
    public MultiPhaseAudio jetSound;
    public CharacterModelControl cmc;
    public ParticleSystem fire;
    public ParticleSystem smoke;

    [SyncVar]
    float currentCharge;

    float currentLocalCharge;
    
    bool throttleOn = false;

    int soundPhase = 0;

	// Use this for initialization
	void Start () {
        currentCharge = maxCharge;
        currentLocalCharge = maxCharge;
        chargeBar.maxValue = maxCharge;
        chargeBar.value = maxCharge;
    }

    int count = 0;
	// Update is called once per frame
	void Update () {
        if (!isLocalPlayer)
        {
            chargeBar.value = currentCharge;
        } else
        {
            chargeBar.value = currentLocalCharge;
        }
    }

    [Command]
    void CmdInformCharge(float newCharge)
    {
        currentCharge = Mathf.Lerp(currentCharge, newCharge, Time.time * 0.2f);
    }
    float yVel;
    float soundChargeTime = 0.191f;
    float currentSoundCharge = 0;
    void FixedUpdate()
    {
        if (!isLocalPlayer)
        {
            return;
        }
        throttleOn = Input.GetMouseButton(1);
        if (throttleOn && (currentLocalCharge > 0))
        {
            currentLocalCharge -= spendRate;
            if (cmc.IsJumpOn())
            {
                cmc.SetJumpAnimation(false);
            }
        }
        else if (currentLocalCharge < maxCharge && !throttleOn)
        {
            currentLocalCharge += rechargeRate;
            if (currentLocalCharge > maxCharge)
                currentLocalCharge = maxCharge;
        }
        if(currentLocalCharge <= 1 && jetSound.IsPlaying())
        {
            jetSound.ResetToStart();
            CmdStopSound();
            fire.Stop();
            smoke.Stop();
        }

        if (!throttleOn && (soundPhase == 2 || soundPhase == 1))
        {
            soundPhase = 0;
            jetSound.PlayOnce(3);
            CmdStopSound();
            fire.Stop();
            smoke.Stop();
        }
        if (throttleOn && currentLocalCharge > 0)
        {
            if(soundPhase == 0)
            {
                currentSoundCharge = 0;
                soundPhase = 1;
                jetSound.PlayOnce(soundPhase);
            } else if (soundPhase == 1)
            {
                currentSoundCharge += Time.fixedDeltaTime;
                if(currentSoundCharge > soundChargeTime)
                {
                    soundPhase = 2;
                    jetSound.PlayLooped(soundPhase);
                    CmdStartSound(); //start sound over network
                    fire.Play();
                    smoke.Play();
                }
            }
            
        }
        if (count < 12)
        {
            count++;
        }
        else
        {
            count = 0; //every 12th frame
            CmdInformCharge(currentLocalCharge);
        }
        if (throttleOn && (currentLocalCharge > 0))
        {
            yVel = GetComponent<Rigidbody>().velocity.y;
            if(yVel < maxYVelocity)
            {
                Vector3 jetForce = new Vector3(0, power, 0);
                GetComponent<Rigidbody>().AddForce(jetForce);
            }
        }
    }
    [Command]
    void CmdStartSound()
    {
        RpcStartSound();
    }
    [Command]
    void CmdStopSound()
    {
        RpcStopSound();
    }
    [ClientRpc]
    void RpcStartSound()
    {
        if (isLocalPlayer)
            return;
        jetSound.Play(2);
        fire.Play();
        smoke.Play();
    }
    [ClientRpc]
    void RpcStopSound()
    {
        if (isLocalPlayer)
            return;
        jetSound.ResetToStart();
        fire.Stop();
        smoke.Stop();
    }
}
