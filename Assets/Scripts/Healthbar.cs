using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Healthbar : MonoBehaviour
{
    public PlayerAttributes playerAttributes;
    
    public Image damageIndicator;
    public Image currentHealthBar;

    private int currentHealthOnDisplay = 0;
    private float currentHealthFill = 0;
    private float currentDamageIndicatorFill = 0;

    private Coroutine damageIndicatorCoroutine;
    
    public void OnEnable()
    {
        if (playerAttributes != null)
        {
            playerAttributes.OnHealthChanged += UpdateHealthbar;
        }
    }

    public void OnDisable()
    {
        if (playerAttributes != null)
        {
            playerAttributes.OnHealthChanged -= UpdateHealthbar;
        }
    }

    public void UpdateHealthbar(int currentHealth, int maxHealth)
    {
        if (damageIndicatorCoroutine != null)
        {
            StopCoroutine(damageIndicatorCoroutine);
        }
        currentHealthFill = (float)currentHealth / maxHealth;
        currentHealthBar.fillAmount = currentHealthFill;
        currentHealthOnDisplay = currentHealth;

        if (currentHealth > currentHealthOnDisplay)
        {
            //Just set the visual, update values
            currentDamageIndicatorFill = currentHealthFill;
            damageIndicator.fillAmount = currentHealthFill;
        }
        else
        {
            damageIndicatorCoroutine = StartCoroutine(DamageIndicatorCoroutine());
        }
    }

    IEnumerator DamageIndicatorCoroutine()
    {
        float totalAnimationTime = 0.5f;
        float startingStay = 0.25f;
        float timeToLerp = totalAnimationTime - startingStay;

        float startingDamageIndicatorFill = currentDamageIndicatorFill;
        float targetDamageIndicatorFill = currentHealthFill;
        
        float timePassed = 0;
        yield return new WaitForSeconds(startingStay);
        while (timePassed <= timeToLerp)
        {
            currentDamageIndicatorFill = Mathf.Lerp(startingDamageIndicatorFill, targetDamageIndicatorFill,
                timePassed / timeToLerp);
            damageIndicator.fillAmount = currentDamageIndicatorFill;
            
            timePassed += Time.deltaTime;
            yield return null;
        }

        currentDamageIndicatorFill = currentHealthFill;
        damageIndicator.fillAmount = currentDamageIndicatorFill;
        damageIndicatorCoroutine = null;
    }
}
