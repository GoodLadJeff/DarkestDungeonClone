using UnityEngine;

public class Hero : SpellCaster
{
    public override void TakeAction()
    {
        base.TakeAction();

        //select the spell to fire
        //CastSpell_0();

        Attack();
    }

    public override void CastSpell_0()
    {
        base.CastSpell_0();

        // min of zero
        int minimum = 0;

        // max of length bc random range is exclusive
        int maximum = enemies.Length;

        // selecting randomly an enemy and damage it
        SpellCaster enemy = enemies[Random.Range(minimum, maximum)];
        enemy.GetHurt(25);

        // change to sprite to attack sprite
        Attack();
    }

    public void GetTarget(Enemy enemy)
    {
        enemy.GetHurt(25);
    }
}
