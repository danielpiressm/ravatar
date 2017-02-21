using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectShooter : MonoBehaviour {

    bool shot1 = false;
    bool shot2 = false;
    bool shot3 = false;
    bool shot4 = false;
    public GameObject ball = null;
    // Use this for initialization
    float elapsedTime = 0;
	void Start () {
        Debug.Log("Object shooter online!");
	}
	
	// Update is called once per frame
	void Update () {
        elapsedTime += Time.deltaTime;
        if(!shot1 && elapsedTime > 1)
        {
            GameObject s = Instantiate(ball, this.transform.position, Quaternion.identity);
            s.GetComponent<Rigidbody>().AddForce(new Vector3(0.4f,1.2f,1.0f) * 35);
            shot1 = true;
            Debug.Log("Shooting ball one");
        }
        if (!shot2 && elapsedTime > 2)
        {
            GameObject s = Instantiate(ball, this.transform.position, Quaternion.identity);
            s.GetComponent<Rigidbody>().AddForce(new Vector3(0.6f, 1.2f, 1.0f) * 35);
            shot2 = true;
            Debug.Log("Shooting ball two");
        }
        if (!shot3 && elapsedTime > 3)
        {
            GameObject s = Instantiate(ball, this.transform.position, Quaternion.identity);
            s.GetComponent<Rigidbody>().AddForce(new Vector3(0.4f, 1f, 1.0f) * 35);
            shot3 = true;
            Debug.Log("Shooting ball three");
        }
        if (!shot4 && elapsedTime > 4)
        {
            GameObject s = Instantiate(ball, this.transform.position, Quaternion.identity);
            s.GetComponent<Rigidbody>().AddForce(new Vector3(0.6f, 1f, 1.0f) * 35);
            shot4 = true;
            Debug.Log("Shooting ball four");
        }
        if(elapsedTime > 5)
        {
            shot1 = shot2 = shot3 = shot4 = false;
            elapsedTime = 0;
        }
    }
}
