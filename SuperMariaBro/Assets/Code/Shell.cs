using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shell : MonoBehaviour, IRestartGameElement
{
    Rigidbody ShellRigidbody;
    public MarioPlayerController m_Mario;
    // Start is called before the first frame update
    void Start()
    {
        GameController.GetGameController().AddRestartGameElement(this);
        ShellRigidbody = GetComponent<Rigidbody>();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {        
        if(other.tag == "Goomba")
        {
            other.gameObject.SetActive(false);
        }
        else if(other.tag == "Koopa")
        {
            other.gameObject.SetActive(false);
        }

    }
    public void RestartGame()
    {
        gameObject.SetActive(false);
    }
}
