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

    private Point startPoint;
    private Point endPoint;

    private List<AStarTile> _openList;
    private List<AStarTile> _closeList;
    private List<AStarTile> _path;

    private Direction[] _OddDirections = { new Direction(0, 1, 1f), new Direction(1, 1, 1f), new Direction(0, -1, 1f), new Direction(1, -1, 1f) };
    private Direction[] _EvenDirections = { new Direction(0, 1, 1f), new Direction(-1, 1, 1f), new Direction(0, -1, 1f), new Direction(-1, -1, 1f) };

    void Awake() {
        _openList = new List<AStarTile>();
        _closeList = new List<AStarTile>();
        _path = new List<AStarTile>();
    }

    private void SetTile() {
        AStarTile init = GameManager.instance.loGrid.aStarData[startPoint.y, startPoint.x];
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

            _openList.Remove(tile);
            _closeList.Add(tile);
            AddNearTile(tile);
        }
    }

    private void AddNearTile(AStarTile centerTile) {
        Direction[] _directions;

        if(centerTile.index.y % 2 == 0) {
            _directions = _EvenDirections;
        } else {
            _directions = _OddDirections;
        }

        for(int i = 0; i < _directions.Length; i++) {
            Point point = new Point(centerTile.index.x + _directions[i].point.x, centerTile.index.y + _directions[i].point.y);

            if (point.x < 0 || point.x >= LOGrid.levelWidth || point.y < 0 || point.y >= LOGrid.levelHeight || GameManager.instance.loGrid.aStarData[point.y, point.x] == null)
                continue;

            AStarTile tile = GameManager.instance.loGrid.aStarData[point.y, point.x];

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
        AStarTile tile = GameManager.instance.loGrid.aStarData[endPoint.y, endPoint.x];
        while(tile != null) {
            _path.Add(tile);

            if (tile.nextTile.Equals(GameManager.instance.loGrid.aStarData[startPoint.y, startPoint.x]))
                break;

            tile = tile.nextTile;
        }
        _path.Reverse();
    }

    public List<AStarTile> FindPath(Point startTile, Point endTile) {
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
