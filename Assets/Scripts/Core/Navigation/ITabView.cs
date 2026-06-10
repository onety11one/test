using UnityEngine;

namespace Core.Navigation
{
    public interface ITabView
    {
        GameObject GameObject { get; }
        void Show();
        void Hide();
    }
}