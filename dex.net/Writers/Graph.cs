using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Threading;
using System.Text;
using System.Collections;

namespace dex.net
{
	/// <summary>
	/// A code sequence without branches. Jumps can only reference
	/// the beginning of a code block.
	/// </summary>
	class CodeBlock 
	{
		internal int Id { get; set; }
		internal CodeBlock Parent { get; set; }
		internal int BeginIndex;
		internal int EndIndex;
		internal List<OpCode> Code;
		private List<CodeBlock> _tempTargets;

		protected List<CodeBlock> _targets;
		internal List<CodeBlock> Targets
		{
			get { return _targets; }
		}

		private List<CodeBlock> _referrers;
		internal List<CodeBlock> Referrers
		{
			get { return _referrers; }
		}

		public CodeBlock(int id, CodeBlock parent)
		{
			Id = id;
			Parent = parent;
			Code = new List<OpCode> ();
		}

		public void AddTarget(CodeBlock target)
		{
			if (_targets == null) {
				_targets = new List<CodeBlock> ();
			}

			_targets.Add (target);
		}

		public void AddReferrer(CodeBlock referrer)
		{
			if (_referrers == null) {
				_referrers = new List<CodeBlock> ();
			}

			_referrers.Add (referrer);
		}

		public override string ToString ()
		{
			return string.Format ("[CodeBlock Id: {0}]", Id);
		}

		public override int GetHashCode ()
		{
			return Id.GetHashCode ();
		}

		public CodeBlock GetTarget(int index)
		{
			if (Targets == null) {
				return null;
			}

			return Targets [index] as CodeBlock;
		}

		public void UseTargets(bool flag)
		{
			if (flag && _tempTargets != null) {
				_targets = _tempTargets;
			} else {
				_tempTargets = _targets;
				_targets = null;
			}
		}
	}


	class Graph
	{
		internal List<CodeBlock> Nodes = new List<CodeBlock>();
		internal CodeBlock Root { get { return Nodes [0]; } }

		private const int COLOR_WHITE = 0;
		private const int COLOR_GRAY = 1;
		private const int COLOR_BLACK = 2;

		internal void AddBlock(CodeBlock block)
		{
			Nodes.Add (block);
		}

		internal void UpdateNodeIds()
		{
			int id = 0;

			foreach (var node in Nodes) {
				node.Id = id++;
			}
		}

		/// <summary>
		/// Search the graph starting at startBlock for the node where the paths converge.
		/// </summary>
		/// <returns>The node where some paths converge</returns>
		/// <param name="startBlock">Start block</param>
		/// <param name="targetNode">Will only stop the search when the paths converge into this node</param>
		internal CodeBlock FindNodeJoinPath(CodeBlock startBlock, CodeBlock targetNode = null)
		{
			// Edge case for an empty if statement
			var soleTarget = -1;
			var isSingleTarget = true;
			foreach (var target in startBlock.Targets) {
				if (soleTarget >= 0) {
					if (soleTarget != target.Id) {
						isSingleTarget = false;
						break;
					}
				}

				soleTarget = target.Id;
			}

			if (isSingleTarget) {
				return null;
			}

			var results = new ConcurrentStack<CodeBlock> ();
			var colors = new int[Nodes.Count];
			var currentColor = 1;
			var targetColor = 2;

			// Set the color of the initial block to '1'
			colors [startBlock.Id] = currentColor;

			if (targetNode != null) {
				currentColor++;
			}

			Parallel.ForEach (startBlock.Targets, (currentBlock, loopState) => {
				// Get a new color for this path
				var pathColor = targetNode != null && currentBlock == targetNode ? targetColor : Interlocked.Increment (ref currentColor);

				var queue = new Queue<CodeBlock> ();
				queue.Enqueue (currentBlock);

				while (queue.Count > 0 && !loopState.IsStopped) {
					var block = queue.Dequeue ();

					// Color the block for this path
					var oldColor = Interlocked.CompareExchange (ref colors [block.Id], pathColor, 0);
					if (oldColor != 0 && oldColor != pathColor) {
						// Do not stop the search when the paths converge onto a different
						// path than specified by targetNode
						if (targetNode != null && oldColor != targetColor && pathColor != targetColor) {
							return;
						}

						// Found it, save the block where paths converged and stop all other searches
						results.Push (block);
						loopState.Stop ();
						return;
					}

					// No hits, keep searching down this path
					if (block.Targets != null) {
						foreach (var target in block.Targets) {
							var targetBlockColor = colors [target.Id];
							if (targetBlockColor == 0 || targetBlockColor != pathColor) {
								queue.Enqueue (target);
							}
						}
					}
				}
			});

			// What to do when there are multiple hits? For now simply return the first hit
			CodeBlock commonBlock;
			results.TryPop (out commonBlock);

			return commonBlock;
		}

		private struct Hit
		{
			internal readonly CodeBlock Node;
			internal readonly int TargetIndex;

			internal Hit(CodeBlock node, int index) {
				Node = node;
				TargetIndex = index;
			}
		}

		public List<int[]> Search(CodeBlock start, CodeBlock targetNode)
		{
			var colors = new int[Nodes.Count];
			var hits = new List<Hit> ();

			// Search for the value
			var queue = new Queue<CodeBlock> ();
			queue.Enqueue (start);

			while (queue.Count > 0) {
				var currentNode = queue.Dequeue ();

				if (currentNode.Targets == null) {
					if (targetNode == null) {
						hits.Add (new Hit(currentNode, -1));
					}

					continue;
				}

				for (int index=0; index<currentNode.Targets.Count; index++) {
					var node = currentNode.Targets[index];

					if (node == null) {
						if (targetNode == null) {
							hits.Add (new Hit(currentNode, index));
						}

						continue;
					}

					if (colors [node.Id] == COLOR_WHITE) {
						colors [node.Id] = COLOR_BLACK;

						if (node == targetNode) {
							hits.Add (new Hit(currentNode, index));
						}

						queue.Enqueue (node);
					}
				}
			}

			// Build the paths to each of the search hits
			var paths = new List<int[]> ();

			foreach (var hit in hits) {
				var path = new List<int> ();
				path.Add(hit.TargetIndex);

				CodeBlock node = hit.Node;
				while (node.Parent != null) {
					for (int index = 0; index < node.Parent.Targets.Count; index++) {
						if (node.Parent.Targets [index] == node) {
							path.Add (index);
						}
					}

					node = node.Parent;
				}

				paths.Add (path.ToArray ());
			}

			return paths;
		}

		public static List<List<CodeBlock>> DepthFirstSearch(Graph graph, out int[] outfinishing)
		{
			int time = 0;
			var colors = new int[graph.Nodes.Count];
			var finishing = new int[graph.Nodes.Count];
			var forest = new List<List<CodeBlock>> ();
			List<CodeBlock> currentDfsTree = null;

			// Recursively visits nodes for DFS
			Action<CodeBlock> visit = null;
			visit = new Action<CodeBlock> ((node) => {
				colors [node.Id] = COLOR_GRAY;
				currentDfsTree.Add(node);

				if (node.Targets != null) {
					foreach (var target in node.Targets) {
						if (colors [target.Id] == COLOR_WHITE) {
							visit(target);
						}
					}
				}

				colors [node.Id] = COLOR_BLACK;

				time++;
				finishing [node.Id] = time;
			});

			// Visit all nodes in the graph
			foreach (var node in graph.Nodes) {
				if (colors [node.Id] == COLOR_WHITE) {
					currentDfsTree = new List<CodeBlock> ();
					forest.Add (currentDfsTree);

					visit (node);
				}
			}

			outfinishing = finishing;
			return forest;
		}

		public sealed class ReverseComparer<T> : IComparer<T>
		{
			private readonly IComparer<T> original;

			public ReverseComparer(IComparer<T> original)
			{
				this.original = original;
			}

			public int Compare(T left, T right)
			{
				return original.Compare(right, left);
			}
		}

		public List<List<CodeBlock>> StronglyConnectedComponents()
		{
			int[] finishing;
			DepthFirstSearch (this, out finishing);

			// Index contains pointers to the nodes in decreasing
			// order of finishing
			var index = new SortedDictionary<int,int> (new ReverseComparer<int>(Comparer<int>.Default));
			for (int i=0; i<finishing.Length; i++) {
				index.Add (finishing [i], i);
			}

			// Create the transpose graph in the index order
			var transposedGraph = new Graph ();
			var originalIdToTransposedIndex = new Dictionary<int,int> ();
			foreach (var nodeInfo in index) {
				originalIdToTransposedIndex.Add (nodeInfo.Value, transposedGraph.Nodes.Count);
				transposedGraph.AddBlock (new CodeBlock(nodeInfo.Value, null));
			}

			// Connect the edges
			foreach (var originalNode in this.Nodes) {
				var target = transposedGraph.Nodes[originalIdToTransposedIndex[originalNode.Id]];

				if (originalNode.Targets != null) {
					foreach (var originalTarget in originalNode.Targets) {
						var node = transposedGraph.Nodes [originalIdToTransposedIndex [originalTarget.Id]];
						node.AddTarget (target);
					}
				}
			}

			var dfsForestTransposed = DepthFirstSearch (transposedGraph, out finishing);

			var dfsForest = new List<List<CodeBlock>> ();
			foreach (var transposedTree in dfsForestTransposed) {
				var tree = new List<CodeBlock> (transposedTree.Count);
				foreach (var transposedNode in transposedTree) {
					tree.Add (Nodes [transposedNode.Id]);
				}
				dfsForest.Add (tree);
			}

			return dfsForest;
		}

		// TODO: Lots of opportunities for optimization here
		private bool IsAncestor(CodeBlock start, CodeBlock target)
		{
			var currentBlock = start;

			while (currentBlock != null) {
				if (currentBlock.Id == target.Id) {
					return true;
				}

				currentBlock = currentBlock.Parent;
			}

			return false;
		}

		public void FindLoops()
		{
			var visitedNodes = new BitArray(Nodes.Count);

			var queue = new Queue<CodeBlock> ();
			queue.Enqueue (Root);

			while (queue.Count > 0) {
				var currentNode = queue.Dequeue ();

				// Mark the current node as being visited
				visitedNodes.Set (currentNode.Id, true);

				if (currentNode.Targets != null) {
					foreach (var target in currentNode.Targets) {
						if (!visitedNodes.Get (target.Id)) {
							// Haven't visited this node
							queue.Enqueue (target);
						} else if (IsAncestor (currentNode, target)) {
							target.AddReferrer (currentNode);
						}
					}
				}
			}
		}

		public override string ToString ()
		{
			var builder = new StringBuilder ();

			foreach (var node in Nodes) {
				builder.Append(string.Format("CodeBlock {0} -> {1}\n", node.Id, node.Targets != null ? string.Join (", ", node.Targets) : "<none>"));
			}

			return builder.ToString();
		}
	}

}