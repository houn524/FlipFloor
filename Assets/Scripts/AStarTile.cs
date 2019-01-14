using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TILE_TYPE {
    GROUND,
    END,
    WALL
}

public struct Point {
    public int x;
    public int y;

    public Point(int x, int y) {
        this.x = x;
        this.y = y;
    }
}

public class AStarTile
{
    public TILE_TYPE type;

    public Point index;

    public float f;
    public float g;
    public float h;
    public AStarTile nextTile;

    public bool flipped = false;

}
