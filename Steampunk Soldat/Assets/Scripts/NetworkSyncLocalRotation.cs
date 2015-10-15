using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class NetworkSyncLocalRotation : NetworkBehaviour {

    [SyncVar]
    Quaternion currentLocalRot;

    NetworkIdentity owner;

	// Use this for initialization
	void Start () {
        currentLocalRot = Quaternion.identity;
        this.transform.localPosition = new Vector3(0, 0, 0);
    }

    [Command]
    void CmdSetRotation(Quaternion newRotation)
    {
        currentLocalRot = Quaternion.Slerp(currentLocalRot, newRotation, Time.time * 0.1f);
    }

    // Update is called once per frame
    int count = 0;
	void Update () {
        owner = transform.parent.GetComponent<NetworkIdentity>();
        if (owner.isLocalPlayer)
        {
            count++;
            currentLocalRot = this.transform.localRotation;
            if (!owner.isServer && count > 6)
            {
                CmdSetRotation(currentLocalRot);
                count = 0;
            }
            
            //Debug.Log("is local in rotation sync");
        }
        else
        {
            //Debug.Log("is not local in rotation sync");
            this.transform.localRotation = currentLocalRot;
        }
        
    }

}
