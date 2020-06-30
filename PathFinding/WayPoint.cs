using UnityEngine;
using System;

//use this script to get grid size and gridCoordinates
//this script sets aa grid size
//this script creates a vetor2Int with two axis integers that cause the cubeeditor to snap cubes to grid
//this can also be used to define locaitons of the cube


public enum WayPointType //use tags to trigger type
{
    Open = 0,
    Blocked = 10,
    Stairs = 2,
    door = 3,
    fire = 4
}

public class WayPoint : MonoBehaviour, IComparable<WayPoint>
{
    public WayPointType wayPointType = WayPointType.Open;

    [SerializeField] Color exploredColor;

    //public ok here as is data class
    public bool isExplored = false;
    public WayPoint exploredFrom;

    public float distanceTraveled = Mathf.Infinity;

    public int priority;


    Vector2Int gridPos;

    const int gridSize = 10;

    public int GetGridSize()
    //Get sides of each cube
    {
        return gridSize;
    }   

    public Vector3Int GetGridPos()
    //Get position of a grid
    {
        return new Vector3Int(
          Mathf.RoundToInt(transform.position.x / gridSize) ,
          Mathf.RoundToInt(transform.position.y / gridSize),
          Mathf.RoundToInt(transform.position.z / gridSize)

        );
    }

    public void SetTopColor(Color color)
    //Change top color of a grid
    {
        if (gameObject.name == "BFS Capsule")
        {

        }
        else
        {
            MeshRenderer topMeshRenderer = transform.Find("up").GetComponent<MeshRenderer>();
            topMeshRenderer.material.color = color;
        }
    }

    public int CompareTo(WayPoint other)
    //Compare priority with another cube
    {
        if (this.priority < other.priority)
        {
            return -1;
        }
        else if (this.priority > other.priority)
        {
            return 1;
        }
        else
        {
            return 0;
        }
    }
}

