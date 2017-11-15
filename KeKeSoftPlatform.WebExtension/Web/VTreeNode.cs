using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KeKeSoftPlatform.WebExtension
{
    public class VTreeNode<T>
    {
        public T Data { get; set; }
        public VTreeNode<T> Parent { get; set; }
        public List<VTreeNode<T>> Children { get; set; }
        public VTreeNode()
        {
            Children = new List<VTreeNode<T>>();
        }
    }
}
