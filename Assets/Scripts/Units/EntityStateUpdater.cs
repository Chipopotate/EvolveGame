public class EntityStateUpdater
{
    private Entity.ObjectStatus currentStatus;

    public EntityStateUpdater()
    {
        currentStatus = Entity.ObjectStatus.Unavailable; // Начальное состояние
    }

    public Entity.ObjectStatus GetCurrentStatus() => currentStatus;

    public void SetStatus(Entity.ObjectStatus newStatus)
    {
        currentStatus = newStatus;
    }
}