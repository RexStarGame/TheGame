using System.Collections.Generic;
using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    public List<Checkpoint> checkpoints = new List<Checkpoint>();

    void Awake()
    {
        // Find alle checkpoints i scenen og tilf�j dem til listen
        checkpoints.AddRange(FindObjectsOfType<Checkpoint>());
    }

    public void ResetCheckpoints()
    {
        foreach (Checkpoint cp in checkpoints)
        {
            cp.ResetCheckpoint();
        }
    }
}
