using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;
using Anonym.Isometric;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;

    public const int TILE_WIDTH = 10;

    [HideInInspector]
    public int totalNormalTileCount;

    [HideInInspector]
    public int flippedNormalTileCount;

    [HideInInspector]
    public int currentLevel = 1;

    [HideInInspector]
    public int totalLife = 10;

    [HideInInspector]
    public int currentLife = 10;

    [HideInInspector]
    public bool isClearDelay = false;

    public bool isDoorOpen = false;
    public bool isPause = false;

    public ResolutionManager resolutionManager;

    public GameObject character;
    public LOGrid loGrid;
    public LOTile doorTile;

    public UIManager uiManager;

    [HideInInspector]
    public Level savedLevel;

    private bool isFirstLoad = true;

    [HideInInspector]
    public bool isGameOver = false;

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
        resolutionManager = GameObject.Find("ResolutionManager").GetComponent<ResolutionManager>();

        if (PlayerManager.Instance.currentPlayer.isFirst) {
            uiManager.ShowHowToPlay();
            PlayerManager.Instance.currentPlayer.isFirst = false;
            IOManager.PlayerBinarySerialize(PlayerManager.Instance.currentPlayer, Application.persistentDataPath + "/savedData/player.player");
        }

        InitStage();
    }

    public void InitStage() {

        uiManager.fxSoundManager._isMute = true;

        totalNormalTileCount = 0;
        flippedNormalTileCount = 0;

        isDoorOpen = false;
        //currentLevel = 1;

        character.GetComponent<Character>().Reset();

        Level level = IOManager.LevelSavedBinaryDeserialize(Application.persistentDataPath + "/savedData/savedlevel.level");

        if(PlayerManager.Instance.currentPlayer.isFirst == false && isFirstLoad && level != null) {
            totalLife = level.life;
            currentLife = level.currentLife;
            currentLevel = level.number;
            isFirstLoad = false;
        } else {
            level = IOManager.LevelResourceBinaryDeserialize("levels/level" + currentLevel);
            level.currentLife = level.life;
            totalLife = level.life;
            currentLife = totalLife;
        }

        savedLevel = level;

        loGrid.characterSpawnIndex = level.characterSpawnPoint;

        loGrid.levelCodeData = level.levelCodeData;

        uiManager.UpdateUI();

        loGrid.GenerateTiles();

        uiManager.fxSoundManager._isMute = false;
    }

    public void DecreaseLife() {
        currentLife--;
        savedLevel.currentLife--;

        Debug.Log(savedLevel.currentLife);

        uiManager.UpdateUI();

        if (currentLife <= 0) {
            uiManager.GameOver();
            isGameOver = true;
        }
            
    }

    public void ResetLevel() {
        loGrid.PushAllTileToPool();

        InitStage();
    }

    public void OpenDoor() {
        loGrid.OpenDoor();

        isDoorOpen = true;
    }

    public void CloseDoor() {
        loGrid.CloseDoor();

        isDoorOpen = false;
    }

    public void FlipTile(Point index) {
        loGrid.levelData[index.y, index.x].Flip();

        if(loGrid.levelData[index.y, index.x].isFlipped) {
            flippedNormalTileCount++;

            if(flippedNormalTileCount >= totalNormalTileCount) {
                OpenDoor();
            }
        } else {
            flippedNormalTileCount--;

            if(isDoorOpen) {
                CloseDoor();
            }
        }

    }

    public void NextLevel() {
        //SceneManager.LoadScene("Level2");

        if(currentLevel < 20) {
            currentLevel++;
            savedLevel.number++;
        } else
            isGameOver = true;

        uiManager.Clear();

        
    }

    public void MainMenu() {
        SceneManager.LoadScene("MainMenu");
        //Destroy(gameObject);
    }

    void OnDestroy() {
        

        
        if (isGameOver) {
            IOManager.DeleteSavedFile(Application.persistentDataPath + "/savedData/savedlevel.level");
            IOManager.DeleteSavedFile(Application.persistentDataPath + "/savedData/player.player");
            PlayerManager.Instance.createPlayer();
        }
        else
            IOManager.LevelBinarySerialize(savedLevel, Application.persistentDataPath + "/savedData/savedlevel.level");
        
        Debug.Log("Quit!!!");
    }
}
