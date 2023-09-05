namespace Game
{
    public class MapSettings
    {
        private int _width = 15, _length = 15;
        
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