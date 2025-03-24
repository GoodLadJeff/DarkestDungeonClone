
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Hero[] heroes;
    [SerializeField] private Enemy[] enemies;

    [SerializeField] private TextMeshProUGUI combatText;
    private Queue<SpellCaster> turnQueue = new Queue<SpellCaster>();
    private float actionRecoveryTime = 2f;
    private int activeHeroIndex = default;
    private bool heroTurn = false;

    [SerializeField] private UI_Manager UI_Manager;
    private Vector3[] enemiesBasePositions = new Vector3[3];
    private float enemyPositionOffsetHovered = 10;
    private bool _enemySelectingEnabled = false;
    private bool enemySelectingEnabled
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

        InitializeTurnQueue();
        StartCoroutine(BattleLoop());

        UI_Manager.OnEnemyHover.AddListener(MakeEnemyBig);
        UI_Manager.OnEnemyClick.AddListener(HeroAttack);
        UI_Manager.OnEnemyExitHover.AddListener(MakeEnemySmall);

        for (int i = 0; i < 3; i++)
        {
            enemiesBasePositions[i] = enemies[i].transform.position;
        }
        
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

        Debug.Log(list);

        foreach (var spellcaster in list) turnQueue.Enqueue(spellcaster);
    }

    private IEnumerator BattleLoop()
    {
        while (true)
        {
            SpellCaster currentUnit = turnQueue.Dequeue();

            // Hero's turn
            if (currentUnit.GetType() == typeof(Hero))  
            {
                heroTurn = true;
                yield return PlayerTurn(currentUnit);
            }
            // Enemy's turn
            else
            {
                yield return EnemyTurn(currentUnit);
            }

            SetEveryoneIdle();
            turnQueue.Enqueue(currentUnit);
        }
    }

    private IEnumerator PlayerTurn(SpellCaster hero)
    {
        combatText.text = $"{hero.name}'s turn! Select enemy to attack";
        enemySelectingEnabled = true;

        // hero selects his action
        // if it is an attack, we wait for enemy selection

        while (heroTurn) yield return null;

        hero.TakeAction();
        combatText.text = "Well Done!";
        enemySelectingEnabled = false;

        yield return new WaitForSeconds(actionRecoveryTime);
    }

    private IEnumerator EnemyTurn(SpellCaster enemy)
    {
        combatText.text = $"{enemy.name} is attacking!";

        yield return new WaitForSeconds(1f);

        enemy.TakeAction();
        combatText.text = "Ouch!";

        yield return new WaitForSeconds(actionRecoveryTime);
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
        if (!enemySelectingEnabled) return;

        // enemy to main camera vector, normalized
        Vector3 enemyToCam = (Camera.main.transform.position - enemiesBasePositions[enemyIndex]).normalized;

        // enemies are placed at vector enemy to cam * offset
        enemies[enemyIndex].transform.position = enemiesBasePositions[enemyIndex] + enemyToCam * enemyPositionOffsetHovered;
    }

    void MakeEnemySmall(int enemyIndex)
    {
        // place back all enemies
        for (int i = 0; i < 3; i++) enemies[i].transform.position = enemiesBasePositions[i];
    }

    void HeroAttack(int enemyIndex)
    {
        // activeHeroIndex is set to -1 when it's not a hero turn
        if (activeHeroIndex < 0) return;

        heroes[activeHeroIndex].GetTarget(enemies[enemyIndex]);
        heroTurn = false;
    }
}

