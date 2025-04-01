namespace HuffmanCompressorLib
{
    /// <summary>
    /// Represents a node in a binary tree.
    /// </summary>
    /// <typeparam name="T">Data type.</typeparam>
    internal class Node<T>
    {
        private Node<T>? left;
        private Node<T>? right;
        private T? value;

        /// <summary>
        /// Creates a new instance of <see cref="Node{T}"/> class.
        /// </summary>
        /// <param name="value">Value of the node</param>
        public Node(T value)
        {
            this.left = null;
            this.right = null;
            this.value = value;
        }

        /// <summary>
        /// Creates a new instance of <see cref="Node{T}"/> class.
        /// </summary>
        /// <param name="left">Left node.</param>
        /// <param name="right">Right node.</param>
        public Node(Node<T> left, Node<T> right)
        {
            this.left = left;
            this.right = right;
        }

        /// <summary>
        /// Gets the left node.
        /// </summary>
        /// <returns>Left node.</returns>
        public Node<T>? GetLeft()
        {
            return this.left;
        }

        /// <summary>
        /// Gets the right node.
        /// </summary>
        /// <returns>Right node.</returns>
        public Node<T>? GetRight()
        {
            return this.right;
        }

        /// <summary>
        /// Gets a value indicating whether the node is a leaf node.
        /// </summary>
        public bool IsLeafNode
        {
            get
            {
                return left == null && right == null;
            }
        }

        /// <summary>
        /// Gets the value of the node.
        /// </summary>
        public T? Value
        { 
            get
            {
                return value;
            }
         }
    }
}
