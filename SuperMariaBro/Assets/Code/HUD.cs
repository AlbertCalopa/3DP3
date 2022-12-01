using UnityEngine;
using UnityEngine.UI;
public class HUD : MonoBehaviour
{
    public Text score;
    private void Start()
    {
        DependencyInjector.GetDependency<IScoreManager>().scoreChangedDelegate += updateScore;
    }
    private void OnDestroy()
    {
        DependencyInjector.GetDependency<IScoreManager>().scoreChangedDelegate -= updateScore;
    }
    public void updateScore(IScoreManager scoreManager)
    {         
        score.text = scoreManager.getPoints().ToString("0");
    }
}
