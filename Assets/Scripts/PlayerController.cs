using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 50f;
    public float jumpSpeed = 20f;
    public float gravity = 5.0f;
    public Material[] materials;
    private bool canRun = true;
    private Rigidbody rb;
    private bool grounded = true;

    private GameManager gameManager;

    private Vector3 initialPosition;
    private Quaternion initialRotation;

    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        rb = GetComponent<Rigidbody>();

        initialPosition = transform.position;
        initialRotation = transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        if (canRun)
        {

            if (Input.GetKeyDown(KeyCode.Space) && grounded)
            {
                grounded = false;
                rb.velocity = new Vector3(0, jumpSpeed, rb.velocity.z * 0.5f);
            }

            if (Input.GetKey(KeyCode.LeftArrow) && grounded && transform.position.z < 4)
            {
                rb.velocity = new Vector3(0, rb.velocity.y, moveSpeed);
            }

            if (Input.GetKey(KeyCode.RightArrow) && grounded && transform.position.z > -4)
            {
                rb.velocity = new Vector3(0, rb.velocity.y, -moveSpeed);
            }
        }
    }

    void FixedUpdate()
    {
        if (canRun)
        {
            rb.AddForce(new Vector3(0, -gravity * rb.mass, 0));

            grounded = false;
        }
    }

    void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            grounded = true;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Hurdle")
        {
            gameManager.GameOver();
        }
    }

    public void SetMaterial(string material)
    {
        Material mat = materials[0];
        foreach (Material m in materials)
        {
            if (m.name == material)
            {
                mat = m;
            }
        }
        GetComponent<Renderer>().material = mat;

        transform.position = initialPosition;
        transform.rotation = initialRotation;
    }

    public void StartRun()
    {
        canRun = true;
    }
    public void StopRun()
    {
        canRun = false;
    }
}
