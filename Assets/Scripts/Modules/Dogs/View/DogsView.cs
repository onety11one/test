using UnityEngine;
using Core.Navigation;

namespace Modules.Dogs.View
{
    public class DogsView : MonoBehaviour, ITabView
    {
        [SerializeField] private GameObject _contentPanel;
        
        public GameObject GameObject => gameObject;
        
        public void Show()
        {
            if (_contentPanel != null)
                _contentPanel.SetActive(true);
            else
                gameObject.SetActive(true);
            Debug.Log("Dogs tab shown");
        }
        
        public void Hide()
        {
            if (_contentPanel != null)
                _contentPanel.SetActive(false);
            else
                gameObject.SetActive(false);
            Debug.Log("Dogs tab hidden");
        }
    }
}