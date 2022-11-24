using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;

public class MarioPlayerController : MonoBehaviour
{
    public enum TPunchType
    {
        RIGHT_HAND = 0,
        LEFT_HAND,
        KICK
    }
    Animator m_Animator;
    CharacterController m_CharacterController;
    float m_VerticalSpeed = 0.0f;

    public Camera m_Camera;
    public float m_LerpRotation = 0.85f;
    public float m_WalkSpeed = 2.5f;
    public float m_RunSpeed = 6.5f;

    [Header("Punch")]
    public float m_ComboPunchTime = 2.5f;
    float m_ComboPunchCurrentTime;
    TPunchType m_CurrentComboPunch;
    public Collider m_LeftHandCollider;
    public Collider m_RightHandCollider;
    public Collider m_RightKickCollider;
    bool m_IsPunchEnabled = false;

    private void Awake()
    {
        m_Animator = GetComponent<Animator>();
        m_CharacterController = GetComponent<CharacterController>();
    }
    void Start()
    {
        m_ComboPunchCurrentTime = -m_ComboPunchTime;
        m_LeftHandCollider.gameObject.SetActive(false);
        m_RightHandCollider.gameObject.SetActive(false);
        m_RightKickCollider.gameObject.SetActive(false);
    }


    public void SetPunchActive(TPunchType PunchType, bool Active)
    {
        if(PunchType == TPunchType.RIGHT_HAND)
        {
            m_RightHandCollider.gameObject.SetActive(Active);
        }
        else if(PunchType == TPunchType.LEFT_HAND)
        {
            m_LeftHandCollider.gameObject.SetActive(Active);
        }
        else if(PunchType == TPunchType.KICK)
        {
            m_RightKickCollider.gameObject.SetActive(Active);
        }
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

        m_VerticalSpeed += Physics.gravity.y * Time.deltaTime;
        l_Movement.y = m_VerticalSpeed * Time.deltaTime;
        if (Input.GetMouseButtonDown(0) && CanPunch())
        {
            if (MustRestartComboPunch())
            {
                SetComboPunch(TPunchType.RIGHT_HAND);
            }
            else
            {
                NextComboPunch();
            }
        }
        //m_CharacterController.Move(l_Movement);
        CollisionFlags l_CollisionFlags = m_CharacterController.Move(l_Movement);
        if((l_CollisionFlags & CollisionFlags.Below)!=0 && m_VerticalSpeed < 0.0f)
        {
            m_VerticalSpeed = 0.0f;
        }
    }

    bool CanPunch()
    {
        return !m_IsPunchEnabled;
    }

    public void SetIsPunchEnabled(bool IsPunchEnabled)
    {
        m_IsPunchEnabled = IsPunchEnabled;
    }

    bool MustRestartComboPunch()
    {
        return (Time.time - m_ComboPunchCurrentTime) > m_ComboPunchTime;
    }

    void NextComboPunch()
    {
        if(m_CurrentComboPunch == TPunchType.RIGHT_HAND)
        {
            SetComboPunch(TPunchType.LEFT_HAND);
        }
        else if (m_CurrentComboPunch == TPunchType.LEFT_HAND)
        {
            SetComboPunch(TPunchType.KICK);
        }
        else if (m_CurrentComboPunch == TPunchType.KICK)
        {
            SetComboPunch(TPunchType.RIGHT_HAND);
        }
    }
    void SetComboPunch(TPunchType PunchType)
    {
        m_CurrentComboPunch = PunchType;
        m_ComboPunchCurrentTime = Time.time;
        m_IsPunchEnabled = true;
        if(m_CurrentComboPunch == TPunchType.RIGHT_HAND)
        {
            m_Animator.SetTrigger("PunchRightHand");
        }
        else if (m_CurrentComboPunch == TPunchType.LEFT_HAND)
        {
            m_Animator.SetTrigger("PunchLeftHand");
        }
        else if (m_CurrentComboPunch == TPunchType.KICK)
        {
            m_Animator.SetTrigger("PunchKick");
        }
    }
}
