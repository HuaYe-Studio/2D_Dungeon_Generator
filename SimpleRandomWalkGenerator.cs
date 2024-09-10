using System.Collections.Generic;
using System.Linq;
using Data;
using UnityEngine;

public class SimpleRandomWalkDungeonGenerator : AbstractDungeonGenerator
{
    [SerializeField] protected RandomWalkSO randomWalkParameters;

    protected override void RunGeneration()
    {
        var floorPositions = RunRandomWalk(randomWalkParameters, startPosition);
        tilemapVisualizer.Clear();
        tilemapVisualizer.PaintFloorTiles(floorPositions);
        WallGenerator.CreateWalls(floorPositions, tilemapVisualizer);
    }

    protected HashSet<Vector2Int> RunRandomWalk(RandomWalkSO parameters, Vector2Int position)
    {
        var currentPosition = position;
        var floorPositions = new HashSet<Vector2Int>();
        for (var i = 0; i < parameters.iterations; i++)
        {
            var path = GenerationAlgorithm.SimpleRandomWalk(currentPosition, parameters.walkLength);
            floorPositions.UnionWith(path);
            if (parameters.startRandomlyEachIteration)
                currentPosition = path.ElementAt(Random.Range(0, floorPositions.Count));
        }

        return floorPositions;
    }
}