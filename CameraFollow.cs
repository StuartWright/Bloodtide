using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    GameObject Player;
    
    void Start()
    {
        Player = GameObject.Find("Player");
    }

    void Update()
    {
        transform.position = Player.transform.position;

        if (Input.GetKey(KeyCode.Mouse1))
        {
            float mouseInputX = Input.GetAxis("Mouse X");
            Vector3 lookhere = new Vector3(0, mouseInputX * 3, 0);
            transform.Rotate(lookhere);

        }
    }
}
