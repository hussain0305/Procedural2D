using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Healthbar : MonoBehaviour
{
    public Image damageIndicator;
    public Image currentHealthImage;

    [HideInInspector] 
    public int currentHealth;

    [HideInInspector] 
    public int maxHealth;

    public void SetHealth()
    {
        
    }
}
