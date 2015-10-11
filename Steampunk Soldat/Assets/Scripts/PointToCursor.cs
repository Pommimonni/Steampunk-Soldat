using UnityEngine;
using System.Collections;

public class PointToCursor : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        Debug.Log("Combat Bang");
        Debug.Log("requesting to shoot");
        Vector3 pPos = transform.parent.position;
        Vector3 mPos = Input.mousePosition;
        //Debug.Log("Mouse: " + mPos);
        Vector3 mWorldPos = Camera.main.ScreenToWorldPoint(new Vector3(mPos.x, mPos.y, -Camera.main.transform.position.z));

        if(mWorldPos.x < pPos.x)
        {
            //this.transform.localPosition = new Vector3(-1, 0, 0);
        } else
        {
            //this.transform.localPosition = new Vector3(1, 0, 0);
        }

        Vector3 shootDirection = (mWorldPos - pPos);
        shootDirection.z = 0; // just to be sure
        shootDirection.Normalize();
        this.transform.LookAt(mWorldPos);
    }
}
