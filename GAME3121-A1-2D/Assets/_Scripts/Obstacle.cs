using NUnit.Framework.Internal;
using UnityEngine;

public class Obstacle : MonoBehaviour
{

    public float minSize = 0.5f;

    public float maxSize = 2.0f;

    public float minSpeed = 50.0f;

    public float maxSpeed = 150.0f;


    public float maxSpinSpeed = 10.0f;

    private Soundmanager sm;

    // Start is called once before the first execution of Update after the MonoBehaviour is createdvoid Start()
    void Start()
    {

        float randomSize = Random.Range(minSize, maxSize);

        transform.localScale = new Vector3(randomSize, randomSize, 1);


        float randomSpeed = Random.Range(minSpeed, maxSpeed) / randomSize;


        Rigidbody2D rb = GetComponent<Rigidbody2D>();

        Vector2 randomDirection = Random.insideUnitCircle;

        rb.AddForce(randomDirection* randomSpeed);


        float randomTorque = Random.Range(-maxSpinSpeed, maxSpinSpeed);

        rb.AddTorque(randomTorque);

        sm = FindAnyObjectByType<Soundmanager>();

    }


    // Update is called once per framevoid Update()
    void Update()
    {



    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Obstacle"))
        {
            sm.PlayObstacleSFX();
        }
    }
}