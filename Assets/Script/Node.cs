using UnityEngine;
using UnityEngine.Tilemaps;

public class Node : Tile
{
    public Node parent;

    public Vector2 position; 
    public float G;
    public float H;
    public float F { get { return G + H; } }

    public bool isWalkable;

    public Node(bool isWalkable, Vector2 position)
    {
        this.isWalkable = isWalkable;
        this.position = position;

    }
}
