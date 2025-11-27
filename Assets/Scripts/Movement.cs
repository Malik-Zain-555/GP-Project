using NUnit.Framework;
using Unity.VisualScripting;
using UnityEngine;

public class Movement : MonoBehaviour
{
    float horizontalInput;
    public float moveSpeed = 5f;
    bool jump = false;
    bool crouch = false;
    public CharacterController2D controller;
    Animator animator;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        // Movement Left/Right
        horizontalInput = Input.GetAxis("Horizontal") * moveSpeed;
        // Movement Animation
        animator.SetFloat("Speed", Mathf.Abs(horizontalInput));

        // Jump
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.W))
        {
            jump = true;
            // Jump Animation
            animator.SetBool("IsJumping", true);
        }

        // crouch 
        if (Input.GetKeyDown(KeyCode.S))
        {
            crouch = true;
        }
        else if (Input.GetKeyUp(KeyCode.S))
        {
            crouch = false;
        }
    }

    public void OnLanding()
    {
        animator.SetBool("IsJumping", false);
    }

    public void OnCrouching(bool isCrouching)
    {
        animator.SetBool("IsCrouching", isCrouching);
    }

    void FixedUpdate()
    {
        controller.Move(horizontalInput * Time.deltaTime, crouch, jump);
        jump = false;
    }
}