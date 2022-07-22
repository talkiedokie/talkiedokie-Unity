using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;
using UnityEngine.Events;
using Gameplay;
using System.Text.RegularExpressions;

namespace Minigame.Cups{
    /// <summary>
    /// Game Manager for Shuffling Cups Minigame.<br/>
    /// Located at: CupShuffle (scene), Gameplay (GameObject)
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        [Header("Configuration")]
        [SerializeField, LabelOverride("Cup GameObject")] 
        private GameObject _cupObj;

        [SerializeField, LabelOverride("Ball GameObject")] 
        private GameObject _ballObj;

        //location of the cups
        [SerializeField]
        private List<CupLocationPair> _cupPosList;

        [SerializeField] private TMP_Text _progressText, _livesText, _gameResultText;

        [Foldout("Events")]
        [SerializeField] private UnityEvent onGameStarted, onGameEnded;
        [SerializeField] private UnityEvent onAnswered;

        private int _progress, _lives, _totalGuesses, _cupAmount;
        private int _shuffleAmount = 3;
        private int[] _lastSwap, _cupSequence;
        private bool _waitingForPlayerInput, _isShuffling;
        private int _ballCupNum;

        private void Awake(){
			// GeneralAudio.Instance.PlayMusic();
            UpdateUIProgress();
        }

        /// <summary>
        /// Gets called when the player presses the start button.
        /// </summary>
        public void StartGame(){
            _progress = 0;
            _lives = 3;
            _totalGuesses = 3;
            _lastSwap = new int[2];
            _cupAmount = _cupPosList.Count;

            //this where the cup numbers and their positions will be placed.
            _cupSequence = Enumerable.Range(0, _cupAmount).ToArray();

            onGameStarted.Invoke();
            SpawnObjects();
            CheckProgress();
        }

        /// <summary>
        /// Spawns the cups.
        /// </summary>
        private void SpawnObjects(){
            //get the parent layer to place it inside
            var parentLayer = _cupObj.transform.parent;

            //spawn the cups
            for (int i = 0; i < _cupAmount; i++)
            {   
                var clone = Instantiate(_cupObj, _cupPosList[i].position, Quaternion.identity, parentLayer);
                LabelCup(clone, i);
            }
        }

        /// <summary>
        /// Labels the clone for organization
        /// </summary>
        /// <param name="clone">Instantiated Cup Object</param>
        /// <param name="num">Cup Number</param>
        private void LabelCup(GameObject clone, int num){
            clone.name = $"Cup {num}";
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
            _ballObj.SetActive(true);

            //randomly select cup number and set location
            SetInitialBallLocation();

            //lift the cup

            //delay to see the ball
            yield return new WaitForSeconds(2);

            //hide ball and put down cup
            _ballObj.SetActive(false);

            for (int i = 0; i < _shuffleAmount; i++)
            {
                //shuffle the cups
                //lastSwap will record which indexes are swapped
                _lastSwap = _cupSequence.ShufflePair(_lastSwap);

                yield return new WaitForSeconds(2);

                //animate the cups
            }

            //move ball to updated location
            UpdateBallLocation();

            //player input
            Invoke(nameof(EnablePlayerInput), 2);
        }

        /// <summary>
        /// Randomly choose which cup should the ball go under.
        /// Gets called before shuffling.
        /// </summary>
        private void SetInitialBallLocation(){
            //randomly select cup number
            _ballCupNum = Random.Range(0, _cupAmount);
            UpdateBallLocation();
        }


        /// <summary>
        /// Updates the location of the ball by finding its designated cup number.
        /// </summary>
        private void UpdateBallLocation(){
            //use _ballCupNum to find the index from the list of cup positions
            var index = GetBallLocation();
            Debug.Log($"location: {(CupLocation) index}");

            //use the index to get position
            _ballObj.transform.position = _cupPosList[index].position;
        }

        /// <summary>
        /// Gets the current location of the ball.<br/>
        /// Throws exception if the ball does not have a cup number or out of bounds.
        /// </summary>
        private int GetBallLocation(){
            //find the index of the cup w/ ball
            for (int i = 0; i < _cupAmount; i++)
            {
                //use _ballCupNum to find the index
                if(_cupSequence[i] == _ballCupNum)
                    return i;                
            }
            return -1;
        }

        private void EnablePlayerInput(){
            Debug.Log("Listening to player");
            Speech.Instance.StartListening(CheckInput);
        }

        private void CheckInput(string speech){
            Speech.Instance.StopListening();
            speech = speech.ToLower();
            string answer = ((CupLocation)GetBallLocation()).ToString().ToLower();
            Regex pattern = new Regex(@$"\b({answer})\b", RegexOptions.IgnoreCase);

            Debug.Log($"Speech: {speech}");

            if(pattern.IsMatch(speech)){
                //correct answer
                Debug.Log("correct");
                CorrectGuess();
            }
            else{
                Debug.Log("wrong");
                WrongGuess();
            }

            onAnswered.Invoke();
            CheckProgress();
        }

        private void CorrectGuess(){
            _progress++;
        }

        private void WrongGuess(){
            _lives--;
        }

        private void EndGame(){

        }
    }

    /// <summary>
    /// Labels the direction of the cup should be placed.
    /// </summary>
    [System.Serializable]
    public struct CupLocationPair{
        public CupLocation direction;
        public Transform transform;

        public Vector3 position{
            get {return transform.position;}
        }
    }

    /// <summary>
    /// General Direction of the cups/ball.
    /// </summary>
    public enum CupLocation{
        None = -1,
        Left,
        Center,
        Right
    }
}
