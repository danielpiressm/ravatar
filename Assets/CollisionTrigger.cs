using UnityEngine;
using System.Collections;

public class CollisionTrigger : MonoBehaviour {

    public string Id;
    float lastTime = 0;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    string Vec3ToStr(Vector3 vec,string separator)
    {
        return vec.x + separator + vec.y + separator + vec.z;
    }

    

    void OnTriggerEnter(Collider collider)
    {
        float currentTime = Time.realtimeSinceStartup;
        float triggerTime = currentTime - lastTime;
        lastTime = currentTime;
        Vector3 pos = new Vector3(collider.transform.position.x, collider.transform.position.y, collider.transform.position.z);
        Vector3 vec = collider.transform.position - this.transform.position;
        Debug.Log("Collision between  " + this.Id + " and " + collider.gameObject.name);
        string str = string.Join(",", new string[]
        {
            collider.gameObject.name,
            this.Id,
            pos.x.ToString(),
            pos.y.ToString(),
            pos.z.ToString(),
            this.transform.position.x.ToString(),
            this.transform.position.y.ToString(),
            this.transform.position.z.ToString()+"\n"
        });
        SendMessageUpwards("serializeCollision", str);
        

    }

    void OnCollisionEnter(Collision collider)
    {
        float currentTime = Time.realtimeSinceStartup;
        float triggerTime = currentTime - lastTime;
        lastTime = currentTime;
        Vector3 pos = new Vector3(collider.transform.position.x, collider.transform.position.y, collider.transform.position.z);
        Vector3 vec = collider.transform.position - collider.contacts[0].thisCollider.gameObject.transform.position;
        Debug.Log("$$Collision between  " + this.Id + " and " + collider.gameObject.name);
        string str = string.Join(",", new string[]
        {
            collider.gameObject.name,
            this.Id,
            pos.x.ToString(),
            pos.y.ToString(),
            pos.z.ToString(),
            this.transform.position.x.ToString(),
            this.transform.position.y.ToString(),
            this.transform.position.z.ToString()+"\n"
        });
        SendMessageUpwards("serializeCollision", str);

    }

    void OnTriggerStay(Collider collider)
    {
        Vector3 vec = collider.transform.position - this.transform.position;
       // collision.rigidbody.
        //Debug.Log("Contact 1 : " +collision.gameObject.name);
        //Debug.Log("Contact 2 : " + other.ClosestPointOnBounds(other.transform.position).ToString());
    }
}
