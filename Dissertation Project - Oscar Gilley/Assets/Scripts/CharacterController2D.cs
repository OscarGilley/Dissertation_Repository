﻿using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]

//Script adapted from online resource referenced, LARGELY UNEDITED
//Resource Link: https://roystan.net/articles/character-controller-2d.html

public class CharacterController2D : MonoBehaviour
{
    [SerializeField, Tooltip("Max speed, in units per second, that the character moves.")]
    float speed = 9;

    [SerializeField, Tooltip("Acceleration while grounded.")]
    float walkAcceleration = 75;

    [SerializeField, Tooltip("Acceleration while in the air.")]
    float airAcceleration = 30;

    [SerializeField, Tooltip("Deceleration applied when character is grounded and not attempting to move.")]
    float groundDeceleration = 70;

    [SerializeField, Tooltip("Max height the character will jump regardless of gravity")]
    float jumpHeight = 4;

    private BoxCollider2D boxCollider;

    private bool grounded;

    private Vector2 velocity;

    private void Awake()
    {      
        boxCollider = GetComponent<BoxCollider2D>();    
    }

    private void Update()
    {

        float acceleration = grounded ? walkAcceleration : airAcceleration;
        float deceleration = grounded ? groundDeceleration : 0;

        velocity.y += Physics2D.gravity.y * Time.deltaTime;

        //Debug.Log(Time.deltaTime);
        //Debug.Log(velocity.y);
        //Debug.Log(grounded);


        if (grounded)
        {
           velocity.y = 0;

            if (Input.GetButtonDown("Jump"))
            {
                velocity.y = Mathf.Sqrt(2 * jumpHeight * Mathf.Abs(Physics2D.gravity.y));
                
            }

            

        }

        float moveInput = Input.GetAxisRaw("Horizontal");
        if (moveInput != 0)
        {
            velocity.x = Mathf.MoveTowards(velocity.x, speed * moveInput, acceleration * Time.deltaTime);
            transform.Translate(velocity * Time.deltaTime);
        }

        else
        {
            velocity.x = Mathf.MoveTowards(velocity.x, 0, deceleration * Time.deltaTime);
        }

        Collider2D[] hits = Physics2D.OverlapBoxAll(transform.position, boxCollider.size, 0);

        grounded = false;

        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i] == boxCollider)
            {
                continue;
            }

            ColliderDistance2D colliderDistance = hits[i].Distance(boxCollider);

            if (Vector2.Angle(colliderDistance.normal, Vector2.up) < 90 && velocity.y < 0)
            {
                grounded = true;
            }

            if (colliderDistance.isOverlapped)
            {
                transform.Translate(colliderDistance.pointA - colliderDistance.pointB);
            }
        }
    }
}
