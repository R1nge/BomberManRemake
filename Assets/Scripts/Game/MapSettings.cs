namespace Game
{
    public class MapSettings
    {
        private int _width = 100, _length = 100;
        
        public int Width => _width;
        public int Length => _length;

        public void SetWidth(int width)
        {
            _width = width;
        }

        public void SetLength(int length)
        {
            _length = length;
        }
    }
}