using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class EndTile : MyTile
{
    public override void RefreshTile(Vector3Int position, ITilemap tilemap) {
        base.RefreshTile(position, tilemap);
        // 타일을 배치할 때 자기나 주변이 어떻게 업데이트 되어야 하는지
    }

    public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData) {
        base.GetTileData(position, tilemap, ref tileData);
    }

    public override bool GetTileAnimationData(Vector3Int position, ITilemap tilemap, ref TileAnimationData tileAnimationData) {
        tileAnimationData.animatedSprites = animatedSprite;
        tileAnimationData.animationSpeed = 1.0f;
        tileAnimationData.animationStartTime = 0f;

        return false;
    }

#if UNITY_EDITOR
    [MenuItem("Assets/Create/EndTile")]
    public static void CreateEndTile() {
        string path = EditorUtility.SaveFilePanelInProject("Save End Tile", "New End Tile", "Asset", "Save End Tile", "Assets");
        if (path == "")
            return;
        AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<EndTile>(), path);
    }
#endif
}
