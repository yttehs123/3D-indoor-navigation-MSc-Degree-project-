using UnityEngine;


//declare waypoint under player as start waypoint
//update path on everytime obstruction is introduced

//this script tells euser to move next waypoint in list


public class MovementScript : MonoBehaviour
{

    public int reRouteAfter = 2;

    PathFinder PathFinder;

    Vector3Int playerPos;
    int T = 0;
    float timeStart;
    private void Start()
    //Find path first time
    {
        timeStart = Time.time;
        PatchRoute();
        print("time taken = " + (Time.time - timeStart).ToString());
    }

    private void PatchRoute()
    //Get player position and find path
    {      
            PathFinder = FindObjectOfType<PathFinder>();
            playerPos = new Vector3Int(Mathf.RoundToInt(gameObject.transform.position.x), Mathf.RoundToInt(gameObject.transform.position.y), Mathf.RoundToInt(gameObject.transform.position.z));
            PathFinder.GetPath(playerPos);
    }

    private void Update()
    //Check the plaer's movement and decide whether to rerouter every tick
    {
        ProcessTranslation();
        Rerouter();
    }

    private void Rerouter()
    //If player moved more than specific times then find path again
    {    
        if (gameObject.transform.hasChanged)
        {

                    if (T >= reRouteAfter)
                    {
                            timeStart = Time.time;
                            playerPos = new Vector3Int(Mathf.RoundToInt(gameObject.transform.position.x), Mathf.RoundToInt(gameObject.transform.position.y), Mathf.RoundToInt(gameObject.transform.position.z));
                            PathFinder.GetPath(playerPos);
                            gameObject.transform.hasChanged = false;
                            print("time taken = " + (Time.time - timeStart).ToString());
                T = 0;
                    }

                    else
                    {
                        T++;
                        
                        gameObject.transform.hasChanged = false;
                    }
        }
    }

    private void ProcessTranslation() 
    //Receive input keys and move the player. Force player back onto grid
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            transform.Translate(Vector3.forward * 10);
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            transform.Translate(Vector3.left * 10);
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            transform.Translate(Vector3.back * 10);
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            transform.Translate(Vector3.right * 10);
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            transform.Translate(Vector3.up * 10);
        }
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            transform.Translate(Vector3.down * 10);
        }
    }


}








