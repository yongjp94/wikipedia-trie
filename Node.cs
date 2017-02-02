using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Node
{
    public const char Root = ' ';
    public const char Eow = '$';

    public Dictionary<int, Node> Children { get; private set; }
    public char Letter { get; set; }
    public int NumOfPref { get; set; }

    public Node() { }

    public Node(char ch)
    {
        this.Letter = ch;
    }

    public Node this[char index]
    {
        get { return (Node)Children[index]; }
    }

    public bool ContainsKey(char key)
    {
        if (Children == null) return false;
        return Children.ContainsKey(key);
    }

    public Node AddChild(char letter)
    {
        if (Children == null)
        {
            Children = new Dictionary<int, Node>();
        }
        if (!Children.ContainsKey(letter))
        {
            var node = letter != Eow ? new Node(letter) : null;
            Children.Add(letter, node);
            return node;
        }
        return (Node)Children[letter];
    }

    public override string ToString()
    {
        return this.Letter.ToString();
    }
}