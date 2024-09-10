using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RoomFirstDungeonGenerator : SimpleRandomWalkDungeonGenerator
{
    [SerializeField] private int minRoomWidth = 4, minRoomHeight = 4;
    [SerializeField] private int dungeonWidth = 20, dungeonHeight = 20;
    [SerializeField] [Range(0, 10)] private int offset = 1;
    [SerializeField] private bool randomWalkRooms;

    protected override void RunGeneration()
    {
        CreateRooms();
    }

    private void CreateRooms()
    {
        var roomsList =
            GenerationAlgorithm.BSP(
                new BoundsInt((Vector3Int)startPosition, new Vector3Int(dungeonWidth, dungeonHeight, 0)),
                minRoomWidth, minRoomHeight);
        var floor = CreateSimpleRooms(roomsList);

        var roomCenters =
            roomsList.Select(room => (Vector2Int)Vector3Int.RoundToInt(room.center)).ToList();
        var corridors = ConnectRooms(roomCenters);
        floor.UnionWith(corridors);

        tilemapVisualizer.PaintFloorTiles(floor);
        WallGenerator.CreateWalls(floor, tilemapVisualizer);
    }

    private HashSet<Vector2Int> ConnectRooms(List<Vector2Int> roomCenters)
    {
        var corridors = new HashSet<Vector2Int>();
        var curCenter = roomCenters[Random.Range(0, roomCenters.Count)];
        roomCenters.Remove(curCenter);
        while (roomCenters.Count > 0)
        {
            var closestCenter = FindClosestPoint(curCenter, roomCenters);
            roomCenters.Remove(closestCenter);
            var newCorridor = CreateCorridor(curCenter, closestCenter);
            curCenter = closestCenter;
            corridors.UnionWith(newCorridor);
        }

        return corridors;
    }

    private HashSet<Vector2Int> CreateCorridor(Vector2Int curCenter, Vector2Int destination)
    {
        var corridor = new HashSet<Vector2Int>();
        var position = curCenter;
        corridor.Add(position);
        while (position.y != destination.y)
        {
            if (destination.y > position.y)
            {
                position += Vector2Int.up;
            }
            else if (destination.y < position.y)
            {
                position += Vector2Int.down;
            }

            corridor.Add(position);
        }

        while (position.x != destination.x)
        {
            if (destination.x > position.x)
            {
                position += Vector2Int.right;
            }
            else if (destination.x < position.x)
            {
                position += Vector2Int.left;
            }

            corridor.Add(position);
        }

        return corridor;
    }

    private Vector2Int FindClosestPoint(Vector2Int curCenter, List<Vector2Int> roomCenters)
    {
        var closestPoint = Vector2Int.zero;
        var minDistance = float.MaxValue;
        foreach (var position in roomCenters)
        {
            var curDistance = Vector2.Distance(curCenter, position);
            if (curDistance < minDistance)
            {
                minDistance = curDistance;
                closestPoint = position;
            }
        }

        return closestPoint;
    }

    private HashSet<Vector2Int> CreateSimpleRooms(List<BoundsInt> roomsList)
    {
        var floor = new HashSet<Vector2Int>();
        foreach (var room in roomsList)
        {
            for (int col = offset; col < room.size.x - offset; col++)
            {
                for (int row = offset; row < room.size.y - offset; row++)
                {
                    var position = (Vector2Int)room.min + new Vector2Int(col, row);
                    floor.Add(position);
                }
            }
        }

        return floor;
    }
}