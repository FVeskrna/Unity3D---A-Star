using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PathFinding : MonoBehaviour
{
    public LayerMask Layer_Ground;
    public bool CanMoveDiagonally;
    public bool DebugMode;
    public string Mode;
    public Grid Grid;
    private List<Node> OpenList;
    private List<Node> ClosedList;

    private List<Node> TemporaryGreen = new List<Node>();
    private List<Node> FinalPath;
    private Node EndingNode;

    private int Cost_Diagonal = 14;
    private int Cost_Straight = 10;

    private int Start_X;
    private int Start_Y;
    private Node StartingNode;

    private void Start() {
        ResetPathFinding();
        Grid = GameObject.Find("Grid").GetComponent<Grid>();
    }

    private void Update() {
        if(Input.GetMouseButtonDown(0)){
            Vector3 ClickPosition = GetClickCords();
            int Click_X = Mathf.RoundToInt(ClickPosition.x);
            int Click_Y = Mathf.RoundToInt(ClickPosition.z);

            if(ClickPosition != Vector3.zero){  
                switch(Mode){
                    case "Move":
                        List<Node> path = FindPath(Start_X,Start_Y, Click_X,Click_Y);
                        if(DebugMode && path != null){
                            for(int i=0; i<path.Count -1;i++){
                                Debug.DrawLine(path[i].position, path[i+1].position,Color.magenta, 5f, false);
                            }

                            for(int i = 1; i <path.Count;i++){
                                path[i].MakeTileGreen();
                                TemporaryGreen.Add(path[i]);
                            }

                        }
                        //Move character

                        break;
                    case "Build":
                        Grid.BuildWall(Click_X,Click_Y);

                        if(Grid.GridArray[Click_X,Click_Y].isWall)
                        Debug.Log("Node " + Click_X + "," + Click_Y + " is now not walkable");

                        break;
                    case "PlaceStart":
                        if(StartingNode != null)
                        StartingNode.ResetColor();
                        
                        Grid.GridArray[Click_X,Click_Y].MakeTileBlue();
                        Start_X = Click_X;
                        Start_Y = Click_Y;
                        StartingNode = Grid.GridArray[Click_X,Click_Y];
                        Mode = "Move";
                        break;
                }
                
            }
        }   
    }
    
    private List<Node> FindPath(int X_Start, int Y_Start, int X_End, int Y_End){
        ResetPathFinding();
        ResetNodesCosts();

        Node StartingNode = Grid.GridArray[X_Start,Y_Start];
        EndingNode = Grid.GridArray[X_End,Y_End];
        OpenList.Add(StartingNode);

        StartingNode.gCost = 0;
        StartingNode.hCost = CalculateDistance(StartingNode,EndingNode);
        StartingNode.Calculate_F_cost();

        //Main Loop of the algo
        while(OpenList.Count > 0){
            Debug.Log("Loop");
            Node CurrentNode = GetLowestFCostNode(OpenList);

            if(CurrentNode == EndingNode){
                return CalculatePath(EndingNode);
            }

            OpenList.Remove(CurrentNode);
            ClosedList.Add(CurrentNode);

            foreach(Node Node in GetNeigbours(CurrentNode)){
                if(ClosedList.Contains(Node)) continue;

                if(Node.isWall){
                    ClosedList.Add(Node);
                    continue;
                }
                
                float TheoreticalGCost = CurrentNode.gCost + CalculateDistance(CurrentNode,Node);

                if(TheoreticalGCost < Node.gCost){
                    Node.PreviousNode = CurrentNode;
                    Node.gCost = TheoreticalGCost;
                    Node.hCost = CalculateDistance(Node,EndingNode);
                    Node.Calculate_F_cost();

                    if(!OpenList.Contains(Node)){
                        OpenList.Add(Node);
                    }
                }
            }
        }


        return null;

    }

    
    private void ResetPathFinding() {
        FinalPath = new List<Node>();
        OpenList = new List<Node>();
        ClosedList = new List<Node>();

        ResetGreens();  
    }

    public void ResetGreens(){
        if(TemporaryGreen.Count > 0){
            foreach(Node Node in TemporaryGreen){
                Node.ResetColor();
            }
        }
    }

    private void ResetNodesCosts(){
        for(int x=0;x<Grid.GridSizeX;x++){
            for(int y=0;y<Grid.GridSizeY;y++){
                Grid.GridArray[x,y].gCost = float.MaxValue;
                Grid.GridArray[x,y].Calculate_F_cost();
                Grid.GridArray[x,y].PreviousNode = null;
            }
        }
    }

    private float CalculateDistance(Node StartNode, Node EndNode){
        float X_Distance = Mathf.Abs(StartNode.Xposition - EndNode.Xposition);
        float Y_Distance = Mathf.Abs(StartNode.Yposition - EndNode.Yposition);
        float TotalDistance = Mathf.Abs(X_Distance - Y_Distance);
        return Mathf.Min(X_Distance, Y_Distance) * Cost_Diagonal + TotalDistance * Cost_Straight;
    }

    private Node GetLowestFCostNode(List<Node> NodeList){
        Node CurrentLowestNode = NodeList[0];
        foreach(Node Node in NodeList){
            if(Node.fCost < CurrentLowestNode.fCost) 
            CurrentLowestNode = Node;
        }
        return CurrentLowestNode;
    }

    private List<Node> CalculatePath(Node EndingNode){
        List<Node> path = new List<Node>();
        path.Add(EndingNode);
        Node currentNode = EndingNode;
        
        while(currentNode.PreviousNode != null){
            path.Add(currentNode.PreviousNode);
            currentNode = currentNode.PreviousNode;
        }

        path.Reverse();
        return path;
    }

    private List<Node> GetNeigbours(Node Node){
        List<Node> Nodes = new List<Node>();
        if(Node.x-1 >= 0){
            Nodes.Add(Grid.GridArray[Node.x-1,Node.y]);

            if(Node.y-1 >= 0 && CanMoveDiagonally) Nodes.Add(Grid.GridArray[Node.x-1,Node.y-1]);
            if(Node.y+1 < Grid.GridSizeY && CanMoveDiagonally) Nodes.Add(Grid.GridArray[Node.x-1,Node.y+1]);
        }
        if(Node.x+1 < Grid.GridSizeX){
            Nodes.Add(Grid.GridArray[Node.x+1,Node.y]);

            if(Node.y-1 >= 0 && CanMoveDiagonally) Nodes.Add(Grid.GridArray[Node.x+1,Node.y-1]);
            if(Node.y+1 < Grid.GridSizeY && CanMoveDiagonally) Nodes.Add(Grid.GridArray[Node.x+1,Node.y+1]);
        }
        if(Node.y-1 >= 0) Nodes.Add(Grid.GridArray[Node.x,Node.y-1]);
        if(Node.y+1 < Grid.GridSizeY) Nodes.Add(Grid.GridArray[Node.x,Node.y+1]);
        
        return Nodes;
    }

    private Vector3 GetClickCords(){
        Ray myRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitInfo;
        Vector3 ClickPosition;

        if(Physics.Raycast(myRay, out hitInfo, 10000f,Layer_Ground))
        return ClickPosition = hitInfo.point; 
        else 
        return Vector3.zero;
    }

    public void ToggleDebugMode(){
        DebugMode = !DebugMode;
    }

    public void ToggleDiagMode(){
        CanMoveDiagonally = !CanMoveDiagonally;
    }
}
