using System;
using System.Collections.Generic;
using UniRx;

namespace Core.Navigation
{
    public class NavigationController : IDisposable
    {
        private readonly List<ITabView> _tabs = new();
        private readonly List<TabButton> _tabButtons = new();
        private readonly CompositeDisposable _disposables = new();
        private int _currentTabIndex = -1;
        
        public IReadOnlyList<ITabView> Tabs => _tabs;
        public int CurrentTabIndex => _currentTabIndex;
        
        public Subject<int> OnTabChanged { get; } = new();
        
        public void AddTab(ITabView tabView, TabButton tabButton)
        {
            _tabs.Add(tabView);
            _tabButtons.Add(tabButton);
            
            int tabIndex = _tabs.Count - 1;
            
            tabButton.OnClick
                .Subscribe(_ => SwitchTab(tabIndex))
                .AddTo(_disposables);
        }
        
        public void SwitchTab(int index)
        {
            if (index == _currentTabIndex || index < 0 || index >= _tabs.Count)
                return;
            
            if (_currentTabIndex >= 0)
            {
                _tabs[_currentTabIndex].Hide();
                _tabButtons[_currentTabIndex].SetActive(false);
            }
            
            _currentTabIndex = index;
            _tabs[_currentTabIndex].Show();
            _tabButtons[_currentTabIndex].SetActive(true);
            
            OnTabChanged.OnNext(_currentTabIndex);
        }
        
        public void Dispose()
        {
            _disposables?.Dispose();
            OnTabChanged?.Dispose();
        }
    }
}