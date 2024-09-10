using UnityEngine;

namespace Data
{
    [CreateAssetMenu(fileName = "RandomWalkParameters_", menuName = "Data/RandomWalkData")]
    public class RandomWalkSO : ScriptableObject
    {
        public int iterations = 10;
        public int walkLength = 10;
        public bool startRandomlyEachIteration = true;
    }
}