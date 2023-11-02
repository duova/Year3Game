using System;
using UnityEngine;

namespace Core
{
    public abstract class Controller : MonoBehaviour
    {
        public Actor Actor { get; set; }

        protected virtual void Awake()
        {
            Actor = GetComponent<Actor>();
        }
    }
}