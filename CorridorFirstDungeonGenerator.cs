using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CorridorFirstDungeonGenerator : SimpleRandomWalkDungeonGenerator
{
    [SerializeField] private int corridorLength = 15;
    [SerializeField] private int corridorCount = 5;
    [SerializeField] [Range(0.1f, 1f)] public float roomPercent = 0.8f;

    protected override void RunGeneration()
    {
        CorridorFirstGeneration();
    }

    private void CorridorFirstGeneration()
    {
        var floorPositions = new HashSet<Vector2Int>();
        var potentialRoomPositions = new HashSet<Vector2Int>();

        CreateCorridors(floorPositions, potentialRoomPositions);

        var roomPositions = CreateRooms(potentialRoomPositions);
        var deadEnds = FindAllDeadEnds(floorPositions);
        CreateRoomsAtDeadEnds(deadEnds, roomPositions);

        floorPositions.UnionWith(roomPositions);

        tilemapVisualizer.PaintFloorTiles(floorPositions);
        WallGenerator.CreateWalls(floorPositions, tilemapVisualizer);
    }

    private void CreateRoomsAtDeadEnds(List<Vector2Int> deadEnds, HashSet<Vector2Int> roomPositions)
    {
        foreach (var roomFloor in from deadEnd in deadEnds
                 where !roomPositions.Contains(deadEnd)
                 select RunRandomWalk(randomWalkParameters, deadEnd))
        {
            roomPositions.UnionWith(roomFloor);
        }
    }

    private List<Vector2Int> FindAllDeadEnds(HashSet<Vector2Int> floorPositions)
    {
        return (from position in floorPositions
            let neighboursCount =
                Direction2D.CardinalDirectionsList.Count(direction => floorPositions.Contains(position + direction))
            where neighboursCount == 1
            select position).ToList();
    }

    private HashSet<Vector2Int> CreateRooms(HashSet<Vector2Int> potentialRoomPositions)
    {
        var roomPositions = new HashSet<Vector2Int>();
        var roomToCreateCount = Mathf.RoundToInt(potentialRoomPositions.Count * roomPercent);
        var roomToCreate = potentialRoomPositions.OrderBy(x => Guid.NewGuid()).Take(roomToCreateCount).ToList();
        foreach (var roomFloor in
                 roomToCreate.Select(roomPosition => RunRandomWalk(randomWalkParameters, roomPosition)))
        {
            roomPositions.UnionWith(roomFloor);
        }

        return roomPositions;
    }

    private void CreateCorridors(HashSet<Vector2Int> floorPositions, HashSet<Vector2Int> potentialRoomPositions)
    {
        var currentPosition = startPosition;
        potentialRoomPositions.Add(currentPosition);
        for (int i = 0; i < corridorCount; i++)
        {
            var corridor = GenerationAlgorithm.RandomWalkCorridor(currentPosition, corridorLength);
            currentPosition = corridor[^1];
            potentialRoomPositions.Add(currentPosition);
            floorPositions.UnionWith(corridor);
        }
    }
}