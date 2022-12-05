using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifeItem : MonoBehaviour, IRestartGameElement
{
    // Start is called before the first frame update
    void Start()
    {
        GameController.GetGameController().AddRestartGameElement(this);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void RestartGame()
    {
        gameObject.SetActive(true);
    }
}
