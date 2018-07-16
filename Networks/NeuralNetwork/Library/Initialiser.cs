﻿using System;
using NeuralNetwork.Data;

namespace NeuralNetwork.Library
{
    public class Initialiser
    {
        /// <summary>
        ///     Initialises this Node with random weights.
        /// </summary>
        /// <param name="rand"></param>
        /// <param name="node"></param>
        public static void Initialise(Random rand, Node node)
        {
            if (node == null) return;
            foreach (var weightArr in node.Weights)
                for (var j = 0; j < weightArr.Length; j++)
                    weightArr[j] = (double)rand.Next(2000000) / 1000000 - 1;
            for (var i = 0; i < node.BiasWeights.Length; i++)
                node.BiasWeights[i] = (double)rand.Next(2000000) / 1000000 - 1;
        }

        /// <summary>
        ///     Initialises each Node in Nodes with random weights.
        /// </summary>
        /// <param name="rand"></param>
        /// <param name="nodeGroup"></param>
        public static void Initialise(Random rand, NodeLayer nodeGroup)
        {
            foreach (var node in nodeGroup.Nodes)
            {
                Initialise(rand, node);
            }
            foreach (var nodeGroupPrev in nodeGroup.PreviousGroups)
            {
                Initialise(rand, nodeGroupPrev);
            }
        }
    }
}