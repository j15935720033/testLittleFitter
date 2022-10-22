using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorTree
{
    public abstract class Tree : MonoBehaviour
    {

        private Node _root = null;

        protected virtual void Start()
        {
            _root = SetupTree();
        }

        /// <summary>
        /// if it has a tree, it will evaluate it continuously
        /// </summary>
        protected virtual void Update()
        {
            if (_root != null)
                _root.Evaluate();
        }
        /// <summary>
        /// build the behaviour tree 
        /// </summary>
        /// <returns></returns>
        protected abstract Node SetupTree();

    }

}
