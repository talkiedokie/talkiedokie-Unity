using Gameplay;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
// using UnityEngine.SceneManagement;

namespace Minigame.Zoo
{
    public class GameManager : MonoBehaviour
    {

        [Header("Game Configuration")]
        [SerializeField] private List<GameObject> _animalPrefabs = new List<GameObject>();
        [SerializeField] private Transform _animalPosition, _cameraNormalPos, _cameraZoomPos;
        [SerializeField] private TextMeshProUGUI _animalsSolvedText, _gameResultText;
        [SerializeField] private SceneLoader _cityScene;


        // [Header("Events")]
        [Foldout("Events")]
        [SerializeField] private UnityEvent onGameStarted;
        [SerializeField] private StringEvent onAnimalSpawn;
        [SerializeField] private StringEvent onAnimalSolved;
        [SerializeField] private UnityEvent onGameEnded;

        private Animal _currentAnimal;
        private bool _waitingForPlayerInput;
        private Tweener _camTweener;
        private int _solvedAnimals, _totalAnimals;
		
        private void Awake()
        {
            // setup
            _camTweener = Camera.main.GetComponent<Tweener>();
			GeneralAudio.Instance.PlayMusic();
        }

        private void Update()
        {
            HandlePlayerInput();
        }

        public void StartGame()
        {
            _solvedAnimals = 0;
            _totalAnimals = _animalPrefabs.Count;
            UpdateAnimalsSolvedUI();
            onGameStarted.Invoke();
            CheckProgress();
        }

        private void HandlePlayerInput()
        {
            if (!_waitingForPlayerInput)
                return;

            //if (Input.GetMouseButtonDown(0))
            //{
            //    _waitingForPlayerInput = false;

            //    // trigger animal solved. (round win for player)
            //    Speech.Instance.StopListening();
            //    StartCoroutine(AnimalSolved());
            //}
        }

        private void CheckProgress()
        {
            var gameIsDone = _animalPrefabs.Count <= 0 || GameTimer.TimeOver; // or timer is less than zero?????

            if (gameIsDone)
                EndGame();
            else
                SpawnAnimal();
        }

        private void EndGame()
        {
            Debug.Log("Game done!!");
            // update _gameResultText when you have lose scenario

            Invoke(nameof(OnGameEnded), 2);
        }

        private void SpawnAnimal()
        {
            // check for existing animal. Destroy if have any.
            if (_currentAnimal != null)
                Destroy(_currentAnimal.transform.parent.gameObject);

            // determine animal to spawn for the next round by generating random number
            // and using it as array index..
            var randomIndex = Random.Range(0, _animalPrefabs.Count);
            var newAnimalToSpawn = Instantiate(_animalPrefabs[randomIndex]);
            
            // cache current animal
            _currentAnimal = newAnimalToSpawn.GetComponentInChildren<Animal>();

            // remove used animal so it won't spawn for a second time
            _animalPrefabs.RemoveAt(randomIndex);

            // setup current animal
            SetupCurrentAnimal();

            // fire event
            onAnimalSpawn.Invoke(_currentAnimal.name);

            // setup camera
            _camTweener.SetTarget(_cameraNormalPos);

            Invoke(nameof(TurnOnPlayerTurn), 2);
        }

        private void SetupCurrentAnimal()
        {

            // setup initial position
            _currentAnimal.SetupAnimal(_animalPosition);
        }

        private IEnumerator AnimalSolved()
        {
            Debug.Log("Animal Solved");

            // Update animals solved
            _solvedAnimals++;
            UpdateAnimalsSolvedUI();

            // play some animation on animal for player feedback and make the current animal exit the scene
            _currentAnimal.PlayAnimation("positive");

            // todo : UI prompt and effects...
            onAnimalSolved.Invoke(_currentAnimal.name);

            _camTweener.SetTarget(_cameraZoomPos);

            // check game progress after some time
            yield return new WaitForSeconds(2f);

            CheckProgress();
        }

        private void TurnOnPlayerTurn()
        {
            _waitingForPlayerInput = true;
            Debug.Log("Waiting for input...");
            Speech.Instance.StartListening(CheckInput);
         }

        public void CheckInput(string speech)
        {
            Speech.Instance.StopListening();

            Debug.Log(speech.ToLower() + " | " + _currentAnimal._animalName.ToLower());
            if (speech.ToLower().Equals(_currentAnimal._animalName.ToLower()))
            {
                Debug.Log("Correct");
                _waitingForPlayerInput = false;
                StartCoroutine(AnimalSolved());
            }
            else
            {
                Invoke(nameof(TurnOnPlayerTurn), 2);
            }
        }

        private void UpdateAnimalsSolvedUI()
        {
            _animalsSolvedText.text = $"Animals solved : <br> {_solvedAnimals} / {_totalAnimals}";
        }

        private void OnGameEnded()
        {
            onGameEnded.Invoke();
        }

        public void RestartGame()
        {
            // SceneManager.LoadScene(SceneManager.GetActiveScene().name);
			SceneLoader.Current();
        }

        public void BackToCity()
        {
            _cityScene.LoadAsync();
        }
    }


    [System.Serializable]
    public class StringEvent : UnityEvent<string> { }
}



