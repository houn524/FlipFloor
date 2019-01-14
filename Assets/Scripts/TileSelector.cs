using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileSelector : MonoBehaviour
{
    public Grid grid;

    public Tile flippedTile;

    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0)) {
            Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int coord = grid.WorldToCell(pos);
            Debug.Log(string.Format("Co-ords of mouse is [X: {0} Y: {0}]", pos.x, pos.y));
            Tile tile = GetComponent<Tilemap>().GetTile<Tile>(new Vector3Int(coord.x, coord.y, 0));

            if (tile) {
                Debug.Log(string.Format("Tile is: {0}", tile.sprite));
                GetComponent<Tilemap>().SetTile(new Vector3Int(coord.x, coord.y, 0), flippedTile);
                
            }
        }
    }
}
