using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// [ExecuteInEditMode]
public class Billboard : MonoBehaviour
{

    public Vector3Int space = new Vector3Int(1, 1, 1);

    Transform thisTransform;
    Transform cam;

    void Start()
    {
        thisTransform = transform;
        cam = Camera.main.transform;
        LateUpdate();
    }

    public void LateUpdate()
    {
        thisTransform.rotation = Quaternion.LookRotation(Vector3.Scale(cam.position - thisTransform.position, space));
    }
}