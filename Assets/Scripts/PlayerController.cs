using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour {

    public float speed;
    public Text countText;
    public Text winText;
    public AudioClip jumpSound;

    private Rigidbody rb;
    private int count;

    public float jumpTime;
    public float jumpVelocity;

    private bool onGround;
    private bool jumping;
    private float jumpAcceleration;

    private AudioSource ballAudioSource;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        count = 0;
        SetCountText();
        winText.text = "";

        onGround = true;
        jumpAcceleration = jumpVelocity / jumpTime;

        ballAudioSource = GetComponent<AudioSource>();
    }

    void FixedUpdate()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(moveHorizontal, 0f, moveVertical);

        rb.AddForce(movement * speed);

        if (onGround)
        {
            if (Input.GetButtonDown("Jump"))
            {
                jumping = true;
                onGround = false;
                ballAudioSource.Play();
            }
        }

        jumpAcceleration = jumpVelocity / jumpTime;

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

    float jump_acc(float dt)
    {
        return 300 * dt;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name == "Ground")
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
        }
    }
}
