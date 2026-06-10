using System;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Core.Navigation
{
    public class TabButton : MonoBehaviour
    {
        [SerializeField] private Button _button;
        [SerializeField] private GameObject _activeIndicator;
        
        public IObservable<Unit> OnClick => _button.OnClickAsObservable();
        
        public void SetActive(bool isActive)
        {
            if (_activeIndicator != null)
                _activeIndicator.SetActive(isActive);
        }
        
        private void Awake()
        {
            if (_button == null)
                _button = GetComponent<Button>();
        }
    }
}