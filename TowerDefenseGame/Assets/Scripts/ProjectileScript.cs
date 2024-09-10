using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileScript : MonoBehaviour
{
    public Transform target; // The target that the projectile will home in on
    public float speed = 10f; // Speed of the projectile
    public float rotateSpeed = 200f; // How quickly the projectile adjusts its direction
    public float lifeTime = 5f; // Lifetime of the projectile before it is destroyed
    public float damage;

    private Rigidbody rb;


    void FixedUpdate()
    {
        // Ensure there's a target to follow
        if (target == null) return;

        // Calculate the direction to the target
        Vector3 directionToTarget = (target.position - transform.position).normalized;

        // Calculate the rotation needed to point towards the target
        Vector3 rotationDirection = Vector3.RotateTowards(transform.forward, directionToTarget, rotateSpeed * Time.deltaTime, 0.0f);

        // Apply the rotation to the projectile
        rb.MoveRotation(Quaternion.LookRotation(rotationDirection));

        // Move the projectile forward in its current direction
        rb.velocity = transform.forward * speed;
    }

    void OnTriggerEnter(Collider other)
    {
        // Check if the projectile hits the target
        if (other.transform == target)
        {
            other.GetComponent<EnemyScript>().health -= damage;
            Destroy(gameObject);
        }
    }
    public void OnSpawn()
    {
        // Get the Rigidbody component
        rb = GetComponent<Rigidbody>();

        // Destroy the projectile after a certain time to prevent it from existing forever
        Destroy(gameObject, lifeTime);
    }
}