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
    [SyncVar]
    public int playerPing = -2;

    static short IDCount = 1; //which id to give to next player

	// Use this for initialization
	void Start () {
        if (!isServer)
            return;
        SetPlayerID(); //the local player will get an ID from server that is synced to all
        if (isLocalPlayer)
        {
            InvokeRepeating("RefreshPing", 1f, 2f);
            
        }
    }

    void RefreshPing()
    {
        CmdUpdatePing(Network.GetAveragePing(Network.player));
    }
    
    [Server]
    void SetPlayerID()
    {
        if(GetComponent<EnemyAI>() != null)
        {
            playerID = 666;
            return;
        }
        playerID = IDCount++;
        Debug.Log("Server will now set new player to id " + playerID);
        
    }
	
	// Update is called once per frame
	void Update () {
        
	}

    [Command]
    void CmdUpdatePing(int newPing)
    {
        playerPing = newPing;
    }

}
