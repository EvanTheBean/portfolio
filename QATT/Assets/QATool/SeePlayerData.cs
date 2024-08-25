using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeePlayerData : MonoBehaviour
{
    public Vector3 direction, ezraDirection;
    public float gameTime, levelTime;
    public int playerState;
    public List<string> currentKey = new List<string>();
    public GameObject ezraObject;
    public int skippedPoints;
}
