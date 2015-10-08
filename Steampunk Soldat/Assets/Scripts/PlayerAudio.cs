using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class PlayerAudio : NetworkBehaviour {

    AudioSource shootingSound;
	// Use this for initialization
	void Start () {
        shootingSound = GetComponentInChildren<AudioSource>();
	}
	
    [ClientRpc]
    public void RpcShootingSound()
    {
        shootingSound.Play(); //bang
    }

	// Update is called once per frame
	void Update () {
	
	}
}
