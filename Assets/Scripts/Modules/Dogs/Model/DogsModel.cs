using System;
using UniRx;

namespace Modules.Dogs.Model
{
    [Serializable]
    public class DogBreed
    {
        public string id;
        public string name;
    }
    
    [Serializable]
    public class DogFact
    {
        public string breedName;
        public string description;
    }
    
    public class DogsModel : IDisposable
    {
        public ReactiveCollection<DogBreed> Breeds { get; } = new();
        public ReactiveProperty<DogFact> CurrentFact { get; } = new();
        public ReactiveProperty<bool> IsBreedsLoading { get; } = new();
        public ReactiveProperty<bool> IsFactLoading { get; } = new();
        
        public void Dispose()
        {
            Breeds?.Dispose();
            CurrentFact?.Dispose();
            IsBreedsLoading?.Dispose();
            IsFactLoading?.Dispose();
        }
    }
}