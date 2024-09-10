using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    Rigidbody rb;
    public float speed;
    public Transform cameraTransform;
    Vector3 moveDirection;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        
        
    }

    // Update is called once per frame
    void Update()
    {
        float moveVertical = Input.GetAxis("Vertical");
        moveDirection = cameraTransform.forward;
        moveDirection.y = 0;
        moveDirection = moveDirection.normalized;
        rb.velocity = moveDirection * moveVertical * speed;
    }
}
