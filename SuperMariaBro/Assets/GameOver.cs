using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOver : MonoBehaviour
{
    public MarioPlayerController m_Mario;
    // Start is called before the first frame update
    
    public void Restart()
    {
        m_Mario.RestartGame();
    }
    public void Exit()
    {
        Application.Quit();
    }
}
