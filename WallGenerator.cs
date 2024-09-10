using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WallGenerator
{
    public static void CreateWalls(HashSet<Vector2Int> floorPositions, TilemapVisualizer tilemapVisualizer)
    {
        var basicWallPositions = FindWallsInDirections(floorPositions, Direction2D.CardinalDirectionsList);
        foreach (var position in basicWallPositions)
        {
            tilemapVisualizer.PaintSingleBasicWall(position);
        }
    }

    private static HashSet<Vector2Int> FindWallsInDirections(HashSet<Vector2Int> floorPositions,
        List<Vector2Int> cardinalDirectionsList)
    {
        var wallPositions = new HashSet<Vector2Int>();
        foreach (var neighbourPosition in floorPositions
                     .SelectMany(position => cardinalDirectionsList.Select(direction => position + direction)
                         .Where(neighbourPosition => !floorPositions.Contains(neighbourPosition))))
        {
            wallPositions.Add(neighbourPosition);
        }

        return wallPositions;
    }
}