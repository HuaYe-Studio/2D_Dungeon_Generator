using UnityEngine;
using UnityEngine.Serialization;

public abstract class AbstractDungeonGenerator : MonoBehaviour
{
    [SerializeField] protected TilemapVisualizer tilemapVisualizer;
    [SerializeField] protected Vector2Int startPosition = Vector2Int.zero;

    public void GenerateDungeon()
    {
        tilemapVisualizer.Clear();
        RunGeneration();
    }

    protected abstract void RunGeneration();
}