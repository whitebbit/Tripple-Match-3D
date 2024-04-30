using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Cloth : MonoBehaviour
{
    public SkinnedMeshRenderer target;

    void Start()
    {
        if (target) Init(target);
    }

    public void Init(SkinnedMeshRenderer target)
    {
        SkinnedMeshRenderer skinnedMeshRenderer = GetComponent<SkinnedMeshRenderer>();
        skinnedMeshRenderer.bones = target.bones;
        skinnedMeshRenderer.rootBone = target.rootBone;
    }
}
