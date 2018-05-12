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

    public float rollAccelTime;
    public float rollMaxSpeed;
    public float turnMaxSpeed;  //testing
    public float deAccelFactor;
    public float stoppingVelocity;

    private bool onGround;
    private bool jumping;
    private float jumpAcceleration;

    private float rollAcceleration;
    //private bool speedIsMax = false;

    
    private AudioSource ballAudioSource;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        count = 0;
        SetCountText();
        winText.text = "";

        onGround = true;
        jumpAcceleration = jumpVelocity / jumpTime;

        rollAcceleration = rollMaxSpeed / rollAccelTime;

        ballAudioSource = GetComponent<AudioSource>();
    }

    void FixedUpdate()
    {
        MoveBall();
        
        Jump();
    }
    

    void MoveBall()
    {
        float inputFrontal = Input.GetAxis("Horizontal");
        float inputLateral = Input.GetAxis("Vertical");

        Vector3 moveInput = new Vector3(inputFrontal, 0f, inputLateral);
        moveInput = moveInput.normalized;

        Vector3 xzBallMovement = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        float ballSpeed = xzBallMovement.magnitude;
        Vector3 ballDirection = xzBallMovement.normalized;
        float angleMoveToInput;

        if (moveInput != Vector3.zero && ballDirection != Vector3.zero)
            angleMoveToInput = Vector3.Angle(ballDirection, moveInput) * Mathf.Deg2Rad;
        else
            angleMoveToInput = 0f;

        float speedInput = Mathf.Cos(angleMoveToInput);
        float turnInput = Mathf.Sin(angleMoveToInput);

        //if (ballSpeed >= rollMaxSpeed)
        //    speedIsMax = true;
        //else
        //    speedIsMax = false;


        if (moveInput != Vector3.zero)
        {
            float deltaSpeed = rollAcceleration * speedInput * Time.deltaTime;

            ballSpeed += deltaSpeed;
            if (ballSpeed > rollMaxSpeed) ballSpeed = rollMaxSpeed;

            if (rb.velocity == Vector3.zero) //angleMoveToInput == 0f
                ballDirection = moveInput * speedInput;
            else if (angleMoveToInput < 135 * Mathf.Deg2Rad)
            {
                float deltaAngle = angleMoveToInput * turnMaxSpeed * Time.deltaTime;
                ballDirection = Vector3.RotateTowards(ballDirection, moveInput, deltaAngle, 0f);
            }
            
            Vector3 ballMove = ballDirection * ballSpeed;

            rb.velocity = new Vector3(ballMove.x, rb.velocity.y, ballMove.z);
        }


        if (moveInput == Vector3.zero && rb.velocity.magnitude > 0f && onGround)
        {
            float newVelocity = rb.velocity.magnitude;
            newVelocity *= 1 / Mathf.Pow(deAccelFactor, Time.deltaTime);

            if (newVelocity < stoppingVelocity)
                newVelocity = 0f;

            rb.velocity = rb.velocity.normalized * newVelocity;
        }

    }


    //void MoveBall()
    //{
    //    float moveHorizontal = Input.GetAxis("Horizontal");
    //    float moveVertical = Input.GetAxis("Vertical");

    //    Vector3 movement = new Vector3(moveHorizontal, 0f, moveVertical);
    //    movement = movement.normalized;

    //    if (movement != Vector3.zero)
    //    {
    //        float deltaVelocity = rollAcceleration * Time.deltaTime;

    //        if (movable)
    //            rb.velocity += movement * deltaVelocity;

    //        Vector3 xzMove = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

    //        if (xzMove.magnitude >= rollVelocity)
    //            movable = false;
    //        else
    //            movable = true;
    //    }

    //    if (movement == Vector3.zero && rb.velocity.magnitude > 0f && onGround)
    //    {
    //        float newVelocity = rb.velocity.magnitude;
    //        newVelocity *= 1 / Mathf.Pow(deAccelFactor, Time.deltaTime);

    //        if (newVelocity < stoppingVelocity)
    //            newVelocity = 0f;

    //        rb.velocity = rb.velocity.normalized * newVelocity;
    //    }
    //}

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

    //void Jump()
    //{

    //    if (onGround)
    //    {
    //        if (Input.GetButtonDown("Jump"))
    //        {
    //            jumping = true;
    //            onGround = false;
    //            //rb.transform.position += new Vector3(0f, 0.1f, 0f);
    //            ballAudioSource.Play();

    //            timeJumped = Time.time;
    //        }
    //    }

    //    if (!onGround)
    //    {
    //        float deltaSpeed;
    //        float deltaTimeJumped = Time.time - timeJumped;

    //        if (jumping)
    //        {
    //            deltaSpeed = jumpAcceleration * Time.deltaTime;
    //            if (deltaTimeJumped >= jumpTime) //rb.velocity.y >= jumpVelocity
    //            {
    //                jumping = false;
    //            }
    //        }

    //        else
    //        {
    //            deltaSpeed = -gravityAcceleration * Time.deltaTime;

    //        }
    //        rb.velocity += new Vector3(0, deltaSpeed, 0);
    //    }
    //}

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
            SceneManager.LoadScene("Minigame");
        }
    }
}
