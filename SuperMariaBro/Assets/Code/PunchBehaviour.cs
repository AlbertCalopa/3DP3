using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PunchBehaviour : StateMachineBehaviour
{
    MarioPlayerController m_MarioPlayerController;
    public float m_StartPctTime = 0.3f;
    public float m_EndPctTime = 0.3f;
    public MarioPlayerController.TPunchType m_PunchType;
    bool m_PunchActive = false;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        m_MarioPlayerController = animator.GetComponent<MarioPlayerController>();
        m_MarioPlayerController.SetPunchActive(m_PunchType, false);
        m_PunchActive = false;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(!m_PunchActive && stateInfo.normalizedTime >= m_StartPctTime && stateInfo.normalizedTime <= m_EndPctTime)
        {
            m_MarioPlayerController.SetPunchActive(m_PunchType, true);
            m_PunchActive = true;
        }
        else if(m_PunchActive && stateInfo.normalizedTime > m_EndPctTime)
        {
            m_MarioPlayerController.SetPunchActive(m_PunchType, false);
            m_PunchActive = false;
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        m_MarioPlayerController.SetPunchActive(m_PunchType, false);
        m_MarioPlayerController.SetIsPunchEnabled(false);
    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
