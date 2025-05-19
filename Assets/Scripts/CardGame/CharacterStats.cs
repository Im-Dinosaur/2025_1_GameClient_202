using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CharacterStats : MonoBehaviour
{
    public string characterName;
    public int maxHealth = 100;
    public int currentHealth;
    public int currentMana;
    public int maxMana = 10;

    public Slider healthBar;
    public Slider manaBar;
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI manaText;
    // Start is called before the first frame update
    void Start()
    {
        currentMana = maxMana;
        UpdateUI();
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if(DamageEffectManager.Instance != null)
        {
            Vector3 position = transform.position;
            position += new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(1f, 1.5f), 0);
            DamageEffectManager.Instance.ShowDamage(position, damage, false);
        }
    }

    public void Heal(int amount)
    {
        currentHealth += amount;
        if (DamageEffectManager.Instance != null)
        {
            Vector3 position = transform.position;
            position += new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(1f, 1.5f), 0);
            DamageEffectManager.Instance.ShowHeal(position, amount, false);
        }
    }

    public void UseMana(int amount)
    {
        currentMana -= amount;
        if(currentMana < 0)
        {
            currentMana = 0;
        }
        UpdateUI();
    }

    public void GainMana(int amount)
    {
        currentMana += amount;
        if(currentMana > maxHealth)
        {
            currentMana = maxMana;
        }
        UpdateUI();
    }

    private void UpdateUI()
    {
        if(healthBar != null)
        {
            healthBar.value = (float)currentHealth / maxHealth;
        }

        if(healthText != null)
        {
            healthText.text = $"{currentHealth} / {maxHealth}";
        }

        if(manaBar != null)
        {
            manaBar.value = (float)currentMana / maxMana;
        }

        if (manaText != null)
        {
            manaText.text = $"{currentMana} / {maxMana}";
        }
    }
}
