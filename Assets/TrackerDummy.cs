using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackerDummy : MonoBehaviour {

    TrackerClient client;


	// Use this for initialization
	void Start () {
        client = this.GetComponent<TrackerClient>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
