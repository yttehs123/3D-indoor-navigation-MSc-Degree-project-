using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Mode
{
    BFS = 0,
    Dijkstra = 1,
    GreedyBFS = 2,
    Astar = 3
}
public class PathMode : MonoBehaviour
//Set default algorithm to BFS, which can be changed by selection
{
    public Mode mode = Mode.BFS;

    public Mode ReturnMode()
    {
        return mode;
    }

}
