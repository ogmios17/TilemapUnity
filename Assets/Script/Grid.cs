using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Tilemaps;

public class Grid : MonoBehaviour
{
    public GridLayout gridLayout;
    public Tilemap tilemap;
    public Tilemap obstacles;
    private Vector3Int coordinate;
    [HideInInspector] public Dictionary<Vector3, Node> nodes;


    [HideInInspector] public List<Node> finalPath;

    public PathFinding pathFinding;

    public void Awake()
    {

        Vector3Int cellPosition = gridLayout.WorldToCell(transform.position);

        nodes = new Dictionary<Vector3, Node> { };

    }
    public void Start()
    { 

    }

    public void CreateGrid()
    {

        for (int y = tilemap.cellBounds.yMin; y < tilemap.cellBounds.yMax; y++)
        {
            for (int x = tilemap.cellBounds.xMin; x < tilemap.cellBounds.xMax; x++)
            {
                coordinate = new Vector3Int(x, y, 0);
                Vector3 worldPoint = tilemap.GetCellCenterWorld(coordinate);
                if (tilemap.GetTile(coordinate) != null)
                {                    
                    nodes.Add(worldPoint, new Node(true, worldPoint));
                }
                else
                {
                    nodes.Add(worldPoint, new Node(false, worldPoint));
                }
                //Debug.Log("catch node just inserted:: " + nodes.GetValueOrDefault(worldPoint).isWalkable + "from position " + worldPoint);
            }
        }

        for (int y = obstacles.cellBounds.yMin; y < obstacles.cellBounds.yMax; y++)
        {
            for (int x = obstacles.cellBounds.xMin; x < obstacles.cellBounds.xMax; x++)
            {
                coordinate = new Vector3Int(x, y, 0);
                Vector3 worldPoint = obstacles.GetCellCenterWorld(coordinate);
                Node node = getNodeFromWorldPos(worldPoint);
                GameTile foundTile = (GameTile)obstacles.GetTile(coordinate);
                Debug.Log(foundTile);
                if (node!= null && foundTile != null && foundTile.isWalkable == false)
                {
                    node.isWalkable = false;
                }
            }
        }
    }


    public Node getNodeFromWorldPos(Vector3 position)
    {
        //Debug.Log("catch node: "+nodes.GetValueOrDefault(position)+ "from position "+position);
        return nodes.GetValueOrDefault(position) ;    //restituisce il nodo data una WorldPosition
    }

    public List<Node> getNearNodes(Node node)
    {
        List<Node> list = new List<Node>();
        Vector3Int cellPosition = node.isWalkable ? tilemap.WorldToCell(node.position): obstacles.WorldToCell(node.position);
        Vector3 returningPosition;
        int xCheck;
        int yCheck;

        xCheck = cellPosition.x + 1;
        yCheck = cellPosition.y;

        cellPosition =new Vector3Int(xCheck, yCheck, 0) ;
        returningPosition = node.isWalkable ? tilemap.GetCellCenterWorld(cellPosition) : obstacles.GetCellCenterWorld(cellPosition);
        
        if(tilemap.HasTile(cellPosition) || obstacles.HasTile(cellPosition))
        {
            returningPosition = node.isWalkable ? tilemap.GetCellCenterWorld(cellPosition) : obstacles.GetCellCenterWorld(cellPosition);
            list.Add(getNodeFromWorldPos(returningPosition));

        }



        xCheck = cellPosition.x - 2;
        yCheck = cellPosition.y;

        cellPosition = new Vector3Int(xCheck, yCheck, 0);
        returningPosition = node.isWalkable ? tilemap.GetCellCenterWorld(cellPosition) : obstacles.GetCellCenterWorld(cellPosition);

        if (tilemap.HasTile(cellPosition) || obstacles.HasTile(cellPosition))
        {
            returningPosition = node.isWalkable ? tilemap.GetCellCenterWorld(cellPosition) : obstacles.GetCellCenterWorld(cellPosition);
            list.Add(getNodeFromWorldPos(returningPosition));

        }

        xCheck = cellPosition.x+1;
        yCheck = cellPosition.y+1;

        cellPosition = new Vector3Int(xCheck, yCheck, 0);
        returningPosition = node.isWalkable ? tilemap.GetCellCenterWorld(cellPosition) : obstacles.GetCellCenterWorld(cellPosition);

        if (tilemap.HasTile(cellPosition) || obstacles.HasTile(cellPosition))
        {
            returningPosition = node.isWalkable ? tilemap.GetCellCenterWorld(cellPosition) : obstacles.GetCellCenterWorld(cellPosition);
            list.Add(getNodeFromWorldPos(returningPosition));

        }

        xCheck = cellPosition.x;
        yCheck = cellPosition.y-2;

        cellPosition = new Vector3Int(xCheck, yCheck, 0);
        returningPosition = node.isWalkable ? tilemap.GetCellCenterWorld(cellPosition) : obstacles.GetCellCenterWorld(cellPosition);

        if (tilemap.HasTile(cellPosition) || obstacles.HasTile(cellPosition))
        {
            returningPosition = node.isWalkable ? tilemap.GetCellCenterWorld(cellPosition) : obstacles.GetCellCenterWorld(cellPosition);
            list.Add(getNodeFromWorldPos(returningPosition));

        }

        return list;
    }
}
