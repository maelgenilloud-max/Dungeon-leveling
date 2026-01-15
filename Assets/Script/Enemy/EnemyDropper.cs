using UnityEngine;

[System.Serializable]
public class DropEntry
{
    public GameObject pickupPrefab;
    [Range(0f, 1f)] public float chance = 0.25f;
    public int minCount = 1;
    public int maxCount = 1;
}

public class EnemyDropper : MonoBehaviour
{
    public DropEntry[] drops;

    public void Drop()
    {
        if (drops == null) return;

        foreach (var d in drops)
        {
            if (d.pickupPrefab == null) continue;
            if (Random.value > d.chance) continue;

            int count = Random.Range(d.minCount, d.maxCount + 1);
            for (int i = 0; i < count; i++)
            {
                Vector3 pos = transform.position + (Vector3)Random.insideUnitCircle * 0.2f;
                Instantiate(d.pickupPrefab, pos, Quaternion.identity);
            }
        }
    }
}
