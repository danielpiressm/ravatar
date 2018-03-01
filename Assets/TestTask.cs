using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public enum Tasks { NotStarted, Task1, Task2, Task3, Completed  };



public enum AvatarType { mesh_FirstPerson, mesh_ThirdPerson, abstract_FirstPerson, abstract_ThirdPerson, pointCloud_FirstPerson, pointCloud_ThirdPerson };

public class TestTask : MonoBehaviour {

    public AvatarType avatarType;

    public bool started = false;

    public string collisionLogfileName = "collisionLog";
    public string logFileName = "log";
    public string pathLogFileName = "logPath";

    private string pathDirectory = "";

    private string collisionLogStr = "";
    private string logStr = "";
    private string pathStr = "";
    private string testCollision = "";
    public string separator = ",";

    public Tasks currentTask;
    float totalTime =0.0f;

    
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
    List<FinishedCollision> finishedCollisionsHistory;
    Dictionary<string, FinishedCollision> finishedCollisionsAux;

    FinishedCollision finishedAux;

    float timeCollidingWithStuff = 0.0f;
    Dictionary<Tasks,float> timeCollidingWithStuffPerTask;
    Dictionary<Tasks, float> collisionCountPerTask;
    Dictionary<Tasks, List<FinishedCollision>> finishedCollisionsPerTask;

    bool passedOnTrigger2 = false;


    
    int countTriggers = 0;
    Dictionary<string,GameObject> listObjectsInEachTask;
    private bool completed;

    private void OnDisable()
    {
        Debug.Log("e aqui?"+ currentTask + " " + currentTask.ToString());
        if(currentTask == Tasks.Completed)
            CompleteReport();
    }


    public AvatarType determineAvatar()
    {
        string sceneName = SceneManager.GetActiveScene().name;
    
        AvatarType aType = AvatarType.abstract_FirstPerson;
        bool firstPerson = false;

        if(sceneName.Contains("First") || sceneName.Contains("first"))
        {
            firstPerson = true;
        }
        else
        {
            firstPerson = false;
        }

        if(sceneName.Contains("boxMan") || sceneName.Contains("blue"))
        {
            if(firstPerson)
            {
                aType = AvatarType.abstract_FirstPerson;
            }
            else
            {
                aType = AvatarType.abstract_ThirdPerson;
            }
        }
        else if(sceneName.Contains("carl") || sceneName.Contains("Carl"))
        {
            if (firstPerson)
            {
                aType = AvatarType.mesh_FirstPerson;
            }
            else
            {
                aType = AvatarType.mesh_ThirdPerson;
            }
        }
        else if (sceneName.Contains("ravatar"))
        {
            if (firstPerson)
            {
                aType = AvatarType.pointCloud_FirstPerson;
            }
            else
            {
                aType = AvatarType.pointCloud_ThirdPerson;
            }
        }

        return aType;

    }


    // Use this for initialization
    void Start () {
        //currentTask = Tasks.Task1;
        //uncomment this for the task
        currentTask = Tasks.NotStarted;
        //with avatars, change for torso tracking
        _trackedObj = Camera.main;
        lastPos = _trackedObj.transform.position;
        InitializeReport();
        separator = ",";


        avatarType = determineAvatar();
        
        int i = 1;
        
        while(Directory.Exists(Directory.GetCurrentDirectory()+"/user"+i+"_"+avatarType.ToString()))
        {
            i++;
        }
        //se nao houver diretorios

        System.IO.Directory.CreateDirectory(Directory.GetCurrentDirectory() + "/user"+i+ "_"+ avatarType.ToString());



        listObjectsInEachTask = new Dictionary<string, GameObject>();
        listObjectsInEachTask[Tasks.Task1.ToString()] = objectsTask1;
        listObjectsInEachTask[Tasks.Task2.ToString()] = objectsTask2;
        listObjectsInEachTask[Tasks.Task3.ToString()] = objectsTask3;
        listObjectsInEachTask[Tasks.Completed.ToString()] = objectTask4;

        pathDirectory = Directory.GetCurrentDirectory() + "/user" + i + "_"+avatarType.ToString() + "/";
        System.IO.Directory.CreateDirectory(pathDirectory + "fullbodyLog/");
        activeCollisions = new Dictionary<string, List<ActiveCollision>>();
        finishedCollisions = new List<FinishedCollision>();
        finishedCollisionsAux = new Dictionary<string, FinishedCollision>();
        collisionsPerJoint = new Dictionary<string, ActiveCollision>();
        finishedCollisionsHistory = new List<FinishedCollision>();


        timeCollidingWithStuffPerTask = new Dictionary<Tasks, float>();
        collisionCountPerTask = new Dictionary<Tasks, float>();
        finishedCollisionsPerTask = new Dictionary<Tasks, List<FinishedCollision>>();
        for(int j = 0; j < (int) Tasks.Completed;j++)
        {
            timeCollidingWithStuffPerTask.Add((Tasks)j, 0);
            collisionCountPerTask.Add((Tasks)j, 0);
            finishedCollisionsPerTask.Add((Tasks)j, new List<FinishedCollision>());
        }

        GameObject.Find("triggerObject1").GetComponent<BoxCollider>().enabled = false;
        GameObject.Find("triggerObject2").GetComponent<BoxCollider>().enabled = false;

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
        
        if(Input.GetKeyDown(KeyCode.S))
        {
            print("activating test");
            GameObject.Find("triggerObject1").GetComponent<BoxCollider>().enabled = true;
            GameObject.Find("triggerObject2").GetComponent<BoxCollider>().enabled = true;
        }
        
    }

    public bool taskStarted()
    {
        if (currentTask != Tasks.NotStarted)
            return true;
        else
            return false;
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
        collisionLogStr += str;
        System.IO.File.AppendAllText(pathDirectory + collisionLogfileName + ".csv", str);
    }

    


    void InitializeReport()
    {
        collisionLogStr = "Joint,PosX,PosY,PosZ,RotX,RotY,RotZ,ColliderName,PosColliderX,PosColliderY,PosColliderZ,RotColliderX,RotColliderY,RotColliderZ,ErrorX,ErrorY,ErrorZ,Error2X,Error2Y,Error2Z,headPosX,headPosY,headPosZ,cameraPosX,cameraPosY,cameraPosZ,TimeElapsed,TimeStart,TimeFinish,Task\n";

        logStr = "Task1" + separator + "Task2" + separator + "Task3" + ","+ "Total\n";
        pathStr = "Task,Trigger,currentPosX,currentPosY,currentPosZ,pathElapsedX,pathElapsedY,pathElapsedZ,rotX,rotY,rotZ,magnitude\n";
        pathHeaderStr = "Task,Trigger,currentPosX,currentPosY,currentPosZ,pathElapsedX,pathElapsedY,pathElapsedZ,rotX,rotY,rotZ,magnitude,CameraPosX,CameraPosY,CameraPosZ,CameraRotX,CameraRotY,CameraRotZ\n";


    }

    void CompleteReport()
    {
        Debug.Log("@@@@ Completing Report @@@@");
        logStr +=  totalTime+"\n";
        
        System.IO.File.WriteAllText( pathDirectory+"/"+logFileName + ".csv" , logStr);

        int objectsCollidedPerTask = 0;

        collisionLogStr += "!ObjectsCollided," + finishedCollisions.Count + ",TotalTimeCollided(s)," + timeCollidingWithStuff + "\n";
        for (int i = 0; i < timeCollidingWithStuffPerTask.Count; i++)
        {
            objectsCollidedPerTask = finishedCollisionsPerTask[(Tasks)i].Count;
            collisionLogStr += "@Task," + (Tasks)i +  ",CollisionCount," + collisionCountPerTask[(Tasks)i] + ",NumberObjectsCollided,"+ objectsCollidedPerTask + ",TotalTimeCollided(s)," + timeCollidingWithStuffPerTask[(Tasks)i] + ",CollisionNroHistory,"+ finishedCollisionsHistory.Count+"\n";
        }



        foreach (KeyValuePair<string, ActiveCollision> collPerJoint in collisionsPerJoint)
        {
            collisionLogStr += "%JointName," + collPerJoint.Key + "," + "CollisionCount," + collPerJoint.Value.collisionCount + ",TimeCollided," + collPerJoint.Value.timeInit + ",Task,"+ collPerJoint.Value.task.ToString() + "\n";
        }
        System.IO.File.WriteAllText(pathDirectory + "/" + collisionLogfileName + ".csv", collisionLogStr);



        System.IO.File.WriteAllText(pathDirectory + "/" + pathLogFileName + ".csv", pathStr);
        
      
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
                if (currentTask < Tasks.Completed)
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
            Debug.Log("##### starting counter for Task "+ currentTask+ " #####");
        }
            
        //Start the counter
    }


    

    void UpdateReport()
    {
        float currentTime = Time.realtimeSinceStartup;
        logStr += (getTaskTime(currentTime)+",");
        totalTime += getTaskTime(currentTime);
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
                        collisionsPerJoint[jointName].task = currentTask;
                    }
                    else
                    {
                        collisionsPerJoint[jointName].timeInit += (time - activeCollisions[colliderName][i].timeInit);
                        collisionsPerJoint[jointName].collisionCount++;
                        collisionsPerJoint[jointName].task = currentTask;
                    }
                    collisionCountPerTask[currentTask]++;


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
                        bool foundInTasksList = false;
                        bool foundInGeneralList = false;
                        for(int j = 0; j < finishedCollisions.Count;j++)
                        {
                            FinishedCollision aux = finishedCollisions[j];
                            //se ele ja tiver colidido com aquele objecto :-)
                            if (aux.colliderName.Equals(colliderName))
                            {
                                foundInGeneralList = true;
                                break;
                            }
                            else
                            {
                                foundInGeneralList = false;
                            }
                        }
                        for (int j = 0; j < finishedCollisionsPerTask[currentTask].Count; j++)
                        {
                            FinishedCollision aux = finishedCollisionsPerTask[currentTask][j];
                            //se ele ja tiver colidido com aquele objecto :-)
                            if (aux.colliderName.Equals(colliderName))
                            {
                                foundInTasksList = true;
                                break;
                            }
                            else
                            {
                                foundInTasksList = false;
                            }
                        }
                        if(!foundInTasksList)
                            finishedCollisionsPerTask[currentTask].Add(finishedCollisionsAux[colliderName]);
                        if(!foundInGeneralList)
                            finishedCollisions.Add(finishedCollisionsAux[colliderName]);
                        //finishedCollisionsPerTask[currentTask].Add(finishedCollisionsAux[colliderName]);
                        finishedCollisionsHistory.Add(finishedCollisionsAux[colliderName]);
                        timeCollidingWithStuff += (finishedCollisionsAux[colliderName].finishTime - finishedCollisionsAux[colliderName].startTime);
                        timeCollidingWithStuffPerTask[currentTask] += (finishedCollisionsAux[colliderName].finishTime - finishedCollisionsAux[colliderName].startTime);

                        

                        //finishedCollisions.FindIndex()
                        //finishedCollisionsAux[colliderName] = null;
                        finishedCollisionsAux.Remove(colliderName);
                    }

                }
            }
        }
    }



}
