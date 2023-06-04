using UnityEngine;
using UnityEngine.Serialization;

public class PlayerData : MonoBehaviour
{
    private static PlayerData instance;
    public static PlayerData Instance { get { return instance; } }

    [field: SerializeField] public float ForwardSpeed { get; private set; } = 30f;
    [field: SerializeField] public float LateralSpeed { get; private set; } = 60f;
    [field: SerializeField] public float RotationSpeed { get; private set; } = 0.5f;
    [field: SerializeField] public float RotationMaxAngle { get; private set; } = 30f;
    [field: SerializeField] public float RollbackSpeed { get; private set; } = 10f;
    [field: SerializeField] public float RotationTime { get; private set; }= 0.65f;
    [field: SerializeField] public float LaneSnappingTolerance { get; private set; } = 0.1f;
    
    // An alternative to the syntax above.
    // public int Test { get => test; private set => test = value; }
    // [SerializeField] private int test = 0;
    
    private void Awake()
    {
        if (instance != null && instance != this) { Destroy(gameObject); }
        else { instance = this; }
    }
}
