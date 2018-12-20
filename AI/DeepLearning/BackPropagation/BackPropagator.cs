﻿namespace BackPropagation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using AI.Calculations;
    using NeuralNetwork;
    using NeuralNetwork.Data;
    using NeuralNetwork.Library.Extensions;

    public class BackPropagator
    {
        private readonly OutputCalculator _outputCalculator;
        private readonly Func<double, double> _learningRateModifier;
        private readonly double _momentumFactor;
        private readonly Layer _momentumDeltaHolder;

        private double _learningRate;

        public BackPropagator(Layer outputLayer, double learningRate, Func<double, double> learningAction = null, double momentum = 0)
        {
            _outputCalculator = new OutputCalculator(outputLayer);
            _learningRate = learningRate;
            _learningRateModifier = learningAction;

            _momentumFactor = momentum;
            _momentumDeltaHolder = outputLayer.CloneWithNodeReferences();
        }

        public void BackPropagate(double[] inputs, double?[] targetOutputs)
        {
            var currentLayer = _outputCalculator.OutputLayer;
            var currentOutputs = _outputCalculator.GetResults(inputs);

            var backwardsPassDeltas = UpdateOutputLayer(currentLayer, currentOutputs, targetOutputs);

            for (var i = 0; i < currentLayer.PreviousLayers.Length; i++)
            {
                RecurseBackpropagation(currentLayer.PreviousLayers[i], backwardsPassDeltas, _momentumDeltaHolder.PreviousLayers[i]);
            }

            if (_learningRateModifier != null)
            {
                _learningRate = _learningRateModifier(_learningRate);
            }
        }

        private void RecurseBackpropagation(Layer layer, Dictionary<Node, double> backwardsPassDeltas, Layer momentumLayer)
        {
            if (!layer.PreviousLayers.Any())
            {
                // input case
                return;
            }

            var deltas = new Dictionary<Node, double>();
            for (var i = 0; i < layer.Nodes.Length; i++)
            {
                var node = layer.Nodes[i];
                var sumDeltaWeights = (double)0;
                foreach (var backPassNode in backwardsPassDeltas.Keys)
                {
                    sumDeltaWeights += backwardsPassDeltas[backPassNode] * backPassNode.Weights[node].Value;
                }
                var delta = sumDeltaWeights * NetworkCalculations.LogisticFunctionDifferential(node.Output);
                deltas.Add(node, delta);

                foreach (var prevNode in node.Weights.Keys.ToList())
                {
                    UpdateNodeWeight(node, prevNode, delta, momentumLayer.Nodes[i]);
                }

                foreach (var prevLayer in node.BiasWeights.Keys.ToList())
                {
                    UpdateBiasNodeWeight(node, prevLayer, delta, momentumLayer.Nodes[i]);
                }
            }

            for (var i = 0; i < layer.PreviousLayers.Length; i++)
            {
                RecurseBackpropagation(layer.PreviousLayers[i], deltas, momentumLayer.PreviousLayers[i]);
            }
        }

        private Dictionary<Node, double> UpdateOutputLayer(Layer outputLayer, double[] currentOutputs, double?[] targetOutputs)
        {
            var deltas = new Dictionary<Node, double>();

            for (var i = 0; i < outputLayer.Nodes.Length; i++)
            {
                if (!targetOutputs[i].HasValue) continue;
                var node = outputLayer.Nodes[i];
                var delta = BackpropagationCalculations.GetDeltaOutput(currentOutputs[i], targetOutputs[i].Value);
                deltas.Add(node, delta);
                foreach (var prevNode in node.Weights.Keys.ToList())
                {
                    UpdateNodeWeight(node, prevNode, delta, _momentumDeltaHolder.Nodes[i]);
                }

                foreach (var prevLayer in node.BiasWeights.Keys.ToList())
                {
                    UpdateBiasNodeWeight(node, prevLayer, delta, _momentumDeltaHolder.Nodes[i]);
                }
            }

            return deltas;
        }

        private void UpdateNodeWeight(Node node, Node prevNode, double delta, Node momentumNode)
        {
            var change = -(_learningRate * delta * prevNode.Output);
            node.Weights[prevNode].Value += change;

            // apply momentum
            node.Weights[prevNode].Value += _momentumFactor * momentumNode.Weights[prevNode].Value;
            momentumNode.Weights[prevNode].Value = change + _momentumFactor * momentumNode.Weights[prevNode].Value;
        }

        private void UpdateBiasNodeWeight(Node node, Layer prevLayer, double delta, Node momentumNode)
        {
            var change = -(_learningRate * delta);
            node.BiasWeights[prevLayer].Value += change;

            // apply momentum
            node.BiasWeights[prevLayer].Value += _momentumFactor * momentumNode.BiasWeights[prevLayer].Value;
            momentumNode.BiasWeights[prevLayer].Value = change + _momentumFactor * momentumNode.BiasWeights[prevLayer].Value;
        }
    }
}
