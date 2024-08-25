using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;

public class Saver : MonoBehaviour
{
    public float saveTime;

    string fileName;
    string level;
    float levelTime = 0;

    List<DataPoint> data = new List<DataPoint>();

    GameObject player, ezra;
    Rigidbody playerrb, ezrarb;
    PlayerController pc;

    private int[] values;

    // Start is called before the first frame update
    void Start()
    {
        /*
        player = GameObject.Find("PlayerController");
        playerrb = player.GetComponent<Rigidbody>();
        pc = player.GetComponent<PlayerController>();
        ezra = GameObject.Find("Ezra");
        //ezrarb = ezra.GetComponent<Rigidbody>();
        */
        //level = SceneManager.GetActiveScene().ToString();
        //level = "test";

        IEnumerator coroutine;
        coroutine = GetData();
        StartCoroutine(coroutine);

        //values = (int[])System.Enum.GetValues(typeof(KeyCode));
    }

    public void setUsers()
    {
        player = GameObject.Find("PlayerController");
        if(player)
        {
            playerrb = player.GetComponent<Rigidbody>();
            pc = player.GetComponent<PlayerController>();
        }
        ezra = GameObject.Find("Ezra");
        if(ezra)
        {
            //ezrarb = ezra.GetComponent<Rigidbody>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        levelTime += Time.deltaTime;
    }

    public void setFileName()
    {
        level = SceneManager.GetActiveScene().name;
        fileName = Application.dataPath + "/QATracker/" + level + "/" + SetUp.userId + ".csv";
        //Debug.Log(fileName);

        if (!Directory.Exists(Application.dataPath + "/QATracker/" + level))
        {
            Directory.CreateDirectory(Application.dataPath + "/QATracker/" + level);
            Debug.Log("Created directory");
        }

        //file per level, save to folder per level as well
    }

    public void WriteData()
    {
        //Debug.Log("Export " + fileName);

        TextWriter tw = new StreamWriter(fileName, false);
        tw.WriteLine("Point #, Game Time, Level Time, Position, Direction, Rotation, Ezra Position, Ezra Direction, Ezra Rotation, Player State, CurrentKey");
        tw.Close();

        tw = new StreamWriter(fileName, true);
        foreach(DataPoint dp in data)
        {
            string keys = "";
            if(dp.currentKey.Count > 0)
            {
                keys = dp.currentKey[0];
                foreach (string current in dp.currentKey)
                {
                    keys += "," + current;
                }
            }

            tw.WriteLine(
                data.IndexOf(dp) + "," + 
                dp.gameTime + "," +
                dp.levelTime + "," +
                dp.position.x + "|" + dp.position.y + "|" + dp.position.z + "," +
                dp.direction.x + "|" + dp.direction.y + "|" + dp.direction.z + "," +
                dp.rotation.x + "|" + dp.rotation.y + "|" + dp.rotation.z + "|" + dp.rotation.w +  "," +
                dp.ezraPosition.x + "|" + dp.ezraPosition.y + "|" + dp.ezraPosition.z + "," +
                dp.ezraDirection.x + "|" + dp.ezraDirection.y + "|" + dp.ezraDirection.z + "," +
                dp.ezraRotation.x + "|" + dp.ezraRotation.y + "|" + dp.ezraRotation.z + "|" + dp.ezraRotation.w  + "," +
                dp.playerState + "," +
                keys
                );
        }

        tw.Close();

        data.Clear();
    }

    private IEnumerator GetData()
    {
        yield return new WaitForSeconds(saveTime);

        //heres where we get all the data and put into the list
        DataPoint dp = new DataPoint();

        if(player)
        {
            dp.gameTime = SetUp.gameTime;
            dp.levelTime = levelTime;
            dp.position = new SerializableVector3(player.transform.position);
            dp.direction = new SerializableVector3(playerrb.velocity);
            dp.rotation = new SerializableQuaternion(player.transform.rotation);
            if(ezra)
            {
                dp.ezraPosition = new SerializableVector3(ezra.transform.position);
                //dp.ezraDirection = new SerializableVector3(ezrarb.velocity);
                dp.ezraRotation = new SerializableQuaternion(ezra.transform.rotation);
            }
            else
            {
                dp.ezraPosition = new SerializableVector3(Vector3.zero);
                dp.ezraRotation = new SerializableQuaternion(Quaternion.identity);
            }
            dp.playerState = pc.parkourState;

            if (Input.anyKey)
            {
                foreach (KeyCode k in System.Enum.GetValues(typeof(KeyCode)))
                {
                    if (Input.GetKey(k))
                    {
                        dp.currentKey.Add(k.ToString());
                    }
                }
            }
        }

        data.Add(dp);

        IEnumerator coroutine;
        coroutine = GetData();

        //Debug.Log("Saved");

        StartCoroutine(coroutine);
    }
}