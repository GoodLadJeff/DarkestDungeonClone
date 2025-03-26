using System.Collections;
using UnityEngine;

public class SpellCaster : MonoBehaviour
{
    public SpellCaster[] allies = new SpellCaster[2];
    public SpellCaster[] enemies = new SpellCaster[3];

    public SpellCaster target = default;

    public SpriteRenderer sprite_renderer = default;

    public Sprite sprite_idle = default;
    public Sprite sprite_attack = default;
    public Sprite sprite_hurt = default;

    public float health = 100;

    private Vector3 startScale = Vector3.one;
    private Vector3 targetScale = Vector3.one * 1.2f;

    private float duration = 0.1f;

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

    public void SelectedForAction()
    {
        StartCoroutine(SelectedForActionCoroutine());
    }

    private IEnumerator SelectedForActionCoroutine()
    {
        yield return StartCoroutine(ScaleObject(startScale, targetScale, duration));
        yield return StartCoroutine(ScaleObject(targetScale, startScale, duration));
    }

    private IEnumerator ScaleObject(Vector3 from, Vector3 to, float time)
    {
        float elapsed = 0f;
        while (elapsed < time)
        {
            float t = elapsed / time;
            t = Mathf.SmoothStep(0f, 1f, t); // Smooth interpolation
            transform.localScale = Vector3.Lerp(from, to, t);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.localScale = to;
    }
}
