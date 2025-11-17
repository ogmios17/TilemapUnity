using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Threading.Tasks;

public class RoomGeneration : MonoBehaviour
{
    public GameData gameData;
    private List<ScriptableLevel> levels;
    private int NRooms;
    private int currentLevel = 0;
    private List<Room> rooms;
    private Vector3Int position;
    private Tilemap ground;
    private Tilemap obstacles;     //tilemaps to instantiate
    private GameObject grid;
    private bool isFirst = true;
    public AIMovement aiMovement;
    public Grid myGridManager;
    public GameObject player;
    private Room previousRoom;
    private List<Vector3Int> appendedRoomPositions; //verrà usato per simulare una sorta di DFS
    private List<Room> appendedRooms;
    private bool getAppended = false;
    private Vector3Int roomCenter;
    private Vector3Int fillIndex;
    private Vector3Int lastPos;      //variabile che serve per salvare l'ultima posizione delle celle visitate da allPositionsWithin


    public void Start()
    {
        appendedRoomPositions = new List<Vector3Int>();
        appendedRooms = new List<Room>();
        grid = GameObject.FindWithTag("Grid");
        gameData.SetCurrentLevel(currentLevel);
        levels = gameData.levels;
        NRooms = levels[currentLevel].numberOfRooms;
        rooms = new List<Room>(levels[currentLevel].roomPool);
        GenerateRooms();
    }


    public void GenerateRooms()
    {
        for(int i =0; i<NRooms; i++)
        {
            Vector3Int bottomLeft;
            Room randomRoom;
            int index = Random.Range(0, rooms.Count);
            randomRoom = ScriptableObject.Instantiate(rooms[index]);
            randomRoom.ground.CompressBounds();
            randomRoom.obstacles.CompressBounds();
            if (!isFirst)
            {
                if (getAppended)   // se delle stanze stanno aspettando di essere riempite scegli la prima (FIFO)
                {
                    Debug.Log("choosing appended at index " + i);
                    position = appendedRoomPositions[0];
                    previousRoom = appendedRooms[0];
                    appendedRoomPositions.RemoveAt(0);
                    appendedRooms.RemoveAt(0);
                    roomCenter = position;
                }

                int doorIndex = Random.Range(0, previousRoom.doorPlacements.Count);   //scegli un indice randomico dell'array di cardinali
                Room.cardinals selectedDoorLocation = previousRoom.doorPlacements[doorIndex];     //recupera il cardinale scelto
                
                switch (selectedDoorLocation)
                {
                    case Room.cardinals.n:
                        if (!randomRoom.doorPlacements.Contains(Room.cardinals.s) )
                        {
                            i--;
                            continue;
                        }
                        position = new Vector3Int(position.x, position.y + (previousRoom.ground.cellBounds.size.y/2) + 1, 0);
                        
                        ground.SetTile(position, randomRoom.groundTile);
                        obstacles.SetTile(position, randomRoom.doorTile);
                        position.y++;
                        
                        if (ground.HasTile(position))
                        {
                            i--;
                            continue;
                        }
                        fillIndex = position - new Vector3Int(randomRoom.ground.size.x / 2, 0, 0);
                        bottomLeft = fillIndex;

                        DrawRoom(randomRoom, bottomLeft);
                        
                        Debug.Log("Ho riempito a nord a index "+i);
                        randomRoom.doorPlacements.Remove(Room.cardinals.s);
                        previousRoom.doorPlacements.Remove(Room.cardinals.n);  //faccio in modo di eliminare la porta disponibile
                        appendedRoomPositions.Add(new Vector3Int(position.x, position.y + (randomRoom.ground.size.y / 2), 0));  //mi riposiziono e salvo la stanza nella lista di appended
                        appendedRooms.Add(randomRoom);
                        if (previousRoom.doorPlacements.Count > 0 && Random.Range(0, 2) == 1)            // se ci sono ancora stanze libere scelgo casualmente se continuare o meno  
                        {
                            getAppended = false;
                            Debug.Log("scelgo di non spostarmi a index "+i);
                            position = roomCenter;  
                        }
                        else
                        {
                            getAppended = true;
                        }
                        break;
                    case Room.cardinals.e:
                        if (!randomRoom.doorPlacements.Contains(Room.cardinals.w))
                        {
                            i--;
                            continue;
                        }
                        position = new Vector3Int(position.x + (previousRoom.ground.cellBounds.size.x / 2) + 1, position.y , 0);

                        ground.SetTile(position, randomRoom.groundTile);
                        obstacles.SetTile(position, randomRoom.doorTile);
                        position.x++;                       
                        if (ground.HasTile(position))
                        {
                            i--;
                            continue;
                        }
                        fillIndex = position - new Vector3Int(0, randomRoom.ground.size.y / 2, 0);
                        bottomLeft = fillIndex;
                        DrawRoom(randomRoom, bottomLeft);

                        
                        randomRoom.doorPlacements.Remove(Room.cardinals.w);
                                     
                        previousRoom.doorPlacements.Remove(Room.cardinals.e);  //faccio in modo di eliminare la porta disponibile
                        appendedRoomPositions.Add(new Vector3Int(position.x + (randomRoom.ground.size.x / 2), position.y, 0));  //mi riposiziono e salvo la stanza nella lista di appended
                        appendedRooms.Add(randomRoom);
                        Debug.Log("Disegno a est a index " + i);
                        if (previousRoom.doorPlacements.Count > 0 && Random.Range(0, 2) == 1)            // se ci sono ancora stanze libere scelgo casualmente se continuare o meno  
                        {
                            getAppended = false;
                            Debug.Log("scelgo di non spostarmi a index " + i);
                            position = roomCenter;
                        }
                        else
                        {
                            getAppended = true;
                        }
                        break;
                    case Room.cardinals.s:
                        if (!randomRoom.doorPlacements.Contains(Room.cardinals.n))
                        {
                            i--;
                            continue;
                        }
                        position = new Vector3Int(position.x, position.y - (previousRoom.ground.cellBounds.size.y / 2) - 1, 0);
                        ground.SetTile(position, randomRoom.groundTile);
                        obstacles.SetTile(position, randomRoom.doorTile);
                        position.y--;                       
                        if (ground.HasTile(position))
                        {
                            i--;
                            continue;
                        }
                        fillIndex = position - new Vector3Int(randomRoom.ground.size.x / 2, randomRoom.ground.size.y - 1, 0);
                        bottomLeft = fillIndex;
                        DrawRoom(randomRoom, bottomLeft);
                        Debug.Log("Ho riempito a sud");
                        randomRoom.doorPlacements.Remove(Room.cardinals.n);
                        previousRoom.doorPlacements.Remove(Room.cardinals.s);  //faccio in modo di eliminare la porta disponibile
                        appendedRoomPositions.Add(new Vector3Int(position.x, position.y - (randomRoom.ground.size.y / 2), 0));  //mi riposiziono e salvo la stanza nella lista di appended
                        appendedRooms.Add(randomRoom);
                        if (previousRoom.doorPlacements.Count > 0 && Random.Range(0, 2) == 1)            // se ci sono ancora stanze libere scelgo casualmente se continuare o meno  
                        {
                            getAppended = false;
                            Debug.Log("scelgo di non spostarmi a index " + i);
                            position = roomCenter;
                        }
                        else
                        {
                            getAppended = true;
                        }
                        break;
                    case Room.cardinals.w:
                        if (!randomRoom.doorPlacements.Contains(Room.cardinals.e))
                        {
                            i--;
                            continue;
                        }
                        position = new Vector3Int(position.x - (previousRoom.ground.cellBounds.size.x / 2) - 1, position.y, 0);

                        ground.SetTile(position, randomRoom.groundTile);
                        obstacles.SetTile(position, randomRoom.doorTile);
                        position.x--;
                        if (ground.HasTile(position))
                        {
                            i--;
                            continue;
                        }
                        fillIndex = position - new Vector3Int(randomRoom.ground.size.x - 1, randomRoom.ground.size.y / 2, 0);
                        bottomLeft = fillIndex;
                        DrawRoom(randomRoom, bottomLeft);

                        randomRoom.doorPlacements.Remove(Room.cardinals.e);
                        previousRoom.doorPlacements.Remove(Room.cardinals.w);  //faccio in modo di eliminare la porta disponibile
                        appendedRoomPositions.Add(new Vector3Int(position.x - (randomRoom.ground.size.x) / 2, position.y, 0));  //mi riposiziono e salvo la stanza nella lista di appended
                        appendedRooms.Add(randomRoom);
                        Debug.Log("Disegno a west a index " + i);
                        if (previousRoom.doorPlacements.Count > 0 && Random.Range(0, 2) == 1)            // se ci sono ancora stanze libere scelgo casualmente se continuare o meno  
                        {
                            getAppended = false;
                            Debug.Log("scelgo di non spostarmi a index " + i);
                            position = roomCenter;
                        }
                        else
                        {
                            getAppended = true;
                        }
                        break;
                }
                ground.CompressBounds();
                
            }
            else
            {
                position = Vector3Int.zero;
                isFirst = false;
                ground = Instantiate(randomRoom.ground, position, Quaternion.identity);
                obstacles = Instantiate(randomRoom.obstacles, position, Quaternion.identity);
                ground.CompressBounds();
                obstacles.CompressBounds();
                ground.transform.parent = grid.transform;
                obstacles.transform.parent = grid.transform;
                position = new Vector3Int((ground.cellBounds.xMax + ground.cellBounds.xMin)/2 -1, (ground.cellBounds.yMax + ground.cellBounds.yMin)/2 -1, 0);  // setto la posizione al centro della stanza
                roomCenter = position;
                aiMovement.tilemap = ground;
                ground.transform.localScale = new Vector3(1,1,0);
                obstacles.transform.localScale = new Vector3(1, 1, 0);
                myGridManager.tilemap = ground;
                myGridManager.obstacles = obstacles;
                myGridManager.tilemap.CompressBounds();
                myGridManager.obstacles.CompressBounds();
                player.transform.position = ground.GetCellCenterWorld(position);
                previousRoom = randomRoom;
            }
            
            //rooms.RemoveAt(index);

        }
        myGridManager.tilemap.CompressBounds();
        myGridManager.obstacles.CompressBounds();
        myGridManager.CreateGrid();
    }

    public void Update()
    {
        player.transform.position = ground.GetCellCenterWorld(position);
    }

    public void DrawRoom(Room randomRoom, Vector3Int bottomLeft)
    {
        foreach (var pos in randomRoom.ground.cellBounds.allPositionsWithin)
        {
            if (fillIndex == bottomLeft) lastPos = pos;

            if (lastPos.y < pos.y)
            {
                fillIndex.x = bottomLeft.x;
                fillIndex.y++;
            }

            if (randomRoom.ground.HasTile(pos))
            {
                ground.SetTile(fillIndex, randomRoom.groundTile);
            }
            if (randomRoom.obstacles.HasTile(pos))
            {
                obstacles.SetTile(fillIndex, randomRoom.obstacles.GetTile(pos));
            }
            fillIndex.x++;
            lastPos = pos;
        }
    }
}
