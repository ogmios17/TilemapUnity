using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "Room", menuName = "Scriptable Objects/Room")]
public class Room : ScriptableObject
{
    public enum cardinals{n,e,s,w }
    public Tilemap ground;
    public Tilemap obstacles;
    public GameTile doorTile;
    public GameTile groundTile;
    public GameTile destroyedDoorTile;
    public List<cardinals> doorPlacements;


    public void Awake()
    {
        
    }

}
