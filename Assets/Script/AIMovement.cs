using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class AIMovement : MonoBehaviour
{
    public Tilemap tilemap;
    public PathFinding astar;
    private Vector3 target;
    private List<Node> path;
    public Actions actions;
    public float speed;
    private Vector2 nextStep;
    private bool recalibrating = true;
    private int pathIndex;


    private void Awake()
    {
        actions = new Actions();
    }
    void Start()
    {
        path = new List<Node>();
        nextStep = transform.position;
        target = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Vector3 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int cellClicked = tilemap.WorldToCell(worldPoint);
            target = tilemap.GetCellCenterWorld(cellClicked);
            recalibrating = true;
        }

        Vector2 newPos = Vector2.MoveTowards(transform.position, nextStep, speed * Time.deltaTime);
        transform.position=newPos;
        if(transform.position.x == nextStep.x && transform.position.y == nextStep.y)
        {
            if (!recalibrating && pathIndex<path.Count)
            {
                nextStep = path[pathIndex].position;
                pathIndex++;
            }
            else if(transform.position!=target)
            {
                pathIndex = 0;
                path = astar.FindPath(transform.position, target);
                recalibrating = false;
            }
        }
    }

}
