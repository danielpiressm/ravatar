using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;

public enum Tasks { NotStarted, Task1, Task2, Task3, Completed  };



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
    private string testCollision = "";
    public string separator = ";";

    public Tasks currentTask;

    
    public GameObject objectsTask1; 
    public GameObject objectsTask2;
    public GameObject objectsTask3;
    public GameObject objectTask4;
    public GameObject pirulito;
    Dictionary<string, ActiveCollision> collisionsPerJoint;
    Vector3 lastPos;

    Camera _trackedObj;

    int currentTrigger = 0;

    float threshold = 0.00000f;

    float lastTimeBetweenTasks = 0.0f;
    float lastTimeBetweenTriggers = 0.0f;
    float lastTimeBetweenCollisions = 0.0f;
    float startTime = 0.0f;
    int countFullBodiesStr = 0;
    private string pathHeaderStr;

    
    Dictionary<string, List<ActiveCollision>> activeCollisions;
    List<FinishedCollision> finishedCollisions;
    Dictionary<string, FinishedCollision> finishedCollisionsAux;

    FinishedCollision finishedAux;

    float timeCollidingWithStuff = 0.0f;

    bool passedOnTrigger2 = false;

    
    int countTriggers = 0;
    Dictionary<string,GameObject> listObjectsInEachTask;

    // Use this for initialization
    void Start () {
        //currentTask = Tasks.Task1;
        //uncomment this for the task
        currentTask = Tasks.NotStarted;
        //with avatars, change for torso tracking
        _trackedObj = Camera.main;
        lastPos = _trackedObj.transform.position;
        InitializeReport();
        
        int i = 1;
        
        while(Directory.Exists(Directory.GetCurrentDirectory()+"/user"+i+avatarType.ToString()))
        {
            i++;
        }
        //se nao houver diretorios

        System.IO.Directory.CreateDirectory(Directory.GetCurrentDirectory() + "/user"+i+ avatarType.ToString());



        listObjectsInEachTask = new Dictionary<string, GameObject>();
        listObjectsInEachTask[Tasks.Task1.ToString()] = objectsTask1;
        listObjectsInEachTask[Tasks.Task2.ToString()] = objectsTask2;
        listObjectsInEachTask[Tasks.Task3.ToString()] = objectsTask3;

        pathDirectory = Directory.GetCurrentDirectory() + "/user" + i + avatarType.ToString() + "/";
        activeCollisions = new Dictionary<string, List<ActiveCollision>>();
        finishedCollisions = new List<FinishedCollision>();
        finishedCollisionsAux = new Dictionary<string, FinishedCollision>();
        collisionsPerJoint = new Dictionary<string, ActiveCollision>();

    }

    public AvatarType getAvatarType()
    {
        return avatarType;
    }

    public string getPathDirectory()
    {
        return pathDirectory;
    }

    public string getCurrentTask()
    {
        return currentTask.ToString();
    }
	
	// Update is called once per frame
	void Update () {
        if (currentTask != Tasks.Completed )
        {
            UpdatePathReport();
        }
        
    }

    float getTaskTime(float time)
    {
        if (time - startTime < 0)
            return 0;
        else
            return time - startTime;
    }

    

    public void serializeCollision(string str)
    {
        testCollision += (str+","+currentTask.ToString());
        System.IO.File.AppendAllText(pathDirectory + collisionLogfileName, str);
    }

    void serializeBallCollision(string str)
    {
        float currentTime = Time.realtimeSinceStartup;
        logStr += currentTask.ToString() + "," + (currentTime - lastTimeBetweenCollisions) + ","+ currentTime + "\n";
        //lastTimeBetweenTasks = currentTime;
        collisionLogStr += str;
        //Debug.Log("&&" + str);
    }

    void logBallThrow(float time)
    {
        lastTimeBetweenCollisions = time;
        Debug.Log("BetweenCollisions : " + time + ","+ Time.realtimeSinceStartup);
    }


    void InitializeReport()
    {
        //oldVersion of collision Log
        collisionLogStr = "Joint" + separator + "PosX" + separator + "PosY" + separator + "PosZ" + separator + "RotX" + separator + "RotY" + separator + "RotZ" + separator +
                        "ColliderName" + separator + "PosColliderX" + separator + "PosColliderY" + separator + "PosColliderZ" + separator + "RotColliderX" + separator + "RotColliderY" +
                        separator + "RotColliderZ" + separator + "ErrorX" + separator + "ErrorY" + separator + "ErrorZ" + separator+ "PositionColliderTransformedX"+ separator + "PositionColliderTransformedY"+separator+"PositionColliderTransformedZ"+ separator +
                        "CameraPositionX"+ separator + "CameraPositionY" + separator + "CameraPositionZ" + separator + "CameraRotationX" + separator + "CameraRotationY" + separator + "CameraRotationZ" + separator + 
                        "TimeElapsed"+ separator + "CurrentTime"+ separator + "CurrentTask"+"\n";
        //new version of collisionLog
        testCollision = "Joint,PosX,PosY,PosZ,RotX,RotY,RotZ,ColliderName,PosColliderX,PosColliderY,PosColliderZ,RotColliderX,RotColliderY,RotColliderZ,ErrorX,ErrorY,ErrorZ,Error2X,Error2Y,Error2Z,headPosX,headPosY,headPosZ,cameraPosX,cameraPosY,cameraPosZ,TimeElapsed,TimeStart,TimeFinish,currentTask\n";
        logStr = "TriggerNum" + separator + "TimeElapsed" + separator + "CurrentTime\n";
        pathStr = "Task,Trigger,currentPosX,currentPosY,currentPosZ,pathElapsedX,pathElapsedY,pathElapsedZ,rotX,rotY,rotZ,magnitude\n";
        pathHeaderStr = "Task,Trigger,currentPosX,currentPosY,currentPosZ,pathElapsedX,pathElapsedY,pathElapsedZ,rotX,rotY,rotZ,magnitude,CameraPosX,CameraPosY,CameraPosZ,CameraRotX,CameraRotY,CameraRotZ\n";
    }

    void CompleteReport()
    {

        logStr += "TotalTime" + getTaskTime(Time.realtimeSinceStartup)+"\n";
        System.IO.File.WriteAllText( pathDirectory+"/"+ collisionLogfileName + ".csv",collisionLogStr);
        System.IO.File.WriteAllText( pathDirectory+"/"+logFileName + ".csv", logStr);
        
        //else
        {
            System.IO.File.WriteAllText(pathDirectory + "/" + pathLogFileName + ".csv", pathStr);
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

    void triggerPlus(string triggerId)
    {
        if(triggerId == "walkTrigger2")
        {
            passedOnTrigger2 = true;
            Debug.Log("passedOntrigger2");
            // e nao eh o trigger que eu quero
        }
        else if(triggerId == "walkTrigger1")
        {
            if (currentTask == Tasks.NotStarted || passedOnTrigger2 == true )
            {
                if ((int)currentTask > 0)
                {
                    SetActiveChildren(listObjectsInEachTask[currentTask.ToString()], false);
                    UpdateReport();
                    
                    Debug.Log("time = " + getTaskTime(Time.realtimeSinceStartup));
                }
                if (currentTask < Tasks.Task3)
                {
                    currentTask++;
                    listObjectsInEachTask[currentTask.ToString()].gameObject.SetActive(true);
                }
                

                passedOnTrigger2 = false;
            }
            else
            {

            }
            
        }
        
    }

    void startCounter(string triggerId)
    {
        if (triggerId == "walkTrigger1")
        {
            startTime = Time.realtimeSinceStartup;
            Debug.Log("starting counter");
        }
            
        //Start the counter
    }


    

    void UpdateReport()
    {
        float currentTime = Time.realtimeSinceStartup;
        logStr += currentTask.ToString() + "," + (getTaskTime(currentTime) - getTaskTime(lastTimeBetweenTasks)) + "," + getTaskTime(currentTime) + "\n";
        lastTimeBetweenTasks = currentTime;
        lastTimeBetweenCollisions = currentTime;
        lastTimeBetweenTriggers = currentTime;
    }


    void nextTask(string triggerId)
    {
        if(triggerId == "walkTrigger1")
        {
            
        }

        if(triggerId == "walkTrigger2")
        {
            
        }
        
    }

    //collisionStuff
    public void collisionStarted(string colliderName, string jointName, float time)
    {
        if (activeCollisions.ContainsKey(colliderName))
        {
            if (activeCollisions[colliderName].Count > 0)
            {
                activeCollisions[colliderName].Add(new ActiveCollision(jointName, time, false));
            }
            else
            {
                activeCollisions[colliderName].Add(new ActiveCollision(jointName, time, true));
            }
        }
        else
        {
            activeCollisions[colliderName] = new List<ActiveCollision>();
            activeCollisions[colliderName].Add(new ActiveCollision(jointName, time, true));
            Debug.Log("Empilhando " + jointName + " em " + colliderName + " no tempo : " + time);

        }
    }

    public void collisionEnded(string colliderName, string jointName, float time)
    {
        if (activeCollisions.ContainsKey(colliderName))
        {
            List<ActiveCollision> actColList = new List<ActiveCollision>(activeCollisions[colliderName]);
            for (int i = 0; i < activeCollisions[colliderName].Count; i++)
            {
                if (activeCollisions[colliderName][i].jointName == jointName)
                {
                    /*if (finishedAux == null)
                        finishedAux = new FinishedCollision("", 0);*/

                    //add to joint collision dictionary
                    if (!collisionsPerJoint.ContainsKey(jointName))
                    {
                        ActiveCollision colAux = new ActiveCollision(jointName);
                        colAux.collisionCount++;
                        colAux.timeInit += (time - activeCollisions[colliderName][i].timeInit);
                        collisionsPerJoint.Add(jointName, colAux);
                    }
                    else
                    {
                        collisionsPerJoint[jointName].timeInit += (time - activeCollisions[colliderName][i].timeInit);
                        collisionsPerJoint[jointName].collisionCount++;
                    }


                    if (activeCollisions[colliderName][i].first)
                    {
                        if (!finishedCollisionsAux.ContainsKey(colliderName))
                        {
                            finishedCollisionsAux.Add(colliderName, new FinishedCollision("", 0));
                        }
                        else if (finishedCollisionsAux[colliderName] == null)
                        {
                            finishedCollisionsAux[colliderName] = new FinishedCollision("", 0);
                        }
                        else
                        {

                        }

                        finishedCollisionsAux[colliderName].startTime = activeCollisions[colliderName][i].timeInit;
                        finishedCollisionsAux[colliderName].finishTime = time;
                        finishedCollisionsAux[colliderName].colliderName = colliderName;


                        activeCollisions[colliderName].Remove(activeCollisions[colliderName][i]);

                    }
                    else
                    {
                        if (!finishedCollisionsAux.ContainsKey(colliderName))
                            finishedCollisionsAux[colliderName] = new FinishedCollision("", 0);
                        finishedCollisionsAux[colliderName].finishTime = time;
                        finishedCollisionsAux[colliderName].colliderName = colliderName;
                        activeCollisions[colliderName].Remove(activeCollisions[colliderName][i]);
                    }


                    if (activeCollisions[colliderName].Count == 0)
                    {
                        finishedCollisions.Add(finishedCollisionsAux[colliderName]);

                        timeCollidingWithStuff += (finishedCollisionsAux[colliderName].finishTime - finishedCollisionsAux[colliderName].startTime);
                        //finishedCollisions.FindIndex()
                        //finishedCollisionsAux[colliderName] = null;
                        finishedCollisionsAux.Remove(colliderName);
                    }

                }
            }
        }
    }



}
