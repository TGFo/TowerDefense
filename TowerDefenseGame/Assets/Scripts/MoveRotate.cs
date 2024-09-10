using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MoveRotate : MonoBehaviour
{
    public Transform target;
    public float rotationSpeed = 100f;
    public float keyboardRotationSpeed = 100f;
    public float zoomSpeed = 5f;
    public float minDistance = 2f;
    public float maxDistance = 20f;
    public Vector3 minRotation;
    public Vector3 maxRotation;

    private float currentDistance;
    //moves camera with W and S and rotates with S and D or mouse while right click is held
    void Start()
    {
        currentDistance = Vector3.Distance(transform.position, target.position);
    }

    void Update()
    {
        if (Input.GetMouseButton(1)) // Right click held down
        {
            float horizontalMouseInput = Input.GetAxis("Mouse X");
            if (horizontalMouseInput != 0)
            {
                transform.RotateAround(target.position, Vector3.up, horizontalMouseInput * rotationSpeed * Time.deltaTime);
            }
        }
        float horizontalInput = Input.GetAxis("Horizontal"); // A = -1, D = 1
        if (horizontalInput != 0)
        {
            transform.RotateAround(target.position, Vector3.up, horizontalInput * keyboardRotationSpeed * Time.deltaTime);
        }
        float scrollInput = Input.GetAxis("Mouse ScrollWheel"); // Scroll up = positive, scroll down = negative
        if (scrollInput != 0)
        {
            currentDistance -= scrollInput * zoomSpeed;
            currentDistance = Mathf.Clamp(currentDistance, minDistance, maxDistance);
            Vector3 direction = (transform.position - target.position).normalized;
            transform.position = target.position + direction * currentDistance;
        }
    }

}