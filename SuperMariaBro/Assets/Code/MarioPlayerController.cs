using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;

public class MarioPlayerController : MonoBehaviour
{
    Animator m_Animator;
    CharacterController m_CharacterController;

    public Camera m_Camera;
    public float m_LerpRotation = 0.85f;
    public float m_WalkSpeed = 2.5f;
    public float m_RunSpeed = 6.5f;
    
    private void Awake()
    {
        m_Animator = GetComponent<Animator>();
        m_CharacterController = GetComponent<CharacterController>();
    }
    void Start()
    {
        
    }

    void Update()
    {
        float l_Speed = 0.0f;

        Vector3 l_ForwardCamera = m_Camera.transform.forward;
        Vector3 l_RightCamera = m_Camera.transform.right;
        l_ForwardCamera.y = 0.0f;
        l_RightCamera.y = 0.0f;
        l_ForwardCamera.Normalize();
        l_RightCamera.Normalize();
        bool l_HasMovenent = false;

        Vector3 l_Movement = Vector3.zero;

        if (Input.GetKey(KeyCode.W))
        {
            l_HasMovenent = true;
            l_Movement = l_ForwardCamera;            
        }
        if (Input.GetKey(KeyCode.S))
        {
            l_HasMovenent = true;
            l_Movement = -l_ForwardCamera;            
        }
        if (Input.GetKey(KeyCode.A))
        {
            l_HasMovenent = true;
            l_Movement -= l_RightCamera;            
        }
        if (Input.GetKey(KeyCode.D))
        {
            l_HasMovenent = true;
            l_Movement += l_RightCamera;            
        }
        l_Movement.Normalize();

        float l_MovementSpeed = 0.0f;
        if (l_HasMovenent)
        {
            Quaternion l_LookRotation = Quaternion.LookRotation(l_Movement);
            transform.rotation = Quaternion.Lerp(transform.rotation, l_LookRotation, m_LerpRotation);

            l_Speed = 0.5f;
            l_MovementSpeed = m_WalkSpeed;
            if (Input.GetKey(KeyCode.LeftShift))
            {
                l_Speed = 1.0f;
                l_MovementSpeed = m_RunSpeed;
            }
        }
        m_Animator.SetFloat("Speed", l_Speed);        
        l_Movement = l_Movement * l_MovementSpeed * Time.deltaTime;
        if (Input.GetKeyDown(KeyCode.Space))
        {
            m_Animator.SetTrigger("Punch");            
        }
        m_CharacterController.Move(l_Movement);

    }
}
