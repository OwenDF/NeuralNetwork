﻿using System;
using NeuralNetwork.Networks;
using NeuralNetwork.Nodes;

namespace Main
{
    public class Program
    {
        public static void Main()
        {
            var n = new NodeNetwork();

            var layer = new NodeLayer("Input", 20);
            n.AddNodeLayer(layer);

            var inner1 = new NodeLayer("Inner1", 20, new[] {layer});
            n.AddNodeLayer(inner1);

            var inner2 = new NodeLayer("Inner2", 20, new[] {layer});
            n.AddNodeLayer(inner2);

            var output = new NodeLayer("Output", 20, new[] {inner1, inner2});
            n.AddNodeLayer(output);

            n.Initialise(new Random());

            Console.WriteLine(n);
            Console.ReadLine();
        }
    }
}