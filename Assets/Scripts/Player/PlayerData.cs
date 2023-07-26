using System;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerData : MonoBehaviour
{
    [field: SerializeField] public float BaseForwardSpeed { get; private set; } = 30f;
    [field: SerializeField] public float LateralSpeed { get; private set; } = 60f;
    [field: SerializeField] public float RotationSpeed { get; private set; } = 0.5f;
    [field: SerializeField] public float RotationMaxAngle { get; private set; } = 30f;
    [field: SerializeField] public float RollbackSpeed { get; private set; } = 10f;
    [field: SerializeField] public float LaneSnappingTolerance { get; private set; } = 0.1f;
    [field: SerializeField] public float SpeedIncreaseRate { get; private set; } = 5f;
    
    // An alternative to the syntax above.
    // public int Test { get => test; private set => test = value; }
    // [SerializeField] private int test = 0;

    public float currentForwardSpeed;

    private void Awake()
    {
        ResetSpeed();
    }

    public void ResetSpeed()
    {
        currentForwardSpeed = BaseForwardSpeed;
    }
}
