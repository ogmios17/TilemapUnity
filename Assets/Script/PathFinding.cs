using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
public class PathFinding : MonoBehaviour
{
    private Grid grid;
    public Tilemap ground;
    public Tilemap obstacles;

    public void Awake()
    {
        grid = GetComponent<Grid>();
    }

    public void Update()
    {

    }

    public List<Node> FindPath(Vector3 start, Vector3 target)
    {
        Debug.Log("start: "+start + " Target: "+ target);

        Node startNode = grid.getNodeFromWorldPos(start);
        Node targetNode = grid.getNodeFromWorldPos(target);

        List<Node> openList = new List<Node>();
        HashSet<Node> closedList = new HashSet<Node>();

        openList.Add(startNode);

        while (openList.Count > 0)
        {
            Node currentNode = openList[0];
            for(int i =0; i<openList.Count; i++)
            {
                //Debug.Log("node position: "+currentNode.position);
                if(openList[i].F<currentNode.F || ((openList[i].F == currentNode.F) && (openList[i].H < currentNode.H))){
                    currentNode = openList[i];
                }              
            }
            openList.Remove(currentNode);
            closedList.Add(currentNode);

            if(currentNode == targetNode)
            {
                return GetFinalPath(startNode, targetNode);                
            }
            foreach(Node node in grid.getNearNodes(currentNode))
            {
                if (!node.isWalkable || closedList.Contains(node)) continue;

                float moveCost = currentNode.G + 1;

                if(moveCost<node.G || !openList.Contains(node)){
                    node.G = moveCost;
                    node.H = GetManhattenDistance(node, targetNode);
                    node.parent = currentNode;

                    if (!openList.Contains(node))
                    {
                        openList.Add(node);
                    }
                }
            }
        } 
        return null;
    }

    List<Node> GetFinalPath(Node startNode, Node targetNode)
    {
        List<Node> finalPath = new List<Node>();

        Node currentNode = targetNode;

        while(currentNode != startNode)
        {
            finalPath.Add(currentNode);
            currentNode = currentNode.parent;
            Debug.Log(currentNode.position);
        }

        finalPath.Reverse();

        return finalPath;
    }

    float GetManhattenDistance(Node startNode, Node targetNode)
    {

        return (startNode.position.x - targetNode.position.x) - (startNode.position.y - targetNode.position.y);
        
    }
}
