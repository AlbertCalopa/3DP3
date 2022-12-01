using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpBehaviour : StateMachineBehaviour 
{
    MarioPlayerController m_MarioPlayerController;
    public float m_StartJctTime = 0.3f;
    public float m_EndJctTime = 0.3f;
    public MarioPlayerController.TJumpType m_JumpType;
    bool m_JumpActive = false;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        m_MarioPlayerController = animator.GetComponent<MarioPlayerController>();
        m_MarioPlayerController.SetJumpActive(m_JumpType, false);
        m_JumpActive = false;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!m_JumpActive && stateInfo.normalizedTime >= m_StartJctTime && stateInfo.normalizedTime <= m_EndJctTime)
        {
            m_MarioPlayerController.SetJumpActive(m_JumpType, true);
            m_JumpActive = true;
        }
        else if (m_JumpActive && stateInfo.normalizedTime > m_EndJctTime)
        {
            m_MarioPlayerController.SetJumpActive(m_JumpType, false);
            m_JumpActive = false;
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        m_MarioPlayerController.SetJumpActive(m_JumpType, false);
        m_MarioPlayerController.SetIsJumpEnabled(false); 
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
