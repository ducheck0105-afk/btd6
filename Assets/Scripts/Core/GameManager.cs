using System;
using UnityEngine;

namespace BloonsTD.Core
{
    public enum GameState
    {
        MainMenu,
        MapSelect,
        UnitDeploy,
        RoundActive,
        RoundEnd,
        Result
    }

    public class GameManager : SingletonMono<GameManager>
    {
        public GameState CurrentState { get; private set; }
        public event Action<GameState> OnStateChanged;

        void Start() => ChangeState(GameState.MainMenu);

        public void ChangeState(GameState newState)
        {
            CurrentState = newState;
            OnStateChanged?.Invoke(newState);
        }

        public void StartRound()  => ChangeState(GameState.RoundActive);
        public void EndRound()    => ChangeState(GameState.RoundEnd);
        public void WinGame()     => ChangeState(GameState.Result);
        public void LoseGame()    => ChangeState(GameState.Result);
        public void GoMainMenu()  => ChangeState(GameState.MainMenu);
    }
}
