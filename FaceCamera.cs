using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceCamera : MonoBehaviour
{
    private Camera CameraRef;
    void Start()
    {
        CameraRef = Camera.main.GetComponent<Camera>();
    }
    private void LateUpdate()
    {
        transform.LookAt(transform.position + CameraRef.transform.rotation * Vector3.forward,
           CameraRef.transform.rotation * Vector3.up);
    }
}
