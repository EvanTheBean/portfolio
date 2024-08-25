using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;


[CanEditMultipleObjects]
[ExecuteInEditMode]
public class ROP : EditorWindow
{
    bool editing;

    GameObject repeating;
    public ROPobject currentEditing;

    public bool popupOpen;

    int selTypeGridInt = 0;
    int selRotInt = 0, selClipLineInt = 0, selClipObjectInt = 0, selSpacingInt = 0, selGridInt = 0;
    Vector3 customRotation = Vector3.zero;
    string[] selTypeStrings = new string[]{"line", "Grid2d"};
    string[] selGridStrings = new string[]{"Vertical", "Horizontal"};
                bool showSnappingOptions;
    string[] selClipLineStrings = new string[]{"Left", "Center", "Right"};
    string[] selClipObjectStrings = new string[]{"LeftTop", "CenterTop", "RightTop", "LeftCenter", "CenterCenter", "RightCenter", "LeftBottom", "CenterBottom", "RightBottom"};

    string[] selSpacingStrings = new string[]{"Automatic", "Custom"};
    string[] rStrings= new string []{"Repeating", "List", "RandomPick"};

    public SerializedObject _objectSO = null;
    public ReorderableList _listRE = null;

    static RepeatWindow rwindow;

    bool lockX, lockY, lockZ;

    [MenuItem("Tools/RepeatableObjectPlacer")]
    static void Init()
    {
        ROP window = EditorWindow.GetWindow<ROP>();
        window.Show();

        //rwindow = ScriptableObject.CreateInstance(typeof(RepeatWindow)) as RepeatWindow;
    }

    private void OnGUI()
    {
        //GUILayout.ExpandWidth (false);
        if (editing)
        {
            //BeginWindows();
            //Debug.Log(popupOpen);
            //popupOpen = false;
            currentEditing.r = GUILayout.Toolbar(currentEditing.r, rStrings);
            if(currentEditing.r == 0)
            {
                if(popupOpen)
                {                
                    popupOpen = false;
                    rwindow.Close();
                }
                currentEditing.repeating = (GameObject)EditorGUILayout.ObjectField("Object to Repeat", currentEditing.repeating, typeof(GameObject), false);
                if(currentEditing.ROL.objectsToRepeat.Length == 0)
                {
                    currentEditing.ROL.objectsToRepeat = new GameObject[]{currentEditing.repeating};
                }
                else
                {
                    currentEditing.ROL.objectsToRepeat[0] = currentEditing.repeating;
                }
            }
            else if(!popupOpen)
            {
                popupOpen = true;
                //rwindow = ScriptableObject.CreateInstance(typeof(RepeatWindow)) as RepeatWindow;
                //windowRect = GUILayout.Window(1, windowRect, DoWindow, "Hi There");
                //rwindow = GUILayout.Window(RepeatWindow);
                //RepeatWindow lol = new RepeatWindow(this);
                rwindow = EditorWindow.GetWindow<RepeatWindow>();
                rwindow.rop = this;
                rwindow.Show();
                //PopupWindow.Show(new Rect(), new RepeatWindow(this));
            }

            //EndWindows();
            selTypeGridInt = GUILayout.Toolbar(selTypeGridInt, selTypeStrings);
            currentEditing.type = selTypeGridInt;

            if(selTypeGridInt == 1)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Grid Direction: ", new GUILayoutOption[]{GUILayout.Width(100)});
                selGridInt = GUILayout.Toolbar(selGridInt, selGridStrings);
                if(selGridInt == 0)
                {
                    currentEditing.gridDirection = new Vector3(1,0,1);
                }
                else
                {
                    currentEditing.gridDirection = new Vector3(0,1,0);
                }
                EditorGUILayout.EndHorizontal();
            }

            //if(selTypeGridInt == 0)
            //{
                currentEditing.singleCollider = EditorGUILayout.Toggle("One Collider", currentEditing.singleCollider);
                if(currentEditing.singleCollider && (currentEditing.randomizeRot || selRotInt == 3 || selSpacingInt == 1))
                {
                    currentEditing.singleCollider = false;
                }

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Rotation: ", new GUILayoutOption[]{GUILayout.Width(75)});
                string[] selRotStrings = new string[]{"x", "y", "z", "C", "R"};
                selRotInt = GUILayout.Toolbar(selRotInt, selRotStrings, new GUILayoutOption[]{GUILayout.Width(150)});
                currentEditing.objectRotation = selRotInt;

                if(selRotInt == 3)
                {
                    currentEditing.randomizeRot = false;
                    currentEditing.singleCollider = false;
                    customRotation = (Vector3)EditorGUILayout.Vector3Field("", customRotation, new GUILayoutOption[]{GUILayout.Width(300)});
                    if (customRotation.x < 0f)
                    {
                        customRotation.x = 0f;
                    }
                    if (customRotation.y < 0f)
                    {
                        customRotation.y = 0f;
                    }
                    if (customRotation.z < 0f)
                    {
                        customRotation.z = 0f;
                    }
                }
                else if(selRotInt == 4)
                {
                    currentEditing.randomizeRot = true;
                    currentEditing.singleCollider = false;
                    EditorGUILayout.LabelField("Lock on the: ", new GUILayoutOption[]{GUILayout.Width(75)});
                    EditorGUI.indentLevel = 0;
                    //EditorGUILayout.LabelField("X ", new GUILayoutOption[]{GUILayout.Width(25)});
                    lockX = GUILayout.Toggle(lockX,"X",new GUILayoutOption[]{GUILayout.Width(25)});
                    //GUILayout.FlexibleSpace();
                    //EditorGUI.indentLevel = 0;
                    //EditorGUILayout.LabelField("Y ", new GUILayoutOption[]{GUILayout.Width(25)});
                    lockY = GUILayout.Toggle(lockY,"Y", new GUILayoutOption[]{GUILayout.Width(25)});
                    //GUILayout.FlexibleSpace();
                    //EditorGUI.indentLevel = 0;
                    //EditorGUILayout.LabelField("Z ", new GUILayoutOption[]{GUILayout.Width(25)});
                    lockZ = GUILayout.Toggle(lockZ,"Z",new GUILayoutOption[]{GUILayout.Width(25)});

                    //EditorGUI.indentLevel = 0;
                    //EditorGUILayout.LabelField("Keep Current: ", new GUILayoutOption[]{GUILayout.Width(50)});
                   // currentEditing.LockRandom = EditorGUILayout.Toggle(currentEditing.LockRandom);
                    //GUILayout.FlexibleSpace();
                    customRotation = new Vector3(lockX? 0.001f : 1, lockY? 0.001f : 1,lockZ? 0.001f : 1);

                }
                else if(selRotInt == 0)
                {
                    currentEditing.randomizeRot = false;
                    customRotation = new Vector3(90,0,0);
                }
                else if(selRotInt == 1)
                {
                    currentEditing.randomizeRot = false;
                    customRotation = new Vector3(0,90,0);
                }
                else if(selRotInt == 2)
                {
                    currentEditing.randomizeRot = false;
                    customRotation = new Vector3(0,0,90);
                }
            /*}
            else if (selTypeGridInt == 1)
            {

            }
            else if (selTypeGridInt == 2)
            {
                EditorGUILayout.BeginHorizontal();
                //string[] selRotStrings = new string[]{"xy", "yz", "xz"};
                //selRotInt = GUI.SelectionGrid(new Rect(225, 50, 200, 30), selRotInt, selRotStrings, 3);
            }
            */
            currentEditing.rotation = customRotation;
            
            EditorGUILayout.EndHorizontal();

            if(selTypeGridInt == 0)
            {
                EditorGUILayout.BeginHorizontal();
             EditorGUILayout.LabelField("Spacing: ", new GUILayoutOption[]{GUILayout.Width(75)});
            selSpacingInt = GUILayout.Toolbar(selSpacingInt, selSpacingStrings, new GUILayoutOption[]{GUILayout.Width(200)});
            if(selSpacingInt == 1)
            {
                currentEditing.customSpacing = true;
                currentEditing.spacing = (float)EditorGUILayout.FloatField("Spacing:", currentEditing.spacing);//,new GUILayoutOption[]{GUILayout.Width(100)});
                if(currentEditing.spacing < 0.01)
                {
                    currentEditing.spacing = 0.01f;
                }
            }
            else
            {
                currentEditing.customSpacing = false;
            }
            EditorGUILayout.EndHorizontal();
            }
            else if(selTypeGridInt == 1)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Spacing: ", new GUILayoutOption[]{GUILayout.Width(75)});
                selSpacingInt = GUILayout.Toolbar(selSpacingInt, selSpacingStrings, new GUILayoutOption[]{GUILayout.Width(200)});
                if(selSpacingInt == 1)
                {
                    currentEditing.customSpacing = true;
                    currentEditing.spacing2D = EditorGUILayout.Vector2Field("Spacing:", currentEditing.spacing2D);//,new GUILayoutOption[]{GUILayout.Width(100)});
                    if(currentEditing.spacing2D.x < 0.01)
                    {
                        currentEditing.spacing2D.x = 0.01f;
                    }

                    if(currentEditing.spacing2D.y < 0.01)
                    {
                        currentEditing.spacing2D.y = 0.01f;
                    }
                }
                else
                {
                    currentEditing.customSpacing = false;
                }
                EditorGUILayout.EndHorizontal();   
            }

            GUILayout.Label("Snapping to Line");
            currentEditing.lineClipping = GUILayout.Toolbar(currentEditing.lineClipping, selClipLineStrings);
            GUILayout.Label("Snapping of Object");
            currentEditing.objectClipping = GUILayout.SelectionGrid(currentEditing.objectClipping, selClipObjectStrings, 3);
            currentEditing.showClippingOptions = EditorGUILayout.BeginFoldoutHeaderGroup(currentEditing.showClippingOptions, "Change Snapping Axis");
            if(currentEditing.showClippingOptions)
            {
                currentEditing.clippingHorz = EditorGUILayout.Popup("Horizontal:" ,currentEditing.clippingHorz, new string[]{"x","y","z"});
                currentEditing.clippingVer = EditorGUILayout.Popup("Vertical:" ,currentEditing.clippingVer, new string[]{"x","y","z"});
            }

            /*
            if(GUILayout.Button("Change"))
            {
                currentEditing.Change();
            }
            */
        }
        else
        {
            repeating = (GameObject)EditorGUILayout.ObjectField("Object to Repeat", repeating, typeof(GameObject), false);
            if(GUILayout.Button("Create"))
            {
                CreateNew();
            }
        }

        if(GUI.changed)
        {
            if(editing)
            {
                Repaint();
                currentEditing.Change();
            }
            SceneView.RepaintAll(); 
        }
    }

    private void OnInspectorUpdate()
    {
        Repaint();
    }

    void OnSelectionChange()
    {
        if(Selection.gameObjects.Length == 1)
        {
            if(Selection.gameObjects[0].GetComponent<ROPobject>())
            {
                editing = true;
                nowEditing(Selection.gameObjects[0]);
            }
            else if (Selection.gameObjects[0].GetComponentInParent<ROPobject>())
            {
                editing = true;
                nowEditing(Selection.gameObjects[0].GetComponentInParent<ROPobject>().gameObject);
            }
            else
            {
                editing = false;
                nowCreating();
            }
        }
        else
        {
            editing = false;
            nowCreating();
        }
    }

    void nowEditing(GameObject editingOne)
    {
        currentEditing = editingOne.GetComponent<ROPobject>();
        repeating = currentEditing.repeating;
        selRotInt = currentEditing.objectRotation;
        selTypeGridInt = currentEditing.type;
        selClipLineInt = currentEditing.lineClipping;
        selClipObjectInt = currentEditing.objectClipping;

        _objectSO = new SerializedObject(currentEditing.ROL);
 
            //init list
            _listRE = new ReorderableList(_objectSO, _objectSO.FindProperty("objectsToRepeat"), true, 
                true, true, true);
 
            //handle drawing
            _listRE.drawHeaderCallback = (rect) => EditorGUI.LabelField(rect, "Game Objects");
            _listRE.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                rect.y += 2f;
                rect.height = EditorGUIUtility.singleLineHeight;
                GUIContent objectLabel = new GUIContent($"GameObject {index}");
                //the index will help numerate the serialized fields
                EditorGUI.PropertyField(rect, _listRE.serializedProperty.GetArrayElementAtIndex(index), objectLabel);
            };
    }

    void nowCreating()
    {
        repeating = null;
    }

    void CreateNew()
    {
        GameObject newROP = new GameObject("ROP");
         //Add Components
        newROP.AddComponent<ROPobject>();
        newROP.GetComponent<ROPobject>().repeating = repeating;

        newROP.AddComponent<RepeatObjectList>();
        newROP.GetComponent<ROPobject>().ROL = newROP.GetComponent<RepeatObjectList>();

        GameObject newRopP1 = new GameObject("ROPP1");
        GameObject newRopP2 = new GameObject("ROPP2");;
        newRopP1.transform.parent = newROP.transform;
        IconManager.SetIcon(newRopP1, IconManager.Icon.CircleGreen);
        newRopP2.transform.parent = newROP.transform;
        IconManager.SetIcon(newRopP2, IconManager.Icon.CircleRed);

        GameObject newRopCollider = new GameObject("ROPCollider");
        newRopCollider.transform.parent = newROP.transform;
        IconManager.SetIcon(newRopCollider, IconManager.Icon.CircleBlue);
        newRopCollider.AddComponent<BoxCollider>();

        newROP.GetComponent<ROPobject>().ROP1 = newRopP1;
        newROP.GetComponent<ROPobject>().ROP2 = newRopP2;
        newROP.GetComponent<ROPobject>().ROPCollider = newRopCollider;

        GameObject[] selection = new GameObject[1];
        selection[0] = newROP;
        Selection.objects = selection;
    }

    void DoWindow(int unusedWindowID)
    {
        _objectSO = new SerializedObject(currentEditing.ROL);
 
            //init list
            _listRE = new ReorderableList(_objectSO, _objectSO.FindProperty("objectsToRepeat"), true, 
                true, true, true);
 
            //handle drawing
            _listRE.drawHeaderCallback = (rect) => EditorGUI.LabelField(rect, "Game Objects");
            _listRE.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                rect.y += 2f;
                rect.height = EditorGUIUtility.singleLineHeight;
                GUIContent objectLabel = new GUIContent($"GameObject {index}");
                //the index will help numerate the serialized fields
                EditorGUI.PropertyField(rect, _listRE.serializedProperty.GetArrayElementAtIndex(index), objectLabel);
            };


        GUILayout.Label("Repeating Objects", EditorStyles.boldLabel);
        _objectSO.Update();
        _listRE.DoList(new Rect(Vector2.zero,Vector2.one * 500f));
        _objectSO.ApplyModifiedProperties();

        
        if(currentEditing.r == 0)
        {
            this.Close();
        }

        GUI.DragWindow();
    }
}


public class RepeatWindow : EditorWindow
{
    public ROP rop;
/*    public RepeatWindow(ROP ropAccess)
    {
        rop = ropAccess;
        Debug.Log("Hello1");
    }*/

    public void OnGUI()
    {
        
            rop._objectSO = new SerializedObject(rop.currentEditing.ROL);
 
            //init list
            rop._listRE = new ReorderableList(rop._objectSO, rop._objectSO.FindProperty("objectsToRepeat"), true, 
                true, true, true);
 
            //handle drawing
            rop._listRE.drawHeaderCallback = (rect) => EditorGUI.LabelField(rect, "Game Objects");
            rop._listRE.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                rect.y += 2f;
                rect.height = EditorGUIUtility.singleLineHeight;
                GUIContent objectLabel = new GUIContent($"GameObject {index}");
                //the index will help numerate the serialized fields
                EditorGUI.PropertyField(rect, rop._listRE.serializedProperty.GetArrayElementAtIndex(index), objectLabel);
            };


        GUILayout.Label("Repeating Objects", EditorStyles.boldLabel);
        rop._objectSO.Update();
        rop._listRE.DoList(new Rect(new Vector2(0,50),Vector2.one * 500f));
        rop._objectSO.ApplyModifiedProperties();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Different Sized Objects: ", new GUILayoutOption[] { GUILayout.Width(150) });
        rop.currentEditing.ROL.differentSizes = EditorGUILayout.Toggle(rop.currentEditing.ROL.differentSizes);
        EditorGUILayout.EndHorizontal();

        /*
        if(rop.currentEditing.r == 0)
        {
            this.Close();
            //rop.popupOpen = false;
        }
        */
    }

    void OnClose()
    {
        Debug.Log("Closed");
        rop.popupOpen = false;
    }
}