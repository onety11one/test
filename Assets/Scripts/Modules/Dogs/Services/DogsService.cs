using System;
using System.Collections.Generic;
using System.Linq;
using Modules.Dogs.Model;
using UnityEngine;
using UnityEngine.Networking;

namespace Modules.Dogs.Services
{
    public class DogsService
    {
        private const string BREEDS_URL = "https://dogapi.dog/api/v2/breeds";
        private const string FACTS_URL = "https://dogapi.dog/api/v2/breeds/";
        
        public UnityWebRequest CreateBreedsRequest()
        {
            var request = UnityWebRequest.Get(BREEDS_URL);
            request.SetRequestHeader("Accept", "application/json");
            return request;
        }
        
        public UnityWebRequest CreateFactRequest(string breedId)
        {
            var request = UnityWebRequest.Get($"{FACTS_URL}{breedId}");
            request.SetRequestHeader("Accept", "application/json");
            return request;
        }
        
        public List<DogBreed> ParseBreedsResponse(string json)
        {
            try
            {
                var response = JsonUtility.FromJson<BreedsResponse>(json);
                var allBreeds = new List<DogBreed>();
                
                for (int i = 0; i < response.data.Length; i++)
                {
                    allBreeds.Add(new DogBreed
                    {
                        id = response.data[i].id,
                        name = response.data[i].attributes.name
                    });
                }
                
                var random = new System.Random();
                var randomBreeds = allBreeds.OrderBy(x => random.Next()).Take(10).ToList();
                
                return randomBreeds;
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to parse breeds: {e.Message}");
                return null;
            }
        }
        
        public DogFact ParseFactResponse(string json)
        {
            try
            {
                var response = JsonUtility.FromJson<FactResponse>(json);
                return new DogFact
                {
                    breedName = response.data.attributes.name,
                    description = response.data.attributes.description
                };
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to parse fact: {e.Message}");
                return null;
            }
        }
    }
    
    [Serializable]
    public class BreedsResponse
    {
        public BreedData[] data;
    }
    
    [Serializable]
    public class BreedData
    {
        public string id;
        public BreedAttributes attributes;
    }
    
    [Serializable]
    public class BreedAttributes
    {
        public string name;
        public string description;
    }
    
    [Serializable]
    public class FactResponse
    {
        public FactData data;
    }
    
    [Serializable]
    public class FactData
    {
        public FactAttributes attributes;
    }
    
    [Serializable]
    public class FactAttributes
    {
        public string name;
        public string description;
    }
}