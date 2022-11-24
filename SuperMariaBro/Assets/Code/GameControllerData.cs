using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "Tecnocampus/GameControllerData", order = 1)]
public class GameControllerData : ScriptableObject
{
    public float m_Lifes;
    public float m_Shield;
    public float m_MaxBullets;
}
