using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour {

    public float speed;
    public Text countText;
    public Text winText;

    private Rigidbody rb;
    private int count;

    public float jumpTime;
    public float jumpVelocity;
    public float rollTime;
    public float rollVelocity;
    public float deAccelFactor;
    public float stoppingVelocity;

    private bool onGround;
    private bool jumping;
    private float jumpAcceleration;
    private float rollAcceleration;
    private bool movable;
    
    private AudioSource ballAudioSource;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        count = 0;
        SetCountText();
        winText.text = "";

        onGround = true;
        jumpAcceleration = jumpVelocity / jumpTime;
        rollAcceleration = rollVelocity / rollTime;

        ballAudioSource = GetComponent<AudioSource>();
    }

    void FixedUpdate()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(moveHorizontal, 0f, moveVertical);
        movement = movement.normalized;

        //rb.AddForce(movement * speed);

        if (movement != Vector3.zero)
        {
            float deltaVelocity = rollAcceleration * Time.deltaTime;

            if (movable)
                rb.velocity += movement * deltaVelocity;

            Vector3 xzMove = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

            if (xzMove.magnitude >= rollVelocity)
                movable = false;
            else
                movable = true;
        }

        if (movement == Vector3.zero && rb.velocity.magnitude > 0f && onGround)
        {
            float newVelocity = rb.velocity.magnitude;
            newVelocity *= 1 / Mathf.Pow(deAccelFactor, Time.deltaTime);

            if (newVelocity < stoppingVelocity)
                newVelocity = 0f;

            rb.velocity = rb.velocity.normalized * newVelocity;
        }
        
        Jump();
    }

    void Jump()
    {
        if (onGround)
        {
            if (Input.GetButtonDown("Jump"))
            {
                jumping = true;
                onGround = false;
                ballAudioSource.Play();
            }
        }

        if (jumping)
        {
            float deltaVelocity = jumpAcceleration * Time.deltaTime;
            rb.velocity += new Vector3(0, deltaVelocity, 0);

            if (rb.velocity.y >= jumpVelocity)
            {
                jumping = false;
            }
        }
    }
    
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            onGround = true;
        }

        Debug.Log("Collided");
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Pick Up"))
        {
            other.gameObject.SetActive(false);
            count++;
            SetCountText();
        }

        Debug.Log("Trigger" + Time.time);
    }

    void SetCountText()
    {
        countText.text = "Count: " + count.ToString();
        if (count >= 12)
        {
            winText.text = "You Win!";
            SceneManager.LoadScene("Fase 2");
        }
    }
}
