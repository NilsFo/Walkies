using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapSoundCreator : MonoBehaviour
{
    public Tilemap tilemap;
    public TileBase[] tiles;
    public SoundTrigger soundTrigger;
    public GameState gameState;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = tilemap.cellBounds.xMin; i <= tilemap.cellBounds.xMax; i++)
        {
            for (int j = tilemap.cellBounds.yMin; j <= tilemap.cellBounds.yMax; j++)
            {
                TileBase t = tilemap.GetTile<TileBase>(new Vector3Int(i, j, 0));

                if (t != null)
                {
                    if (tiles.Contains(t))
                    {
                        var pos = tilemap.GetCellCenterWorld(new Vector3Int(i, j, 0));
                        // Debug.Log("Tile:" + t + " at " + pos, t);
                        var sound = Instantiate(soundTrigger, pos, Quaternion.identity, transform);
                        sound.gameState = gameState;
                    }
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
    }
}