using System;
using Modules.Dogs.Model;
using UniRx;

public interface IDogsView
{
    IObservable<Unit> OnTabSelected { get; }
    IObservable<Unit> OnTabDeselected { get; }
    IObservable<string> OnBreedClicked { get; }
    
    void SetBreedsList(DogBreed[] breeds);  // Изменили сигнатуру
    void SetBreedsLoading(bool isLoading);
    void SetFactLoading(bool isLoading);
    void ShowFactPopup(string breedName, string description);
    void HideFactPopup();
}