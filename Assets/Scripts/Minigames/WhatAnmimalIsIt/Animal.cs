using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Minigame.Zoo
{
    [RequireComponent(typeof(Tweener))]
    public class Animal : MonoBehaviour
    {
        [Header("Data")]
        public string _animalName;

        private Tweener _tweener;
        private Animator _anim;

        public void SetupAnimal(Transform initialPos)
        {
            _tweener = GetComponent<Tweener>();
            _anim = GetComponent<Animator>();

            transform.parent.position = initialPos.position;
            transform.parent.rotation = initialPos.transform.rotation;
        }

        public void WalkToDestination(Transform destination)
        {
            _tweener.SetTarget(destination);
        }

        public void PlayAnimation(string name)
        {
            _anim.SetTrigger(name);
        }
    }
}

