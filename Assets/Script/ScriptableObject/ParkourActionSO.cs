using UnityEngine;

[CreateAssetMenu(fileName = "ParkourActionSO", menuName = "ParkourSystem/Create Parkour Action", order = 0)]

public class ParkourActionSO : ScriptableObject
{
    [SerializeField] public string animName;
    [SerializeField] private Vector2 height;
    [SerializeField] private Vector2 distance;

    public bool CanParkour(float height, float distance)
    {
        if (height >= this.height.x && height <= this.height.y
           && distance >= this.distance.x && distance <= this.distance.y)
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