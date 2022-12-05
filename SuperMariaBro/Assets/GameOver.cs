using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOver : MonoBehaviour
{
    public MarioPlayerController m_Mario;
    // Start is called before the first frame update
    
    public void Restart()
    {
        if(m_Mario.Vidas >= 0)
        {
            m_Mario.RestartGame();
        }
        else
        {
            Debug.Log("No quedan mas vidas");
        }
       
    }
    public void Exit()
    {
        Application.Quit();
    }
}
