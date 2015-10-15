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
    
    bool throttleOn = false;

	// Use this for initialization
	void Start () {
        currentCharge = maxCharge;
        chargeBar.maxValue = maxCharge;
        chargeBar.value = maxCharge;
    }
	
	// Update is called once per frame
	void Update () {
        chargeBar.value = currentCharge;
        if (!isLocalPlayer)
            return;
        throttleOn = Input.GetMouseButton(1);
        if (throttleOn && (currentCharge > 0))
        {
            currentCharge -= spendRate;
        }
        else if(currentCharge < maxCharge && !throttleOn)
        {
            currentCharge += rechargeRate;
            if (currentCharge > maxCharge)
                currentCharge = maxCharge;
        }
        CmdInformCharge(currentCharge);
        if(!throttleOn && jetSound.isPlaying)
        {
            jetSound.Stop();
        }
        if (throttleOn && !jetSound.isPlaying)
        {
            jetSound.Play();
        }
    }

    [Command]
    void CmdInformCharge(float newCharge)
    {
        currentCharge = newCharge; //clients use jet slower, fix this
    }
    float yVel;
    void FixedUpdate()
    {
        if (throttleOn && (currentCharge > 0))
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
