using UnityEngine;
using System.Collections;

public enum Tasks { Task1, Task2, Task3, Completed };

public class TestTask : MonoBehaviour {

    public string collisionLogfileName = "collisionLog";
    public string logFileName = "log";
    public string pathLogFileName = "logPath";

    private string collisionLogStr = "";
    private string logStr = "";
    private string pathStr = "";

    public string separator = ";";

    public Tasks currentTask;

    
    public GameObject objectsTask1; 
    public GameObject objectsTask2;
    public GameObject objectsTask3;

    Vector3 lastPos;

    Camera _trackedObj;

    int currentTrigger = 0;

    float threshold = 0.05f;

    float lastTimeBetweenTasks = 0.0f;
    float lastTimeBetweenTriggers = 0.0f;

	// Use this for initialization
	void Start () {
        currentTask = Tasks.Task1;
        //with avatars, change for torso tracking
        _trackedObj = Camera.main;
        lastPos = _trackedObj.transform.position;
        InitializeReport(); 
	}
	
	// Update is called once per frame
	void Update () {
        if (currentTask != Tasks.Completed)
        {
            UpdatePathReport();
        }
    }

    void serializeCollision(string str)
    {
        //TODO: implement this
    }


    void InitializeReport()
    {
        collisionLogStr = "Joint" + separator + "PosX" + separator + "PosY" + separator + "PosZ" + separator + "RotX" + separator + "RotY" + separator + "RotZ" + separator +
                        "ColliderName" + separator + "PosColliderX" + separator + "PosColliderY" + separator + "PosColliderZ" + separator + "RotColliderX" + separator + "RotColliderZ" +
                        "ErrorX" + separator + "ErrorY" + separator + "ErrorZ" + separator+ "TimeElapsed"+"\n";
        logStr = "TriggerNum" + separator + "Time" + "\n";
        pathStr = "Task,Trigger,currentPosX,currentPosY,currentPosZ,pathElapsedX,pathElapsedY,pathElapsedZ,rotX,rotY,rotZ,magnitude\n";
    }

    void CompleteReport()
    {
        
        System.IO.File.WriteAllText(collisionLogfileName + ".csv",collisionLogStr);
        System.IO.File.WriteAllText(logFileName + ".csv", logStr);
        System.IO.File.WriteAllText(pathLogFileName + ".csv", pathStr);
      
    }

    void UpdateReport( )
    {
        float currentTime = Time.realtimeSinceStartup;
        logStr += currentTask.ToString() + "," + (currentTime - lastTimeBetweenTasks)+"\n";
        lastTimeBetweenTasks = currentTime;
    }

    void UpdatePathReport()
    {
        Vector3 currentPos = Camera.main.transform.position;
        Vector3 currentPosVector = currentPos - lastPos;
        if (currentPosVector.magnitude > threshold)
        {
            lastPos = Camera.main.transform.position;
            pathStr += string.Join(",", new string[]
            {
                currentTask.ToString(),
                currentTrigger.ToString(),
                _trackedObj.transform.position.x.ToString(),
                _trackedObj.transform.position.y.ToString(),
                _trackedObj.transform.position.z.ToString(),
                currentPosVector.x.ToString(),
                currentPosVector.y.ToString(),
                currentPosVector.z.ToString(),
                _trackedObj.transform.eulerAngles.x.ToString(),
                _trackedObj.transform.eulerAngles.y.ToString(),
                _trackedObj.transform.eulerAngles.z.ToString(),
                currentPosVector.magnitude.ToString(),
                "\n"

            }); 
                
            //countPointsInPath++;
            //cameraPath.Add(new Vector3(currentPos.x, currentPos.y, currentPos.z));

           
        }
        else
        {
            //do nothing
        }
    }

    void serializeCollision( )
    {

    }

    void SetActiveChildren(GameObject obj, bool state)
    {
        obj.SetActive(state);
        for(int i = 0; i < obj.transform.childCount; i++)
        {
            obj.transform.GetChild(i).gameObject.SetActive(state);
        }
    }

    void nextTask(string triggerId)
    {
        if(triggerId == "1")
        {
            if(currentTask == Tasks.Task2)
            {
                UpdateReport();
                currentTask = Tasks.Task3;
                objectsTask2.SetActive(false);
                objectsTask3.SetActive(true);
                //Debug.Log("TEST OVER");
                //CompleteReport();
            }
        }

        if(triggerId == "2")
        {
            if(currentTask == Tasks.Task1)
            {
                UpdateReport();
                currentTask = Tasks.Task2;
                SetActiveChildren(objectsTask1,false);
                SetActiveChildren(objectsTask2, true);
            }
            else if(currentTask == Tasks.Task3)
            {
                UpdateReport();
                currentTask = Tasks.Completed;
                CompleteReport();
            }
        }
    }
}
