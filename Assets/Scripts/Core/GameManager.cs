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
            UpdateMusic(newState);
            OnStateChanged?.Invoke(newState);
        }

        // BGM theo state: menu/chọn map/kết quả → nhạc chính, còn lại (đang chơi) → nhạc ingame
        static void UpdateMusic(GameState state)
        {
            switch (state)
            {
                case GameState.MainMenu:
                case GameState.MapSelect:
                case GameState.Result:
                    AudioManager.instance.PlayMenuMusic();
                    break;
                case GameState.UnitDeploy:
                case GameState.RoundActive:
                case GameState.RoundEnd:
                    AudioManager.instance.PlayIngameMusic();
                    break;
            }
        }

        public void StartRound()  => ChangeState(GameState.RoundActive);
        public void EndRound()    => ChangeState(GameState.RoundEnd);
        public void WinGame()     => ChangeState(GameState.Result);
        public void LoseGame()    => ChangeState(GameState.Result);
        public void GoMainMenu()  => ChangeState(GameState.MainMenu);
    }
}
