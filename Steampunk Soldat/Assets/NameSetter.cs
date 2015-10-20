using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class NameSetter : MonoBehaviour {

    public static string setName = "Player X";
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	    if(setName != "Player X")
        {
            this.GetComponent<Text>().text = setName;
        }
	}
}
