using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class PlayerScore : NetworkBehaviour {

    [SyncVar]
    public int kills = 0;
    [SyncVar]
    public int deaths = 0;
    [SyncVar]
    public short playerID = -1;

    short IDCount = 1;

	// Use this for initialization
	void Start () {
        if (!isLocalPlayer)
            return;
        CmdSetPlayerID(); //the local player will get an ID from server that is synced to all
    }

    [Command]
    void CmdSetPlayerID()
    {
        playerID = IDCount++;
        Debug.Log("Server will now set player to id " + playerID);
        
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
