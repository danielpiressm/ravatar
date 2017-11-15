using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System;

public class FileReader : MonoBehaviour
{

    public Dictionary<string, string> dictionary;
    public Dictionary<string, string> dictionaryHeaders;


    void doStuff()
    {
        dictionary = new Dictionary<string, string>();
        Dictionary<string, int> dictionary2 = new Dictionary<string, int>();
        dictionaryHeaders = new Dictionary<string, string>();


        dictionary.Add("collisionLog.csv", "");
        //dictionary.Add("reportDiscriminateTimes.csv","");
        //dictionary.Add("report.csv", "");
        //dictionary.Add("torso.csv", "");
        //dictionary.Add("optimalPath.csv", "");
        //dictionary.Add("leftFootLog.csv", "");
        //dictionary.Add("leftHandLog.csv", "");
        ///dictionary.Add("reportPath.csv", "");
        //dictionary.Add("optimalPath.csv", "");
        //dictionary.Add("rightFootLog.csv", "");
        //dictionary.Add("rightHandLog.csv", "");


        string rootPath = "D:\\Dropbox\\doutorado\\papers\\vrstFlying\\output\\2ndTest";

        string[] filePaths = Directory.GetFiles(@"D:\Dropbox\doutorado\papers\vrstFlying\userLogs\2ndTest", "*.csv", SearchOption.AllDirectories);
        string[] fileArray = Directory.GetDirectories(@"D:\Dropbox\doutorado\papers\vrstFlying\userLogs\2ndTest");

        foreach (string fileName in filePaths)
        {
            FileInfo fInfo = new FileInfo(fileName);

            string userName = "";

            try
            {
                //string[] str = fInfo.Name.Split(;
                if (dictionary.ContainsKey(fInfo.Name))
                {
                    //dictionary[fInfo.Name]++;// = 
                    if (fInfo.Directory.Name.Contains("fullbody"))
                    {
                        userName = fInfo.Directory.Parent.Parent.Name;
                    }
                    else
                    {
                        userName = fInfo.Directory.Parent.Name;
                    }
                    dictionary[fInfo.Name] += readFile(userName, fileName);
                }


                else
                {
                    // Debug.Log("Devia ter feito isto antes");

                }
            }
            catch (Exception ex)
            {
                Debug.Log("EX = " + fInfo.Name);
            }
            int x = 2;
        }

        Debug.Log("ACABEI");

        foreach (KeyValuePair<string, string> word in dictionary)
        {
            string tmp = word.Key;
            /*if (word.Key != "")
                tmp = word.Value.Substring(0, 30);*/
            Debug.Log(" Word =  " + tmp);
            if (word.Key.Contains("fullBody"))
            {
                int index = word.Key.IndexOf("\\");
                tmp = word.Key.Substring(index + 1);
                Debug.Log("substring = " + tmp);
            }
            System.IO.File.WriteAllText(rootPath + "\\" + word.Key, "User,Avatar,Perspective," + dictionaryHeaders[tmp] + word.Value);
        }


    }


    // Use this for initialization
    void Start()
    {

        string str = "Task,Teste\n0.0,10.0";
        str = str.Replace("\n", "\nOI");
        Debug.Log(str);
        doStuff();
    }

    // Update is called once per frame
    void Update()
    {

    }

    string readFile(string name, string filePath)
    {
        int counter = 0;
        string header = "";
        string line = "";
        string str = "";


        System.IO.StreamReader file = new System.IO.StreamReader(filePath);
        FileInfo fInfo = new FileInfo(filePath);
        string dName = "";
        string perspective = "";

        if (fInfo.Directory.Name.Contains("fullbody"))
        {
            dName = fInfo.Directory.Parent.Name.Split('_')[1];
            perspective = fInfo.Directory.Parent.Name.Split('_')[2];
        }
        else
        {
            dName = fInfo.Directory.Name.Split('_')[1];
            perspective = fInfo.Directory.Name.Split('_')[2];
        }

        header = file.ReadLine();
        if (!dictionaryHeaders.ContainsKey(fInfo.Name))
        {
            dictionaryHeaders.Add(fInfo.Name, header + "\n");
        }
        //string text = file.ReadToEnd();
        //text += (text.Replace("\n", "\n" + name+","));

        while ((line = file.ReadLine()) != null)
        {
            //     Debug.Log(line);
            str += name + "," + dName + "," + perspective + ","+ line + "\n";
            counter++;
        }

        file.Close();

        return str;
    }

}
