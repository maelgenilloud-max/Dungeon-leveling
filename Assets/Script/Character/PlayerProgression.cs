using System;
using UnityEngine;

public class PlayerProgression : MonoBehaviour
{
    [Header("XP points available to spend")]
    [SerializeField] private int xpPoints = 0;
    public int XpPoints => xpPoints;

    [Header("Bonus stats (shown in your empty boxes)")]
    public int bonusLife = 0;
    public int bonusSpeed = 0;
    public int bonusAttack = 0;

    [Header("How bonuses affect gameplay")]
    public int healthPerLifePoint = 1;   // +1 maxHealth per Life point
    public float speedPerPoint = 0.5f;  // +0.5 speed per Speed point
    public int attackPerPoint = 1;      // +1 attack per Attack point (wire later)

    public event Action OnChanged;

    // Called when consuming an XP potion
    public void AddXpPoints(int amount)
    {
        xpPoints = Mathf.Max(0, xpPoints + amount);
        Debug.Log($"[XP] Total XP points now = {xpPoints}");
        OnChanged?.Invoke();
    }


    public bool TrySpendPointOnLife()
    {
        if (xpPoints <= 0) return false;
        xpPoints--;
        bonusLife++;
        OnChanged?.Invoke();
        return true;
    }

    public bool TrySpendPointOnSpeed()
    {
        if (xpPoints <= 0) return false;
        xpPoints--;
        bonusSpeed++;
        OnChanged?.Invoke();
        return true;
    }

    public bool TrySpendPointOnAttack()
    {
        if (xpPoints <= 0) return false;
        xpPoints--;
        bonusAttack++;
        OnChanged?.Invoke();
        return true;
    }

    //reset les points de compétences à la mort
    public void ResetAll()
    {
        xpPoints = 0;
        bonusLife = 0;
        bonusSpeed = 0;
        bonusAttack = 0;

        OnChanged?.Invoke();
    }


    // Used by PlayerController to apply effects
    public int GetExtraMaxHealth() => bonusLife * healthPerLifePoint;
    public float GetExtraSpeed() => bonusSpeed * speedPerPoint;
    public int GetExtraAttack() => bonusAttack * attackPerPoint;
}
