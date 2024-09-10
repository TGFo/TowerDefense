using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Slider healthSlider; 
    Camera cam;
    private void Start()
    {
        cam = Camera.main;
    }
    private void Update()
    {
        //healthbar follows camera
        transform.LookAt(transform.position + cam.transform.rotation * Vector3.forward, Vector3.up);
    }
    public void SetHealth(float health, float maxHealth)
    {
        healthSlider.value = health / maxHealth; 
    }
}