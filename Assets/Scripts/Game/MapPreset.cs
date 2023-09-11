using Game.Powerups;
using UnityEngine;

namespace Game
{
    [CreateAssetMenu(fileName = "MapPreset", menuName = "Map preset")]
    public class MapPreset : ScriptableObject
    {
        //TODO: add skyboxies
        [SerializeField] private GameObject border, floor, destructable, wall;
        [SerializeField] private Powerup[] drops;
        [SerializeField] private int size;
        
        public GameObject Border => border;
        public GameObject Floor => floor;
        public GameObject Destructable => destructable;
        public GameObject Wall => wall;
        public Powerup[] Drops => drops;
        public int Size => size;
    }
}