using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace Core.Network
{
    public class NetworkRequestQueue : IDisposable
    {
        private readonly Queue<QueuedRequest> _requestQueue = new();
        private QueuedRequest _currentRequest;
        private bool _isProcessing;
        
        public class QueuedRequest
        {
            public Func<UnityWebRequest> RequestFactory { get; set; }
            public Action<string> OnSuccess { get; set; }
            public Action<string> OnError { get; set; }
            public UnityWebRequest CurrentRequest { get; set; }
            public bool IsCancelled { get; set; }
        }
        
        public QueuedRequest Enqueue(Func<UnityWebRequest> requestFactory, Action<string> onSuccess = null, Action<string> onError = null)
        {
            var request = new QueuedRequest
            {
                RequestFactory = requestFactory,
                OnSuccess = onSuccess,
                OnError = onError,
                IsCancelled = false
            };
    
            _requestQueue.Enqueue(request);
            Debug.Log($"Weather: queue request added, queue size: {_requestQueue.Count}");
    
            if (!_isProcessing)
            {
                ProcessNext().Forget();
            }
    
            return request;
        }

        public void CancelRequest(QueuedRequest request)
        {
            if (request == null) return;
    
            request.IsCancelled = true;
            
            if (_currentRequest == request && request.CurrentRequest != null)
            {
                Debug.Log("Weather: aborting active request");
                request.CurrentRequest.Abort();
            }
        }

        private void ClearQueue()
        {
            foreach (var request in _requestQueue)
            {
                request.IsCancelled = true;
            }
            _requestQueue.Clear();
            
            if (_currentRequest != null)
            {
                _currentRequest.IsCancelled = true;
                _currentRequest.CurrentRequest?.Abort();
            }
        }
        
        private async UniTaskVoid ProcessNext()
        {
            if (_requestQueue.Count == 0)
            {
                _isProcessing = false;
                return;
            }
            
            _isProcessing = true;
            _currentRequest = _requestQueue.Dequeue();
            
            if (_currentRequest.IsCancelled)
            {
                ProcessNext().Forget();
                return;
            }
            
            try
            {
                var webRequest = _currentRequest.RequestFactory();
                _currentRequest.CurrentRequest = webRequest;
                
                await webRequest.SendWebRequest();
                
                if (!_currentRequest.IsCancelled)
                {
                    if (webRequest.result == UnityWebRequest.Result.Success)
                    {
                        _currentRequest.OnSuccess?.Invoke(webRequest.downloadHandler.text);
                    }
                    else
                    {
                        _currentRequest.OnError?.Invoke(webRequest.error);
                    }
                }
            }
            catch (Exception e)
            {
                if (!_currentRequest.IsCancelled)
                {
                    _currentRequest.OnError?.Invoke(e.Message);
                }
            }
            finally
            {
                _currentRequest = null;
                ProcessNext().Forget();
            }
        }
        
        public void Dispose()
        {
            ClearQueue();
        }
    }
}