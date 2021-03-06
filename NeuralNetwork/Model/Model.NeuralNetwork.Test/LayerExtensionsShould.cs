﻿using Model.NeuralNetwork.Models;

namespace Model.NeuralNetwork.Test
{
    using System;
    using System.Linq;
    using Xunit;
    using Xunit.Abstractions;

    public class LayerExtensionsShould
    {
        private readonly ITestOutputHelper _testOutputHelper;

        private readonly Layer _input;
        private readonly Layer _hidden1;
        private readonly Layer _hidden2;
        private readonly Layer _output;

        public LayerExtensionsShould(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;

            _input = new Layer("input", 5, new Layer[0]);
            _hidden1 = new Layer("hidden1", 10, new[] { _input });
            _hidden2 = new Layer("hidden2", 15, new[] { _input });
            _output = new Layer("output", 20, new[] { _hidden1, _hidden2 });
        }

        [Fact]
        public void DeepCopyCorrectly()
        {
            _output.Initialise(new Random());
            _output.PopulateAllOutputs(new[] { 0.1, 0.2, 0.3, 0.4, 0.5 });
            _testOutputHelper.WriteLine(_output.ToString(true));

            var copiedOutput = _output.DeepCopy();
            var copiedHidden1 = copiedOutput.PreviousLayers.FirstOrDefault(l => l.Name == "hidden1");
            var copiedHidden2 = copiedOutput.PreviousLayers.FirstOrDefault(l => l.Name == "hidden2");
            var copiedInput = copiedHidden1.PreviousLayers.Single();

            // input
            for (var i = 0; i < _input.Nodes.Length; i++)
            {
                Assert.Equal(_input.Nodes[i].Output, copiedInput.Nodes[i].Output);
            }
            // hidden 1
            for (var i = 0; i < _hidden1.Nodes.Length; i++)
            {
                Assert.Equal(_hidden1.Nodes[i].Output, copiedHidden1.Nodes[i].Output);
                for (var j = 0; j < _hidden1.Nodes[i].Weights.Count; j++)
                {
                    Assert.Equal(_hidden1.Nodes[i].Weights.Values.ToArray()[j].Value, copiedHidden1.Nodes[i].Weights.Values.ToArray()[j].Value);
                }
                for (var j = 0; j < _hidden1.Nodes[i].BiasWeights.Count; j++)
                {
                    Assert.Equal(_hidden1.Nodes[i].BiasWeights.Values.ToArray()[j].Value, copiedHidden1.Nodes[i].BiasWeights.Values.ToArray()[j].Value);
                }
            }
            // hidden 2
            for (var i = 0; i < _hidden2.Nodes.Length; i++)
            {
                Assert.Equal(_hidden2.Nodes[i].Output, copiedHidden2.Nodes[i].Output);
                for (var j = 0; j < _hidden2.Nodes[i].Weights.Count; j++)
                {
                    Assert.Equal(_hidden2.Nodes[i].Weights.Values.ToArray()[j].Value, copiedHidden2.Nodes[i].Weights.Values.ToArray()[j].Value);
                }
                for (var j = 0; j < _hidden2.Nodes[i].BiasWeights.Count; j++)
                {
                    Assert.Equal(_hidden2.Nodes[i].BiasWeights.Values.ToArray()[j].Value, copiedHidden2.Nodes[i].BiasWeights.Values.ToArray()[j].Value);
                }
            }
            // output
            for (var i = 0; i < _output.Nodes.Length; i++)
            {
                Assert.Equal(_output.Nodes[i].Output, copiedOutput.Nodes[i].Output);
                for (var j = 0; j < _output.Nodes[i].Weights.Count; j++)
                {
                    Assert.Equal(_output.Nodes[i].Weights.Values.ToArray()[j].Value, copiedOutput.Nodes[i].Weights.Values.ToArray()[j].Value);
                }
                for (var j = 0; j < _output.Nodes[i].BiasWeights.Count; j++)
                {
                    Assert.Equal(_output.Nodes[i].BiasWeights.Values.ToArray()[j].Value, copiedOutput.Nodes[i].BiasWeights.Values.ToArray()[j].Value);
                }
            }
        }

        [Fact]
        public void CloneWithSameWeightKeyReferencesCorrectly()
        {
            var random = new Random();
            var inputs = new[] { 0.1, 0.2, 0.3, 0.4, 0.5 };

            _output.Initialise(random);
            _output.PopulateAllOutputs(inputs);
            _testOutputHelper.WriteLine(_output.ToString(true));

            var copiedOutput = _output.CloneWithSameWeightKeyReferences();
            copiedOutput.Initialise(random);
            copiedOutput.PopulateAllOutputs(inputs);
            _testOutputHelper.WriteLine(copiedOutput.ToString(true));

            var copiedHidden1 = copiedOutput.PreviousLayers.FirstOrDefault(l => l.Name == "hidden1_CLONE");
            var copiedHidden2 = copiedOutput.PreviousLayers.FirstOrDefault(l => l.Name == "hidden2_CLONE");
            var copiedInput = copiedHidden1.PreviousLayers.Single();

            // input
            for (var i = 0; i < _input.Nodes.Length; i++)
            {
                // inputs will be equal (as we supply them!)
                Assert.Equal(_input.Nodes[i].Output, copiedInput.Nodes[i].Output);
            }
            // hidden 1
            for (var i = 0; i < _hidden1.Nodes.Length; i++)
            {
                Assert.NotEqual(_hidden1.Nodes[i].Output, copiedHidden1.Nodes[i].Output);
                for (var j = 0; j < _hidden1.Nodes[i].Weights.Count; j++)
                {
                    Assert.Same(_hidden1.Nodes[i].Weights.Keys.ToArray()[j], copiedHidden1.Nodes[i].Weights.Keys.ToArray()[j]);
                    Assert.NotEqual(_hidden1.Nodes[i].Weights.Values.ToArray()[j].Value, copiedHidden1.Nodes[i].Weights.Values.ToArray()[j].Value);
                }
                for (var j = 0; j < _hidden1.Nodes[i].BiasWeights.Count; j++)
                {
                    Assert.Same(_hidden1.Nodes[i].BiasWeights.Keys.ToArray()[j], copiedHidden1.Nodes[i].BiasWeights.Keys.ToArray()[j]);
                    Assert.NotEqual(_hidden1.Nodes[i].BiasWeights.Values.ToArray()[j].Value, copiedHidden1.Nodes[i].BiasWeights.Values.ToArray()[j].Value);
                }
            }
            // hidden 2
            for (var i = 0; i < _hidden2.Nodes.Length; i++)
            {
                Assert.NotEqual(_hidden2.Nodes[i].Output, copiedHidden2.Nodes[i].Output);
                for (var j = 0; j < _hidden2.Nodes[i].Weights.Count; j++)
                {
                    Assert.Same(_hidden2.Nodes[i].Weights.Keys.ToArray()[j], copiedHidden2.Nodes[i].Weights.Keys.ToArray()[j]);
                    Assert.NotEqual(_hidden2.Nodes[i].Weights.Values.ToArray()[j].Value, copiedHidden2.Nodes[i].Weights.Values.ToArray()[j].Value);
                }
                for (var j = 0; j < _hidden2.Nodes[i].BiasWeights.Count; j++)
                {
                    Assert.Same(_hidden2.Nodes[i].BiasWeights.Keys.ToArray()[j], copiedHidden2.Nodes[i].BiasWeights.Keys.ToArray()[j]);
                    Assert.NotEqual(_hidden2.Nodes[i].BiasWeights.Values.ToArray()[j].Value, copiedHidden2.Nodes[i].BiasWeights.Values.ToArray()[j].Value);
                }
            }
            // output
            for (var i = 0; i < _output.Nodes.Length; i++)
            {
                Assert.NotEqual(_output.Nodes[i].Output, copiedOutput.Nodes[i].Output);
                for (var j = 0; j < _output.Nodes[i].Weights.Count; j++)
                {
                    Assert.Same(_output.Nodes[i].Weights.Keys.ToArray()[j], copiedOutput.Nodes[i].Weights.Keys.ToArray()[j]);
                    Assert.NotEqual(_output.Nodes[i].Weights.Values.ToArray()[j].Value, copiedOutput.Nodes[i].Weights.Values.ToArray()[j].Value);
                }
                for (var j = 0; j < _output.Nodes[i].BiasWeights.Count; j++)
                {
                    Assert.Same(_output.Nodes[i].BiasWeights.Keys.ToArray()[j], copiedOutput.Nodes[i].BiasWeights.Keys.ToArray()[j]);
                    Assert.NotEqual(_output.Nodes[i].BiasWeights.Values.ToArray()[j].Value, copiedOutput.Nodes[i].BiasWeights.Values.ToArray()[j].Value);
                }
            }
        }

        [Fact]
        public void CloneWithSameWeightValueReferencesCorrectly()
        {
            var random = new Random();
            var inputs = new[] { 0.1, 0.2, 0.3, 0.4, 0.5 };

            _output.Initialise(random);
            _output.PopulateAllOutputs(inputs);
            _testOutputHelper.WriteLine(_output.ToString(true));

            var copiedOutput = _output.CloneWithSameWeightValueReferences();
            copiedOutput.Initialise(random);
            copiedOutput.PopulateAllOutputs(inputs);
            _testOutputHelper.WriteLine(copiedOutput.ToString(true));

            var copiedHidden1 = copiedOutput.PreviousLayers.FirstOrDefault(l => l.Name == "hidden1_CLONE");
            var copiedHidden2 = copiedOutput.PreviousLayers.FirstOrDefault(l => l.Name == "hidden2_CLONE");
            var copiedInput = copiedHidden1.PreviousLayers.Single();

            // input
            for (var i = 0; i < _input.Nodes.Length; i++)
            {
                // inputs will be equal (as we supply them!)
                Assert.Equal(_input.Nodes[i].Output, copiedInput.Nodes[i].Output);
            }
            // hidden 1
            for (var i = 0; i < _hidden1.Nodes.Length; i++)
            {
                Assert.NotEqual(_hidden1.Nodes[i].Output, copiedHidden1.Nodes[i].Output);
                for (var j = 0; j < _hidden1.Nodes[i].Weights.Count; j++)
                {
                    Assert.NotSame(_hidden1.Nodes[i].Weights.Keys.ToArray()[j], copiedHidden1.Nodes[i].Weights.Keys.ToArray()[j]);
                    Assert.Equal(_hidden1.Nodes[i].Weights.Values.ToArray()[j].Value, copiedHidden1.Nodes[i].Weights.Values.ToArray()[j].Value);
                }
                for (var j = 0; j < _hidden1.Nodes[i].BiasWeights.Count; j++)
                {
                    Assert.NotSame(_hidden1.Nodes[i].BiasWeights.Keys.ToArray()[j], copiedHidden1.Nodes[i].BiasWeights.Keys.ToArray()[j]);
                    Assert.Equal(_hidden1.Nodes[i].BiasWeights.Values.ToArray()[j].Value, copiedHidden1.Nodes[i].BiasWeights.Values.ToArray()[j].Value);
                }
            }
            // hidden 2
            for (var i = 0; i < _hidden2.Nodes.Length; i++)
            {
                Assert.NotEqual(_hidden2.Nodes[i].Output, copiedHidden2.Nodes[i].Output);
                for (var j = 0; j < _hidden2.Nodes[i].Weights.Count; j++)
                {
                    Assert.NotSame(_hidden2.Nodes[i].Weights.Keys.ToArray()[j], copiedHidden2.Nodes[i].Weights.Keys.ToArray()[j]);
                    Assert.Equal(_hidden2.Nodes[i].Weights.Values.ToArray()[j].Value, copiedHidden2.Nodes[i].Weights.Values.ToArray()[j].Value);
                }
                for (var j = 0; j < _hidden2.Nodes[i].BiasWeights.Count; j++)
                {
                    Assert.NotSame(_hidden2.Nodes[i].BiasWeights.Keys.ToArray()[j], copiedHidden2.Nodes[i].BiasWeights.Keys.ToArray()[j]);
                    Assert.Equal(_hidden2.Nodes[i].BiasWeights.Values.ToArray()[j].Value, copiedHidden2.Nodes[i].BiasWeights.Values.ToArray()[j].Value);
                }
            }
            // output
            for (var i = 0; i < _output.Nodes.Length; i++)
            {
                Assert.NotEqual(_output.Nodes[i].Output, copiedOutput.Nodes[i].Output);
                for (var j = 0; j < _output.Nodes[i].Weights.Count; j++)
                {
                    Assert.NotSame(_output.Nodes[i].Weights.Keys.ToArray()[j], copiedOutput.Nodes[i].Weights.Keys.ToArray()[j]);
                    Assert.Equal(_output.Nodes[i].Weights.Values.ToArray()[j].Value, copiedOutput.Nodes[i].Weights.Values.ToArray()[j].Value);
                }
                for (var j = 0; j < _output.Nodes[i].BiasWeights.Count; j++)
                {
                    Assert.NotSame(_output.Nodes[i].BiasWeights.Keys.ToArray()[j], copiedOutput.Nodes[i].BiasWeights.Keys.ToArray()[j]);
                    Assert.Equal(_output.Nodes[i].BiasWeights.Values.ToArray()[j].Value, copiedOutput.Nodes[i].BiasWeights.Values.ToArray()[j].Value);
                }
            }
        }

        [Fact(Skip = "Debug only")]
        public void SaveCorrectly()
        {
            // TODO
        }
    }
}
