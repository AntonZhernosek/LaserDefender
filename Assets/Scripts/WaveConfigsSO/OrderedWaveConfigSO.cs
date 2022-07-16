using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "Ordered Wave Configuration", fileName = "New Wave Config")]
public class OrderedWaveConfigSO : WaveConfigSO
{
    [SerializeField] List<GameObject> enemiesList;
    [SerializeField] Transform pathPrefab;

    public override List<Transform> GetWaypoints()
    {
        List<Transform> waypoints = new List<Transform>();

        foreach (Transform child in pathPrefab)
        {
            waypoints.Add(child);
        }

        return waypoints;
    }

    public override IEnumerable<GameObject> GetEnemyList()
    {
        return enemiesList;
    }
}
