
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityGoogleDrive;
using System.IO;

public class Uploader : MonoBehaviour
{

    private GoogleDriveFiles.CreateRequest Crequest;
    public string topFolder;
    public List<string> folderNames = new List<string>();
    public List<string> folderIDs = new List<string>();


    //private GoogleDriveFiles.CreateRequest request;
    // Start is called before the first frame update
    void Start()
    {
        //UploadToDrive();
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void UploadToDrive()
    {
        string[] dirs = Directory.GetDirectories(Application.dataPath + "/QATracker/");
        foreach (string dir in dirs)
        {
            string[] dirNames = dir.Split("/"[0]);
            string dirName = dirNames[dirNames.Length - 1];
            var info = new DirectoryInfo(dir);
            var fileInfo = info.GetFiles();
            foreach (var file in fileInfo)
            {
                if (file.Extension == ".csv")
                {
                    var content = File.ReadAllBytes(file.FullName);
                    //Debug.Log(file.FullName);
                    //if (content == null) return false;

                    var Gfile = new UnityGoogleDrive.Data.File() { Name = Path.GetFileName(Application.dataPath + "/QATracker/" + dir + "/" + file), Content = content };
                    //string parent = folderIDs[10];
                    Debug.Log(dirName);
                    string parent;
                    if (folderNames.Contains(dirName))
                    {
                        parent = folderIDs[folderNames.IndexOf(dirName)];
                    }
                    else
                    {
                        parent = topFolder;
                    }
                    Gfile.Parents = new List<string> { parent };
                    Crequest = GoogleDriveFiles.Create(Gfile);
                    Crequest.Fields = new List<string> { "id", "name", "size", "createdTime" };
                    Crequest.Send();
                }
            }
        }

        //Debug.Log("wow");
        //return true;
    }

    public void DeleteFiles()
    {
        string[] dirs = Directory.GetDirectories(Application.dataPath + "/QATracker/");
        foreach (string dir in dirs)
        {
            var info = new DirectoryInfo(dir);
            var fileInfo = info.GetFiles();
            foreach (var file in fileInfo)
            {
                File.Delete(file.FullName);
            }
            Directory.Delete(dir);
        }
    }
}
