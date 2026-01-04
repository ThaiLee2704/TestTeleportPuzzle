using UnityEngine;

public class Teleport : MonoBehaviour
{
    public Teleport targetTeleport;
    public Transform exitPoint;

    void OnValidate()
    {
        // N?u ch?a gán exitPoint, dùng chính transform c?a targetTeleport và c?nh báo
        if (targetTeleport != null && exitPoint == null)
        {
            Debug.LogWarning($"Teleport '{name}': exitPoint ch?a ???c gán. T?m dùng transform c?a '{targetTeleport.name}'. Nên t?o child ExitPoint và gán ?úng v? trí.");
            exitPoint = targetTeleport.transform;
        }
    }
}
