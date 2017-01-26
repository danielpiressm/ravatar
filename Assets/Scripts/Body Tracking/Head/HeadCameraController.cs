using UnityEngine;
using UnityEngine.VR;
using System.Collections;

/// <summary>
/// This class is responsible for updating the caracter head 
/// rotation according to the HMD (Head Mounted Display) rotation.
/// </summary>
public class HeadCameraController : MonoBehaviour 
{
	public Transform cameraParent;
    public Vector3 threshold;
    public Transform CarlHip;

    void Start()
    {
        
    }

 	void LateUpdate()
	{


        CarlHip.position = new Vector3(UnityEngine.VR.InputTracking.GetLocalPosition(UnityEngine.VR.VRNode.CenterEye).x,
            CarlHip.position.y,
            UnityEngine.VR.InputTracking.GetLocalPosition(UnityEngine.VR.VRNode.CenterEye).z);

        //this.transform.position = cameraParent.transform.position;
        this.transform.position = new Vector3((-UnityEngine.VR.InputTracking.GetLocalPosition(UnityEngine.VR.VRNode.CenterEye).x) + threshold.x,
                 (-UnityEngine.VR.InputTracking.GetLocalPosition(UnityEngine.VR.VRNode.CenterEye).y) + threshold.y,
                 (-UnityEngine.VR.InputTracking.GetLocalPosition(UnityEngine.VR.VRNode.CenterEye).z) + threshold.z) +
                 cameraParent.transform.position;
    }
}