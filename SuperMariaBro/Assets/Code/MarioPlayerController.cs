using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.UI;
public class MarioPlayerController : MonoBehaviour, IRestartGameElement
{
    public enum TPunchType
    {
        RIGHT_HAND = 0,
        LEFT_HAND,
        KICK
    }
    public enum TJumpType 
    {
        JUMP_1 = 0, 
        JUMP_2, 
        JUMP_3
    }
    Animator m_Animator;
    public CharacterController m_CharacterController;
    float m_VerticalSpeed = 0.0f;
    public float m_JumpSpeed;

    public Camera m_Camera;
    public float m_LerpRotation = 0.85f;
    public float m_WalkSpeed = 2.5f;
    public float m_RunSpeed = 6.5f;

    public Checkpoint m_CurrentCheckpoint = null;

    [Header("Punch")]
    public float m_ComboPunchTime = 2.5f;
    float m_ComboPunchCurrentTime;
    TPunchType m_CurrentComboPunch;
    public Collider m_LeftHandCollider;
    public Collider m_RightHandCollider;
    public Collider m_RightKickCollider;
    bool m_IsPunchEnabled = false;

    [Header("Jump")]
    public float m_ComboJumpTime = 2.5f; 
    float m_ComboJumpCurrentTime; 
    TJumpType m_CurrentComboJump;  
    bool m_IsJumpEnabled = false;

    Vector3 m_StartPosition;
    Quaternion m_StartRotation;

    [Header("Elevator")]
    public float m_ElevatorDotAngle = 0.95f;
    Collider m_CurrentElevatorCollider = null;

    [Header("Bridge")]
    public float m_BridgeForce = 5.0f;

    [Header("JumpKill")]
    public float m_KillerJumpSpeed = 5.0f;
    public float m_MaxAngleAllowedToKillGoomba = 45.0f;

    [Header("Vida")]
    public float m_MarioVidaQuitada = 0.125f;
    public Image m_MarioVida;
    public float m_CurrentMarioVida;
    public bool m_Hit = false;
    public bool m_Heal = false;
    public Text FullVidaText;
    public float Vidas;

    Vector3 l_Movement;

    public Vector3 KnockBack;

    [Header("HUD")]
    public GameObject HudCanvas;
    public GameObject GameOverCanvas;

    bool ShellCaught;
    public bool ShellReady;
    public Koopa ShellKoopa;

    [Header("Misc")]
    public ParticleSystem KillParticles;
    public ParticleSystem CoinParticles;
    

    List<GameObject> ShellKoopasKilled = new List<GameObject>();

    bool m_OnGround = true;
    float m_TimeOfGround;
    public float m_TimeGrounded = 0.3f;


    private void Awake()
    {
        m_Animator = GetComponent<Animator>();
        m_CharacterController = GetComponent<CharacterController>();
    }
    void Start()
    {
        m_MarioVida.fillAmount = 1.0f;
        m_Hit = false;
        m_Heal = false;
        ShellCaught= false;
        m_CurrentMarioVida = 1.0f;
        Vidas = 3.0f;
        m_ComboPunchCurrentTime = -m_ComboPunchTime;
        m_ComboJumpCurrentTime = -m_ComboJumpTime; 
        m_LeftHandCollider.gameObject.SetActive(false);
        m_RightHandCollider.gameObject.SetActive(false);
        m_RightKickCollider.gameObject.SetActive(false);
        

        m_StartPosition = transform.position;
        m_StartRotation = transform.rotation;
        GameController.GetGameController().AddRestartGameElement(this);
        GameController.GetGameController().SetPlayer(this);
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

    public void SetJumpActive(TJumpType JumpType, bool Active) 
    {
        if (JumpType == TJumpType.JUMP_1)
        {
            m_RightHandCollider.gameObject.SetActive(Active);
        }
        else if (JumpType == TJumpType.JUMP_2)
        {
            m_LeftHandCollider.gameObject.SetActive(Active);
        }
        else if (JumpType == TJumpType.JUMP_3)
        {
            m_RightKickCollider.gameObject.SetActive(Active);
        }
    }

    void Update()
    {
        float l_Speed = 0.0f;

        m_TimeOfGround += Time.deltaTime;

        Vector3 l_ForwardCamera = m_Camera.transform.forward;
        Vector3 l_RightCamera = m_Camera.transform.right;
        l_ForwardCamera.y = 0.0f;
        l_RightCamera.y = 0.0f;
        l_ForwardCamera.Normalize();
        l_RightCamera.Normalize();
        bool l_HasMovenent = false;

        l_Movement = Vector3.zero;

        SetFall();

        if (Input.GetKey(KeyCode.Space) && m_OnGround && CanJump())
        {
            m_VerticalSpeed = m_JumpSpeed;


            if (MustRestartComboJump())
            {
                SetComboJump(TJumpType.JUMP_1);
            }
            else
            {
                NextComboJump();
            }
        }

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

        if (m_Hit)
        {
            m_CharacterController.Move(KnockBack);            
            return;
        }

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

        if (ShellCaught)
        {
            foreach(var S in ShellKoopasKilled)
            {
                if (Vector3.Distance(transform.position, S.transform.position) < 2.5f)
                {
                    if (Input.GetMouseButton(1))
                    {
                        
                        S.transform.SetParent(transform.Find("face"));
                        ShellReady = true;
                        S.transform.position = transform.forward + transform.position + Vector3.up;
                        S.transform.rotation = transform.rotation;
                        S.GetComponent<Rigidbody>().isKinematic = true;
                        


                    }
                    if (Input.GetMouseButtonUp(1))
                    {
                        ShellReady = false;
                        ShellCaught = false;
                        S.transform.SetParent(null);
                        S.GetComponent<Rigidbody>().velocity = transform.forward * 10.0f;
                        S.GetComponent<Rigidbody>().isKinematic = false;
                        
                    }

                }
            }
            
        }

        
        
        //m_CharacterController.Move(l_Movement);
        CollisionFlags l_CollisionFlags = m_CharacterController.Move(l_Movement);
        if((l_CollisionFlags & CollisionFlags.Above)!=0 && m_VerticalSpeed > 0.0f)
        {
            m_VerticalSpeed = 0.0f;
        }
        if ((l_CollisionFlags & CollisionFlags.Below) != 0)
        {
            m_VerticalSpeed = 0.0f;
            m_OnGround = true;
            m_TimeOfGround = 0;
        }
        else
        {
            m_OnGround = false;
        }
        if (m_TimeOfGround < m_TimeGrounded)
        {
            m_OnGround = true;
        }

    }

    void LateUpdate()
    {
        if(m_CurrentElevatorCollider != null)
        {
            Vector3 l_EulerRotation = transform.rotation.eulerAngles;
            transform.rotation = Quaternion.Euler(0.0f, l_EulerRotation.y, 0.0f);
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
    bool CanJump() 
    {
        return !m_IsJumpEnabled;
    }

    public void SetIsJumpEnabled(bool IsJumpEnabled) 
    {
        m_IsJumpEnabled = IsJumpEnabled; 
    }

    bool MustRestartComboPunch()
    {
        return (Time.time - m_ComboPunchCurrentTime) > m_ComboPunchTime;
    }
    bool MustRestartComboJump() 
    {
        return (Time.time - m_ComboJumpCurrentTime) > m_ComboJumpTime; 
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
    void NextComboJump() 
    {
        if (m_CurrentComboJump == TJumpType.JUMP_1)
        {
            SetComboJump(TJumpType.JUMP_2);
        }
        else if (m_CurrentComboJump == TJumpType.JUMP_2)
        {
            SetComboJump(TJumpType.JUMP_3);
        }
        else if (m_CurrentComboJump == TJumpType.JUMP_3)
        {
            SetComboJump(TJumpType.JUMP_1);
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
    void SetComboJump(TJumpType JumpType)
    {
        m_CurrentComboJump = JumpType; 
        m_ComboJumpCurrentTime = Time.time; 
        m_IsJumpEnabled = true; 
        if (m_CurrentComboJump == TJumpType.JUMP_1)
        {
            m_Animator.SetTrigger("JumpFirst");
        }
        else if (m_CurrentComboJump == TJumpType.JUMP_2)
        {
            m_Animator.SetTrigger("JumpSecond");
        }
        else if (m_CurrentComboJump == TJumpType.JUMP_3)
        { 
            m_Animator.SetTrigger("JumpThird");
        }
    }

    void SetFall()
    {
        if (!m_OnGround)
        {
            m_Animator.SetTrigger("Falling");
        }
        else
        { 
            m_Animator.SetTrigger("NotFalling");
        }
    }

    public void RestartGame()
    {
        m_CharacterController.enabled = false;
        m_MarioVida.fillAmount = m_CurrentMarioVida;
        if (m_CurrentCheckpoint == null)
        {
            transform.position = m_StartPosition;
            transform.rotation = m_StartRotation;
        }
        else
        {
            transform.position = m_CurrentCheckpoint.m_SpawnPosition.position;
            transform.rotation = m_CurrentCheckpoint.m_SpawnPosition.rotation;
        }
        m_CharacterController.enabled = true;
        HudCanvas.SetActive(true);
        GameOverCanvas.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;

    }

    public void ActivateGameOverHud()
    {
        Cursor.lockState = CursorLockMode.None;
        HudCanvas.SetActive(false);
        GameOverCanvas.SetActive(true);
        m_CharacterController.enabled = false;

    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Elevator" && CanAttachToElevator(other))
        {
            AttachToElevator(other);
        }
        else if(other.tag == "CheckPoint")
        {
            m_CurrentCheckpoint = other.GetComponent<Checkpoint>();
        }
        else if(other.tag == "Coin")
        {
            other.GetComponent<Coin>().Pick();
            CoinParticles.transform.position = other.transform.position + Vector3.up;
            CoinParticles.Play();
        }
        else if (other.tag == "DamagePlayer")
        {
            DamagePlayer();
            m_Animator.SetTrigger("Damage");
            KnockBack = ((transform.position  - other.transform.position)+ Vector3.up).normalized * 0.025f;
        }
        else if (other.tag == "DeadZone")
        {
            Vidas -= 1.0f;
            FullVidaText.text = "" + Vidas;
            m_CurrentMarioVida = 1.0f;
            ActivateGameOverHud();
        }
        else if (other.tag == "DamageShell")
        {
            DamagePlayer();            
        }
        else if (other.tag == "LifeItem")
        {
            HealPlayer();
            if (m_CurrentMarioVida < 1.01f)
            {
                other.gameObject.SetActive(false);
            }
        }
        else if(other.tag == "Shell")
        {
            ShellCaught = true;            
        }

    }
    private void OnTriggerExit(Collider other)
    {
        if(other.tag == "Elevator" && other==m_CurrentElevatorCollider)
        {
            DetachElevator();
        }        
    }
    private void OnTriggerStay(Collider other)
    {
        if(other.tag == "Elevator")
        {
            if(m_CurrentElevatorCollider == other && Vector3.Dot(other.transform.up, Vector3.up) < m_ElevatorDotAngle)
            {
                DetachElevator();
            }
            if (CanAttachToElevator(other))
            {
                AttachToElevator(other);
            }
        }
    }

    bool CanAttachToElevator(Collider other)
    {
        return m_CurrentElevatorCollider == null && Vector3.Dot(other.transform.up, Vector3.up) >= m_ElevatorDotAngle;
    }
    void AttachToElevator(Collider other)
    {
        transform.SetParent(other.transform);
        m_CurrentElevatorCollider = other;
    }
    void DetachElevator()
    {
        transform.SetParent(null);
        m_CurrentElevatorCollider = null;
    }
    void JumpOverEnemy()
    {
        m_VerticalSpeed = m_KillerJumpSpeed;
    }
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if(hit.gameObject.tag == "Bridge")
        {
            hit.gameObject.GetComponent<Rigidbody>().AddForceAtPosition(-hit.normal * m_BridgeForce, hit.point);
        }
        else if(hit.gameObject.tag == "Goomba")
        {
            if (CanKillGoomba(hit.normal))
            {
                hit.gameObject.GetComponent<Goomba>().Kill();
                JumpOverEnemy();
                KillParticles.transform.position = hit.gameObject.transform.position;
                KillParticles.Play();
            }                
        }
        else if(hit.gameObject.tag == "Koopa")
        {
            ShellKoopasKilled.Add(hit.gameObject.GetComponent<Koopa>().Kill());
            KillParticles.transform.position = hit.gameObject.transform.position;
            KillParticles.Play();
            hit.gameObject.SetActive(false);
        }
        
    }

    

    public void DamagePlayer()
    {
        m_CurrentMarioVida = m_MarioVida.fillAmount - m_MarioVidaQuitada;
        m_MarioVida.fillAmount = m_CurrentMarioVida;
        m_Hit = true;
        if(m_CurrentMarioVida <= 0)
        {
            Vidas -= 1.0f;
            FullVidaText.text = "" +Vidas;
            m_CurrentMarioVida = 1.0f;
            ActivateGameOverHud();
        }
        StartCoroutine(PlayerHit());
        
    }

    public void HealPlayer()
    {
        m_CurrentMarioVida = m_MarioVida.fillAmount + m_MarioVidaQuitada;
        m_MarioVida.fillAmount = m_CurrentMarioVida;
        m_Heal = true;
        StartCoroutine(PlayerHeal());
    }
    IEnumerator PlayerHit()
    {
        yield return new WaitForSeconds(0.2f);
        m_Hit = false;
    }

    IEnumerator PlayerHeal()
    {
        yield return new WaitForSeconds(0.2f);
        m_Heal = false;
    }

    bool CanKillGoomba(Vector3 Normal)
    {
        return Vector3.Dot(Normal, Vector3.up) >= Mathf.Cos(m_MaxAngleAllowedToKillGoomba * Mathf.Deg2Rad);
    }
}
