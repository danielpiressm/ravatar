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
       /* collisionLogStr = "Joint" + separator + "PosX" + separator + "PosY" + separator + "PosZ" + separator + "RotX" + separator + "RotY" + separator + "RotZ" + separator +
                        "ColliderName" + separator + "PosColliderX" + separator + "PosColliderY" + separator + "PosColliderZ" + separator + "RotColliderX" + separator + "RotColliderZ" +
                        "ErrorX" + separator + "ErrorY" + separator + "ErrorZ" + separator + "TimeElapsed" + "\n";*/
        float currentTime = Time.realtimeSinceStartup;
        float triggerTime = currentTime - lastTime;
        lastTime = currentTime;
        Vector3 pos = new Vector3(collider.transform.position.x, collider.transform.position.y, collider.transform.position.z);
        Vector3 rot = new Vector3(collider.transform.eulerAngles.x, collider.transform.eulerAngles.y, collider.transform.eulerAngles.z);
        Vector3 vec = collider.transform.position - this.transform.position;
        Debug.Log("Collision between  " + this.Id + " and " + collider.gameObject.name);
        if (collider.name == "Plane")
            return;
        string str = string.Join(",", new string[]
        {
            collider.gameObject.name,
            //this.Id,
            pos.x.ToString(),
            pos.y.ToString(),
            pos.z.ToString(),
            rot.x.ToString(),
            rot.y.ToString(),
            rot.z.ToString(),
            this.Id,
            this.transform.position.x.ToString(),
            this.transform.position.y.ToString(),
            this.transform.position.z.ToString(),
            this.transform.eulerAngles.x.ToString(),
            this.transform.eulerAngles.y.ToString(),
            this.transform.eulerAngles.z.ToString(),
            vec.x.ToString(),
            vec.y.ToString(),
            vec.z.ToString()
        });
        SendMessageUpwards("serializeCollision", str);
        

    }

    void OnCollisionEnter(Collision collider)
    {
        float currentTime = Time.realtimeSinceStartup;
        float triggerTime = currentTime - lastTime;
        lastTime = currentTime;
        string colliderName = collider.contacts[0].thisCollider.gameObject.name;
        Vector3 pos = new Vector3(collider.transform.position.x, collider.transform.position.y, collider.transform.position.z);
        Vector3 rot = new Vector3(collider.transform.eulerAngles.x, collider.transform.eulerAngles.y, collider.transform.eulerAngles.z);
        Vector3 vec = collider.transform.position - collider.contacts[0].thisCollider.gameObject.transform.position;
        Transform bounds = collider.contacts[0].thisCollider.gameObject.transform;
        Debug.Log("$$Collision between  " + this.Id + " and " + collider.gameObject.name);
        if (collider.gameObject.name == "Plane" || collider.gameObject.name.Contains("ground") || collider.gameObject.name == "triggerObject1" || collider.gameObject.name == "triggerObject2" )
            return;
        string str = string.Join(",", new string[]
        {
            collider.gameObject.name,
            //this.Id,
            pos.x.ToString(),
            pos.y.ToString(),
            pos.z.ToString(),
            rot.x.ToString(),
            rot.y.ToString(),
            rot.z.ToString(),
            this.Id,
            bounds.position.x.ToString(),
            bounds.position.y.ToString(),
            bounds.position.z.ToString(),
            bounds.eulerAngles.x.ToString(),
            bounds.eulerAngles.y.ToString(),
            bounds.eulerAngles.z.ToString(),
            vec.x.ToString(),
            vec.y.ToString(),
            vec.z.ToString()
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
