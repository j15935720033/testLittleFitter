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
        /// 當前節點的children，加入新節點
        /// </summary>
        /// <param name="children"></param>
        public Node(List<Node> children)
        {
            foreach (Node child in children)
                _Attach(child);
        }
        /// <summary>
        /// 把要加入節點的parent指向當前node
        /// </summary>
        /// <param name="node"></param>
        private void _Attach(Node node)
        {
            node.parent = this;//把要加入新節點的parent指向當前node
            children.Add(node);//當前節點的children加入新節點到
        }
        /// <summary>
        /// Now, we can prepare the prototype of the Evaluate() function – 
        /// it will be virtual so that each derived-Node class can implement its own evaluation 
        /// function and have a unique role in the behaviour tree:
        /// </summary>
        /// <returns></returns>
        public virtual NodeState Evaluate() => NodeState.FAILURE;//Lambda寫法
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
            if (_dataContext.TryGetValue(key, out value))//out:傳參考，可不出始化(object value = null;)
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
