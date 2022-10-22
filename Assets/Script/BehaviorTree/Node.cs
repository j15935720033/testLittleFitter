using System.Collections;
using System.Collections.Generic;

namespace BehaviorTree
{
    public enum NodeState
    {
        RUNNING,
        SUCCESS,
        FAILURE
    }

    public class Node
    {
        protected NodeState state;
        protected string _name;
        public Node parent;
        protected List<Node> children = new List<Node>();

        private Dictionary<string, object> _dataContext = new Dictionary<string, object>();

        public Node()
        {
            parent = null;
        }
        /// <summary>
        /// ��e�`�I��children�A�[�J�s�`�I
        /// </summary>
        /// <param name="children"></param>
        public Node(List<Node> children)
        {
            foreach (Node child in children)
                _Attach(child);
        }
        /// <summary>
        /// ��n�[�J�`�I��parent���V��enode
        /// </summary>
        /// <param name="node"></param>
        private void _Attach(Node node)
        {
            node.parent = this;//��n�[�J�s�`�I��parent���V��enode
            children.Add(node);//��e�`�I��children�[�J�s�`�I��
        }
        /// <summary>
        /// Now, we can prepare the prototype of the Evaluate() function �V 
        /// it will be virtual so that each derived-Node class can implement its own evaluation 
        /// function and have a unique role in the behaviour tree:
        /// </summary>
        /// <returns></returns>
        public virtual NodeState Evaluate() => NodeState.FAILURE;//Lambda�g�k
        /*public virtual NodeState Evaluate() {
            return NodeState.FAILURE;
        }*/

        public void SetData(string key, object value)
        {
            _dataContext[key] = value;
        }

        public object GetData(string key)
        {
            object value = null;
            if (_dataContext.TryGetValue(key, out value))//out:�ǰѦҡA�i���X�l��(object value = null;)
                return value;

            Node node = parent;
            while (node != null)
            {
                value = node.GetData(key);
                if (value != null)
                    return value;
                node = node.parent;
            }
            return null;
        }

        public bool ClearData(string key)
        {
            if (_dataContext.ContainsKey(key))
            {
                _dataContext.Remove(key);
                return true;
            }

            Node node = parent;
            while (node != null)
            {
                bool cleared = node.ClearData(key);
                if (cleared)
                    return true;
                node = node.parent;
            }
            return false;
        }
    }

}
