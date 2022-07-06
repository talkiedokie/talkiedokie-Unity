using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Zoo
{
    public class GameManager : MonoBehaviour
    {

        [Header("Game Configuration")]
        [SerializeField] private List<GameObject> _animalPrefabs = new List<GameObject>();
        [SerializeField] private Transform _animalStartingDestination, _animalCenterDestination, _animalExitDestination;

        
        private Animal _currentAnimal;
        private bool _waitingForPlayerInput;
        

        private void Update()
        {
            HandlePlayerInput();
        }

        [ContextMenu("Start Game")]
        private void StartGame()
        {
            CheckProgress();
        }

        private void HandlePlayerInput()
        {
            if (!_waitingForPlayerInput)
                return;

            if (Input.GetMouseButtonDown(0))
            {
                _waitingForPlayerInput = false;

                // trigger animal solved. (round win for player)
                StartCoroutine(AnimalSolved(_currentAnimal));
            }
        }

        private void CheckProgress()
        {
            var gameIsDone = _animalPrefabs.Count <= 0; // or timer is less than zero?????

            if (gameIsDone)
                EndGame();
            else
                SpawnAnimal();
        }

        private void EndGame()
        {
            Debug.Log("Game done!!");
            // todo : UI prompt...
        }

        private void SpawnAnimal()
        {
            // check for existing animal. Destroy if have any.
            if (_currentAnimal != null)
                Destroy(_currentAnimal);

            // determine animal to spawn for the next round by generating random number
            // and using it as array index..
            var randomIndex = Random.Range(0, _animalPrefabs.Count);
            var newAnimalToSpawn = Instantiate(_animalPrefabs[randomIndex]);
            
            // cache current animal
            _currentAnimal = newAnimalToSpawn.GetComponent<Animal>();

            // remove used animal so it won't spawn for a second time
            _animalPrefabs.RemoveAt(randomIndex);

            // setup current animal
            SetupAnimal(_currentAnimal);
        }

        private void SetupAnimal(Animal animal)
        {

            // setup initial position
            animal.SetupAnimal(_animalStartingDestination);
            animal.WalkToDestination(_animalCenterDestination);

            Invoke(nameof(TurnOnWaitingForInput), 3);
        }

        private IEnumerator AnimalSolved(Animal animal)
        {
            Debug.Log("Animal Solved");

            // play some animation on animal for player feedback and make the current animal exit the scene
            animal.PlayPositiveAnimation();
            yield return new WaitForSeconds(3f);
            animal.WalkToDestination(_animalExitDestination);


            // todo : UI prompt and effects...

            // check game progress after some time
            yield return new WaitForSeconds(3f);
            CheckProgress();


        }


        private void TurnOnWaitingForInput()
        {
            _waitingForPlayerInput = true;
            Debug.Log("Waiting for input...");
            // todo : UI prompt...
        }
    }
}

