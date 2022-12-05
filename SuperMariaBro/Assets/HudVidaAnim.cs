using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HudVidaAnim : MonoBehaviour
{
    public Animator VidaAnim;    
    public MarioPlayerController m_Mario;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(m_Mario.m_CurrentMarioVida != 0.9)
        {
            VidaAnim.SetBool("Hit", true);
            if(VidaAnim.GetBool("Hit") == true)
            {                
                StartCoroutine(AnimationVida());
            }
            
        }
    }

    IEnumerator AnimationVida()
    {
        yield return new WaitForSeconds(2.0f);
        VidaAnim.SetBool("Heal", true);
        VidaAnim.SetBool("Hit", false);
    }
}
