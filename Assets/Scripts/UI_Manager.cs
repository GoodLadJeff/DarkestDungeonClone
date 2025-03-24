using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UI_Manager : MonoBehaviour
{
    public UnityEvent<int> OnEnemyHover = new();
    public UnityEvent<int> OnEnemyClick = new();
    public UnityEvent<int> OnEnemyExitHover = new();

    public void EnemyHover(int index)
    {
        OnEnemyHover.Invoke(index);
    }
    
    public void EnemyClick(int index)
    {
        OnEnemyClick.Invoke(index);
    }
    
    public void EnemyExitHover(int index)
    {
        OnEnemyExitHover.Invoke(index);
    }
}