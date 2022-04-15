using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModeSwitcher : MonoBehaviour
{
    public PathFinding PathFinding;

    public void SwitchToMoveMode(){
        PathFinding.Mode = "Move";
    }
    public void SwitchToBuildMode(){
        PathFinding.Mode = "Build";
    }
    public void SwitchDebugMode(){
        PathFinding.DebugMode = !PathFinding.DebugMode;
    }
    
    public void ToggleStartMode(){
        PathFinding.Mode = "PlaceStart";
        PathFinding.ResetGreens();
    }
}
