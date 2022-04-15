using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    //general 
    public int x,y;
    public float Xposition;
    public float Yposition;
    public Vector3 position;
    public GameObject _SpawnedObject;
    private GameObject _ParentObject;
    public bool isWall;
    public Material DefaultGround;
    public Material GreenGround;
    public Material BlueGround;

    //A*
    public float gCost,hCost,fCost;
    public Node PreviousNode;


    public Node(float X,float Y,GameObject ParentObject){
        x = (int)X;
        y = (int)Y;
        Xposition = X;
        Yposition = Y;
        position = new Vector3(Xposition,0,Yposition);
        _ParentObject = ParentObject;
        isWall = false;
    }

    public void RemoveSpawnedGameObject(){
        if(_SpawnedObject != null) 
        GameObject.Destroy(_SpawnedObject);

        //_SpawnedObject.transform.SetParent(_ParentObject.transform);
    }

    public void Calculate_F_cost(){
        fCost = gCost + hCost;
    }

    public void MakeTileGreen(){
        if(!isWall){
            _SpawnedObject.GetComponent<MeshRenderer>().material = GreenGround;
            Debug.Log("Tile is green now");
        } 
    }
    public void MakeTileBlue(){
        if(!isWall){
            _SpawnedObject.GetComponent<MeshRenderer>().material = BlueGround;
        } 
    }

    public void ResetColor(){
        if(!isWall)
        _SpawnedObject.GetComponent<MeshRenderer>().material = DefaultGround;
    }
}
