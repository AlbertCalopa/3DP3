using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goomba : MonoBehaviour, IRestartGameElement
{
    public float m_KillTime = 0.5f;
    public float m_KillScale = 0.2f;

    void Start()
    {
        GameController.GetGameController().AddRestartGameElement(this);
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
        gameObject.SetActive(true);
    }
    
}
