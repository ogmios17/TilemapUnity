using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "GameTile", menuName = "Scriptable Objects/GameTile")]
public class GameTile : Tile
{
    public bool isWalkable;
}
