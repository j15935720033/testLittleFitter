using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace BehaviorTree
{
	public class Leaf : Node
	{
		public delegate NodeState Task(Node node);
		protected Task ProcessMethod;

		public Leaf()
		{

		}

		public Leaf(string name, Task ProcessMethod)
		{
			this._name = name;
			this.ProcessMethod = ProcessMethod;
		}
		
		public override NodeState Evaluate()
		{
			if (ProcessMethod != null)
				return ProcessMethod(this);//當前class是Leaf，所以thi代表此Leaf

			return NodeState.FAILURE;
		}
	}
}