using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Modules.Dogs.View
{
    public class BreedButton : MonoBehaviour
    {
        [SerializeField] private TMP_Text _nameText;
        [SerializeField] private Button _button;
        
        public Button Button => _button;
        
        public void Setup(string breedId, string breedName, int index)
        {
            _nameText.text = $"{index} - {breedName}";
            _button.onClick.RemoveAllListeners();
            _button.onClick.AddListener(() => OnClick(breedId));
        }
        
        public void Clear()
        {
            _button.onClick.RemoveAllListeners();
        }
        
        private void OnClick(string breedId)
        {
            
        }
        
        private void OnValidate()
        {
            if (_nameText == null)
                _nameText = GetComponentInChildren<TMP_Text>();
            if (_button == null)
                _button = GetComponent<Button>();
        }
    }
}