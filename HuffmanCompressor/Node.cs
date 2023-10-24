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

        public Node()
        {
            left = null;
            right = null;
            value = default(T);
        }

        public Node<T> GetLeft()
        {
            throw new NotImplementedException();
        }

        public Node<T> GetRight()
        {
            throw new NotImplementedException();
        }
    }
}
