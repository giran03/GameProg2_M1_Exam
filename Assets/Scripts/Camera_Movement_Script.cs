using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera_Movement_Script : MonoBehaviour
{
    public Transform cameraPosition;

    void Update()
    {
        transform.position = cameraPosition.position;
    }
}
