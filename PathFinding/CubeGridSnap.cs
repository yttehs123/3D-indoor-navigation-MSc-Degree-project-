using UnityEngine;

[ExecuteInEditMode]
[SelectionBase]
[RequireComponent(typeof(WayPoint))]

//use this script to setup grid
//this script calls the waypoint script and allows us to snap cubes to grid in the editor
//this script also updates the albel of the cube
public class CubeGridSnap : MonoBehaviour
{


    //Vector3 gridPos;
    WayPoint wayPoint;

    private void Awake()
    {
        wayPoint = GetComponent<WayPoint>();
    }

    void Update()
    {      
        SnapToGrid();
        //UpdateLabel();

    }

    private void SnapToGrid()
    //force cubes to place on the grid
    {
        int gridSize = wayPoint.GetGridSize();
        
        transform.position = new Vector3(
            wayPoint.GetGridPos().x * gridSize,
            wayPoint.GetGridPos().y * gridSize,
            wayPoint.GetGridPos().z * gridSize);
    }

    private void UpdateLabel()
    //update label if needed
    {

        if (transform.childCount < 0)
        {
            //do nothing
        }
        else
        {
            TextMesh textMesh = GetComponentInChildren<TextMesh>();
            string blockLabel = wayPoint.GetGridPos().x + "," + wayPoint.GetGridPos().y + "," + wayPoint.GetGridPos().z; //positions by gridsize to get coordinates based on local grids rather than world positions
            textMesh.text = blockLabel;
            gameObject.name = blockLabel;
        }

      
    }


  

 
}
