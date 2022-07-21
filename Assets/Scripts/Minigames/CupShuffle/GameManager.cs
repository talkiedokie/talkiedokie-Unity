using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;
using UnityEngine.Events;

namespace Minigame.Cups{
    /// <summary>
    /// Game Manager for Shuffling Cups Minigame.<br/>
    /// Located at: CupShuffle (scene), Gameplay (GameObject)
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        [Header("Configuration")]
        [SerializeField, InspectorName("Cup GameObject")] 
        private GameObject _cupObj;

        [SerializeField, InspectorName("Ball GameObject")] 
        private GameObject _ballObj;

        //location of the cups
        [SerializeField, InspectorName("Cup Locations")]
        private List<Transform> _cupPos;

        [SerializeField] private TMP_Text _progressText, _livesText, _gameResultText;

        [Foldout("Events")]
        [SerializeField] private UnityEvent onGameStarted, onGameEnded;
        [SerializeField] private UnityEvent<string> onCupGuessed;

        private int _progress, _lives, _totalGuesses, _ballIndex;
        private int _shuffleAmount = 3;
        private int[] _cupSequence, _lastSwap;
        private bool _waitingForPlayerInput, _isShuffling;

        private void Awake(){
			// GeneralAudio.Instance.PlayMusic();
        }

        /// <summary>
        /// Gets called when the player presses the start button.
        /// </summary>
        public void StartGame(){
            _progress = 0;
            _lives = 3;
            _totalGuesses = 3;
            _lastSwap = new int[2];

            //creates an array that counts through _totalGuesses.
            //e.g. [0,1,2] if _totalGuesses is 3.
            _cupSequence = Enumerable.Range(0, _totalGuesses).ToArray();

            onGameStarted.Invoke();
            SpawnObjects();
            CheckProgress();
        }

        /// <summary>
        /// Spawns the cups and the ball.
        /// </summary>
        private void SpawnObjects(){
            //get the parent layer to place it inside
            var parentLayer = _cupObj.transform.parent;

            //spawn the cups
            for (int i = 0; i < _cupPos.Count; i++)
            {   
                Instantiate(_cupObj, _cupPos[i].position, Quaternion.identity, parentLayer);
            }

            //spawn the ball

        }

        /// <summary>
        /// Determines whether to continue or end the game on certain conditions.
        /// </summary>
        private void CheckProgress(){
            //checks if it ran out of lives or guessed all shuffles
            var isDone = _lives == 0 || _progress == _totalGuesses;

            UpdateUIProgress();

            if(isDone)
                EndGame();
            else
                StartCoroutine(nameof(ShuffleCups));
        }

        /// <summary>
        /// Updates the UI of the minigame.
        /// </summary>
        private void UpdateUIProgress(){
            _progressText.text = $"{_progress} of {_totalGuesses}";
            _livesText.text = $"Lives: {_lives}";
        }

        /// <summary>
        /// Shuffles the cups by random pairs.
        /// </summary>
        private IEnumerator ShuffleCups(){
            _isShuffling = true;
            for (int i = 0; i < _shuffleAmount; i++)
            {
                //move the cups
                //lastSwap will record which indices are swapped
                _lastSwap = _cupSequence.ShufflePair(_lastSwap);

                yield return new WaitForSeconds(2);

                //animate the cups
            }
            _isShuffling = false;
        }

        private void ShufflePair(){

        }

        private void EndGame(){

        }
    }
}
