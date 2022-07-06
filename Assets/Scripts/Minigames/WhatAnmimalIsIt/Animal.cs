using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Zoo
{
    [RequireComponent(typeof(Tweener))]
    public class Animal : MonoBehaviour
    {
        [Header("Data")]
        [SerializeField] private string _animalName;
        private Tweener _tweener;

        public void SetupAnimal(Transform initialPos)
        {
            _tweener = GetComponent<Tweener>();

            if (initialPos != null)
            {
                transform.position = initialPos.position;
                transform.rotation = initialPos.transform.rotation;
            }
        }

        public void WalkToDestination(Transform destination)
        {
            _tweener.SetTarget(destination);
        }

        public void PlayPositiveAnimation()
        {
            // todo : trigger positive animation in animator
        }
    }
}

