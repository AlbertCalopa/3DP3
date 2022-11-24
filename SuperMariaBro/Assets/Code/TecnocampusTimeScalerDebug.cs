using UnityEngine;

public class TecnocampusTimeScalerDebug : MonoBehaviour
{
    public KeyCode m_FastKeyCode = KeyCode.RightControl;
    public KeyCode m_SlowKeyCode = KeyCode.RightShift;


#if UNITY_EDITOR
    private void Update()
    {
        if (Input.GetKeyDown(m_FastKeyCode))
        {
            Time.timeScale = 10.0f;
        }
        if (Input.GetKeyDown(m_SlowKeyCode))
        {
            Time.timeScale = 0.1f;
        }
        if (Input.GetKeyUp(m_FastKeyCode))
        {
            Time.timeScale = 1.0f;
        }
        if (Input.GetKeyUp(m_SlowKeyCode))
        {
            Time.timeScale = 1.0f;
        }
    }
#endif
}


