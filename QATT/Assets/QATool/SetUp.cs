using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class SetUp : MonoBehaviour
{
    public static string userId;
    public static float gameTime;
    public bool save;
    private static SetUp _instance = null;
    Saver saver;
    Uploader uploader;
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
            saver = gameObject.GetComponent<Saver>();
            uploader = gameObject.GetComponent<Uploader>();
            UserIDGenerator();
            saver.setUsers();
            saver.setFileName();
            //SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.activeSceneChanged += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        saver = gameObject.GetComponent<Saver>();
        uploader = gameObject.GetComponent<Uploader>();
        UserIDGenerator();
        saver.setUsers();
        saver.setFileName();
        //FullUpload();
    }
    // Update is called once per frame
    void Update()
    {
        if (save)
        {
            saver.WriteData();
            save = false;
        }
        gameTime += Time.deltaTime;
    }

    public void UserIDGenerator()
    {
        Debug.Log("New ID");
        userId = System.Guid.NewGuid().ToString();
        GameObject.Find("PlayerID").GetComponent<Text>().text = userId;
        //saver.WriteData();
        saver.setFileName();
    }

    public void CopyUserID()
    {
        GUIUtility.systemCopyBuffer = userId;
    }
    public void FullUpload()
    {
        uploader.UploadToDrive();
        uploader.DeleteFiles();
    }
    /*
    void OnDisable()
    {
        Debug.Log("Changing Scene");
        saver.WriteData();
    }
    void OnEnable()
    {
        Debug.Log("Scene Changed");
        saver.setFileName();
        saver.setUsers();
    }
    */
    void OnSceneLoaded(Scene scene, Scene newScene)
    {
        Debug.Log("Changing Scene");
        saver.WriteData();
        saver.setFileName();
        saver.setUsers();

        if (newScene.name == "MainMenu")
        {
            foreach (Button button in GameObject.FindObjectsOfType<Button>())
            {
                switch (button.name)
                {
                    case "NewId": button.onClick.AddListener(UserIDGenerator); break;
                    case "CopyId": button.onClick.AddListener(CopyUserID); break;
                    case "UploadData": button.onClick.AddListener(FullUpload); break;
                }
            }
            GameObject.Find("PlayerID").GetComponent<Text>().text = userId;
        }
    }
}