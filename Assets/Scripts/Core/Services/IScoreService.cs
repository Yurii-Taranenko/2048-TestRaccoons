namespace Game.Core.Services
{
    public interface IScoreService
    {
        int CurrentScore { get; }
        void AddScore(int points);
        void ResetScore();
        void Dispose();
    }
}
