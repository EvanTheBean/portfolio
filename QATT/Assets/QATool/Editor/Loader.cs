using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.IO;

[ExecuteInEditMode]
public class Loader : EditorWindow
{
    Dictionary<string, List<DataPoint>> playerData = new Dictionary<string, List<DataPoint>>();
    Dictionary<GameObject, List<GameObject>> playerObjects = new Dictionary<GameObject, List<GameObject>>();
    Dictionary<string, GameObject> parentObjects = new Dictionary<string, GameObject>();
    DataPoint lastPoint;
    SeePlayerData lastPlayer;
    bool first;

    float neededDifference = 2f;

    Mesh mesh;
    Material material;

    [MenuItem("Tools/QAT/Loader")]
    static void Init()
    {
        Loader window = EditorWindow.GetWindow<Loader>();
        window.Show();
    }

    private void OnGUI()
    {
        if(GUILayout.Button("Load Player Data"))
        {
            CreateAll();
        }

        mesh = (Mesh)EditorGUILayout.ObjectField("Shape (for now)", mesh, typeof(Mesh), false);
        material = (Material)EditorGUILayout.ObjectField("Material (for now)", material, typeof(Material), false);

        //neededDifference = EditorGUILayout.FloatField(neededDifference);
    }

    void CreateObject(string[] lineData, GameObject parentPlayer)
    {
        if(lineData.Length > 22)
        {
            DataPoint newPoint = new DataPoint();
            playerData[parentPlayer.name].Add(newPoint);

            newPoint.gameTime = float.Parse(lineData[1]);
            newPoint.levelTime = float.Parse(lineData[2]);
            newPoint.position.x = float.Parse(lineData[3]); newPoint.position.y = float.Parse(lineData[4]); newPoint.position.z = float.Parse(lineData[5]);
            newPoint.direction.x = float.Parse(lineData[6]); newPoint.direction.y = float.Parse(lineData[7]); newPoint.direction.z = float.Parse(lineData[8]);
            newPoint.rotation.x = float.Parse(lineData[9]); newPoint.rotation.y = float.Parse(lineData[10]); newPoint.rotation.z = float.Parse(lineData[11]); newPoint.rotation.w = float.Parse(lineData[12]);
            newPoint.ezraPosition.x = float.Parse(lineData[13]); newPoint.ezraPosition.y = float.Parse(lineData[14]); newPoint.ezraPosition.z = float.Parse(lineData[15]);
            newPoint.ezraDirection.x = float.Parse(lineData[16]); newPoint.ezraDirection.y = float.Parse(lineData[17]); newPoint.ezraDirection.z = float.Parse(lineData[18]);
            newPoint.ezraRotation.x = float.Parse(lineData[19]); newPoint.ezraRotation.y = float.Parse(lineData[20]); newPoint.ezraRotation.z = float.Parse(lineData[21]); newPoint.ezraRotation.w = float.Parse(lineData[22]);
            newPoint.playerState = int.Parse(lineData[23]);

            for (int i = 24; i < lineData.Length; i++)
            {
                newPoint.currentKey.Add(lineData[i]);
            }

            if (newPoint.gameTime > 0 && newPoint.levelTime > 0)
            {
                if (first)
                {
                    GameObject newObject = new GameObject(lineData[0]);
                    newObject.transform.parent = parentPlayer.transform;
                    SeePlayerData newPlayerData = newObject.AddComponent<SeePlayerData>();
                    playerObjects[parentPlayer].Add(newObject);
                    newObject.AddComponent<MeshFilter>();
                    newObject.GetComponent<MeshFilter>().mesh = mesh;
                    newObject.AddComponent<MeshRenderer>();
                    newObject.GetComponent<MeshRenderer>().material = material;

                    GameObject newEzra = new GameObject(newObject.name + "EZRA");
                    newEzra.transform.parent = newObject.transform;

                    newObject.transform.position = newPoint.position;
                    newObject.transform.rotation = newPoint.rotation;
                    newPlayerData.ezraObject = newEzra;
                    newEzra.transform.position = newPoint.ezraPosition;
                    newEzra.transform.rotation = newPoint.ezraRotation;
                    newPlayerData.direction = newPoint.direction;
                    newPlayerData.ezraDirection = newPoint.ezraDirection;
                    newPlayerData.gameTime = newPoint.gameTime;
                    newPlayerData.levelTime = newPoint.levelTime;
                    newPlayerData.playerState = newPoint.playerState;
                    newPlayerData.currentKey = newPoint.currentKey;

                    lastPoint = newPoint;
                    lastPlayer = newPlayerData;

                    first = false;
                }
                else if (((Vector3)lastPoint.position - (Vector3)newPoint.position).sqrMagnitude > neededDifference)
                {
                    GameObject newObject = new GameObject(lineData[0]);
                    newObject.transform.parent = parentPlayer.transform;
                    SeePlayerData newPlayerData = newObject.AddComponent<SeePlayerData>();
                    playerObjects[parentPlayer].Add(newObject);
                    newObject.AddComponent<MeshFilter>();
                    newObject.GetComponent<MeshFilter>().mesh = mesh;
                    newObject.AddComponent<MeshRenderer>();
                    newObject.GetComponent<MeshRenderer>().material = material;

                    GameObject newEzra = new GameObject(newObject.name + "EZRA");
                    newEzra.transform.parent = newObject.transform;

                    newObject.transform.position = newPoint.position;
                    newObject.transform.rotation = newPoint.rotation;
                    newPlayerData.ezraObject = newEzra;
                    newEzra.transform.position = newPoint.ezraPosition;
                    newEzra.transform.rotation = newPoint.ezraRotation;
                    newPlayerData.direction = newPoint.direction;
                    newPlayerData.ezraDirection = newPoint.ezraDirection;
                    newPlayerData.gameTime = newPoint.gameTime;
                    newPlayerData.levelTime = newPoint.levelTime;
                    newPlayerData.playerState = newPoint.playerState;
                    newPlayerData.currentKey = newPoint.currentKey;

                    lastPoint = newPoint;
                    lastPlayer = newPlayerData;
                }
                else
                {
                    lastPoint.skippedPoints++;
                    lastPlayer.skippedPoints++;
                }
            }
        }
    }

    void CreatePlayer(string[] lines, string playerId)
    {
        GameObject player = new GameObject(playerId);

        playerData.Add(playerId, new List<DataPoint>());
        playerObjects.Add(player, new List<GameObject>());
        parentObjects.Add(playerId, player);

        first = true;

        for (int i = 1; i < lines.Length; i++)
        {
            string newline = lines[i].Replace("|", ",");
            string[] linedata = newline.Trim().Split(","[0]);
            CreateObject(linedata, player);
        }
    }

    void CreateAll()
    {
        playerData.Clear();
        playerObjects.Clear();
        parentObjects.Clear();

        string level = EditorSceneManager.GetActiveScene().name;
        string folderName = Application.dataPath + "/QATracker/" + level;

        if (!Directory.Exists(folderName))
        {
            Debug.LogError("There is not data from this scene, either this was not a tested level, or you need to download the data again");
        }
        else
        {
            var info = new DirectoryInfo(folderName);
            var fileInfo = info.GetFiles();
            foreach (var file in fileInfo)
            {
                if(file.Extension == ".csv")
                {
                    string fileData = System.IO.File.ReadAllText(file.FullName);
                    string[] lines = fileData.Split("\n"[0]);
                    CreatePlayer(lines, file.Name);
                }
            }
        }
    }
}
