using UnityEngine;

public class SpellCaster : MonoBehaviour
{
    public SpellCaster[] allies = new SpellCaster[2];
    public SpellCaster[] enemies = new SpellCaster[3];

    public float health = 100;
    public Sprite sprite_idle = default;
    public Sprite sprite_attack = default;
    public Sprite sprite_hurt = default;

    public SpriteRenderer sprite_renderer = default;

    protected void Start()
    {
        sprite_renderer = GetComponent<SpriteRenderer>();
    }

    public virtual void TakeAction() { }

    public void Attack() 
    {
        sprite_renderer.sprite = sprite_attack;
    }

    public void GetHurt(int damage) 
    {
        sprite_renderer.sprite = sprite_hurt;
        health -= damage;
    }

    public void Idle()
    {
        sprite_renderer.sprite = sprite_idle;
    }

    public virtual void CastSpell_0() { }

    public virtual void CastSpell_1() { }
}
