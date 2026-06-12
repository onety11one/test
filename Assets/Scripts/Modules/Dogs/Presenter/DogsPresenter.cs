using System;
using System.Linq;
using UniRx;
using UnityEngine;
using Core.Network;
using Modules.Dogs.Model;
using Modules.Dogs.Services;

namespace Modules.Dogs.Presenter
{
    public class DogsPresenter : IDisposable
    {
        private readonly DogsModel _model;
        private readonly IDogsView _view;
        private readonly DogsService _dogsService;
        private readonly NetworkRequestQueue _requestQueue;
        private readonly CompositeDisposable _disposables = new();
        
        private NetworkRequestQueue.QueuedRequest _breedsRequest;
        private NetworkRequestQueue.QueuedRequest _factRequest;
        private bool _isTabActive;
        
        public DogsPresenter(DogsModel model, IDogsView view, DogsService dogsService, NetworkRequestQueue requestQueue)
        {
            _model = model;
            _view = view;
            _dogsService = dogsService;
            _requestQueue = requestQueue;
        }
        
        public void Initialize()
        {
            _view.OnTabSelected
                .Subscribe(_ =>
                {
                    Debug.Log("Dog: loading breeds");
                    _isTabActive = true;
                    RequestBreeds();
                })
                .AddTo(_disposables);
            
            _view.OnTabDeselected
                .Subscribe(_ =>
                {
                    Debug.Log("Dogs: cancelling all requests");
                    _isTabActive = false;
                    CancelAllRequests();
                })
                .AddTo(_disposables);
            
            _view.OnBreedClicked
                .Subscribe(RequestFact)
                .AddTo(_disposables);
            
            _model.Breeds
                .ObserveAdd()
                .Subscribe(_ => UpdateBreedsList())
                .AddTo(_disposables);
            
            _model.CurrentFact
                .Subscribe(fact =>
                {
                    if (fact != null)
                    {
                        _view.ShowFactPopup(fact.breedName, fact.description);
                    }
                })
                .AddTo(_disposables);
            
            _model.IsBreedsLoading
                .Subscribe(isLoading => _view.SetLoading(isLoading))
                .AddTo(_disposables);
            
            _model.IsFactLoading
                .Subscribe(isLoading => _view.SetLoading(isLoading))
                .AddTo(_disposables);
        }
        
        private void RequestBreeds()
        {
            if (!_isTabActive) return;
            
            Debug.Log("Dogs:adding breeds request to queue");
            _model.IsBreedsLoading.Value = true;
            
            _breedsRequest = _requestQueue.Enqueue(
                requestFactory: () => _dogsService.CreateBreedsRequest(),
                onSuccess: (json) =>
                {
                    Debug.Log("Dogs:breeds request completed");
                    var breeds = _dogsService.ParseBreedsResponse(json);
                    if (breeds != null)
                    {
                        _model.Breeds.Clear();
                        foreach (var breed in breeds)
                        {
                            _model.Breeds.Add(breed);
                        }
                    }
                    _model.IsBreedsLoading.Value = false;
                },
                onError: (error) =>
                {
                    Debug.LogError($"Dogs: breeds request failed: {error}");
                    _model.IsBreedsLoading.Value = false;
                }
            );
        }
        
        private void RequestFact(string breedId)
        {
            if (!_isTabActive) return;
            
            if (_factRequest != null)
            {
                _requestQueue.CancelRequest(_factRequest);
                _factRequest = null;
            }
            
            Debug.Log($"Dogs: adding fact request to queue for breed: {breedId}");
            _model.IsFactLoading.Value = true;
            _view.HideFactPopup();
            
            _factRequest = _requestQueue.Enqueue(
                requestFactory: () => _dogsService.CreateFactRequest(breedId),
                onSuccess: (json) =>
                {
                    Debug.Log("Dogs: fact request completed");
                    var fact = _dogsService.ParseFactResponse(json);
                    if (fact != null)
                    {
                        _model.CurrentFact.Value = fact;
                    }
                    _model.IsFactLoading.Value = false;
                },
                onError: (error) =>
                {
                    Debug.LogError($"Dogs: fact request failed: {error}");
                    _model.IsFactLoading.Value = false;
                }
            );
        }
        
        private void CancelAllRequests()
        {
            if (_breedsRequest != null)
            {
                Debug.Log("Dogs: cancelling breeds request");
                _requestQueue.CancelRequest(_breedsRequest);
                _breedsRequest = null;
            }
            
            if (_factRequest != null)
            {
                Debug.Log("Dogs: cancelling fact request");
                _requestQueue.CancelRequest(_factRequest);
                _factRequest = null;
            }
            
            _view.HideFactPopup();
            _model.IsBreedsLoading.Value = false;
            _model.IsFactLoading.Value = false;
        }
        
        private void UpdateBreedsList()
        {
            var breeds = _model.Breeds.ToArray();
            _view.SetBreedsList(breeds);
        }
        
        public void Dispose()
        {
            CancelAllRequests();
            _disposables?.Dispose();
            _model?.Dispose();
        }
    }
}