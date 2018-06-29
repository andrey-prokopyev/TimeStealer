using System;
using Control;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class HealthDisplay : MonoBehaviour
{
    private float maxHealth;

    private Text textIndicator;

    private void Start()
    {
        this.textIndicator = this.GetComponent<Text>();
    }

    [Inject]
    public void Construct(SignalBus signalBus, PlayerState.Settings settings)
    {
        this.maxHealth = settings.Health;
        signalBus.Subscribe<PlayerState.HealthChanged>(this.OnHealthChanged);
    }

    private void OnHealthChanged(PlayerState.HealthChanged healthChanged)
    {
        this.textIndicator.text = string.Format("{0} / {1}", healthChanged.HealthAfter, this.maxHealth);
    }
}
