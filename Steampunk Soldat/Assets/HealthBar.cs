using UnityEngine;
using System.Collections;

public class HealthBar : MonoBehaviour {

    public GameObject parent;
    public CombatControl comCon;
	// Use this for initialization
	void Awake () {
        comCon = parent.GetComponent<CombatControl>();
	}
	
	// Update is called once per frame
	void Update () {
        var wantedPos = Camera.main.WorldToScreenPoint(parent.transform.position+new Vector3(0,0.8f,0));
        transform.position = wantedPos;
    }
}
