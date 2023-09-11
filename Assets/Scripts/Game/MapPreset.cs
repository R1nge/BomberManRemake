using Game.Powerups;
using UnityEngine;

namespace Game
{
    [CreateAssetMenu(fileName = "MapPreset", menuName = "Map preset")]
    public class MapPreset : ScriptableObject
    {
        //TODO: add skyboxies, corner prefab
        [SerializeField] private GameObject border, floor, destructable, wall, corner;
        [SerializeField] private Powerup[] drops;
        [SerializeField] private int size;
        [SerializeField] private Vector3 leftBottomCornerRotation, leftTopCornerRotation;
        [SerializeField] private Vector3 leftBorderRotation, bottomBorderRotation, topBorderRotation;

        public GameObject Border => border;
        public GameObject Floor => floor;
        public GameObject Destructable => destructable;
        public GameObject Wall => wall;
        public GameObject Corner => corner;
        public Powerup[] Drops => drops;
        public int Size => size;
        public Vector3 LeftBottomCornerRotation => leftBottomCornerRotation;
        public Vector3 LeftTopCornerRotation => leftTopCornerRotation;
        public Vector3 LeftBorderRotation => leftBorderRotation;
        public Vector3 BottomBorderRotation => bottomBorderRotation;
        public Vector3 TopBorderRotation => topBorderRotation;
    }
}