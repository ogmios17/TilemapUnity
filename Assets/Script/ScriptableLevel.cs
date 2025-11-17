using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ScriptableLevel", menuName = "Scriptable Objects/ScriptableLevel")]
public class ScriptableLevel : ScriptableObject
{
    public int index;
    public int numberOfRooms;
    public Room[] roomPool;
}
