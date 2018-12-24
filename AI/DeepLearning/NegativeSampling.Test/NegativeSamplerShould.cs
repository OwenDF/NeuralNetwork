using NegativeSampling;
using NeuralNetwork;
using NeuralNetwork.Data;
using System;
using System.Collections.Generic;
using Xunit;

namespace NegativeSampling.Test
{
    public class NegativeSamplerShould
    {
        [Fact]
        public void TrainBasicNetworksSortofWell()
        {
            var input = new Layer("input", 5, new Layer[0]);
            var h1 = new Layer("hidden", 10, new Layer[] { input });
            var output = new Layer("output", 5, new Layer[] { h1 });

            LayerInitialiser.Initialise(new Random(), output);

            var og = new OutputCalculator(output);
            var ns = new NegativeSampler(output, 0.25);

            for (int i = 0; i < 2000; i++)
            {
                ns.NegativeSample(0, 0, false);
                ns.NegativeSample(1, 1, false);
                ns.NegativeSample(2, 2, true);
                ns.NegativeSample(3, 3, false);
                ns.NegativeSample(4, 4, false);
            }

            Assert.True(og.GetResult(0, 0) < 0.05);
            Assert.True(og.GetResult(1, 1) < 0.05);
            Assert.True(og.GetResult(2, 2) > 0.95);
            Assert.True(og.GetResult(3, 3) < 0.05);
            Assert.True(og.GetResult(4, 4) < 0.05);
        }

        [Fact]
        public void TrainComplexNetworksSortofWell()
        {
            var input = new Layer("input", 5, new Layer[0]);
            var h1 = new Layer("hidden1", 10, new Layer[] { input });
            var h2 = new Layer("hidden2", 10, new Layer[] { input });
            var h3 = new Layer("hidden3", 10, new Layer[] { h1, h2 });
            var output = new Layer("output", 5, new Layer[] { h3 });

            LayerInitialiser.Initialise(new Random(), output);

            var og = new OutputCalculator(output);
            var ns = new NegativeSampler(output, 0.25);

            for (int i = 0; i < 2000; i++)
            {
                ns.NegativeSample(0, 0, false);
                ns.NegativeSample(1, 1, false);
                ns.NegativeSample(2, 2, true);
                ns.NegativeSample(3, 3, false);
                ns.NegativeSample(4, 4, false);
            }

            Assert.True(og.GetResult(0, 0) < 0.05);
            Assert.True(og.GetResult(1, 1) < 0.05);
            Assert.True(og.GetResult(2, 2) > 0.95);
            Assert.True(og.GetResult(3, 3) < 0.05);
            Assert.True(og.GetResult(4, 4) < 0.05);
        }

        [Fact]
        public void TrainComplexNetworksOnTheSameTargetSortofWell()
        {
            var input = new Layer("input", 100, new Layer[0]);
            var h1 = new Layer("hidden1", 50, new Layer[] { input });
            var output = new Layer("output", 100, new Layer[] { h1 });

            LayerInitialiser.Initialise(new Random(), output);

            var og = new OutputCalculator(output);
            var ns = new NegativeSampler(output, 0.25);

            for (int i = 0; i < 2000; i++)
            {
                ns.NegativeSample(0, 2, false);
                ns.NegativeSample(1, 2, true);
                ns.NegativeSample(2, 2, false);
                ns.NegativeSample(3, 2, true);
                ns.NegativeSample(4, 2, false);
            }

            Assert.True(og.GetResult(0, 2) < 0.05);
            Assert.True(og.GetResult(1, 2) > 0.95);
            Assert.True(og.GetResult(2, 2) < 0.05);
            Assert.True(og.GetResult(3, 2) > 0.95);
            Assert.True(og.GetResult(4, 2) < 0.05);
        }

        [Fact]
        public void NotChangeUnrelatedWeights()
        {
            var input = new Layer("input", 100, new Layer[0]);
            var h1 = new Layer("hidden1", 50, new Layer[] { input });
            var h2 = new Layer("hidden2", 50, new Layer[] { h1 });
            var h3 = new Layer("hidden3", 50, new Layer[] { h1 });
            var h4 = new Layer("hidden4", 50, new Layer[] { h2, h3 });
            var output = new Layer("output", 100, new Layer[] { h4 });

            LayerInitialiser.Initialise(new Random(), output);

            var initialHiddenWeights = new Dictionary<Node, Weight>[h1.Nodes.Length];
            var initialOutputWeights = new Dictionary<Node, Weight>[output.Nodes.Length];
            for (int i = 0; i < h1.Nodes.Length; i++)
            {
                var dict = new Dictionary<Node, Weight>();
                for (var j = 0; j < input.Nodes.Length; j++)
                {
                    dict.Add(input.Nodes[j], new Weight(h1.Nodes[i].Weights[input.Nodes[j]].Value));
                }
                initialHiddenWeights[i] = dict;
            }
            for (int i = 0; i < output.Nodes.Length; i++)
            {
                var dict = new Dictionary<Node, Weight>();
                for (var j = 0; j < h4.Nodes.Length; j++)
                {
                    dict.Add(h4.Nodes[j], new Weight(output.Nodes[i].Weights[h4.Nodes[j]].Value));
                }
                initialOutputWeights[i] = dict;
            }

            var og = new OutputCalculator(output);
            var ns = new NegativeSampler(output, 0.25);

            for (int i = 0; i < 2000; i++)
            {
                ns.NegativeSample(49, 49, true);
            }

            for (int i = 0; i < h1.Nodes.Length; i++)
            {
                for (var j = 0; j < input.Nodes.Length; j++)
                {
                    if (j != 49)
                    {
                        Assert.Equal(initialHiddenWeights[i][input.Nodes[j]].Value, h1.Nodes[i].Weights[input.Nodes[j]].Value);
                    }
                    else
                    {
                        Assert.NotEqual(initialHiddenWeights[i][input.Nodes[j]].Value, h1.Nodes[i].Weights[input.Nodes[j]].Value);
                    }
                }
            }
            for (int i = 0; i < output.Nodes.Length; i++)
            {
                for (var j = 0; j < h4.Nodes.Length; j++)
                {
                    if (i != 49)
                    {
                        Assert.Equal(initialOutputWeights[i][h4.Nodes[j]].Value, output.Nodes[i].Weights[h4.Nodes[j]].Value);
                    }
                    else
                    {
                        Assert.NotEqual(initialOutputWeights[i][h4.Nodes[j]].Value, output.Nodes[i].Weights[h4.Nodes[j]].Value);
                    }
                }
            }
        }
    }
}