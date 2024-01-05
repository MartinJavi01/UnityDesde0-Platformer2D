using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityHealthController : MonoBehaviour
{
    [SerializeField]
    private int maxHealth;
    private int currentHealth;

    private void Start() {
        currentHealth = maxHealth;
    }

    public void SubstractHealth(int substractingHealth) {
        currentHealth -= substractingHealth;
        currentHealth = (currentHealth < 0) ? 0 : currentHealth;
    }

    public int getCurrentHealth() {
        return currentHealth;
    }

    public void setCurrentHealth(int health) {
        currentHealth = health;
    }
}
