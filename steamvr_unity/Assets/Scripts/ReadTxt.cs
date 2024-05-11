using Byn.Unity.Examples;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class ReadTxt : MonoBehaviour
{
    public static ReadTxt readTxt;
    public Text conferenceApp;
    public string path;
    private void Awake()
    {
        readTxt = this;
    }
    void Start()
    {
        string path = Application.streamingAssetsPath+ "/ipconfig.txt"; // 文件路径
        path = ReadFirstLine(path);
        conferenceApp.text = path;
        Debug.Log(path);
    }

    string ReadFirstLine(string filePath)
    {
        try
        {
            using (StreamReader reader = new StreamReader(filePath))
            {
                return reader.ReadLine();
            }
        }
        catch (IOException e)
        {
            Debug.LogError("An IO exception has been thrown!");
            Debug.LogException(e);
            return null;
        }
    }
}
