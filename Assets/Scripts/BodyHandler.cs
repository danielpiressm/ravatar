using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum BodyLog { rightFoot, leftFoot, rightHand, leftHand, head, torso, rightShin, leftShin, circle, leftUpLeg, rightUpLeg, rightForearm, leftForearm, rightArm, leftArm };

public static class ObjectExtensions
{
    public static string GetVariableName<T>(this T obj)
    {
        System.Reflection.PropertyInfo[] objGetTypeGetProperties = obj.GetType().GetProperties();

        if (objGetTypeGetProperties.Length == 1)
            return objGetTypeGetProperties[0].Name;
        else
            throw new ArgumentException("object must contain one property");
    }
}

public class BodyPart
{
    public Transform transform;
    public string strToPersist;

    public BodyPart(Transform transform,string header)
    {
        this.transform = transform;
        strToPersist += header;
    }

    public void clearString()
    {
        strToPersist = "";
    }

    public void appendToString(string strToAppend)
    {
        strToPersist += strToAppend;
    }
}


public class BodyHandler : MonoBehaviour {

    Transform head;

    Dictionary<string, BodyPart> dictionaryBody;
    GameObject[] bodyPartsGO;
    GameObject[] jointSpheresGO;


    public bool abstractAvatar;
    public bool logBody;

    string header = "";

    public TestTask tTask;

    int countStrings = 0;

    // Use this for initialization
    void Start () {
        dictionaryBody = new Dictionary<string, BodyPart>();

        tTask = GameObject.Find("triggerObjects").GetComponent<TestTask>();
        header = "Task,PosX,PosY,PosZ,RotX,RotY,RotZ,HeadPosX,HeadPosY,HeadPosZ,HeadRotX,HeadRotY,HeadRotZ,Time,Task\n";

        jointSpheresGO = GameObject.FindGameObjectsWithTag("jointSphere");
        bodyPartsGO = GameObject.FindGameObjectsWithTag("boneCube");

        foreach(GameObject bodyGO in bodyPartsGO)
        {
            int index = bodyGO.name.IndexOf("Cube");
            string bodyPartName = "";
            if (index > 0)
                bodyPartName = bodyGO.name.Substring(0, index);
            else
                bodyPartName = bodyGO.name;
            //Debug.Log("bodyGO = " + bodyGO.name + " name = " + index);
            if(bodyGO.name.Contains("head"))
            {
                head = bodyGO.transform;
            }
            bodyGO.GetComponent<BoxCollider>().isTrigger = true;
            dictionaryBody.Add(bodyPartName, new BodyPart(bodyGO.transform, header));
        }
        
    }

    bool isAFinger(GameObject go)
    {
        if(go.name.Contains("Index") || go.name.Contains("Thumb") || go.name.Contains("Ring") || go.name.Contains("Pinky") || go.name.Contains("Middle"))
        {
            return true;
        }
        else
            return false;
    } 
	
	// Update is called once per frame
	void Update ()
    {
        foreach (GameObject sphere in jointSpheresGO)
        {
            if (abstractAvatar)
            {
                sphere.GetComponent<MeshRenderer>().enabled = true;
            }
            else
            {
                sphere.GetComponent<MeshRenderer>().enabled = false;
            }
        }

        HeadCameraController headControl = Camera.main.transform.parent.gameObject.GetComponent<HeadCameraController>();


        foreach (KeyValuePair<string, BodyPart> b in dictionaryBody)
        {
            if (abstractAvatar)
            {
                GameObject go = b.Value.transform.gameObject;
                go.GetComponent<MeshRenderer>().enabled = true;
                if(go.name.Contains("head"))
                {
                    if (headControl.thirdPerson)
                        head.GetComponent<MeshRenderer>().enabled = true;
                    else
                        head.GetComponent<MeshRenderer>().enabled = false;
                }

            }
            else
            {
                GameObject go = b.Value.transform.gameObject;
                go.GetComponent<MeshRenderer>().enabled = false;

            }
            float timeToNextWrite = 0;
            int countBodyParts = dictionaryBody.Count;


            if (logBody && !isAFinger(b.Value.transform.gameObject))
            {
                string str =
                     tTask.getCurrentTask() + "," +
                     b.Value.transform.position.x.ToString() + "," +
                     b.Value.transform.position.y.ToString() + "," +
                     b.Value.transform.position.z.ToString() + "," +
                     b.Value.transform.eulerAngles.x.ToString() + "," +
                     b.Value.transform.eulerAngles.y.ToString() + "," +
                     b.Value.transform.eulerAngles.z.ToString() + "," +
                     head.transform.position.x.ToString() + "," +
                     head.transform.position.y.ToString() + "," +
                     head.transform.position.z.ToString() + "," +
                     head.transform.eulerAngles.x.ToString() + "," +
                     head.transform.eulerAngles.y.ToString() + "," +
                     head.transform.eulerAngles.z.ToString() + "," +
                     Time.realtimeSinceStartup.ToString() + "," +
                     tTask.getCurrentTask()+
                     "\n";
                



                b.Value.appendToString(str);

                float timeToWrite = 0.0f;
                countStrings++;
                countBodyParts--;
                if (countStrings > 500)
                {
                    //writeToFile
                    //System.IO.File.AppendAllText(tTask.getPathDirectory() + "/" + b.Key +".csv" , b.Value.strToPersist);
                    StartCoroutine(printToFile(b.Value.strToPersist, b.Key, timeToNextWrite));
                    b.Value.clearString();
                    timeToWrite += 0.5f;
                    //countStrings = 0;
                }
                if(countBodyParts <= 0)
                {
                    countStrings = 0;
                }
            }

        }

    }

    private IEnumerator printToFile(string str, string path, float time)
    {
        yield return new WaitForSeconds(time);
        System.IO.File.AppendAllText(tTask.getPathDirectory() + "/fullbodyLog/" + path + ".csv", str);
        yield return null;
    }
}
