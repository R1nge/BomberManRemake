namespace Game
{
    public class GameSettings
    {
        private int _mapWidth = 15, _mapLength = 15;
        private int _roundsAmount = 1;
        private GameModes _gameMode;
        
        public int MapWidth => _mapWidth;
        public int MapLength => _mapLength;
        public int RoundsAmount => _roundsAmount;
        public GameModes GameMode => _gameMode;

        public void SetMapWidth(int width) => _mapWidth = width;

        public void SetMapLength(int length) => _mapLength = length;

        public void SetRoundsAmount(int amount) => _roundsAmount = amount;

        public void SetGameMode(GameModes gameMode) => _gameMode = gameMode; 
        
        public enum GameModes
        {
            Fps,
            Tps
        }
    }
}