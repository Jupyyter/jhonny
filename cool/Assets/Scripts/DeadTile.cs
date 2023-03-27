using System.Collections;
using System.Collections.Generic;
//using Unity.Burst.CompilerServices;
using UnityEngine;
using Mirror;
using UnityEngine.Tilemaps;
public class DeadTile : NetworkBehaviour
{// ⋆｡˚ᎶᎾᎾⅅ ℕᏐᎶℍᎢ⋆｡˚✩
    private Tilemap Map;
    [SerializeField] private Tilemap backWall2;
    private MapManager mapManager;
    private Dictionary<Vector3Int, int> tilePile;

    private void Awake()
    {
        tilePile = new Dictionary<Vector3Int, int>();
        Map = GetComponent<Tilemap>();
        mapManager = FindObjectOfType<MapManager>();
    }
    private void OnCollisionEnter2D(Collision2D col)//called when collides
    {
        if (col.gameObject.CompareTag("Bullet"))//if a bullet strikes
        {
            Vector3 hitPosition = Vector3.zero;
            ContactPoint2D[] contacts = new ContactPoint2D[3];
            int cumContacts = col.GetContacts(contacts);
            for (int i = 0; i < 1; i++)
            {
                hitPosition.x = contacts[i].point.x + 0.05f * contacts[i].normal.x;
                hitPosition.y = contacts[i].point.y + 0.05f * contacts[i].normal.y;
                Vector3Int truLocation = Map.WorldToCell(hitPosition);
                if (Map.GetTile(truLocation) != null && mapManager.getTileHP(truLocation) != -1)
                {
                    if (!tilePile.ContainsKey(truLocation))
                    {
                        tilePile.Add(truLocation, mapManager.getTileHP(truLocation));
                    }
                    CmdharasTile(truLocation);
                }
            }
        }
    }
    //[Command(requiresAuthority = false)]
    private void CmdharasTile(Vector3Int location)//called to damage and destroy tile
    {
        tilePile[location]--;
        if (tilePile[location] == 0)//if tile at location has hp==0
        {
            Vector3Int upTile = new Vector3Int(location.x, location.y + 1, location.z);
            if (Map.GetTile(upTile) != null)
            {
                StartCoroutine(destroyUpTile(upTile));
            }
            mapManager.CmdSpawnParticles(location);
            eraseTile(location);
            tilePile.Remove(location);
        }
    }
    public void giveTiles(Vector3 vec)//called when a bullet hits a tile
    {
        Vector3Int truLocation = Map.WorldToCell(vec);
        if (Map.GetTile(truLocation) != null && mapManager.getTileHP(truLocation) != -1)
        {
            if (!tilePile.ContainsKey(truLocation))
            {
                tilePile.Add(truLocation, mapManager.getTileHP(truLocation));
            }
            CmdharasTile(truLocation);
        }
    }
    private IEnumerator destroyUpTile(Vector3Int upTile)//called when a tile dies by a bullet
    {
        yield return new WaitForSeconds(0.10f);
        if (Map.GetTile(upTile) != null)
        {
            mapManager.CmdSpawnParticles(upTile);
            eraseTile(upTile);
        }
    }
    //:):):):):):):):):):):):):):):):):):):):):):):):):):):):):):):):):):):):):):):):):):):):):):):):):):):):):):):):):):):):):):):):):):):):):):):)
    //:):):):):):):):):):):):):):):):):):):):):):):):):):):):):):):):):):):):):):):):):):):):):):):):):):):):):):):):):):):):):):):):):):):):):):):)
    //:):):):):):):):):):):):):):):):):):):):):):):):):):):):):):):):):):):):):):):):):):):):):):):):):):):):):):):):):):):):):):):):):):):):):):):)
    //prevent "RPC function called on client" error by doing haker stuff i wish death toa ll humanity
    //https://answers.unity.com/questions/1140791/rpc-function-called-on-client.html
    //or
    //the client is calling the RpcSpawnParticles method, which then calls the CmdSpawnParticles method on the server.
    private void eraseTile(Vector3Int eraseTile)
    {
        if (isServer)
        {
            RpcEraseTile(eraseTile);
        }
        else
        {
            CmdEraseTile(eraseTile);
        }
    }
    [Command(requiresAuthority = false)]
    private void CmdEraseTile(Vector3Int eraseTile)
    {
        RpcEraseTile(eraseTile);
    }
    [ClientRpc]
    private void RpcEraseTile(Vector3Int eraseTile)
    {
        Map.SetTile(eraseTile, null);
    }
}
/*
⠄⠄⠄⠄⠄⠄⠄⠄⣀⣤⡴⠶⠟⠛⠛⠛⠛⠻⠶⢦⣤⣀⠄⠄⠄⠄⠄⠄⠄⠄
⠄⠄⠄⠄⠄⣠⣴⡟⠋⠁⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠈⠙⢻⣦⣄⠄⠄⠄⠄⠄
⠄⠄⠄⣠⡾⠋⠈⣿⣶⣄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⣠⣶⣿⠁⠙⢷⣄⠄⠄⠄
⠄⠄⣴⠏⠄⠄⠄⠸⣇⠉⠻⣦⣀⠄⠄⠄⠄⣀⣴⠟⠉⣸⠇⠄⠄⠄⠹⣦⠄⠄
⠄⣼⠏⠄⠄⠄⠄⠄⢻⡆⠄⠄⠙⠷⣦⣴⠾⠋⠄⠄⢰⡟⠄⠄⠄⠄⠄⠹⣧⠄
⢰⡏⠄⠄⠄⠄⠄⠄⠈⣷⠄⢀⣤⡾⠋⠙⢷⣤⡀⠄⣾⠁⠄⠄⠄⠄⠄⠄⢹⡆
⣿⠁⠄⠄⠄⠄⠄⠄⠄⣸⣷⠛⠁⠄⠄⠄⠄⠈⠛⣾⣇⠄⠄⠄⠄⠄⠄⠄⠄⣿
⣿⠄⠄⠄⠄⠄⣠⣴⠟⠉⢻⡄⠄ AYAYA ⠄⣾⡟⠻⣦⣄⠄⠄⠄⠄⠄⣿
⣿⡀⠄⢀⣴⠞⠋⠄⠄⠄⠈⣷⠄⠄⠄⠄⠄⠄⣾⠁⠄⠄⠄⠙⠳⣦⡀⠄⠄⣿
⠸⣧⠾⠿⠷⠶⠶⠶⠶⠶⠶⢾⣷⠶⠶⠶⠶⣾⡷⠶⠶⠶⠶⠶⠶⠾⠿⠷⣼⠇
⠄⢻⣆⠄⠄⠄⠄⠄⠄⠄⠄⠄⢿⡄⠄⠄⢠⡿⠄⠄⠄⠄⠄⠄⠄⠄⠄⣰⡟⠄
⠄⠄⠻⣆⠄⠄⠄⠄⠄⠄⠄⠄⠘⣷⠄⠄⣾⠃⠄⠄⠄⠄⠄⠄⠄⠄⣰⠟⠄⠄
⠄⠄⠄⠙⢷⣄⠄⠄⠄⠄⠄⠄⠄⢹⣇⣸⡏⠄⠄⠄⠄⠄⠄⠄⣠⡾⠋⠄⠄⠄
⠄⠄⠄⠄⠄⠙⠳⣦⣄⡀⠄⠄⠄⠄⢿⡿⠄⠄⠄⠄⢀⣠⣴⠞⠋⠄⠄⠄⠄⠄
⠄⠄⠄⠄⠄⠄⠄⠄⠉⠛⠳⠶⣦⣤⣼⣧⣤⣴⠶⠞⠛⠉⠄⠄⠄⠄⠄⠄⠄⠄
*/