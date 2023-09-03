using UnityEngine;

namespace Game
{
    [CreateAssetMenu(fileName = "MapConfig", menuName = "Map config")]
    public class MapConfig : ScriptableObject
    {
        [SerializeField] private int width, length;
        [SerializeField] private GameObject border, floor, destructable, wall;
        [SerializeField] private int size;

        public int Width => width;
        public int Length => length;
        public GameObject Border => border;
        public GameObject Floor => floor;
        public GameObject Destructable => destructable;
        public GameObject Wall => wall;
        public int Size => size;
    }
}