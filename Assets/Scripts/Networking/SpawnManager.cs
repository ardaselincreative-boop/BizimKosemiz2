using System.Collections.Generic;
using UnityEngine;

namespace BizimKosemiz.Networking
{
    /// <summary>
    /// Sahnedeki tüm SpawnPoint bileşenlerini yönetir ve oyunculara sırayla (Round-Robin)
    /// zemin üstünde doğrulanmış spawn konumu sağlar.
    /// </summary>
    public class SpawnManager : MonoBehaviour
    {
        private readonly List<SpawnPoint> spawnPoints = new List<SpawnPoint>();
        private int nextSpawnIndex;

        private void Awake()
        {
            RefreshSpawnPoints();
        }

        public void RefreshSpawnPoints()
        {
            spawnPoints.Clear();
            SpawnPoint[] found = FindObjectsOfType<SpawnPoint>();
            spawnPoints.AddRange(found);
            spawnPoints.Sort((a, b) => a.SpawnIndex.CompareTo(b.SpawnIndex));
        }

        public void GetNextSpawnPose(out Vector3 position, out Quaternion rotation)
        {
            if (spawnPoints.Count == 0)
            {
                RefreshSpawnPoints();
            }

            if (spawnPoints.Count > 0)
            {
                SpawnPoint chosen = spawnPoints[nextSpawnIndex % spawnPoints.Count];
                nextSpawnIndex++;
                position = chosen.GetGroundedSpawnPosition();
                rotation = chosen.transform.rotation;
                return;
            }

            position = new Vector3(0f, 0.02f, -4f);
            rotation = Quaternion.identity;
        }
    }
}
