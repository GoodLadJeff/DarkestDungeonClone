using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UI_Manager : MonoBehaviour
{
    [SerializeField] private List<GameObject> HeroSpellButtons = default;

    public UnityEvent<int> OnEnemyHover = new();
    public UnityEvent<int> OnEnemyClick = new();
    public UnityEvent<int> OnEnemyExitHover = new();
    public UnityEvent OnHeroSpell0Click = new();

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


    public void HeroSpell0Click()
    {
        OnHeroSpell0Click.Invoke();
    }

    public void HideHeroActionButtons(bool ShouldHide = true)
    {
        // hide the buttons
        for (int i = 0; i < HeroSpellButtons.Count; i++)
        {
            HeroSpellButtons[i].SetActive(!ShouldHide);
        }
    }
}