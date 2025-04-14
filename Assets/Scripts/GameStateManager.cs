using UnityEngine;

public class GameStateManager : BaseManager
{
    [Header("Debug")]
    [SerializeField] private bool debug = false;

    public enum GameState
    {
        MonsterTurn,
        HunterTurn,
        NeutralTurn,
        EventTurn,
        GamePause
    }

    public GameState CurrentGameState { get; private set; }
    private GameState _previousGameState; // Хранит предыдущее состояние

    public void SetState(GameState newState)
    {
        // Сохраняем предыдущее состояние перед изменением
        if (newState != GameState.GamePause)
        {
            _previousGameState = CurrentGameState;
        }

        CurrentGameState = newState;
    }

    public void RestorePreviousState()
    {
        CurrentGameState = _previousGameState; // Возвращаем предыдущее состояние
    }

    public override void Boot()
    {
        SetState(GameState.MonsterTurn);
    }
}
