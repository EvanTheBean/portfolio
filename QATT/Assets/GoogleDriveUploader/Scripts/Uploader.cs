
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityGoogleDrive;
using System.IO;

/*Copyright Evan Koppers
 * Last updated 2/24/22
 * For personal and professional use if bought through the unity asset store or itch.io
 * 
 * Requires Package: https://github.com/Elringus/UnityGoogleDrive.git#package
 * Documentation: https://docs.google.com/document/d/1MZM2oo_DziZlzMTYNIa46pkIHzij60nIm1gZl9TMlHc/edit?usp=sharing
 */

//Add this class to a manager
//Call functions in this class to upload to drive
public class Uploader : MonoBehaviour
{
    private GoogleDriveFiles.CreateRequest Crequest;

    [Header("Local Variables")]
    public string localPath;
    public List<string> fileType = new List<string>();
    [Tooltip("local folders, link up with drive folder ids. Null if no nested folders")]
    public List<string> folderNames = new List<string>();

    [Header("GoogleDrive IDs")]
    public string topFolder;
    [Tooltip("drive folder ids, link up with local folder names. Null if no nested folders")]
    public List<string> folderIDs = new List<string>();

    /*
     * Uploads all files within the local path to google drive
     * can upload nested folders
     */
    public void UploadToDrive()
    {
        string[] dirs = Directory.GetDirectories(Application.dataPath + "/" + localPath + "/");
        foreach (string dir in dirs)
        {
            string[] dirNames = dir.Split("/"[0]);
            string dirName = dirNames[dirNames.Length - 1];
            var info = new DirectoryInfo(dir);
            var fileInfo = info.GetFiles();
            foreach (var file in fileInfo)
            {
                if (fileType.Contains(file.Extension))
                {
                    var content = File.ReadAllBytes(file.FullName);
                    var Gfile = new UnityGoogleDrive.Data.File() { Name = Path.GetFileName(Application.dataPath + "/" + localPath + "/" + dir + "/" + file), Content = content };

                    //nests folders in google drive
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
    }

    /*
     * Deletes all files within the local path to avoid uploading duplicated
     * Do not call if you dont want local files to be deleted :)
     */
    public void DeleteFiles()
    {
        string[] dirs = Directory.GetDirectories(Application.dataPath + "/" + localPath + "/");
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

