using System.Collections.Generic;

namespace BehaviorTree
{
    /// <summary>
    /// Node:節點
    /// Controlnode:控制節點通常是父節點。可以分成1.composite節點 2.decorate節點
    /// {composit節點:[sequence節點,decorate節點]}
    /// sequence，从左到右依次执行子节点，当任一子节点返回false后，结束执行。
    /// </summary>
    public class Sequence : Node
    {
        public Sequence() : base() { }
        public Sequence(List<Node> children) : base(children) { }

        public override NodeState Evaluate()
        {
            bool anyChildIsRunning = false;

            foreach (Node node in children)
            {
                switch (node.Evaluate())
                {
                    case NodeState.FAILURE:
                        state = NodeState.FAILURE;
                        return state;
                    case NodeState.SUCCESS:
                        continue;
                    case NodeState.RUNNING:
                        anyChildIsRunning = true;
                        continue;
                    default:
                        state = NodeState.SUCCESS;
                        return state;
                }
            }

            state = anyChildIsRunning ? NodeState.RUNNING : NodeState.SUCCESS;
            return state;
        }

    }

}
