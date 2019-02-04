using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AnimDirection {
    LEFT_FRONT = 1,
    RIGHT_FRONT = 2,
    LEFT_BACK = 3,
    RIGHT_BACK = 4
}

public class Character : MonoBehaviour
{
    public PathFinder pathFinder;

    public float baseSpeed;

    [HideInInspector]
    public float moveSpeed;

    public Vector3Int currentCoord;
    private Vector3 currentCharacterCoord;
    private Vector2 targetPos;

    [HideInInspector]
    public Point currentCharacterIndex;

    private List<AStarTile> path;

    private bool isMoving = false;
    private bool isStepMoving = false;
    private bool isHold = false;

    public AStarTile currentTargetTile;
    private LOTile prevTile;

    private AnimDirection animDir = AnimDirection.LEFT_FRONT;

    // Start is called before the first frame update
    void Start()
    {
        currentCharacterCoord = transform.position;
    }

    public void Reset() {
        isMoving = false;
        isStepMoving = false;
        isHold = false;
        animDir = AnimDirection.LEFT_FRONT;
        GetComponent<Animator>().SetInteger("AnimDirection", 1);
    }

    public void MoveToTile(LOTile targetTile) {
        if (currentCharacterIndex.x == targetTile.index.x && currentCharacterIndex.y == targetTile.index.y && !isMoving)
            return;

        if((GameManager.instance.isDoorOpen) || !targetTile.type.Equals(LOTileType.END)) { 

            if (isMoving) {
                if (currentTargetTile.index.Equals(targetTile.index)) {
                    isHold = true;
                    return;
                }
                else
                    path = pathFinder.FindPath(new Point(currentTargetTile.index.x, currentTargetTile.index.y), new Point(targetTile.index.x, targetTile.index.y));
            } else {
                path = pathFinder.FindPath(currentCharacterIndex, new Point(targetTile.index.x, targetTile.index.y));
            }
            StartCoroutine(WaitMove(path));
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
        GetComponent<Animator>().SetBool("IsMoving", true);

        int indexCount = 0;
        float timer = 0f;
        currentTargetTile = movePath[indexCount];
        prevTile = GameManager.instance.loGrid.levelData[currentCharacterIndex.y, currentCharacterIndex.x];

        Vector3 targetPosition = GameManager.instance.loGrid.levelData[currentTargetTile.index.y, currentTargetTile.index.x].transform.position;

        if (targetPosition.x <= transform.position.x && targetPosition.y <= transform.position.y) {
            animDir = AnimDirection.LEFT_FRONT;
            Debug.Log("LEFT_FRONT");
        } else if (targetPosition.x >= transform.position.x && targetPosition.y <= transform.position.y) {
            animDir = AnimDirection.RIGHT_FRONT;
            Debug.Log("RIGHT_FRONT");
        } else if (targetPosition.x <= transform.position.x && targetPosition.y >= transform.position.y) {
            animDir = AnimDirection.LEFT_BACK;
            Debug.Log("LEFT_BACK");
        } else if (targetPosition.x >= transform.position.x && targetPosition.y >= transform.position.y) {
            animDir = AnimDirection.RIGHT_BACK;
            Debug.Log("RIGHT_BACK");
        }

        GetComponent<Animator>().SetInteger("AnimDirection", (int)animDir);

        while (true) {
            targetPosition = GameManager.instance.loGrid.levelData[currentTargetTile.index.y, currentTargetTile.index.x].transform.position;

            if (targetPosition.Equals(transform.position)) {
                currentCharacterIndex = currentTargetTile.index;
                GameManager.instance.savedLevel.characterSpawnPoint = currentCharacterIndex;

                LOTile currentTile = GameManager.instance.loGrid.levelData[currentTargetTile.index.y, currentTargetTile.index.x];

                

                if (currentTile.type.Equals(LOTileType.END)) {
                    GameManager.instance.NextLevel();
                    break;
                }

                GameManager.instance.FlipTile(currentTargetTile.index);
                GameManager.instance.DecreaseLife();

                if (indexCount >= movePath.Count - 1) {
                    isMoving = false;
                    break;
                }

                prevTile = currentTile;
                currentTargetTile = movePath[++indexCount];
                timer = 0f;

                targetPosition = GameManager.instance.loGrid.levelData[currentTargetTile.index.y, currentTargetTile.index.x].transform.position;

                if (targetPosition.x <= transform.position.x && targetPosition.y <= transform.position.y) {
                    animDir = AnimDirection.LEFT_FRONT;
                    Debug.Log("LEFT_FRONT");
                } else if (targetPosition.x >= transform.position.x && targetPosition.y <= transform.position.y) {
                    animDir = AnimDirection.RIGHT_FRONT;
                    Debug.Log("RIGHT_FRONT");
                } else if (targetPosition.x <= transform.position.x && targetPosition.y >= transform.position.y) {
                    animDir = AnimDirection.LEFT_BACK;
                    Debug.Log("LEFT_BACK");
                } else if (targetPosition.x >= transform.position.x && targetPosition.y >= transform.position.y) {
                    animDir = AnimDirection.RIGHT_BACK;
                    Debug.Log("RIGHT_BACK");
                }

                GetComponent<Animator>().SetInteger("AnimDirection", (int)animDir);

                isStepMoving = false;

                if(isHold) {
                    break;
                }
                    
            }

            timer += Time.deltaTime;
            if(GameManager.instance.isDoorOpen == false && currentTargetTile.index.Equals(GameManager.instance.loGrid.doorTile.index)) {
                break;
            } else if(GameManager.instance.isDoorOpen == true && currentTargetTile.index.Equals(GameManager.instance.loGrid.doorTile.index)) {
                GameManager.instance.loGrid.endTile.transform.Find("Mask").gameObject.SetActive(true);
            }

            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            isStepMoving = true;
            yield return null;
        }
        isMoving = false;

        if(isHold) {
            isHold = false;
        } else {
            GetComponent<Animator>().SetBool("IsMoving", false);
        }
        


        //GetComponent<Animator>().SetInteger("AnimDirection", (int)animDir);
    }

    
}
