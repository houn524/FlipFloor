using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileFlipper : MonoBehaviour
{
    public MyTile doorTile;

    public void FlipTile(Vector3Int coord) {
        MyTile flippedTile = GetComponent<Tilemap>().GetTile<MyTile>(new Vector3Int(coord.x, coord.y, 0)).otherTile;

        GetComponent<Tilemap>().SetTile(new Vector3Int(coord.x, coord.y, 0), flippedTile);
    }

    public void DeleteTile(Vector3Int coord) {
        GetComponent<Tilemap>().SetTile(new Vector3Int(coord.x, coord.y, coord.z), null);
    }

    public void SetDoorTile(Vector3Int coord) {
        GetComponent<Tilemap>().SetTile(new Vector3Int(coord.x, coord.y, coord.z), doorTile);
    }
}
