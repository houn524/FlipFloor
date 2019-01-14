using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public struct Direction {
    public Point point;
    public float weight;

    public Direction(Point point, float weight) {
        this.point = point;
        this.weight = weight;
    }

    public Direction(int x, int y, float weight) {
        this.point.x = x;
        this.point.y = y;
        this.weight = weight;
    }
}

public class PathFinder : MonoBehaviour
{
    private Grid grid;
    private Tilemap tileMap;

    private Point startPoint;
    private Point endPoint;

    private List<AStarTile> _openList;
    private List<AStarTile> _closeList;
    private List<AStarTile> _path;

    private Direction[] _directions = { new Direction(1, 0, 1f), new Direction(-1, 0, 1f), new Direction(0, 1, 1f), new Direction(0, -1, 1f) };

    void Awake() {
        _openList = new List<AStarTile>();
        _closeList = new List<AStarTile>();
        _path = new List<AStarTile>();
    }

    private void SetTile() {
        Debug.Log(startPoint.x + ", " + startPoint.y);
        Debug.Log(GameManager.instance.mapData);
        AStarTile init = GameManager.instance.mapData[startPoint.x + (GameManager.TILE_WIDTH / 2), startPoint.y + (GameManager.TILE_WIDTH / 2)];
        init.g = 0;
        init.h = Mathf.Abs(endPoint.x - startPoint.x) + Mathf.Abs(endPoint.y - startPoint.y);
        init.f = init.g + init.h;

        _openList.Add(init);
        while(_openList.Count > 0) {
            AStarTile tile = _openList[0];
            for(int i = 0; i < _openList.Count; i++) {
                if (_openList[i].f < tile.f)
                    tile = _openList[i];
            }

            //if (tile == endPoint) {
            //    break;
            //}
            Debug.Log("SetTile : " + tile.index.x + ", " + tile.index.y);

            _openList.Remove(tile);
            _closeList.Add(tile);
            AddNearTile(tile);
        }
    }

    private void AddNearTile(AStarTile centerTile) {
        for(int i = 0; i < _directions.Length; i++) {
            Point point = new Point(centerTile.index.x + _directions[i].point.x, centerTile.index.y + _directions[i].point.y);

            if (point.x < -(GameManager.TILE_WIDTH / 2) || point.x >= (GameManager.TILE_WIDTH / 2) - 1 || point.y < -(GameManager.TILE_WIDTH / 2) || point.y >= (GameManager.TILE_WIDTH / 2) - 1 || GameManager.instance.mapData[point.x + 5, point.y + 5].type.Equals(TILE_TYPE.WALL))
                continue;

            AStarTile tile = GameManager.instance.mapData[point.x + (GameManager.TILE_WIDTH / 2), point.y + (GameManager.TILE_WIDTH / 2)];

            if (_closeList.Contains(tile))
                continue;

            if(!_openList.Contains(tile)) {
                tile.g = centerTile.g + _directions[i].weight;
                tile.h = Mathf.Abs(endPoint.x - point.x) + Mathf.Abs(endPoint.y - point.y);
                tile.f = tile.g + tile.h;
                tile.nextTile = centerTile;
                _openList.Add(tile);
            } else if(tile.g > centerTile.g + 1) {
                tile.g = centerTile.g + _directions[i].weight;
                tile.f = tile.g + tile.h;
                tile.nextTile = centerTile;
            }
        }
    }

    private void FindResultPath() {
        AStarTile tile = GameManager.instance.mapData[endPoint.x + (GameManager.TILE_WIDTH / 2), endPoint.y + (GameManager.TILE_WIDTH / 2)];
        while(tile != null) {
            _path.Add(tile);

            Debug.Log(tile.index.x + ", " + tile.index.y);

            if (tile.nextTile.Equals(GameManager.instance.mapData[startPoint.x + (GameManager.TILE_WIDTH / 2), startPoint.y + (GameManager.TILE_WIDTH / 2)]))
                break;

            tile = tile.nextTile;
        }
        _path.Reverse();
    }

    public List<AStarTile> FindPath(Grid _grid, Tilemap _tileMap, Point startTile, Point endTile) {
        grid = _grid;
        tileMap = _tileMap;
        startPoint = startTile;
        endPoint = endTile;

        _openList.Clear();
        _closeList.Clear();
        _path.Clear();

        SetTile();
        FindResultPath();
        return _path;
    }
}
