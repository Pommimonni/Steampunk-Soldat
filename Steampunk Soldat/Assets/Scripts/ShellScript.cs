using UnityEngine;
using System.Collections;

public class ShellScript : MonoBehaviour {

    public static int shellCount = 0;
	// Use this for initialization
	void Start () {
        ShellScript.shellCount++;
	}

    void OnDestroy()
    {
        ShellScript.shellCount--;
    }
	
	// Update is called once per frame
	void Update ()
    {

    }

    void OnCollisionEnter(Collision collision)
    {
        GameObject other = collision.collider.gameObject;
        if (other.layer == LayerMask.NameToLayer("Ground"))
        {
            GetComponent<RandomAudio>().Play();
        }
    }
}
