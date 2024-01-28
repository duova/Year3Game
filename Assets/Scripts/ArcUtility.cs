using UnityEngine;

public static class ArcUtility
{
    public static Vector3 ArcToTarget(Vector3 origin, Vector3 target)
    {
        var horizontalVector = target - origin;
        horizontalVector.y = 0;
        
        //This probably needs some calculus
        return new Vector3();
    }
    
    //Also need a arc calculator for the arc itself
}