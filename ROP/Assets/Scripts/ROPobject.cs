using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[ExecuteInEditMode]
public class ROPobject : MonoBehaviour
{
    //[HideInInspector]
    public GameObject repeating, ROP1, ROP2, ROPCollider;
    //public List<GameObject> RepeatingList;
    public int r = 0;
    GameObject lastRepeating;
    public int lineClipping, objectClipping, objectRotation;
    public int clippingHorz, clippingVer;
    public float spacing;
    float length, width, height;
    public bool showClippingOptions;
    public bool swapXZ;
    bool lastRando;
    Vector3 lastLock;
    int lastR;
    public bool differentSizes;

    public bool LockRandom;

    public bool randomizeRot;

    public bool singleCollider;

    public bool customSpacing;
    Vector3 customRotation;

    public int type; //0 line, 1 grid2d, 2 grid3d 

    float horz = 0, vert = 0;
    public Vector3 rotation;

    public Vector3 gridDirection;

    public bool OneObject;

    public Vector3 spacing2D;

    public Vector3 childrenInDirection = Vector3.zero;
    public int MultiObjectChoice; //0 pattern, 1 random

    public List<GameObject> children = new List<GameObject>();

    Vector3 objectClippingNums = Vector3.zero;

    public RepeatObjectList ROL;

    public float count = 0;
           public Vector3 count2d = Vector3.zero;

    GameObject[] lastROL = new GameObject[] { };
#if (UNITY_EDITOR)
    private void Update() {
        if(ROP1.transform.hasChanged || ROP2.transform.hasChanged || this.gameObject.transform.hasChanged)
        {
            Change();
            ROP1.transform.hasChanged =true; ROP2.transform.hasChanged =true; this.gameObject.transform.hasChanged=true;
        }

        Debug.DrawLine(ROP2.transform.position, ROP1.transform.position, Color.green,Time.deltaTime/2f,false);
    }

    void UpdateChildren()
    {
        children.Clear();
        foreach (Transform child in this.gameObject.transform)
        {
            if (child.name == "ROPP1")
            {
                ROP1= child.gameObject;
            }
            else if (child.name == "ROPP2")
            {
                ROP2= child.gameObject;
            }
            else if (child.name == "ROPCollider")
            {
                ROPCollider = child.gameObject;
            }
            else
            {
                children.Add(child.gameObject);
            }
        }
    }

    void ChangeChildren()
    {
        while(children.Count > 0)
        {
            GameObject temp = children[0];
            children.Remove(temp);
            DestroyImmediate(temp);
        }
    }

    public void Change()
    {
        if(lastRepeating != repeating)
        {
            ChangeChildren();
        }
        UpdateChildren();

        if(repeating)
        {
            if (ROL.objectsToRepeat[0].GetComponent<MeshFilter>())
            {
                if(ROL.differentSizes)
                {
                    height = 0;
                    width = 0;
                    length = 0;
                    float total = 0;
                    foreach(GameObject temp in ROL.objectsToRepeat)
                    {
                        MeshFilter mesh = temp.GetComponent<MeshFilter>();
                        height += mesh.sharedMesh.bounds.extents.x * 2f * temp.transform.localScale.x;
                        width += mesh.sharedMesh.bounds.extents.z * 2f * temp.transform.localScale.z;
                        length += mesh.sharedMesh.bounds.extents.y * 2f * temp.transform.localScale.y;
                        total++;
                    }
                    height /= total;
                    width /= total;
                    length /= total;
                }
                else
                {
                    height = ROL.objectsToRepeat[0].GetComponent<MeshFilter>().sharedMesh.bounds.extents.x * 2f * ROL.objectsToRepeat[0].transform.localScale.x;
                    width = ROL.objectsToRepeat[0].GetComponent<MeshFilter>().sharedMesh.bounds.extents.z * 2f * ROL.objectsToRepeat[0].transform.localScale.z;
                    length = ROL.objectsToRepeat[0].GetComponent<MeshFilter>().sharedMesh.bounds.extents.y * 2f * ROL.objectsToRepeat[0].transform.localScale.y;
                }
            }
            else
            {
                customSpacing = true;
            }

            
//            Debug.Log(length + " " + repeating.GetComponent<MeshFilter>().sharedMesh.bounds.extents.x * 2f + " " +repeating.transform.localScale.x);
  //          Debug.Log(width + " " + repeating.GetComponent<MeshFilter>().sharedMesh.bounds.extents.y * 2f+ " " +repeating.transform.localScale.y);
    //        Debug.Log(width + " " + repeating.GetComponent<MeshFilter>().sharedMesh.bounds.extents.z * 2f+ " " +repeating.transform.localScale.z);
            

            if((!customSpacing || !(spacing > 0)) && type == 0)
            {
                spacing = rotation.normalized.x * length+ rotation.normalized.y * height + rotation.normalized.z * width;
                //Debug.Log(rotation.normalized.x * length+ " " + rotation.normalized.y * height + " " + rotation.normalized.z * width);
            }
            else if(type == 1 && !customSpacing)
            {
                spacing2D = new Vector3(height,length,width);
            }
            //Debug.Log(spacing);

            Vector3 vec = (ROP1.transform.position - ROP2.transform.position);

            count = 0;
            count2d = Vector3.zero;
            if(type == 0)
            {
                float distance = vec.magnitude;
             count = Mathf.Floor(distance/ spacing);

            if(lineClipping != 1 && objectClipping != 1 && objectClipping != 4 && objectClipping != 7)
            {
                count ++;
            }
            else if (lineClipping == 1 && count%2 == 0 && (objectClipping != 0 && objectClipping != 2 && objectClipping != 3 && objectClipping != 5 && objectClipping != 6 && objectClipping != 8))
            {
                count ++;
            }
            }
            if(type == 1)
            {
                count2d.x = new Vector2(vec.x,vec.z).magnitude / spacing2D.x;
                count2d.y = new Vector2(vec.x, vec.y).magnitude / spacing2D.x;
                count2d.z = Mathf.Floor(vec.z / spacing2D.z);
                if (gridDirection.x == 1 && gridDirection.z == 1)
                {
                    count = Mathf.Abs(Mathf.Floor(count2d.x) *  Mathf.Floor(vec.y / spacing2D.y));
                }
                else if(gridDirection.y == 1)
                {
                    count = Mathf.Abs(Mathf.Floor(count2d.y) *  Mathf.Floor(vec.z / spacing2D.y));
                }

                //Debug.Log(count2d + " " + gridDirection + " " + count);
            }

            bool listchanged = false;
            if(ROL.objectsToRepeat.Length == lastROL.Length)
            {
                //Debug.Log("Checking");
                for (int i = 0; i < ROL.objectsToRepeat.Length; i++)
                {
                    //Debug.Log(ROL.objectsToRepeat[i].name +" "+ lastROL[i].name);
                    if (ROL.objectsToRepeat[i].name != lastROL[i].name)
                    {
                        Debug.Log("Checked");
                        listchanged = true;
                    }
                }
            }

            if (lastR != r || lastROL != ROL.objectsToRepeat || listchanged)
            {
                //Debug.Log("big yeet");
                ChangeChildren();
                lastR = r;
                lastROL = ROL.objectsToRepeat;
            }
            

            //Debug.Log(distance + " " + length + " " + count);

            if(children.Count < count)
            {
                for (int i = children.Count; i<count; i++)
                {
                    GameObject temp = null;
                    if(r == 0)
                    {
                        temp = PrefabUtility.InstantiatePrefab(repeating) as GameObject;
                    }
                    else if (r ==1)
                    {
                        temp = PrefabUtility.InstantiatePrefab(ROL.objectsToRepeat[i%(ROL.objectsToRepeat.Length)]) as GameObject;
                    }
                    else
                    {
                        temp = PrefabUtility.InstantiatePrefab(ROL.objectsToRepeat[Random.Range(0,ROL.objectsToRepeat.Length)]) as GameObject;
                    }
                    temp.name = repeating.name;
                    temp.transform.parent = this.gameObject.transform;
                    if(randomizeRot)
                    {
                        temp.transform.rotation = Quaternion.LookRotation(vec);
                        temp.transform.Rotate(rotation.x * Random.Range(0, 360), rotation.y * Random.Range(0, 360), rotation.z * Random.Range(0, 360));
                    }
                }
            }
            else if (children.Count > count)
            {
                while (children.Count > count)
                {
                    GameObject temp = children[0];
                    children.Remove(temp);
                    DestroyImmediate(temp);
                }
            }

            UpdateChildren();

            if(showClippingOptions)
                {
                                    switch(clippingHorz)
                {
                    case 0: horz = length;
                    break;
                    case 1: horz = height;
                    break;
                    case 2: horz = width;
                    break;
                }
                switch(clippingVer)
                {
                    case 0: vert = length;
                    break;
                    case 1: vert = height;
                    break;
                    case 2: vert = width;
                    break;
                }
                }
                else
                {
                    clippingHorz = 0;
                    horz = length;
                    clippingVer = 1;
                    vert = height;
                }


                switch (objectClipping)
                {
                    case 0:
                        objectClippingNums = new Vector3(-horz/2 , -vert/2, -horz/2);
                        break;
                    case 1:
                        objectClippingNums = new Vector3(0, -vert/2, 0);
                        break;
                    case 2:
                        objectClippingNums = new Vector3(horz/2, -vert/2, horz/2);
                        break;
                    case 3:
                        objectClippingNums = new Vector3(-horz/2 , 0f, -horz/2);
                        break;
                    case 4:
                        objectClippingNums = new Vector3(0, 0, 0);
                        break;
                    case 5:
                        objectClippingNums = new Vector3(horz/2, 0f, horz/2);
                        break;
                    case 6:
                        objectClippingNums = new Vector3(-horz/2, vert/2, -horz/2);
                        break;
                    case 7:
                        objectClippingNums = new Vector3(0, vert/2, 0);
                        break;
                    case 8:
                        objectClippingNums = new Vector3(horz/2, vert/2, horz/2);
                        break;
                }


            for (int i =0; i < children.Count; i++)
            {
                GameObject temp = children[i];

                if(ROL.differentSizes)
                {
                    height = temp.GetComponent<MeshFilter>().sharedMesh.bounds.extents.x * 2f * temp.transform.localScale.x;
                    width = temp.GetComponent<MeshFilter>().sharedMesh.bounds.extents.z * 2f * temp.transform.localScale.z;
                    length = temp.GetComponent<MeshFilter>().sharedMesh.bounds.extents.y * 2f * temp.transform.localScale.y;

                    if ((!customSpacing || !(spacing > 0)) && type == 0)
                    {
                        spacing = rotation.normalized.x * length + rotation.normalized.y * height + rotation.normalized.z * width;
                        //Debug.Log(rotation.normalized.x * length+ " " + rotation.normalized.y * height + " " + rotation.normalized.z * width);
                    }
                    else if (type == 1 && !customSpacing)
                    {
                        spacing2D = new Vector3(height, length, width);
                    }
                }
            
                if(type == 0 && !randomizeRot)
                {
                    temp.transform.rotation = Quaternion.LookRotation(vec);
                }
                else if(type == 1)
                {
                    Vector3 lookDirection = new Vector3(ROP2.transform.position.x, gridDirection.y == 1 ? ROP2.transform.position.y : temp.transform.position.y, gridDirection.z == 1 ? ROP2.transform.position.z : temp.transform.position.z);
                    temp.transform.LookAt(lookDirection);
                }
                if (!randomizeRot)
                {
                    //Debug.Log("I am now rotating");
                    temp.transform.Rotate(rotation); //<-- you commented out this line, pls recomment at some point
                }


                /*
                else if (randomizeRot && !LockRandom)
                {
                    //temp.transform.Rotate(rotation);
                    temp.transform.Rotate(rotation.x * Random.Range(0,360), rotation.y * Random.Range(0,360), rotation.z * Random.Range(0,360));
                }
                */
                //temp.transform.rotation = Quaternion.Euler(vec);

                //                Debug.Log(new Vector3(objectClippingNums.x * vec.normalized.x, objectClippingNums.y,objectClippingNums.z * vec.normalized.z));

                if ((lastRando != randomizeRot && randomizeRot) || (randomizeRot && rotation != lastLock))
                {
                    RandomizeAll(vec);
                    lastRando = randomizeRot;
                    lastLock = rotation;
                }
                
                if(lastR != r)
                {
                    Debug.Log("smth is happening");
                    ChangeChildren();
                    lastR = r;
                }
                

                if(type == 0)
                {
                    if(lineClipping == 0)
                    {
                        temp.transform.position = ROP1.transform.position - new Vector3(i * spacing * vec.normalized.x- (objectClippingNums.x * vec.normalized.x), i * spacing * vec.normalized.y - (objectClippingNums.y),i * spacing * vec.normalized.z - (objectClippingNums.z * vec.normalized.z));
                    }
                    else if(lineClipping == 2)
                    {
                        temp.transform.position = ROP2.transform.position + new Vector3(i * spacing * vec.normalized.x + (objectClippingNums.x * vec.normalized.x), i * spacing * vec.normalized.y + (objectClippingNums.y),i * spacing * vec.normalized.z + (objectClippingNums.z * vec.normalized.z));
                    }
                    else if(lineClipping == 1)
                    {
                        Vector3 center =  Vector3.Lerp(ROP1.transform.position, ROP2.transform.position, 0.5f);
                        float halfCount = children.Count/2;
                        Vector3 startingPos = center - ((ROP1.transform.position - center).normalized) * spacing * halfCount + new Vector3(objectClippingNums.x* vec.normalized.x, objectClippingNums.y, objectClippingNums.z* vec.normalized.x) ;
                        temp.transform.position = startingPos + new Vector3(i * spacing * vec.normalized.x, i * spacing * vec.normalized.y,i * spacing * vec.normalized.z);// + objectClippingNums;
                    }
                }
                else if(type == 1)
                {
                    if (gridDirection.x == 1 && gridDirection.z == 1)
                    {
                        int j = i % (int)count2d.x;
                        int k = i / (int)count2d.x;

                        temp.transform.position = ROP1.transform.position -new Vector3(j * spacing2D.x * new Vector2(vec.x, vec.z).normalized.x, k * spacing2D.y * Mathf.Sign(vec.y), (vec.z/count2d.x) * j);
                    }
                    else if(gridDirection.y == 1)
                    {
                        int j = i % (int)count2d.y;
                        int k = i / (int)count2d.y;
                        //temp.transform.position = ROP1.transform.position -new Vector3(j * spacing2D.x * new Vector2(vec.x, vec.y).normalized.x, (vec.y / count2d.x) * k * new Vector2(vec.x, vec.y).normalized.y, k * spacing2D.z * new Vector2(vec.x, vec.y).normalized.y);
                        temp.transform.position = ROP1.transform.position -new Vector3(j * spacing2D.x * new Vector2(vec.x, vec.y).normalized.x, (vec.y / count2d.y) * j, k * spacing2D.y * Mathf.Sign(vec.z));
                    }
                }

                if(singleCollider)
                {
                    if(temp.gameObject.GetComponent<Collider>())
                    {
                        temp.gameObject.GetComponent<Collider>().enabled = false;
                        //ROP1.GetComponent<BoxCollider>().enabled = true;
                        //ROP1.GetComponent<BoxCollider>().size = new Vector3(i * spacing * vec.normalized.x, i * spacing * vec.normalized.y,i * spacing * vec.normalized.z);
                    }
                }
                else if (!singleCollider)
                {
                    if(temp.gameObject.GetComponent<Collider>())
                    {
                        temp.gameObject.GetComponent<Collider>().enabled = true;
                        //ROP1.GetComponent<Collider>().enabled = false;
                        //Debug.Log("trying to turn it back on");
                    }
                }
            }

            if(singleCollider)
                {
                    ROPCollider.GetComponent<BoxCollider>().enabled = true;
                    childrenInDirection = rotation.normalized * children.Count;
                    if(childrenInDirection.x == 0)
                    {
                        childrenInDirection.x = 1;
                    }
                    if(childrenInDirection.y == 0)
                    {
                        childrenInDirection.y = 1;
                    }
                    if(childrenInDirection.z == 0)
                    {
                        childrenInDirection.z = 1;
                    }
                    //Debug.Log(childrenInDirection);

                    Vector3 tempRotation = rotation;
                    if(tempRotation.z == 90)
                    {
                        tempRotation.z = 0;
                    }
                    else
                    {
                        tempRotation.z = 90;
                    }

                    //rotation.z = 90;
                    ROPCollider.GetComponent<BoxCollider>().size = new Vector3(childrenInDirection.x * length,childrenInDirection.y *height,childrenInDirection.z *width);//new Vector3((ROP2.transform.position - ROP1.transform.position).magnitude, height,width);
                    ROPCollider.transform.rotation = Quaternion.LookRotation(vec);
                    //ROPCollider.transform.rotation = Quaternion.Euler(ROPCollider.transform.rotation.x, ROPCollider.transform.rotation.y, 90);
                    ROPCollider.transform.Rotate(tempRotation);

                    //Debug.Log(children.Count + " " + spacing + " " + vec);
                    if(lineClipping == 0)
                    {
                        //ROPCollider.transform.position = Vector3.Lerp(ROP2.transform.position, ROP1.transform.position, 0.5f);
                        //temp.transform.position = ROP1.transform.position - new Vector3(i * spacing * vec.normalized.x- (objectClippingNums.x * vec.normalized.x), i * spacing * vec.normalized.y - (objectClippingNums.y),i * spacing * vec.normalized.z - (objectClippingNums.z * vec.normalized.z));
                        ROPCollider.transform.position = ROP1.transform.position - new Vector3(children.Count/2 * spacing * vec.normalized.x,children.Count/2 * spacing * vec.normalized.y,children.Count/2 * spacing * vec.normalized.z);// - new Vector3(objectClippingNums.x * vec.normalized.x,objectClippingNums.y,objectClippingNums.z * vec.normalized.z);
                        ROPCollider.transform.position += new Vector3(horz/2f * vec.normalized.x,0,horz/2f * vec.normalized.z);
                        if(objectClipping == 0 || objectClipping == 1 || objectClipping == 3 || objectClipping == 4 || objectClipping == 6 || objectClipping == 7)
                        {
                            ROPCollider.transform.position += new Vector3(objectClippingNums.x * vec.normalized.x * 2f,objectClippingNums.y,objectClippingNums.z * vec.normalized.z * 2f);
                        }
                        else 
                        {
                            ROPCollider.transform.position += new Vector3(0,objectClippingNums.y, 0);
                        }
                    }
                    else if(lineClipping == 2)
                    {
                        //ROPCollider.transform.position = Vector3.Lerp(ROP2.transform.position, ROP1.transform.position, 0.5f);
                        //temp.transform.position = ROP2.transform.position + new Vector3(i * spacing * vec.normalized.x + (objectClippingNums.x * vec.normalized.x), i * spacing * vec.normalized.y + (objectClippingNums.y),i * spacing * vec.normalized.z + (objectClippingNums.z * vec.normalized.z));
                        ROPCollider.transform.position = ROP2.transform.position + new Vector3(children.Count/2 * spacing * vec.normalized.x,children.Count/2 * spacing * vec.normalized.y,children.Count/2 * spacing * vec.normalized.z);
                        ROPCollider.transform.position -= new Vector3(horz/2f * vec.normalized.x,0,horz/2f * vec.normalized.z);
                        if(objectClipping == 2 || objectClipping == 5 ||objectClipping == 8)
                        {
                            ROPCollider.transform.position += new Vector3(objectClippingNums.x * vec.normalized.x * 2f,objectClippingNums.y,objectClippingNums.z * vec.normalized.z * 2f);
                        }
                        else 
                        {
                            ROPCollider.transform.position += new Vector3(0,objectClippingNums.y, 0);
                        }
                        
                    }
                    else if(lineClipping == 1)
                    {
                        ROPCollider.transform.position = Vector3.Lerp(ROP2.transform.position, ROP1.transform.position, 0.5f);
                        if(objectClipping == 1 || objectClipping == 4 || objectClipping == 7)
                        {
                            ROPCollider.transform.position += new Vector3(objectClippingNums.x * vec.normalized.x * 2f,objectClippingNums.y,objectClippingNums.z * vec.normalized.z * 2f);
                        }
                        else if(objectClipping == 0 || objectClipping == 3 || objectClipping == 6)
                        {
                            //Debug.LogError(new Vector3(objectClippingNums.x * vec.normalized.x * 2f,objectClippingNums.y,objectClippingNums.z * vec.normalized.z * 2f));
                            //ROPCollider.transform.position -= new Vector3(horz/2f * vec.normalized.x,0,horz/2f * vec.normalized.z);
                            ROPCollider.transform.position += new Vector3(objectClippingNums.x * vec.normalized.x,objectClippingNums.y,objectClippingNums.z * vec.normalized.z);
                        }
                        else
                        {
                            //Debug.LogError("Right");
                            //ROPCollider.transform.position += new Vector3(horz/2f * vec.normalized.x,0,horz/2f * vec.normalized.z);
                            ROPCollider.transform.position += new Vector3(objectClippingNums.x * vec.normalized.x,objectClippingNums.y,objectClippingNums.z * vec.normalized.z);
                        }
                        //Vector3 center =  Vector3.Lerp(ROP1.transform.position, ROP2.transform.position, 0.5f);
                        //float halfCount = children.Count/2;
                        //Vector3 startingPos = center - ((ROP1.transform.position - center).normalized) * spacing * halfCount + new Vector3(objectClippingNums.x* vec.normalized.x, objectClippingNums.y, objectClippingNums.z* vec.normalized.x) ;
                        //temp.transform.position = startingPos + new Vector3(i * spacing * vec.normalized.x, i * spacing * vec.normalized.y,i * spacing * vec.normalized.z);// + objectClippingNums;
                    }
                }
                else if (!singleCollider)
                {
                    ROPCollider.GetComponent<Collider>().enabled = false;
                    //Debug.Log("trying to turn it back on");
                }
        }
        else
        {
            Debug.LogError("No object assigned to repeat");
        }

        lastRepeating = repeating;
        lastRando = randomizeRot;
        lastR = r;
        lastROL = ROL.objectsToRepeat;


        if (randomizeRot)
        {
            lastLock = rotation;
        }

        foreach(GameObject temp in children)
        {
            PrefabUtility.RecordPrefabInstancePropertyModifications(temp);
        }
        
    }

    public void RandomizeAll(Vector3 vec)
    {
        //Debug.Log("randomizing");
        
        foreach(GameObject temp in children)
        {
            temp.transform.rotation = Quaternion.LookRotation(vec);
            temp.transform.Rotate(rotation.x * Random.Range(0, 360), rotation.y * Random.Range(0, 360), rotation.z * Random.Range(0, 360));
            //Debug.Log(temp.name);
        }
        
    }
#endif
}
