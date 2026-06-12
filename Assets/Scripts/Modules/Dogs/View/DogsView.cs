using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Core.Navigation;
using Modules.Dogs.Model;
using TMPro;

namespace Modules.Dogs.View
{
    public class DogsView : MonoBehaviour, ITabView, IDogsView
    {
        [Header("UI Elements")]
        [SerializeField] private GameObject _contentPanel;
        [SerializeField] private Transform _breedsContainer;
        [SerializeField] private Button _breedButtonPrefab;
        [SerializeField] private GameObject _breedsLoadingIndicator;
        
        [Header("Fact Popup")]
        [SerializeField] private GameObject _factPopup;
        [SerializeField] private TMP_Text _factBreedName;
        [SerializeField] private TMP_Text _factDescription;
        [SerializeField] private Button _factCloseButton;
        [SerializeField] private GameObject _factLoadingIndicator;
        
        [Header("Content")]
        [SerializeField] private RectTransform _popupContent;
        
        public GameObject GameObject => gameObject;
        
        private readonly Subject<Unit> _onTabSelected = new();
        private readonly Subject<Unit> _onTabDeselected = new();
        private readonly Subject<string> _onBreedClicked = new();
        private readonly List<Button> _breedButtons = new();
        
        public IObservable<Unit> OnTabSelected => _onTabSelected;
        public IObservable<Unit> OnTabDeselected => _onTabDeselected;
        public IObservable<string> OnBreedClicked => _onBreedClicked;
        
        public void Show()
        {
            if (_contentPanel != null)
                _contentPanel.SetActive(true);
            else
                gameObject.SetActive(true);
            
            _onTabSelected.OnNext(Unit.Default);
        }
        
        public void Hide()
        {
            if (_contentPanel != null)
                _contentPanel.SetActive(false);
            else
                gameObject.SetActive(false);
            
            _onTabDeselected.OnNext(Unit.Default);
        }
        
        public void SetBreedsList(DogBreed[] breeds)
        {
            foreach (var button in _breedButtons)
            {
                Destroy(button.gameObject);
            }
            _breedButtons.Clear();
            
            for (int i = 0; i < breeds.Length; i++)
            {
                var button = Instantiate(_breedButtonPrefab, _breedsContainer);
                var text = button.GetComponentInChildren<TMP_Text>();
                text.text = $"{i + 1} - {breeds[i].name}";
        
                var breedId = breeds[i].id;
                button.onClick.AddListener(() => _onBreedClicked.OnNext(breedId));
        
                _breedButtons.Add(button);
            }
        }
        
        public void SetBreedsLoading(bool isLoading)
        {
            if (_breedsLoadingIndicator != null)
                _breedsLoadingIndicator.SetActive(isLoading);
        }
        
        public void SetFactLoading(bool isLoading)
        {
            if (_factLoadingIndicator != null)
                _factLoadingIndicator.SetActive(isLoading);
        }
        
        public void ShowFactPopup(string breedName, string description)
        {
            if (_factPopup != null)
            {
                _factBreedName.text = breedName;
                _factDescription.text = description;
                _factPopup.SetActive(true);
                
                //Canvas.ForceUpdateCanvases();
                //LayoutRebuilder.ForceRebuildLayoutImmediate(_popupContent);
            }
        }
        
        public void HideFactPopup()
        {
            if (_factPopup != null)
                _factPopup.SetActive(false);
        }
        
        void Start()
        {
            if (_factCloseButton != null)
            {
                _factCloseButton.onClick.AddListener(HideFactPopup);
            }
        }
    }
}