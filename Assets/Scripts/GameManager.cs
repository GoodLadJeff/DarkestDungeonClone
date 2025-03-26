
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Hero[] heroes;
    [SerializeField] private Enemy[] enemies;
    [SerializeField] private UI_Manager UI_Manager;
    [SerializeField] private TextMeshProUGUI combatText;
    [SerializeField] private Vector3 highLightHeroPos = Vector3.zero;
    [SerializeField] private Vector3 highLightEnemyPos = Vector3.zero;

    private SpellCaster currentUnit = default;
    private Hero currentHero = default;
    private Queue<SpellCaster> turnQueue = new Queue<SpellCaster>();

    private float actionImpactTime = 2f;
    private float actionRecoveryTime = .5f;

    private int activeHeroIndex = default;

    private bool heroTurn = false;
    private bool _enemySelectingEnabled = false;
    private bool EnemySelectingEnabled
    {
        get { return _enemySelectingEnabled; }
        set
        {
            _enemySelectingEnabled = value;
            if (value == false)
            {
                MakeEnemySmall(0);
            }
        }
    }

    private void Start()
    {
        FillHeroesAndEnemiesArrays();

        UI_Manager.OnEnemyHover.AddListener(MakeEnemyBig);
        UI_Manager.OnEnemyClick.AddListener(HeroAttack);
        UI_Manager.OnEnemyExitHover.AddListener(MakeEnemySmall);
        UI_Manager.OnHeroSpell0Click.AddListener(HeroSpell0Select);
        UI_Manager.HideHeroActionButtons();

        combatText.text = "";

        InitializeTurnQueue();
        StartCoroutine(BattleLoop());
    }

#region battle

    private void FillHeroesAndEnemiesArrays()
    {
        // filling heroes arrays
        for (int i = 0; i < heroes.Length; i++)
        {
            int index = 0;
            for (int j = 0; j < heroes.Length; j++)
            {
                // exclude self
                if (i != j)
                {
                    heroes[i].allies[index] = heroes[j];
                    index++;
                }
            }
            // all heroes have the same enemies
            heroes[i].enemies = enemies;
        }

        // filling heroes arrays
        for (int i = 0; i < enemies.Length; i++)
        {
            int index = 0;
            for (int j = 0; j < enemies.Length; j++)
            {
                // exclude self
                if (i != j)
                {
                    enemies[i].allies[index] = enemies[j];
                    index++;
                }
            }
            // all heroes have the same enemies
            enemies[i].enemies = heroes;
        }
    }

    private void InitializeTurnQueue()
    {
        List<SpellCaster> list = new List<SpellCaster>();
        
        foreach (var spellcaster in heroes) list.Add(spellcaster);
        foreach (var spellcaster in enemies) list.Add(spellcaster);

        list = list.OrderBy(i => Guid.NewGuid()).ToList();

        foreach (var spellcaster in list) turnQueue.Enqueue(spellcaster);
    }

    private IEnumerator BattleLoop()
    {
        while (true)
        {
            currentUnit = turnQueue.Dequeue();

            currentUnit.SelectedForAction();

            // Hero's turn
            if (currentUnit.GetType() == typeof(Hero))  
            {
                currentHero = (Hero)currentUnit;
                heroTurn = true;
                UI_Manager.HideHeroActionButtons(false);
                yield return PlayerTurn(currentUnit);
            }
            // Enemy's turn
            else
            {
                yield return EnemyTurn(currentUnit);
            }

            // I need them to come forward here
            StartCoroutine(HighlightSequence());

            yield return new WaitForSeconds(actionImpactTime);

            SetEveryoneIdle();

            if (turnQueue.Count <= 0) 
                InitializeTurnQueue();

            yield return new WaitForSeconds(actionRecoveryTime);
            currentHero = null;
        }
    }

    private IEnumerator PlayerTurn(SpellCaster hero)
    {
        while (heroTurn) yield return null;

        hero.TakeAction();
        EnemySelectingEnabled = false;
    }

    private IEnumerator EnemyTurn(SpellCaster enemy)
    {
        combatText.text = "Sword attack";

        yield return new WaitForSeconds(1f);

        enemy.TakeAction();
        combatText.text = "";
    }

    private void SetEveryoneIdle()
    {
        foreach (var hero in heroes) hero.Idle();
        foreach (var enemy in enemies) enemy.Idle();
    }

#endregion

    void MakeEnemyBig(int enemyIndex)
    {
        // only run if selecting is enabled
        if (!EnemySelectingEnabled) return;

        // enemies local scale mult
        enemies[enemyIndex].transform.localScale = Vector3.one * 1.05f;
    }

    void MakeEnemySmall(int enemyIndex)
    {
        // scale back all enemies
        for (int i = 0; i < 3; i++) enemies[i].transform.localScale = Vector3.one;
    }

    void HeroAttack(int enemyIndex)
    {
        // activeHeroIndex is set to -1 when it's not a hero turn
        if (!currentHero || !EnemySelectingEnabled) return;

        currentHero.GetTarget(enemies[enemyIndex]);
        currentHero.target = enemies[enemyIndex];
        heroTurn = false;
    }

    void HeroSpell0Select()
    {
        EnemySelectingEnabled = true;
        UI_Manager.HideHeroActionButtons(true);
    }

    private IEnumerator HighlightSequence()
    {
        Vector3 basePos = currentUnit.transform.position;
        Vector3 baseTargetPos = currentUnit.target.transform.position;

        Vector3 hightlightPos;
        Vector3 hightlightTargetPos;

        float travelDuration = actionImpactTime / 10;
        float spotlightDuration = actionImpactTime - (travelDuration*1.5f);

        if(currentUnit.GetType() == typeof(Hero))
        {
            hightlightPos = highLightHeroPos;
            hightlightTargetPos = highLightEnemyPos;
        }
        else
        {
            hightlightPos = highLightEnemyPos;
            hightlightTargetPos = highLightHeroPos; 
        }

        StartCoroutine(MoveToPosition(currentUnit.target.transform, hightlightTargetPos, travelDuration));
        yield return MoveToPosition(currentUnit.transform, hightlightPos, travelDuration);

        yield return new WaitForSeconds(spotlightDuration); // Pause while in the spotlight

        StartCoroutine(MoveToPosition(currentUnit.target.transform, baseTargetPos, travelDuration));
        yield return MoveToPosition(currentUnit.transform, basePos, travelDuration);
    }

    private IEnumerator MoveToPosition(Transform unit, Vector3 destination, float duration)
    {
        Vector3 startPosition = unit.position;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            t = Mathf.SmoothStep(0f, 1f, t); // Smooth transition
            unit.position = Vector3.Lerp(startPosition, destination, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        unit.position = destination; // Ensure final position is set precisely
    }
}

