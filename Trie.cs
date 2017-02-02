using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Trie
{
    public class Trie
    {
        private const int MAX_RESULT = 10;
        private Node root;
        
        public Trie()
        {
            root = new Node { Letter = Node.Root };
        }
        
        public void Add(string word)
        {
            word = word.ToLower() + Node.Eow;
            var currentNode = root;
            foreach(var c in word)
            {
                currentNode = currentNode.AddChild(c);
            }
        }

        public List<String> Search(string input)
        {
            input = input.ToLower();
            List<String> result = new List<String>();

            Search(root, result, "", input);
            return result;
        }

        private static void Search(Node node, List<String> rtn, string letters, string prefix)
        {
            if (rtn.Count >= MAX_RESULT)
            {
                return;
            }

            if (node == null)
            {
                if (!rtn.Contains(letters.Trim())) rtn.Add(letters.Trim());
                return;
            }

            letters += node.Letter.ToString();

            if (prefix.Length > 0)
            {
                if (node.ContainsKey(prefix[0]))
                {
                    Search(node[prefix[0]], rtn, letters, prefix.Remove(0, 1));
                }
            }
            else
            {
                if (node.Children != null)
                {
                    foreach (char key in node.Children.Keys)
                    {
                        Search(node[key], rtn, letters, prefix);
                    }
                }
            }
        }
    }
}
