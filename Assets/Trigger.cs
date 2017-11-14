using UnityEngine;
using System.Collections;

public class Trigger : MonoBehaviour {

    public string triggerId = "";

    public int countEnters = 0;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void receiveMessage(string triggerId)
    {
        if(triggerId == this.triggerId)
        {
            Debug.Log(this.triggerId + "receivedMessagedSuccessfully");
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag != "boneCube" && other.gameObject.tag != "jointSphere")
        {
            return;
        }
        if (countEnters == 0 || triggerId == "walkTrigger2")
            SendMessageUpwards("triggerPlus", triggerId);
        countEnters++;
        Debug.Log("countenters " + countEnters);
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag != "boneCube" && other.gameObject.tag != "jointSphere")
        {
            return;
        }
        countEnters--;
        if (countEnters == 0)
            SendMessageUpwards("startCounter", triggerId);
        Debug.Log("countenters " + countEnters);
        //SendMessageUpwards("startTheTask", triggerId);
    }
}
