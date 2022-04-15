using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{   
    public GameObject BackgroundPlane;
    private GameObject Map;
    public GameObject GroundTile;
    public GameObject Wall;
    public Node[,] GridArray;
    [HideInInspector] public int GridSizeX = 20;
    [HideInInspector] public int GridSizeY = 20;
    public Material DefaultGround;
    public Material GreenGround;
    public Material BlueGround;


    private void Start() {
        SpawnGrid(GridSizeX,GridSizeY);
    }
    public void SpawnGrid(int SizeX, int SizeY){
        Map = new GameObject("Map");
        GridArray = new Node[SizeX,SizeY];

        //Background plane
        GameObject Background = Instantiate(BackgroundPlane);
        Background.GetComponent<MeshRenderer>().material.color = Color.black;
        Background.transform.SetParent(Map.transform);
        Background.transform.localScale = new Vector3(GridSizeX/10,1,GridSizeY/10);
        Background.transform.position = new Vector3(GridSizeX/2,-0.005f,GridSizeY/2) + new Vector3 (-0.5f,0,-0.5f);

        //GridArray initialization
        for(int x=0;x<SizeX;x++){
            for(int y=0;y<SizeY;y++){
                GridArray[x,y] = new Node(x,y,Map);
                GameObject Object = Instantiate(GroundTile, GridArray[x,y].position,Quaternion.identity);
                Object.transform.SetParent(Map.transform);
                GridArray[x,y]._SpawnedObject = Object;
                GridArray[x,y].DefaultGround = DefaultGround;
                GridArray[x,y].GreenGround = GreenGround;
                GridArray[x,y].BlueGround = BlueGround;
            }
        }
    }

    //Toggle between wall and empty ground
    public void BuildWall(int x, int y){
        if(GridArray[x,y].isWall){
            GridArray[x,y].RemoveSpawnedGameObject();
            GameObject _Object = Instantiate(GroundTile, GridArray[x,y].position,Quaternion.identity);
            _Object.transform.SetParent(Map.transform);
            GridArray[x,y]._SpawnedObject = _Object;
            GridArray[x,y].isWall = false;

            
        }
        else{
            GridArray[x,y].RemoveSpawnedGameObject();
            GameObject Object = Instantiate(Wall, GridArray[x,y].position,Quaternion.identity);
            Object.transform.SetParent(Map.transform);
            GridArray[x,y]._SpawnedObject = Object;
            Debug.Log("Wall build");
            GridArray[x,y].isWall = true;
        }        
            
    }
}


