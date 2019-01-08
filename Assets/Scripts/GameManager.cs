using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;

    public AStarTile[,] mapData;

    void Awake() {
        if (instance == null) {
            instance = this;
        } else if (instance != null) {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        mapData = new AStarTile[10, 10];
        Tilemap tileMap = GameObject.Find("Tilemap").GetComponent<Tilemap>();

        for(int i = 0; i < 10; i++) {
            for(int j = 0; j < 10; j++) {
                MyTile tile = tileMap.GetTile<MyTile>(new Vector3Int(i - 5, j - 5, 0));
                if(tile) {
                    mapData[i, j] = new AStarTile();
                    mapData[i, j].type = TILE_TYPE.GROUND;
                    mapData[i, j].index.x = i - 5;
                    mapData[i, j].index.y = j - 5;
                } else {
                    mapData[i, j] = new AStarTile();
                    mapData[i, j].type = TILE_TYPE.WALL;
                    mapData[i, j].index.x = i - 5;
                    mapData[i, j].index.y = j - 5;
                }
            }
        }

        for (int i = 0; i < 10; i++) {
            for (int j = 0; j < 10; j++) {
                Debug.Log("[" + i + ", " + j + "] : " + mapData[i, j].type);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
