using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlendShapes : MonoBehaviour
{
    [Range(0, 100)]
    public List<float> blendWeight = new List<float>();

    SkinnedMeshRenderer skinnedMeshRenderer;
    int count;
    // Start is called before the first frame update
    void Start()
    {
        skinnedMeshRenderer = GetComponent<SkinnedMeshRenderer>();
        count = skinnedMeshRenderer.sharedMesh.blendShapeCount;
        while (blendWeight.Count < count)
        {
            blendWeight.Add(0);
        }
    }

    // Update is called once per frame
    void Update()
    {
        for(int i = 0; i < count; i++)
        {
            skinnedMeshRenderer.SetBlendShapeWeight(i, blendWeight[i]);
        }
    }
}
