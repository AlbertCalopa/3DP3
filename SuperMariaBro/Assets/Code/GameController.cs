using System.Collections.Generic;
using UnityEngine;
public class GameController : MonoBehaviour
{
    static GameController m_GameController = null;
    float m_PlayerHealth = 1.0f;
    float m_PlayerShield = 1.0f;
    float maxBullets = 100.0f;


    MarioPlayerController m_MarioController;
    List<IRestartGameElement> m_RestartGameElements = new List<IRestartGameElement>();
    private void Start()
    {
        DontDestroyOnLoad(this.gameObject);
    }
    public static GameController GetGameController()
    {
        if (m_GameController == null)
        {
            m_GameController = new GameObject("GameController").AddComponent<GameController>();
            GameControllerData l_GameControllerData = Resources.Load<GameControllerData>("Data");
            //m_GameController.m_PlayerHealth = l_GameControllerData.m_Lifes;
            //m_GameController.m_PlayerShield = l_GameControllerData.m_Shield;
            //m_GameController.maxBullets = l_GameControllerData.m_MaxBullets;
            //Debug.Log("Data loaded with file" + m_GameController.m_PlayerHealth);
        }
        return m_GameController;
    }

    public void AddRestartGameElement(IRestartGameElement RestartGameElement)
    {
        m_RestartGameElements.Add(RestartGameElement);
    }

    public void RestartGame()
    {
        foreach(IRestartGameElement l_RestartGameElement in m_RestartGameElements)
        {
            l_RestartGameElement.RestartGame();
        }
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            RestartGame();
        }
    }
    /* public static void DestroySingleton()
     {
         if (m_GameController != null)
         {
             GameObject.Destroy(m_GameController);            
         }
         m_GameController = null;
     }
     public void SetPlayerHealth(float PlayerHealth)
     {
         m_PlayerHealth = m_MarioController.getLife();
     }
     public float GetPlayerHealth()
     {
         return m_PlayerHealth;
     }

     public void SetMaxBullets(float Bullets)
     {
         maxBullets = m_Player.m_MaxBullets;
     }

     public float GetMaxBullets()
     {
         return maxBullets;
     }
     public void SetPlayerShield(float PlayerShield)
     {
         m_PlayerShield = m_Player.getShield();
     }
     public float GetPlayerShield()
     {
         return m_PlayerShield;
     }
     public FPPlayerController GetPlayer()
     {
         return m_Player;
     } */
    public void SetPlayer(MarioPlayerController Player)
     {
         m_MarioController = Player;
     }


}
