using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;

    public const int TILE_WIDTH = 10;

    public int totalNormalTileCount;
    public int flippedNormalTileCount;

    public AStarTile[,] mapData;
    public Point doorTilePoint;

    private GameObject grid;
    private Tilemap tileMap;

    public bool isDoorOpen = false;

    public ResolutionManager resolutionManager;

    void Awake() {
        if (instance == null) {
            instance = this;
        } else if (instance != null) {
            Destroy(gameObject);
        }

        //DontDestroyOnLoad(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        mapData = new AStarTile[TILE_WIDTH, TILE_WIDTH];
        resolutionManager = GetComponent<ResolutionManager>();

        InitStage();
    }

    private void InitStage() {
        
        grid = GameObject.Find("Grid");
        grid.transform.localScale = new Vector3(resolutionManager.scaleValue, resolutionManager.scaleValue, 1);
        tileMap = GameObject.Find("Tilemap").GetComponent<Tilemap>();

        totalNormalTileCount = 0;
        flippedNormalTileCount = 0;

        for (int i = 0; i < TILE_WIDTH; i++) {
            for (int j = 0; j < TILE_WIDTH; j++) {
                MyTile tile = tileMap.GetTile<MyTile>(new Vector3Int(i - (TILE_WIDTH / 2), j - (TILE_WIDTH / 2), 0));
                if (tile is EndTile) {
                    mapData[i, j] = new AStarTile();
                    mapData[i, j].type = TILE_TYPE.END;
                    mapData[i, j].index.x = i - (TILE_WIDTH / 2);
                    mapData[i, j].index.y = j - (TILE_WIDTH / 2);

                    doorTilePoint.x = i - (TILE_WIDTH / 2);
                    doorTilePoint.y = j - (TILE_WIDTH / 2);
                } else if (tile) {
                    mapData[i, j] = new AStarTile();
                    mapData[i, j].type = TILE_TYPE.GROUND;
                    mapData[i, j].index.x = i - (TILE_WIDTH / 2);
                    mapData[i, j].index.y = j - (TILE_WIDTH / 2);

                    totalNormalTileCount++;
                } else {
                    mapData[i, j] = new AStarTile();
                    mapData[i, j].type = TILE_TYPE.WALL;
                    mapData[i, j].index.x = i - (TILE_WIDTH / 2);
                    mapData[i, j].index.y = j - (TILE_WIDTH / 2);
                }
            }
        }
    }

    public void OpenDoor() {
        tileMap.GetComponent<TileFlipper>().DeleteTile(new Vector3Int(doorTilePoint.x, doorTilePoint.y, -1));

        isDoorOpen = true;
    }

    public void FlipTile(Vector3Int coord) {
        tileMap.GetComponent<TileFlipper>().FlipTile(coord);

        if(mapData[coord.x + (TILE_WIDTH / 2), coord.y + (TILE_WIDTH / 2)].flipped) {
            mapData[coord.x + (TILE_WIDTH / 2), coord.y + (TILE_WIDTH / 2)].flipped = false;
            flippedNormalTileCount--;
            if(isDoorOpen) {
                tileMap.GetComponent<TileFlipper>().SetDoorTile(new Vector3Int(doorTilePoint.x, doorTilePoint.y, -1));
                isDoorOpen = false;
            }
        } else {
            mapData[coord.x + (TILE_WIDTH / 2), coord.y + (TILE_WIDTH / 2)].flipped = true;
            flippedNormalTileCount++;
        }

        if (flippedNormalTileCount >= totalNormalTileCount)
            OpenDoor();
    }

    public void NextLevel() {
        SceneManager.LoadScene("SampleScene");

        InitStage();
    }
}
