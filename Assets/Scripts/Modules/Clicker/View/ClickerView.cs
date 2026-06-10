using System;
using DG.Tweening;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Modules.Clicker.View
{
    public class ClickerView : MonoBehaviour, IClickerView
    {
        [Header("UI Elements")]
        [SerializeField] private Button _clickButton;
        [SerializeField] private TMP_Text _currencyText;
        [SerializeField] private TMP_Text _energyText;
        
        [Header("Animation")]
        [SerializeField] private float _pressScale = 0.9f;
        [SerializeField] private float _pressDuration = 0.1f;
        
        [Header("VFX Points")]
        [SerializeField] private Transform _particleSpawnPoint;
        [SerializeField] private Transform _coinTarget;
        
        [Header("Audio")]
        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private AudioClip _clickSound;
        
        public IObservable<Unit> OnClickButton => _clickButton.OnClickAsObservable();
        public RectTransform ButtonTransform => _clickButton.transform as RectTransform;
        public Transform CoinTarget => _coinTarget;
        public Transform ParticleSpawnPoint => _particleSpawnPoint;
        
        public void SetCurrencyText(string text) => _currencyText.text = text;
        public void SetEnergyText(string text) => _energyText.text = text;
        
        public void PlayClickAnimation()
        {
            if (_clickButton == null) return;
            
            _clickButton.transform
                .DOScale(_pressScale, _pressDuration)
                .SetEase(Ease.InOutQuad)
                .OnComplete(() =>
                {
                    _clickButton.transform
                        .DOScale(1f, _pressDuration)
                        .SetEase(Ease.InOutQuad);
                });
        }
        
        public void PlayParticleEffect()
        {
            Debug.Log("Particle effect played");
        }
        
        public void PlayCoinFlyAnimation(Vector3 from, Vector3 to)
        {
            Debug.Log($"Coin flying from {from} to {to}");
        }
        
        public void PlayClickSound()
        {
            if (_audioSource != null && _clickSound != null)
            {
                _audioSource.PlayOneShot(_clickSound);
            }
        }
        
        public void ShowEnergyDepletedEffect()
        {
            _clickButton.transform.DOShakePosition(0.3f, 10f, 20);
        }
    }
}