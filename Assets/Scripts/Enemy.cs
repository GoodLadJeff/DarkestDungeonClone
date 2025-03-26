using Unity.VisualScripting;
using UnityEngine;

public class Enemy : SpellCaster
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    new void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void TakeAction()
    {
        base.TakeAction();

        // min of zero
        int minimum = 0;

        // max of length - 1 bc random range is inclusive
        int maximum = enemies.Length - 1;

        // selecting randomly an enemy and damage it
        SpellCaster enemy = enemies[Random.Range(minimum, maximum)];
        enemy.GetHurt(25);

        target = enemy;

        // change to sprite to attack sprite
        Attack();
    }
}