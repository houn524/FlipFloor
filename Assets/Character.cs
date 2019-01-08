using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Character : MonoBehaviour
{
    public Grid grid;
    public Tilemap tileMap;

    public PathFinder pathFinder;

    public float moveSpeed;

    public Vector3Int currentCoord;
    private Vector2 targetPos;

    private List<AStarTile> path;

    private bool isMoving = false;
    private bool isStepMoving = false;
    private bool isHold = false;

    private AStarTile currentTargetTile;

    // Start is called before the first frame update
    void Start()
    {
        currentCoord = grid.WorldToCell(transform.position);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) {
            
            Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int coord = grid.WorldToCell(pos);
            MyTile tile = tileMap.GetTile<MyTile>(new Vector3Int(coord.x, coord.y, 0));
            currentCoord = grid.WorldToCell(transform.position);
            if (tile) {
                Debug.Log("Find");
                Debug.Log("currentCoord : " + currentCoord.x + ", " + currentCoord.y);
                Debug.Log("targetCoord : " + coord.x + ", " + coord.y);

                if(isMoving) {
                    path = pathFinder.FindPath(grid, tileMap, new Point(currentTargetTile.index.x, currentTargetTile.index.y), new Point(coord.x, coord.y));
                } else {
                    path = pathFinder.FindPath(grid, tileMap, new Point(currentCoord.x, currentCoord.y), new Point(coord.x, coord.y));
                }
                
                Debug.Log(path.Count);

                StartCoroutine(WaitMove(path));
            }
        }
    }

    IEnumerator WaitMove(List<AStarTile> movePath) {
        if (isMoving)
            isHold = true;
        else
            isHold = false;

        while(isMoving) {
            yield return null;
        }

        StartCoroutine(Move(movePath));
    }

    IEnumerator Move(List<AStarTile> movePath) {
        isMoving = true;

        int indexCount = 0;
        float timer = 0f;
        currentTargetTile = movePath[indexCount];

        while(true) {
            Vector3 targetPosition = grid.CellToWorld(new Vector3Int(currentTargetTile.index.x, currentTargetTile.index.y, 0));
            targetPosition.y += (grid.cellSize.y / 2);

            if (targetPosition.Equals(transform.position)) {
                isStepMoving = false;

                if(isHold) {
                    isMoving = false;
                    isHold = false;
                    break;
                }
                    
                if (indexCount >= movePath.Count - 1) {
                    isMoving = false;
                    break;
                }
                    
                currentTargetTile = movePath[++indexCount];
                timer = 0f;
            }

            timer += Time.deltaTime;
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            isStepMoving = true;
            yield return null;
        }
    }
}
