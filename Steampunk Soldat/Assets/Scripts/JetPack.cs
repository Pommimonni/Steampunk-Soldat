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
    public AudioSource jetSound;

    [SyncVar]
    float currentCharge;

    float currentLocalCharge;
    
    bool throttleOn = false;

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
        chargeBar.value = currentCharge;
        if (!isLocalPlayer)
        {
            return;
        }
        chargeBar.value = currentLocalCharge;
        throttleOn = Input.GetMouseButton(1);
        if (throttleOn && (currentLocalCharge > 0))
        {
            currentLocalCharge -= spendRate;
        }
        else if(currentLocalCharge < maxCharge && !throttleOn)
        {
            currentLocalCharge += rechargeRate;
            if (currentLocalCharge > maxCharge)
                currentLocalCharge = maxCharge;
        }
        
        if(!throttleOn && jetSound.isPlaying)
        {
            jetSound.Stop();
        }
        if (throttleOn && !jetSound.isPlaying)
        {
            jetSound.Play();
        }
        if (count < 12)
        {
            count++;
        } else
        {
            count = 0; //every 12th frame
            CmdInformCharge(currentLocalCharge);
        }
    }

    [Command]
    void CmdInformCharge(float newCharge)
    {
        currentCharge = Mathf.Lerp(currentCharge, newCharge, Time.time * 0.2f);
    }
    float yVel;
    void FixedUpdate()
    {
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
}
