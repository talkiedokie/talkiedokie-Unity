using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using System.Linq;

namespace Minigame.Cups{
    public static class Shuffler
    {
        /// <summary>
        /// Shuffles a pair from an array sequence.
        /// Throws exception if array size is less than 2. <br/>
        /// Usage: fruits.SwapPair([2,3])
        /// </summary>
        /// <typeparam name="T">Data Type of the array</typeparam>
        /// <param name="arr">Array to shuffle</param>
        /// <param name="lastPair">Previous pair that got swapped, to prevent repetition.</param>
        public static int[] ShufflePair<T>(this T[] arr, int[] lastPair = null){
            int[] randPair = new int[2];

            do
            {
                //selected index pair to swap
                for (int i = 0; i < randPair.Length; i++)
                    randPair[i] = Random.Range(0, arr.Length);
            }
            //re-select the pairs if it's similar with the last pair
            while (!(arr.Length <= 2 || lastPair == null || CheckPairs(randPair, lastPair)));

            foreach(var i in randPair){
                Debug.Log(i);
            }
            Debug.Log("----");

            //swap the elements
            var temp = arr[randPair[0]];
            arr[randPair[0]] = arr[randPair[1]];
            arr[randPair[1]] = temp;

            return randPair;
        }

        /// <summary>
        /// Checks the random pair if it's accepted for swapping index values.
        /// </summary>
        /// <param name="randomPair">Pair that was selected by random</param>
        /// <param name="lastPair">Last pair that got swapped</param>
        private static bool CheckPairs(int[] randomPair, int[] lastPair){
            //check if random pair have unique values
            var isRandomUnique = randomPair.Distinct().Count() != 1;
            //check if the two pair have different numbers
            var isDifferent = randomPair.Union(lastPair).Count() != 2;

            return isRandomUnique && isDifferent;
        }
    }
}