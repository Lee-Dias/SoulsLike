using System;
using System.Collections.Generic;
using UnityEngine;

public class ChanceToGiveItem : MonoBehaviour
{
    [System.Serializable]
    public struct ItemsToGiveAndChance
    {
        public Item itemToGive;
        public float Chance;
    }
    [System.Serializable]
    public struct SoulsToGiveAndChance
    {
        public float SoulsToGive;
        public float Chance;
    }

    [SerializeField] private ItemsToGiveAndChance itemToGive;
    [SerializeField] private SoulsToGiveAndChance[] chanceToGiveAndAmount;
    [SerializeField] private GameObject soulPrefab;
    private Inventory inventory;
    

    void Start()
    {
        inventory = FindFirstObjectByType<Inventory>();
        // Build a weighted pool: each entry is (label, weight, action)
        var pool = new List<(string label, float weight, Action action)>();

        // Add item entry
        if (itemToGive.Chance > 0)
            pool.Add(("item", itemToGive.Chance, () => GiveItem(itemToGive.itemToGive)));

        // Add soul entries
        if (chanceToGiveAndAmount != null)
        {
            foreach (var souls in chanceToGiveAndAmount)
            {
                if (souls.Chance > 0)
                {
                    float amount = souls.SoulsToGive;
                    pool.Add(("souls", souls.Chance, () => GiveSouls(amount)));
                }
            }
        }

        // Calculate total weight and add a "nothing" slot for the remainder
        float totalWeight = 0f;
        foreach (var entry in pool) totalWeight += entry.weight;

        if (totalWeight < 100f)
            pool.Add(("nothing", 100f - totalWeight, null));

        // Roll
        float roll = UnityEngine.Random.Range(0f, Mathf.Max(totalWeight, 100f));
        float cumulative = 0f;

        foreach (var entry in pool)
        {
            cumulative += entry.weight;
            if (roll < cumulative)
            {
                if (entry.action != null)
                {
                    /*/Do*/
                    entry.action.Invoke();
                }
                break;
            }
        }
    }

    private void GiveItem(Item item)
    {
        inventory.SpawnInventoryItem(item);
        if (ItemPickedUp.Instance != null) {
            ItemPickedUp.Instance.ShowItem(item);
        }
    }

    private void GiveSouls(float amount)
    {
        FindFirstObjectByType<PlayerStats>().GiveCrystalShards((int)amount);
        Instantiate(soulPrefab, this.transform.position, Quaternion.identity);
    }
}