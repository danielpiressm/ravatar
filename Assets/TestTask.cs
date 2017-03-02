using UnityEngine;
using System.Collections;
using System.IO;

public enum Tasks { Task1, Task2, Task3, Task4, ReachGreenLollipop0thTime, ReachRedLollipop1stTime, ReachRedLollipop2ndTime, ReachGreenLollipop1stTime, ReachGreenLollipop2ndTime, ThrowingObjects, Completed };

public enum BodyLog { head, hip, torso, rightHand, leftHand, rightFoot, leftFoot, rightShin, leftShin };

public enum AvatarType { carlFirstPerson, carlThirdPerson, robotFirstPerson, robotThirdPerson, pointCloudFirstPerson, pointCloudThirdPerson };

public class TestTask : MonoBehaviour {

    public AvatarType avatarType;

    public string collisionLogfileName = "collisionLog";
    public string logFileName = "log";
    public string pathLogFileName = "logPath";

    private string pathDirectory = "";

    private string collisionLogStr = "";
    private string logStr = "";
    private string pathStr = "";

    public string separator = ";";

    public Tasks currentTask;

    
    public GameObject objectsTask1; 
    public GameObject objectsTask2;
    public GameObject objectsTask3;
    public GameObject objectTask4;

    Vector3 lastPos;

    Camera _trackedObj;

    public GameObject head;
    public GameObject rightHand;
    public GameObject leftHand;
    public GameObject rightFoot;
    public GameObject leftFoot;
    public GameObject rightShin; //shin=canela
    public GameObject leftShin;

    private Vector3[] lastBodyPos;
    private string[] bodyStr;
    private string[] bodyStrPath;

    public bool fullbodyLog = false;

    int currentTrigger = 0;

    float threshold = 0.05f;

    float lastTimeBetweenTasks = 0.0f;
    float lastTimeBetweenTriggers = 0.0f;
    float lastTimeBetweenCollisions = 0.0f;
    float startTime = 0.0f;

	// Use this for initialization
	void Start () {
        //currentTask = Tasks.Task1;
        //uncomment this for the task
        currentTask = Tasks.ReachGreenLollipop0thTime;
        //with avatars, change for torso tracking
        _trackedObj = Camera.main;
        lastPos = _trackedObj.transform.position;
        InitializeReport();
        InitializeFullbodyReport();
        int i = 1;
        
        while(Directory.Exists(Directory.GetCurrentDirectory()+"/user"+i+avatarType.ToString()))
        {
            i++;
        }
        //se nao houver diretorios
        
        System.IO.Directory.CreateDirectory(Directory.GetCurrentDirectory() + "/user"+i+ avatarType.ToString());
        pathDirectory = Directory.GetCurrentDirectory() + "/user" + i + avatarType.ToString() + "/";
            
        
        
        
        Debug.Log("log");
	}
	
	// Update is called once per frame
	void Update () {
        if (currentTask != Tasks.Completed )
        {
            UpdatePathReport();
        }
        
    }

    void serializeCollision(string str)
    {
        float currentTime = Time.realtimeSinceStartup;
        logStr += currentTask.ToString() + "," + (currentTime - lastTimeBetweenCollisions) +  currentTime + "\n";
        //lastTimeBetweenTasks = currentTime;
        collisionLogStr += str + "\n";
        //Debug.Log("&&" + str);
    }

    void serializeBallCollision(string str)
    {
        float currentTime = Time.realtimeSinceStartup;
        logStr += currentTask.ToString() + "," + (currentTime - lastTimeBetweenCollisions) + currentTime + "\n";
        //lastTimeBetweenTasks = currentTime;
        collisionLogStr += str;
        //Debug.Log("&&" + str);
    }

    void logBallThrow(float time)
    {
        float lastTimeBetweenCollisions = time;
    }


    void InitializeReport()
    {
        collisionLogStr = "Joint" + separator + "PosX" + separator + "PosY" + separator + "PosZ" + separator + "RotX" + separator + "RotY" + separator + "RotZ" + separator +
                        "ColliderName" + separator + "PosColliderX" + separator + "PosColliderY" + separator + "PosColliderZ" + separator + "RotColliderX" + separator + "RotColliderZ" +
                        "ErrorX" + separator + "ErrorY" + separator + "ErrorZ" + separator+ "TimeElapsed"+ separator + "CurrentTime"+"\n";
        logStr = "TriggerNum" + separator + "Time" + "\n";
        pathStr = "Task,Trigger,currentPosX,currentPosY,currentPosZ,pathElapsedX,pathElapsedY,pathElapsedZ,rotX,rotY,rotZ,magnitude\n";
    }

    void CompleteReport()
    {

        logStr += "TotalTime" + Time.realtimeSinceStartup;
        System.IO.File.WriteAllText( pathDirectory+"/"+ collisionLogfileName + ".csv",collisionLogStr);
        System.IO.File.WriteAllText( pathDirectory+"/"+logFileName + ".csv", logStr);
        if(fullbodyLog)
        {
            for (int i = 0; i < 9; i++)
            {
                System.IO.File.WriteAllText(pathDirectory + "/" + bodyStrPath[i], bodyStr[i]);
            }
            
        }
        else
        {
            System.IO.File.WriteAllText(pathDirectory + "/" + pathLogFileName + ".csv", pathStr);
        }
        
      
    }

    void UpdateReport( )
    {
        float currentTime = Time.realtimeSinceStartup;
        logStr += currentTask.ToString() + "," + (currentTime - lastTimeBetweenTasks)+"\n";
        lastTimeBetweenTasks = currentTime;
        lastTimeBetweenCollisions = currentTime;
        lastTimeBetweenTriggers = currentTime;
    }

    

    void InitializeFullbodyReport()
    {
        lastBodyPos = new Vector3[9];
        bodyStrPath = new string[9];
        bodyStr = new string[9];
        if(rightFoot!=null)
        {
            lastBodyPos[(int)BodyLog.rightFoot] = rightFoot.transform.position;
            bodyStrPath[(int)BodyLog.rightFoot] = "rightFootLog.csv";
            //create a file for each part of the body
        }
        if(leftFoot!=null)
        {
            //log this
            lastBodyPos[(int)BodyLog.leftFoot] = leftFoot.transform.position;
            bodyStrPath[(int)BodyLog.leftFoot] = "leftFootLog.csv";
        }
        if(rightHand!=null)
        {
            lastBodyPos[(int)BodyLog.rightHand] = rightHand.transform.position;
            bodyStrPath[(int)BodyLog.rightHand] = "rightHandLog.csv";
        }
        if(leftHand!=null)
        {
            lastBodyPos[(int)BodyLog.leftHand] = leftHand.transform.position;
            bodyStrPath[(int)BodyLog.leftHand] = "leftHandLog.csv";
        }
        if(rightShin!=null)
        {
            lastBodyPos[(int)BodyLog.rightShin] = rightShin.transform.position;
            bodyStrPath[(int)BodyLog.rightShin] = "rightShinLog.csv";
        }
        if(leftShin!=null)
        {
            lastBodyPos[(int)BodyLog.leftShin] = leftShin.transform.position;
            bodyStrPath[(int)BodyLog.leftShin] = "leftShinLog.csv";
        }
        if(head!=null)
        {
            lastBodyPos[(int)BodyLog.head] = head.transform.position;
            bodyStrPath[(int)BodyLog.head] = "headLog.csv";
        }
        
    }

    void updateFullbodyReport()
    {
        Vector3 currentPosVector = new Vector3();
        if (rightFoot != null)
        {
            currentPosVector = rightFoot.transform.position - lastBodyPos[(int)BodyLog.rightFoot];
            if (currentPosVector.magnitude < threshold )
            {
                lastBodyPos[(int)BodyLog.rightFoot] = rightFoot.transform.position;
                bodyStr[(int)BodyLog.rightFoot] += pathStr += string.Join(",", new string[]
                {
                    currentTask.ToString(),
                    currentTrigger.ToString(),
                    rightFoot.transform.position.x.ToString(),
                    rightFoot.transform.position.y.ToString(),
                    rightFoot.transform.position.z.ToString(),
                    currentPosVector.x.ToString(),
                    currentPosVector.y.ToString(),
                    currentPosVector.z.ToString(),
                    rightFoot.transform.eulerAngles.x.ToString(),
                    rightFoot.transform.eulerAngles.y.ToString(),
                    rightFoot.transform.eulerAngles.z.ToString(),
                    rightFoot.transform.position.magnitude.ToString(),
                    "\n"

                });
            }
            //create a file for each part of the body
        }
        if (leftFoot != null)
        {
            currentPosVector = leftFoot.transform.position - lastBodyPos[(int)BodyLog.rightFoot];
            if (currentPosVector.magnitude < threshold)
            {
                lastBodyPos[(int)BodyLog.leftFoot] = leftFoot.transform.position;
                bodyStr[(int)BodyLog.leftFoot] += pathStr += string.Join(",", new string[]
                {
                    currentTask.ToString(),
                    currentTrigger.ToString(),
                    leftFoot.transform.position.x.ToString(),
                    leftFoot.transform.position.y.ToString(),
                    leftFoot.transform.position.z.ToString(),
                    currentPosVector.x.ToString(),
                    currentPosVector.y.ToString(),
                    currentPosVector.z.ToString(),
                    leftFoot.transform.eulerAngles.x.ToString(),
                    leftFoot.transform.eulerAngles.y.ToString(),
                    leftFoot.transform.eulerAngles.z.ToString(),
                    leftFoot.transform.position.magnitude.ToString(),
                    "\n"

                });
            }
        }
        if (rightHand != null)
        {
            currentPosVector = rightHand.transform.position - lastBodyPos[(int)BodyLog.rightHand];
            if (currentPosVector.magnitude < threshold)
            {
                lastBodyPos[(int)BodyLog.rightHand] = rightHand.transform.position;
                bodyStr[(int)BodyLog.rightHand] += pathStr += string.Join(",", new string[]
                {
                    currentTask.ToString(),
                    currentTrigger.ToString(),
                    rightHand.transform.position.x.ToString(),
                    rightHand.transform.position.y.ToString(),
                    rightHand.transform.position.z.ToString(),
                    currentPosVector.x.ToString(),
                    currentPosVector.y.ToString(),
                    currentPosVector.z.ToString(),
                    rightHand.transform.eulerAngles.x.ToString(),
                    rightHand.transform.eulerAngles.y.ToString(),
                    rightHand.transform.eulerAngles.z.ToString(),
                    rightHand.transform.position.magnitude.ToString(),
                    "\n"

                });
            }
        }
        if (leftHand != null)
        {
            lastBodyPos[(int)BodyLog.leftHand] = leftHand.transform.position;
            currentPosVector = leftHand.transform.position - lastBodyPos[(int)BodyLog.leftHand];
            if (currentPosVector.magnitude < threshold)
            {
                lastBodyPos[(int)BodyLog.leftHand] = rightHand.transform.position;
                bodyStr[(int)BodyLog.leftHand] += pathStr += string.Join(",", new string[]
                {
                    currentTask.ToString(),
                    currentTrigger.ToString(),
                    leftHand.transform.position.x.ToString(),
                    leftHand.transform.position.y.ToString(),
                    leftHand.transform.position.z.ToString(),
                    currentPosVector.x.ToString(),
                    currentPosVector.y.ToString(),
                    currentPosVector.z.ToString(),
                    leftHand.transform.eulerAngles.x.ToString(),
                    leftHand.transform.eulerAngles.y.ToString(),
                    leftHand.transform.eulerAngles.z.ToString(),
                    leftHand.transform.position.magnitude.ToString(),
                    "\n"

                });
            }
        }
        if (rightShin != null)
        {
            lastBodyPos[(int)BodyLog.rightShin] = rightShin.transform.position;
            currentPosVector = rightShin.transform.position - lastBodyPos[(int)BodyLog.rightShin];
            if (currentPosVector.magnitude < threshold)
            {
                lastBodyPos[(int)BodyLog.rightShin] = rightShin.transform.position;
                bodyStr[(int)BodyLog.rightShin] += pathStr += string.Join(",", new string[]
                {
                    currentTask.ToString(),
                    currentTrigger.ToString(),
                    rightShin.transform.position.x.ToString(),
                    rightShin.transform.position.y.ToString(),
                    rightShin.transform.position.z.ToString(),
                    currentPosVector.x.ToString(),
                    currentPosVector.y.ToString(),
                    currentPosVector.z.ToString(),
                    rightShin.transform.eulerAngles.x.ToString(),
                    rightShin.transform.eulerAngles.y.ToString(),
                    rightShin.transform.eulerAngles.z.ToString(),
                    rightShin.transform.position.magnitude.ToString(),
                    "\n"

                });
            }
        }
        if (leftShin != null)
        {
            lastBodyPos[(int)BodyLog.leftShin] = leftShin.transform.position;
            currentPosVector = leftShin.transform.position - lastBodyPos[(int)BodyLog.leftShin];
            if (currentPosVector.magnitude < threshold)
            {
                lastBodyPos[(int)BodyLog.leftShin] = leftShin.transform.position;
                bodyStr[(int)BodyLog.leftShin] += pathStr += string.Join(",", new string[]
                {
                    currentTask.ToString(),
                    currentTrigger.ToString(),
                    leftShin.transform.position.x.ToString(),
                    leftShin.transform.position.y.ToString(),
                    leftShin.transform.position.z.ToString(),
                    currentPosVector.x.ToString(),
                    currentPosVector.y.ToString(),
                    currentPosVector.z.ToString(),
                    leftShin.transform.eulerAngles.x.ToString(),
                    leftShin.transform.eulerAngles.y.ToString(),
                    leftShin.transform.eulerAngles.z.ToString(),
                    leftShin.transform.position.magnitude.ToString(),
                    "\n"

                });
            }
        }
        if (head != null)
        {
            lastBodyPos[(int)BodyLog.head] = head.transform.position;
            currentPosVector = head.transform.position - lastBodyPos[(int)BodyLog.head];
            if (currentPosVector.magnitude < threshold)
            {
                lastBodyPos[(int)BodyLog.head] = head.transform.position;
                bodyStr[(int)BodyLog.head] += pathStr += string.Join(",", new string[]
                {
                    currentTask.ToString(),
                    currentTrigger.ToString(),
                    head.transform.position.x.ToString(),
                    head.transform.position.y.ToString(),
                    head.transform.position.z.ToString(),
                    currentPosVector.x.ToString(),
                    currentPosVector.y.ToString(),
                    currentPosVector.z.ToString(),
                    head.transform.eulerAngles.x.ToString(),
                    head.transform.eulerAngles.y.ToString(),
                    head.transform.eulerAngles.z.ToString(),
                    head.transform.position.magnitude.ToString(),
                    "\n"

                });
            }
        }
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
        if(triggerId == "walkTrigger1")
        {
            if(currentTask == Tasks.Task2)
            {
                UpdateReport();
                currentTask = Tasks.ReachGreenLollipop1stTime;
                Debug.Log(currentTask.ToString());
                
                //objectsTask2.SetActive(false);
                //objectsTask3.SetActive(true);
                //Debug.Log("TEST OVER");
                //CompleteReport();
            }
        }

        if(triggerId == "walkTrigger2")
        {
            if(currentTask == Tasks.Task1)
            {
                UpdateReport();
                currentTask = Tasks.ReachRedLollipop1stTime;
                Debug.Log(currentTask.ToString());
            }
            else if(currentTask == Tasks.Task3)
            {
                UpdateReport();
                currentTask = Tasks.ReachRedLollipop2ndTime;
                Debug.Log(currentTask.ToString());

                //currentTask = Tasks.Completed;
                //CompleteReport();
            }
        }
        if(triggerId == "redLollipop")
        {
            
            if(currentTask == Tasks.ReachRedLollipop1stTime)
            {
                UpdateReport();
                currentTask = Tasks.Task2;
                SetActiveChildren(objectsTask1, false);
                SetActiveChildren(objectsTask2, true);
                Debug.Log(currentTask.ToString());
            }
            else if(currentTask == Tasks.ReachRedLollipop2ndTime)
            {
                lastTimeBetweenCollisions = Time.realtimeSinceStartup;
                currentTask = Tasks.ThrowingObjects;
                Debug.Log("Start Throwing Object Task");
                SetActiveChildren(objectsTask3, false);
                SetActiveChildren(objectTask4,true);
                CompleteReport();
                //throwing objectsToBeImplemented
            }
        }
        else if(triggerId == "greenLollipop")
        {
            if (currentTask == Tasks.ReachGreenLollipop0thTime)
            {
                currentTask = Tasks.Task1;
                SetActiveChildren(objectsTask1, true);
                Debug.Log(currentTask.ToString());
                lastTimeBetweenTasks = Time.realtimeSinceStartup;
                startTime = lastTimeBetweenTasks;
            }
            if (currentTask == Tasks.ReachGreenLollipop1stTime)
            {
                UpdateReport();
                currentTask = Tasks.Task3;
                SetActiveChildren(objectsTask2, false);
                SetActiveChildren(objectsTask3, true);
            }
        }
        else if( triggerId == "objectShooter")
        {
            if(currentTask == Tasks.ThrowingObjects)
            {
                SetActiveChildren(objectTask4, false);
                CompleteReport();
                currentTask = Tasks.Completed;
            }
        }
    }
}
