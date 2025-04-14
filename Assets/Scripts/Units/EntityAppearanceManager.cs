using UnityEngine;

public class EntityAppearanceManager
{
    private SpriteRenderer objRenderer;

    public EntityAppearanceManager(SpriteRenderer renderer)
    {
        objRenderer = renderer;
    }

    public void UpdateAppearance(Entity.ObjectStatus status)
    {
        switch (status)
        {
            case Entity.ObjectStatus.Unavailable:
                UpdateOutlineColor(Color.gray);
                break;
            case Entity.ObjectStatus.Selectable:
                UpdateOutlineColor(Color.green);
                break;
            case Entity.ObjectStatus.Available:
                UpdateOutlineColor(Color.yellow);
                break;
            case Entity.ObjectStatus.Enemy:
                UpdateOutlineColor(Color.red);
                break;
        }
    }

    private void UpdateOutlineColor(Color color)
    {
        objRenderer.material.SetColor("_SolidOutline", color);
    }

    public void SwitchVisibility(GameObject gameObject, bool isVisible)
    {
        gameObject.SetActive(isVisible);
    }
}
