
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using UnityGoogleDrive;
using System.Text;

/*Copyright Evan Koppers
 * Last updated 3/2/22
 * For personal and professional use if bought through the unity asset store or itch.io
 * 
 * Requires Package: https://github.com/Elringus/UnityGoogleDrive.git#package
 * Documentation: https://docs.google.com/document/d/1s0QSxGy4HlnzAtAwi0dL3d-zIEoahZeBbxhNMfrNDCw/edit?usp=sharing
 */

[ExecuteInEditMode]
public class Downloader : EditorWindow
{
    //private GoogleDriveFiles.ExportRequest request;
    private GoogleDriveFiles.DownloadRequest request;
    int monthMin, dayMin, yearMin;
    int monthMax, dayMax, yearMax;
    string downloadFolder = "AssetFolder";
    [MenuItem("Tools/Downloader")]
    static void Init()
    {
        Downloader window = EditorWindow.GetWindow<Downloader>();
        window.Show();
    }
    private void OnGUI()
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Folder ", GUILayout.Width(50));
        downloadFolder = EditorGUILayout.TextField(downloadFolder);
        EditorGUILayout.EndHorizontal();

        if (GUILayout.Button("Download"))
        {
            DeleteAllExisting();
            DownloadAll();
        }

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Get Data After (MM/DD/YYYY):", GUILayout.Width(175));
        monthMin = EditorGUILayout.IntField(monthMin, GUILayout.MaxWidth(25));
        dayMin = EditorGUILayout.IntField(dayMin, GUILayout.MaxWidth(25));
        yearMin = EditorGUILayout.IntField(yearMin, GUILayout.MaxWidth(45));
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Get Data Before (MM/DD/YYYY):", GUILayout.Width(175));
        monthMax = EditorGUILayout.IntField(monthMax, GUILayout.MaxWidth(25));
        dayMax = EditorGUILayout.IntField(dayMax, GUILayout.MaxWidth(25));
        yearMax = EditorGUILayout.IntField(yearMax, GUILayout.MaxWidth(45));
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.LabelField("Is inclusive");
        EditorGUILayout.LabelField("If you don't want to limit, set to 0/0/0");

        GUILayout.Space(50);
        if (GUILayout.Button("Delete Data"))
        {
            DeleteAllExisting();
        }
    }

    void DownloadData(string ID, string FOLDER, string NAME)
    {
        string fileName2 = Application.dataPath + "/" + downloadFolder + "/ " + FOLDER + "/" + NAME;
        request = GoogleDriveFiles.Download(ID);
        if (!Directory.Exists(Application.dataPath + "/" + downloadFolder + "/ " + FOLDER))
        {
            Directory.CreateDirectory(Application.dataPath + "/" + downloadFolder + "/ " + FOLDER);
            //Debug.Log("Created directory " + Application.dataPath + "/QATracker/" + FOLDER);
        }
        request.Send().OnDone += delegate (UnityGoogleDrive.Data.File file) { SetResult(file, fileName2); };
    }
    private void SetResult(UnityGoogleDrive.Data.File file, string newFile)
    {
        string result = Encoding.UTF8.GetString(file.Content);
        TextWriter tw = new StreamWriter(newFile, false);
        tw.Close();
        tw = new StreamWriter(newFile, false);
        tw.WriteLine(result);
        tw.Close();
    }
    public void DownloadAll()
    {
        //Debug.Log("Start");
        string fileData = System.IO.File.ReadAllText(Application.dataPath + "/" + downloadFolder + "/" + "PARENT.csv");
        if (fileData == null)
        {
            Debug.Log("Please download and name PARENT.csv");
        }
        else
        {
            string[] lines = fileData.Split("\n"[0]);
            DownloadFolders(lines);
        }
    }
    public void DownloadFolders(string[] lines)
    {
        for (int i = 1; i < lines.Length; i++)
        {
            string newline = lines[i].Replace("/", ",");
            string[] linedata = newline.Trim().Split(","[0]);
            string level = linedata[0];
            int month = int.Parse(linedata[1]);
            int day = int.Parse(linedata[2]);
            int year = int.Parse(linedata[3]);
            string name = linedata[4];
            string id = linedata[5];
            //CHECK DATE
            bool inRange = true;
            if (!(dayMin == 0 || monthMin == 0 || yearMin == 0))
            {
                if (dayMin <= day && monthMin <= month && yearMin <= year)
                {
                    inRange = false;
                }
            }
            if (!(dayMax == 0 || monthMax == 0 || yearMax == 0))
            {
                if (dayMax >= day && monthMax >= month && yearMax >= year)
                {
                    inRange = false;
                }
            }
            if (inRange)
            {
                DownloadData(id, level, name);
            }
        }
    }
    public void DeleteAllExisting()
    {
        string[] dirs = Directory.GetDirectories(Application.dataPath + "/" + downloadFolder + "/");
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
