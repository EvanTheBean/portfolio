using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DataPoint
{
    public SerializableVector3 position, direction, ezraPosition, ezraDirection;
    public SerializableQuaternion rotation, ezraRotation;
    public float gameTime, levelTime;
    public int playerState;
    public List<string> currentKey = new List<string>();
    public int skippedPoints;
}

[System.Serializable]
public struct SerializableVector3
{
    public float x;
    public float y;
    public float z;

    public SerializableVector3(float rX, float rY, float rZ)
    {
        x = rX;
        y = rY;
        z = rZ;
    }

    public SerializableVector3(Vector3 r)
    {
        x = r.x;
        y = r.y;
        z = r.z;
    }

    public override string ToString()
    {
        return string.Format("[{0}, {1}, {2}]", x, y, z);
    }

    public static implicit operator Vector3(SerializableVector3 rValue)
    {
        return new Vector3(rValue.x, rValue.y, rValue.z);
    }

    public static implicit operator SerializableVector3(Vector3 rValue)
    {
        return new SerializableVector3(rValue.x, rValue.y, rValue.z);
    }
}

[System.Serializable]
public struct SerializableQuaternion
{
    public float x;
    public float y;
    public float z;
    public float w;

    public SerializableQuaternion(float rX, float rY, float rZ, float rW)
    {
        x = rX;
        y = rY;
        z = rZ;
        w = rW;
    }

    public SerializableQuaternion(Quaternion r)
    {
        x = r.x;
        y = r.y;
        z = r.z;
        w = r.w;
    }

    public override string ToString()
    {
        return string.Format("[{0}, {1}, {2}, {3}]", x, y, z, w);
    }

    public static implicit operator Quaternion(SerializableQuaternion rValue)
    {
        return new Quaternion(rValue.x, rValue.y, rValue.z, rValue.w);
    }

    public static implicit operator SerializableQuaternion(Quaternion rValue)
    {
        return new SerializableQuaternion(rValue.x, rValue.y, rValue.z, rValue.w);
    }
}

/*

[System.Serializable]
public enum GameKeys
{

}

[System.Serializable]
public enum MovementState
{

}
*/
