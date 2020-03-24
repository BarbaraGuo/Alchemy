using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerMovement : MonoBehaviour //This class name must match that in the asset, no automatic refactoring
{

    [SerializeField] float      m_speed = 5.0f;
    [SerializeField] float      m_jumpForce = 2.0f;

    private Vector2 m_Movement; 
    private Animator m_animator;
    private Rigidbody2D m_body2d;
    private Sensor_Bandit m_groundSensor;
    private bool m_grounded;

    // Start is called before the first frame update
    void Start()
    {
        m_grounded = false;
        m_animator = GetComponent<Animator>();
        m_body2d = GetComponent<Rigidbody2D>();
        m_groundSensor = transform.Find("GroundSensor").GetComponent<Sensor_Bandit>();
    }

    // Update is called once per frame
    void Update() 
    {
        //Check if character just landed on the ground
        if (!m_grounded && m_groundSensor.State()) {
            m_grounded = true;
            m_animator.SetBool("Grounded", m_grounded);
        }

        //Check if character just started falling
        if(m_grounded && !m_groundSensor.State()) {
            m_grounded = false;
            m_animator.SetBool("Grounded", m_grounded);
        }

        // Use Unity's input manager 
        // Horizontal = a,d,left,right keys
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        bool isWalking = !Mathf.Approximately(horizontal,0f);
        m_animator.SetBool("IsWalking", isWalking);

        if(m_grounded){
            m_body2d.velocity = new Vector2(horizontal * m_speed, m_body2d.velocity.y);
        }
    
        m_animator.SetFloat("AirSpeed", m_body2d.velocity.y);

        if ( horizontal > 0)
            GetComponent<SpriteRenderer>().flipX = true;
        else if (horizontal < 0)
            GetComponent<SpriteRenderer>().flipX = false;

        // Handle movement
        if ((Input.GetKeyDown("space") || Input.GetKeyDown("up")) && m_grounded) {
            m_grounded = false;
            m_animator.SetBool("Grounded", m_grounded);
            m_body2d.velocity = new Vector2(m_body2d.velocity.x, m_jumpForce);
            m_groundSensor.Disable(0.2f);
        }

    }
    
}
