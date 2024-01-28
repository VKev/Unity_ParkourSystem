using UnityEngine;

[CreateAssetMenu(fileName = "ParkourActionSO", menuName = "ParkourSystem/Create Parkour Action", order = 0)]

public class ParkourActionSO : ScriptableObject
{
    [SerializeField] public string animName;
    [SerializeField] private float minHeight;
    [SerializeField] private float maxHeight;

    public bool CanParkour(float height)
    {
        if (height >= minHeight && height <= maxHeight)
            return true;
        return false;
    }

    public bool IsName(string name)
    {
        if (animName == name)
            return true;
        return false;
    }

}