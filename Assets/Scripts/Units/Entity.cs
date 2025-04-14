using System;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    [Header("Debug")]
    public bool debug = false;

    [Header("Entity Info")]
    public string entityName = "Name";
    public Sprite entityIcon;

    [Header("Settings")]
    public int maxHealth = 10;
    public int currentHealth;

    public enum ObjectStatus { Unavailable, Selectable, Available, Enemy }
    public ObjectStatus currentStatus;

    public event Action<Entity> OnDeath; // Событие смерти

    [Header("Components")]
    [SerializeField] private SpriteRenderer objRenderer;
    [SerializeField] private StatusEffectsBar effectsBar;

    [Header("Dependencies")]
    [SerializeField] private EntityStateUpdater stateUpdater;
    [SerializeField] private EntityAppearanceManager appearanceManager;
    public EntityEffectManager effectManager;

    public virtual void Boot()
    {
        objRenderer = GetComponent<SpriteRenderer>();
        if (objRenderer == null) Debug.LogError("Отсутствует SpriteRenderer у " + gameObject.name);
        appearanceManager = new EntityAppearanceManager(objRenderer);
        stateUpdater = new EntityStateUpdater();
        effectManager = new EntityEffectManager(this);
    }

    public void SetStatus(Entity.ObjectStatus newStatus)
    {
        stateUpdater.SetStatus(newStatus);
        appearanceManager.UpdateAppearance(newStatus);
    }

    public void SwitchVisibility(bool isVisible)
    {
        appearanceManager.SwitchVisibility(gameObject, isVisible);
    }

    public void ApplyEffect(StatusEffect effect)
    {
        Debug.Log("Применяем эффект " + effect.name + " к " + gameObject.name);
        effectManager.ApplyEffect(effect, this);
    }

    public void ProcessEffects()
    {
        Debug.Log("Обрабатываем эффекты для " + gameObject.name);
        effectManager.ProcessEffects();
    }

    public void ClearAllEffects()
    {
        effectManager.ClearAllEffects();
    }

    public void Die()
    {
        Debug.Log(gameObject.name + " погиб.");
        OnDeath?.Invoke(this);

        // Переносим Destroy в конец
        Destroy(gameObject);
    }
}
