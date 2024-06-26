using UnityEngine;

[CreateAssetMenu(fileName = "New Room", menuName = "Room/Create New Room")]

public class Room : ScriptableObject
{

    public float rotation = 0f;

    public GameObject roomObject;
    public GameObject northConnector;
    public GameObject eastConnector;
    public GameObject southConnector;
    public GameObject westConnector;

    public bool northConnectorUsed = false;
    public bool eastConnectorUsed = false;
    public bool southConnectorUsed = false;
    public bool westConnectorUsed = false;

    public bool hasNorthConnector() {

        if (northConnector != null) {

            return true;
        }

        return false;
    }

    public bool hasEastConnector() {

        if (eastConnector != null) {

            return true;
        }

        return false;
    }

    public bool hasSouthConnector() {

        if (southConnector != null) {

            return true;
        }

        return false;
    }

    public bool hasWestConnector() {

        if (westConnector != null) {

            return true;
        }

        return false;
    }

    public void setNorthConnectorStatus(bool status) {

        northConnectorUsed = status;
    }

    public void setEastConnectorStatus(bool status) {

        eastConnectorUsed = status;
    }

    public void setSouthConnectorStatus(bool status) {

        southConnectorUsed = status;
    }
    
    public void setWestConnectorStatus(bool status) {

        westConnectorUsed = status;
    }
}
