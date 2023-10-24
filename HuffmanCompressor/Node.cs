using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HuffmanCompressor
{
    internal class Node<T>
    {
        private Node<T> left;
        private Node<T> right;
        private T? value;

        public Node(T value)
        {
            this.left = null;
            this.right = null;
            this.value = value;
        }

        public Node(Node<T> left, Node<T> right)
        {
            this.left = left;
            this.right = right;
        }

        public Node<T> GetLeft()
        {
            return this.left;
        }

        public Node<T> GetRight()
        {
            return this.right;
        }

        public bool IsLeafNode
        {
            get
            {
                return left == null && right == null;
            }
        }

        public T? Value
        { 
            get
            {
                return value;
            }
         }
    }
}
