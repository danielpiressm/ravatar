using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateAbstractAvatar : MonoBehaviour {

    public const float ballSize = 0.1f;
    public const float boneSize = 0.05f;

    // Spine
    public Transform spineBase;
    public Transform spineShoulder;
    public Transform head;

    private Transform spineBaseJoint;
    private Transform spineShoulderJoint;
    private Transform headJoint;

    // Left arm
    public Transform leftShoulder;
    public Transform leftElbow;
    public Transform leftArm;

    private Transform leftShoulderJoint;
    private Transform leftElbowJoint;
    private Transform leftWristJoint;

    // Left leg
    public Transform leftHip;
    public Transform leftKnee;
    public Transform leftAnkle;

    private Transform leftHipJoint;
    private Transform leftKneeJoint;
    private Transform leftAnkleJoint;

    // Right arm
    public Transform rightShoulder;
    public Transform rightElbow;
    public Transform rightArm;

    private Transform rightShoulderJoint;
    private Transform rightElbowJoint;
    private Transform rightWristJoint;

    // Right leg
    public Transform rightHip;
    public Transform rightKnee;
    public Transform rightAnkle;


    private Transform rightHipJoint;
    private Transform rightKneeJoint;
    private Transform rightAnkleJoint;

    // Bones
    private Transform boneNeck, boneSpine;
    private Transform boneLeftShoulder, boneLeftArm, boneLeftForearm;
    private Transform boneRightShoulder, boneRightArm, boneRightForearm;
    private Transform boneLeftHip, boneLeftThigh, boneLeftCalf;
    private Transform boneRightHip, boneRightThigh, boneRightCalf;


    // Use this for initialization
    void Start () {
        spineBaseJoint = createAvatarJoint(spineBase, "spineBaseGO");
        headJoint = createAvatarJoint(head, "headJointGO");
	}

    Transform createAvatarJoint(Transform parent, string name, float scale = ballSize)
    {
        GameObject gameObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        gameObject.name = name;
        Rigidbody r = gameObject.AddComponent<Rigidbody>();
        r.isKinematic = true;
        r.useGravity = false;
        //Transform transform = Vector3.zero; //gameObject.transform;
        Transform transform = gameObject.transform;
        transform.parent = parent;
        transform.localPosition = Vector3.zero;
        transform.localScale *= scale;
        return transform;
    }

    // Update is called once per frame
    void Update () {
		
	}

    Transform createAvatarBone(Transform parent, float scale = boneSize)
    {
        GameObject gameObject = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        Transform transform = gameObject.transform;
        transform.parent = parent;
        return transform;
    }

    void updateAvatarBone(Transform bone, Vector3 joint1, Vector3 joint2)
    {
        if (bone == null)
            return;
        bone.localScale = new Vector3(boneSize, 0.5f * Vector3.Distance(joint1, joint2), boneSize);
        bone.position = (joint1 + joint2) * 0.5f;
        bone.up = joint2 - joint1;
    }
}
