using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterUIController : MonoBehaviour
{
    [SerializeField]
    private RectTransform healthBar;

    private int maxHealthBarWidth;

    public void Start() {
        maxHealthBarWidth = (int)healthBar.sizeDelta.x;
    }

    public void Update() {
        healthBar.sizeDelta = new Vector2(maxHealthBarWidth * GetHealthBarPercentage(), healthBar.sizeDelta.y);
    }

    private float GetHealthBarPercentage() {
        return (float)GlobalVariables.CURRENT_PLAYER_HEALTH / (float)GlobalVariables.MAX_PLAYER_HEALTH;
    }
}
