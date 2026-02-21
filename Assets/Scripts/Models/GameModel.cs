using UnityEngine;
using Game.Core.Events;
using Game.Core.Utils;
using Game.Core.Services;

namespace Game.Game.Models
{
    // MVP Model — stores and updates game score
    public class GameModel : IScoreService
    {
        private int _currentScore;

        public int CurrentScore => _currentScore;

        public void Initialize()
        {
            _currentScore = 0;
            EventBus.Subscribe<CubeMergedEvent>(OnCubeMerged);
        }

        private void OnCubeMerged(CubeMergedEvent evt)
        {
            AddScore(evt.ScoreGained);
        }

        public void AddScore(int points)
        {
            if (points < 0)
                return;

            _currentScore += points;
            EventBus.Publish(new ScoreChangedEvent { Score = _currentScore });
        }

        public void ResetScore()
        {
            _currentScore = 0;
            EventBus.Publish(new ScoreChangedEvent { Score = _currentScore });
        }

        public void Dispose()
        {
            EventBus.Unsubscribe<CubeMergedEvent>(OnCubeMerged);
        }
    }
}