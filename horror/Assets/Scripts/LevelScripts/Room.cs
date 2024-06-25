using UnityEngine;

[CreateAssetMenu(fileName = "New Room", menuName = "Room/Create New Room")]

public class Room : ScriptableObject
{
    public GameObject roomObject;
    public GameObject northConnector;
    public GameObject eastConnector;
    public GameObject southConnector;
    public GameObject westConnector;
}
