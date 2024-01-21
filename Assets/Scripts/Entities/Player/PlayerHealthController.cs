using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealthController : MonoBehaviour
{

    public void SubstractHealth(int substractingHealth) {
        GlobalVariables.CURRENT_PLAYER_HEALTH -= substractingHealth;
        GlobalVariables.CURRENT_PLAYER_HEALTH = (GlobalVariables.CURRENT_PLAYER_HEALTH < 0) ? 0 : GlobalVariables.CURRENT_PLAYER_HEALTH;
    }

    public void setCurrentHealth(int health) {
        GlobalVariables.CURRENT_PLAYER_HEALTH = health;
    }
}
