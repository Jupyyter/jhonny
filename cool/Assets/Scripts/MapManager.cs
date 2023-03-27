using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Mirror;

public class MapManager : NetworkBehaviour
{
    [SerializeField] private Tilemap map;
    [SerializeField] private List<FKTILES> tileDatas;
    private Dictionary<TileBase, FKTILES> dataFromTiles;//first is the key
    private void Start()
    {
        dataFromTiles = new Dictionary<TileBase, FKTILES>();//storing all the FKTILES in a dictionary
        foreach (FKTILES tileData in tileDatas)
        {
            try
            {
                foreach (Tile tile in tileData.tiles)
                {
                    dataFromTiles.Add(tile, tileData);
                }
            }
            catch
            {
                foreach (RuleTile tile in tileData.tiles)
                {
                    dataFromTiles.Add(tile, tileData);
                }
            }
        }
    }
    public int getTileHP(Vector3Int location)//returns the  hp of a tile at a location
    {
        if (dataFromTiles.ContainsKey(map.GetTile(location)))
        {
            TileBase tilee = map.GetTile(location);
            return dataFromTiles[tilee].hp;
        }
        else return -1;
    }
    [Command(requiresAuthority = false)]
    public void CmdSpawnParticles(Vector3Int location)//spawns particles
    {
        RpcSpawnParticles(location);
    }

    [ClientRpc]
    public void RpcSpawnParticles(Vector3Int location)
    {
        Debug.Log("dsadsadsa");
        if (map.GetTile(location) != null && dataFromTiles.ContainsKey(map.GetTile(location)))
        {
            Vector3 midTile = map.GetCellCenterWorld(location);
            TileBase tilee = map.GetTile(location);
            Instantiate(dataFromTiles[tilee].particles, midTile, new Quaternion());
        }
    }
}
