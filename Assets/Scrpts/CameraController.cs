using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] Vector3 offset;
    [SerializeField] float smootSpeed=0.2f;
 
    
    void Update()
    {
        Vector3 newPos=target.transform.position+offset;
        Vector3 smootPos=Vector3.Lerp(transform.position,newPos,smootSpeed*Time.deltaTime);
        transform.position = smootPos;
    }
}
