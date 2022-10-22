using System.Collections.Generic;

namespace BehaviorTree
{
    /// <summary>composites
    /// Node:節點
    /// Controlnode:控制節點通常是父節點。可以分成1.composite節點 2.decorate節點
    /// {composit節點:[sequence節點,decorate節點]}
    /// selector，从左到右依次执行子节点，当任一子节点返回true后，结束执行。

    /// </summary>
    public class Selector : Node
    {
        public Selector() : base() { }
        public Selector(List<Node> children) : base(children) { }

        public override NodeState Evaluate()
        {
            foreach (Node node in children)
            {
                switch (node.Evaluate())
                {
                    case NodeState.FAILURE:
                        continue;
                    case NodeState.SUCCESS:
                        state = NodeState.SUCCESS;
                        return state;
                    case NodeState.RUNNING:
                        state = NodeState.RUNNING;
                        return state;
                    default:
                        continue;
                }
            }

            state = NodeState.FAILURE;
            return state;
        }

    }

}
