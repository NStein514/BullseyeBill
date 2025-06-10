using UnityEngine;

public enum ZoneType { Single, Double, Triple, Bullseye }

public class DartboardZone : MonoBehaviour
{
    public int baseScore = 1;
    public ZoneType zoneType = ZoneType.Single;

    public int GetScore()
    {
        switch(zoneType)
        {
            case ZoneType.Double:
                return baseScore * 2;
            case ZoneType.Triple:
                return baseScore * 3;
            case ZoneType.Bullseye:
                return 50; // bullseye fixed score
            default:
                return baseScore;
        }
    }
}
