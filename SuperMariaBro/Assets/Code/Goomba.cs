using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Goomba : MonoBehaviour, IRestartGameElement
{
    public enum TState
    {
        IDLE = 0,
        PATROL,
        ATTACK
       
    }
    [SerializeField]
    float SightDistance = 8.0f;
    float EyesHeight = 0.8f;
    float EyesPlayerHeight = 1f;
    float VisualConeAngle = 60.0f;

    LayerMask SightLayerMask;

    Vector3 m_StartPosition;
    Quaternion m_StartRotation;

    Vector3 l_PlayerPosition;
    Vector3 l_DirectionToPlayerXZ = Vector3.zero;

    public float m_KillTime = 0.5f;
    public float m_KillScale = 0.2f;

    float m_StartSpeed = 3.0f;
    public float m_GoombaSpeed = 2.0f;

    bool SetDestination = false;

    NavMeshAgent NavMeshAgent;
    public List<Transform> PatrolList;
    Queue<Transform> PatrolQueue = new Queue<Transform>();

    [SerializeField]
    TState _State;

    public MarioPlayerController MarioPlayer;

    bool canHit = false;

    public TState State
    {
        get { return _State; }
        set
        {
            SetterState(value);
            _State = value;
        }
    }

    void SetterState(TState state)
    {
        NavMeshAgent.speed = m_StartSpeed;
        switch (state)
        {
            case TState.IDLE:
                break;
            case TState.PATROL:               
                NavMeshAgent.SetDestination(PatrolQueue.Peek().position);
                break;
           case TState.ATTACK:
                NavMeshAgent.speed = m_GoombaSpeed;
                break;            
        }
    }

    private void Awake()
    {
        NavMeshAgent = GetComponent<NavMeshAgent>();
    }

    void Start()
    {
        GameController.GetGameController().AddRestartGameElement(this);
        foreach (var t in PatrolList)
        {
            PatrolQueue.Enqueue(t);
        }
        canHit = false;
        SetIdleState();
        this.NavMeshAgent.isStopped = false;

        SetDestination = false;

        m_StartPosition = transform.position;
        m_StartRotation = transform.rotation;
    }

    private void Update()
    {
        switch (State)
        {
            case TState.IDLE:
                State = TState.PATROL;
                break;
            case TState.PATROL:
                UpdatePatrolState();
                break;
            case TState.ATTACK:
                UpdateAttackState();
                break;            
        }
        Vector3 l_PlayerPosition = MarioPlayer.transform.position;
    }
    void SetIdleState()
    {
        State = TState.IDLE;
    }

    void UpdatePatrolState()
    {
        NavMeshAgent.isStopped = false;
        if (SeesPlayer())
        {
            State = TState.ATTACK;
            return;
        }
        if (PatrolTargetPositionArrived())
        {
            SetNextPatrolPosition();
        }
    }

    IEnumerator WaitForReset()
    {
        yield return new WaitForSeconds(5);
        SetDestination = false;

    }

    void UpdateAttackState()
    {
        if (SetDestination == false)
        {
            StartCoroutine(WaitForReset());
            l_PlayerPosition = MarioPlayer.transform.position;
            l_DirectionToPlayerXZ = (l_PlayerPosition - transform.position);            
            l_DirectionToPlayerXZ.Normalize();
            SetDestination = true;            
        }
        NavMeshAgent.SetDestination(this.transform.position + l_DirectionToPlayerXZ);

        if (Vector3.Distance(MarioPlayer.transform.position, this.transform.position) < 0f)
        {
            NavMeshAgent.isStopped = true;            
            
        }
        if (Vector3.Distance(MarioPlayer.transform.position, this.transform.position) > 6.0f)
        {            
            State = TState.PATROL;
        }
    }   
        

    bool PatrolTargetPositionArrived()
    {
        return NavMeshAgent.isStopped || (!NavMeshAgent.hasPath && !NavMeshAgent.pathPending && NavMeshAgent.pathStatus == NavMeshPathStatus.PathComplete);
    }

    void SetNextPatrolPosition()
    {
        PatrolQueue.Enqueue(PatrolQueue.Peek());
        PatrolQueue.Dequeue();
        NavMeshAgent.destination = PatrolQueue.Peek().position;
    }

    bool SeesPlayer()
    {
        Vector3 l_PlayerPosition = MarioPlayer.transform.position;
        Vector3 l_DirectionToPlayerXZ = (l_PlayerPosition - transform.position);
        l_DirectionToPlayerXZ.y = 0.0f;
        l_DirectionToPlayerXZ.Normalize();
        Vector3 l_ForwardXZ = transform.forward;
        l_ForwardXZ.y = 0.0f;
        l_ForwardXZ.Normalize();
        Vector3 l_EyesPosition = transform.position + Vector3.up * EyesHeight;
        Vector3 l_PlayerEyesPosition = l_PlayerPosition + Vector3.up * EyesPlayerHeight;
        Vector3 l_Direction = l_PlayerEyesPosition - l_EyesPosition;
        float l_Lenght = l_Direction.magnitude;
        l_Direction /= l_Lenght;


        Ray l_Ray = new Ray(l_EyesPosition, l_Direction);

        return (Vector3.Distance(l_PlayerPosition, transform.position)) < SightDistance && Vector3.Dot(l_ForwardXZ, l_DirectionToPlayerXZ) > Mathf.Cos(VisualConeAngle * Mathf.Deg2Rad / 2.0f) &&
            !Physics.Raycast(l_Ray, l_Lenght, SightLayerMask.value);
    }
    public void Kill()
    {
        transform.localScale = new Vector3(1.0f, m_KillScale, 1.0f);
        StartCoroutine(Hide());
        
    }

    IEnumerator Hide()
    {
        yield return new WaitForSeconds(m_KillTime);
        gameObject.SetActive(false);
    }
    public void RestartGame()
    {
        transform.localScale = Vector3.one;
        transform.position = m_StartPosition;
        transform.rotation = m_StartRotation;
        gameObject.SetActive(true);
        //this.NavMeshAgent.isStopped=true;
        SetIdleState();
    }
    
}
