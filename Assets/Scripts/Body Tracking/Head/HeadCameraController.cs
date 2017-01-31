using UnityEngine;
using UnityEngine.VR;
using System.Collections;

/// <summary>
/// This class is responsible for updating the caracter head 
/// rotation according to the HMD (Head Mounted Display) rotation.
/// </summary>
public class HeadCameraController : MonoBehaviour 
{
    public Vector3 threshold;
    public Transform CarlHip;

    public Transform headTransform;

    public bool thirdPerson;

    private Transform pivot1st;
    private Transform pivot3rd;
    private Transform pivot3rdAux;

    void Start()
    {
        GameObject go = new GameObject();
        pivot1st = go.transform;
        pivot1st.parent = headTransform;
        pivot1st.localPosition = threshold;


        go = new GameObject();
        pivot3rdAux = go.transform;
        pivot3rdAux.parent = headTransform;
        pivot3rdAux.localPosition = new Vector3(0, 2.54f, 0); // PARAMETRIZAR

        go = new GameObject();
        pivot3rd = go.transform;
        pivot3rd.parent = pivot3rdAux;
        pivot3rd.localPosition = new Vector3(0, 0, -1); // PARAMETRIZAR
    }

 	void LateUpdate()
	{


        CarlHip.position = new Vector3(UnityEngine.VR.InputTracking.GetLocalPosition(UnityEngine.VR.VRNode.Head).x,
            CarlHip.position.y,
            UnityEngine.VR.InputTracking.GetLocalPosition(UnityEngine.VR.VRNode.Head).z);

        pivot3rdAux.rotation = UnityEngine.VR.InputTracking.GetLocalRotation(UnityEngine.VR.VRNode.CenterEye);
        pivot3rdAux.eulerAngles = new Vector3(0, pivot3rd.eulerAngles.y, 0);

        this.transform.position = new Vector3((-UnityEngine.VR.InputTracking.GetLocalPosition(UnityEngine.VR.VRNode.Head).x),
                 (-UnityEngine.VR.InputTracking.GetLocalPosition(UnityEngine.VR.VRNode.Head).y),
                 (-UnityEngine.VR.InputTracking.GetLocalPosition(UnityEngine.VR.VRNode.Head).z)) +
                 (thirdPerson? pivot3rd.position : pivot1st.position);

    }
}