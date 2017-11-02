using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PutCollisionTrigger : MonoBehaviour {

	// Use this for initialization
	void Start () {

        for (int i = 0; i < this.transform.childCount; i++)
        {
            GameObject currentObj = this.transform.GetChild(i).gameObject;

            BoxCollider[] boxColliders = currentObj.GetComponentsInChildren<BoxCollider>();
            MeshCollider[] meshColliders = currentObj.GetComponentsInChildren<MeshCollider>();
            foreach (BoxCollider bCollider in boxColliders)
            {
                if (bCollider.name.Contains("triggerObject"))
                {

                }
                else if (!bCollider.GetComponent<CollisionTrigger>())
                {
                    CollisionTrigger trigger = bCollider.gameObject.AddComponent<CollisionTrigger>();
                    trigger.Id = bCollider.transform.parent.name + "_" + bCollider.gameObject.name;
                    bCollider.isTrigger = true;
                }
                else
                {
                    CollisionTrigger trigger = bCollider.gameObject.GetComponent<CollisionTrigger>();
                    trigger.Id = bCollider.transform.parent.name + "_" + bCollider.gameObject.name;
                    bCollider.isTrigger = true;
                }
            }
            foreach (MeshCollider mCollider in meshColliders)
            {
                if (!mCollider.GetComponent<CollisionTrigger>())
                {
                    CollisionTrigger trigger = mCollider.gameObject.AddComponent<CollisionTrigger>();
                    trigger.Id = mCollider.transform.parent.parent.name + "_" + mCollider.transform.parent.name;
                    mCollider.inflateMesh = true;
                    mCollider.isTrigger = true;
                }
                else
                {
                    CollisionTrigger trigger = mCollider.gameObject.GetComponent<CollisionTrigger>();
                    trigger.Id = mCollider.transform.parent.parent.name + "_" + mCollider.transform.parent.name;
                    mCollider.inflateMesh = true;
                    mCollider.isTrigger = true;
                }
            }

        }

        
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
