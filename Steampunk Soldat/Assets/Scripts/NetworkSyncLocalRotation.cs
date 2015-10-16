using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class NetworkSyncLocalRotation : NetworkBehaviour {

    [SyncVar]
    Quaternion currentLocalRot;

    NetworkIdentity owner;

    public CharacterModelControl cmc;

	// Use this for initialization
	void Start () {
        currentLocalRot = Quaternion.identity;
        this.transform.localPosition = new Vector3(0, 0, 0);
        owner = transform.parent.GetComponent<NetworkIdentity>();
        cmc = transform.parent.gameObject.GetComponentInChildren<CharacterModelControl>();
    }

    [Command]
    void CmdSetRotation(Quaternion newRotation)
    {
        currentLocalRot = Quaternion.Slerp(currentLocalRot, newRotation, Time.time * 0.1f);
    }

    // Update is called once per frame
    int count = 0;
	void Update () {
        if (owner.isLocalPlayer)
        {
            count++;
            currentLocalRot = this.transform.localRotation;
            if (!owner.isServer && count > 6)
            {
                CmdSetRotation(currentLocalRot);
                count = 0;
            }

            cmc.SetArmDirection(this.transform.localRotation);
            //Debug.Log("is local in rotation sync");
        }
        else
        {
            //Debug.Log("is not local in rotation sync");
            this.transform.localRotation = currentLocalRot;
            cmc.SetArmDirection(this.transform.localRotation);
        }
        
    }

}
