using System.Collections.Generic;
using UnityEngine;


//use this script to find a path
//creates a dictionary or the vector2Int that defines the position of the cube, and the cube itself
//prevents overlapping cubes to be taken into account
//script can only find distance to one exit
public class PathFinder : MonoBehaviour
{

    [SerializeField] WayPoint endWaypoint;

    PlayerWayPoint user;
    WayPoint startWaypoint;
    Dictionary<Vector3Int, WayPoint> grid = new Dictionary<Vector3Int, WayPoint>();
    PriorityQueue<WayPoint> priorityQueue = new PriorityQueue<WayPoint>();
    bool isRunning = true;
    WayPoint searchCenter;
    public List<WayPoint> path = new List<WayPoint>();
    int exploredCount ;
    PathMode pathMode;

    Vector3Int[] directions = {  new Vector3Int(0, 0, 1), new Vector3Int(1, 0, 0),
                                new Vector3Int(0, 0, -1), new Vector3Int(-1, 0, 0),

                                new Vector3Int(0, 1, 1), new Vector3Int(1, 1, 0),
                                new Vector3Int(0, 1, -1), new Vector3Int(-1, 1, 0),

                                new Vector3Int(0, -1, 1), new Vector3Int(1, -1, 0),
                                new Vector3Int(0, -1, -1), new Vector3Int(-1, -1, 0),

                             };

    public string open = "open";
    public string blocked = "blocked";
    public string stairs = "stairs";
    public string door = "door";
    public string fire = "fire";


    static Dictionary<string, WayPointType> terrainLUT = new Dictionary<string, WayPointType>();

    //private void Awake()
    //{
    //    SetupLUT(); //setup system for finding game objects with tags and assigning lut value

    //    var obstacles = FindObjectsOfType<Obstacle>();
    //    var waypoints = FindObjectsOfType<WayPoint>();


    //    foreach (Obstacle obs in obstacles)
    //    {
    //        Collider collider = obs.GetComponent<Collider>();
    //        foreach (WayPoint waypoint in waypoints)
    //        {
    //            if (collider.bounds.Contains(waypoint.gameObject.transform.position))
    //            {
    //                if (terrainLUT.ContainsKey(obs.tag))
    //                {
    //                    waypoint.wayPointType = terrainLUT[obs.tag];

    //                    print("type " + waypoint.wayPointType);
    //                }
    //            }
    //            else
    //            {
    //                waypoint.wayPointType = terrainLUT[open];
    //            }
    //        }
    //    }
    //}

    public List<WayPoint> GetPath(Vector3Int startPos)
    //Reset and initial path finding
    {
        //resetting lists and dictionaries
        foreach (var entry in grid)
        {
            Vector3Int key = entry.Key;
            WayPoint value = entry.Value;
            value.isExplored = false;
        }
        path.Clear();
        grid.Clear();
        priorityQueue.Clear();
        exploredCount = 0;
        ResetNeigborDistances();

        pathMode = FindObjectOfType<PathMode>();
        //Mode mode = pathMode.ReturnMode();

        isRunning = true;

        //initiating pathfinding
        LoadBlocks(startPos);
        PathFind();
        CreatePath();
        return path;
    }

    private void LoadBlocks(Vector3Int startPos)
    //Store the cubes into grids. Avoid unavailable and overlapping cubes
    {
        Vector3Int playerLocation = startPos;
        var waypoints = FindObjectsOfType<WayPoint>();
        foreach (WayPoint waypoint in waypoints)
        {
            waypoint.gameObject.tag = "NotPath";
            waypoint.SetTopColor(Color.white);

            var gridPos = waypoint.GetGridPos() * waypoint.GetGridSize();

            if (grid.ContainsKey(gridPos))
            {
                Debug.LogWarning("Overlapping");
            }
            else
            {
                grid.Add(gridPos, waypoint);

                if (gridPos == playerLocation)
                {
                    startWaypoint = waypoint;
                    startWaypoint.distanceTraveled = 0;
                }

            }
        }
        if (!grid.ContainsKey(playerLocation))
        {
            isRunning = false;
        }
    }

    private void PathFind()
    //Put the start node into queue, and explore neighbors if the process is still running
    {

        priorityQueue.Enqueue(startWaypoint);

        while (priorityQueue.Count > 0 && isRunning)
        {
            searchCenter = priorityQueue.Dequeue();
            ReachedEndNode(); //breaks while loop
            ExploreNeighbors();
            searchCenter.isExplored = true;
        }
    }

    private void ReachedEndNode()
    //BFS: if the search center reachs the end node then stop running
    {
        if (searchCenter == endWaypoint)
        {
            isRunning = false;

        }
    }

    private void ExploreNeighbors()
    //Get neighbours of the center node and see which algorithm should go
    {
        if (!isRunning) { return; }

        foreach (Vector3Int direction in directions)
        {
            Vector3Int NeighborCoordinates = searchCenter.GetGridPos() * searchCenter.GetGridSize() + direction * searchCenter.GetGridSize();

            if (grid.ContainsKey(NeighborCoordinates))
            {
                if (pathMode.ReturnMode() == Mode.Astar)
                {
                    QueueNeighborsAstar(NeighborCoordinates);
                }
                if (pathMode.ReturnMode() == Mode.Dijkstra)
                {
                    QueueNeighborsDijkstra(NeighborCoordinates);
                }
                if (pathMode.ReturnMode() == Mode.GreedyBFS)
                {
                    QueueNeighborsGreedy(NeighborCoordinates);
                }
                if (pathMode.ReturnMode() == Mode.BFS)
                {
                    QueueNeighborsBFS(NeighborCoordinates);
                }

            }
            else
            {
                //do nothing;
            }

        }
    }


    private void QueueNeighborsAstar(Vector3Int NeighborCoordinates)
    //Astar algorithm implementation
    {
        WayPoint neighbor = grid[NeighborCoordinates];


        if (neighbor.isExplored || priorityQueue.Contains(neighbor))
        {
            //do nothing
        }
        else
        {
            float distanceToNeighbor = GetNodeDistance(searchCenter, neighbor);
            float newDistanceTraveled = distanceToNeighbor + searchCenter.distanceTraveled + (int)searchCenter.wayPointType;//plays role of fscore

            if (float.IsPositiveInfinity(neighbor.distanceTraveled) || newDistanceTraveled < neighbor.distanceTraveled)
            {
                neighbor.exploredFrom = searchCenter;
                neighbor.distanceTraveled = newDistanceTraveled;
            }


            int hscore = (int) GetNodeDistance(neighbor, endWaypoint);
            neighbor.priority = (int) neighbor.distanceTraveled + hscore;
            priorityQueue.Enqueue(neighbor);

        }
    }

    private void QueueNeighborsGreedy(Vector3Int NeighborCoordinates)
    //Greedy algorithm implementation
    {
        WayPoint neighbor = grid[NeighborCoordinates];


        if (neighbor.isExplored || priorityQueue.Contains(neighbor))
        {
            //do nothing
        }
        else
        {
            float distanceToNeighbor = GetNodeDistance(searchCenter, neighbor);
            float newDistanceTraveled = distanceToNeighbor + searchCenter.distanceTraveled + (int)searchCenter.wayPointType;

            neighbor.distanceTraveled = newDistanceTraveled;

            neighbor.exploredFrom = searchCenter;




            neighbor.priority = (int)GetNodeDistance(neighbor, endWaypoint);


            priorityQueue.Enqueue(neighbor);
        }
    }

    private void QueueNeighborsDijkstra(Vector3Int NeighborCoordinates)
    //Dijkstra algorithm implementation
    {
        WayPoint neighbor = grid[NeighborCoordinates];


        if (neighbor.isExplored || priorityQueue.Contains(neighbor))
        {
            //do nothing
        }
        else
        {
            float distanceToNeighbor = GetNodeDistance(searchCenter, neighbor);
            float newDistanceTraveled = distanceToNeighbor + searchCenter.distanceTraveled + (int)searchCenter.wayPointType;

            if (float.IsPositiveInfinity(neighbor.distanceTraveled) || newDistanceTraveled < neighbor.distanceTraveled)
            {
                neighbor.exploredFrom = searchCenter;
                neighbor.distanceTraveled = newDistanceTraveled;
            }


            neighbor.priority = (int)neighbor.distanceTraveled;
            priorityQueue.Enqueue(neighbor);

        }
    }


    private void QueueNeighborsBFS(Vector3Int NeighborCoordinates)
    //BFS algorithms implementation
    {
        WayPoint neighbor = grid[NeighborCoordinates];
        
        if (neighbor.isExplored || priorityQueue.Contains(neighbor))
        {
            //do nothing
        }
        else
        {
            float distanceToNeighbor = GetNodeDistance(searchCenter, neighbor);
            float newDistanceTraveled = distanceToNeighbor + searchCenter.distanceTraveled + (int)searchCenter.wayPointType;
            neighbor.distanceTraveled = newDistanceTraveled;
            
            neighbor.exploredFrom = searchCenter;
            exploredCount++;
            neighbor.priority = exploredCount;


            priorityQueue.Enqueue(neighbor);

        }
    }

    private void CreatePath()
    //Trace the path and set the color
    {

        path.Add(endWaypoint);
        WayPoint previous = endWaypoint.exploredFrom;

        int loopBreaker = 0;
        while (previous != null && previous != startWaypoint && loopBreaker <= 999 && startWaypoint != endWaypoint)
        {
            path.Add(previous);
            previous = previous.exploredFrom;
            loopBreaker += 1;

        }

        path.Add(startWaypoint);
        path.Reverse();
        if (path.Count >= 1)
        {
            foreach (WayPoint point in path)
            {
                point.SetTopColor(Color.blue);  //set color
                point.gameObject.tag = "path";  //set tag
                endWaypoint.SetTopColor(Color.yellow);

            }
            if (startWaypoint == endWaypoint)
            {
                path.Clear();
                startWaypoint.SetTopColor(Color.green);
            }


        }
        print("total nodes in path " + path.Count);

    }

    public float GetNodeDistance(WayPoint source, WayPoint target)
    //Calculate distance between two nodes
    {
        float dx = Mathf.Abs(source.transform.position.x - target.transform.position.x);//horizontal
        float dy = Mathf.Abs(source.transform.position.y - target.transform.position.y);//vertical
        float dz = Mathf.Abs(source.transform.position.z - target.transform.position.z);//horizontal



        float minXZ;
        float maxXZ;

        float minYZ;
        float maxYZ;

        float minYX;
        float maxYX;




        minXZ = Mathf.Min(dx, dz);
        maxXZ = Mathf.Max(dx, dz);

        minYZ = Mathf.Min(dy, dz);
        maxYZ = Mathf.Max(dy, dz);

        minYX = Mathf.Min(dy, dx);
        maxYX = Mathf.Max(dy, dx);



        float diagonalStepsXZ = minXZ;
        float straightStepsXZ = maxXZ - minXZ;

        float diagonalStepsYX = minYX;
        float straightStepsYX = maxYX - minYX;

        float diagonalStepsYZ = minYZ;
        float straightStepsYZ = maxYZ - minYZ;

        float total = 1.4f * diagonalStepsXZ + straightStepsXZ
                      + 1.4f * diagonalStepsYX + straightStepsYX
                      + 1.4f * diagonalStepsYZ + straightStepsYZ;
       

        return (total);
    }

    public float GetManhattanDistance(WayPoint source, WayPoint target)
    //Calculate Manhattan distance of two nodes
    {
        float dx = Mathf.Abs(source.transform.position.x - target.transform.position.x);//horizontal
        float dy = Mathf.Abs(source.transform.position.y - target.transform.position.y);//vertical
        float dz = Mathf.Abs(source.transform.position.z - target.transform.position.z);//horizontal
        return (dx + dy + dz);
    }

    void SetupLUT()
    //Set a look up table for tags
    {
        terrainLUT.Add(open, WayPointType.Open);
        terrainLUT.Add(blocked, WayPointType.Blocked);
        terrainLUT.Add(stairs, WayPointType.Stairs);
        terrainLUT.Add(door, WayPointType.door);
        terrainLUT.Add(fire, WayPointType.fire);

    }

    private void ResetNeigborDistances()
    //Reset distances between neighbors
    {
        var waypoints = FindObjectsOfType<WayPoint>();
        foreach (WayPoint waypoint in waypoints)
        {
            waypoint.distanceTraveled = Mathf.Infinity;
            waypoint.priority = 0;
        }
    }


}


