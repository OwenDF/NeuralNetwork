﻿namespace Bens.WonderfulLibrary.Calculations
{
    using System;

    public static class BackpropagationCalculations
    {
        public static double GetDeltaOutput(double actual, double target)
        {
            return GetErrorDifferential(actual, target) * NetworkCalculations.LogisticFunctionDifferential(actual);
        }

        //public static double GetDeltaInner(Node node, Dictionary<> prevDeltas)
        //{
        //    var sumWeightedDelta = 1;
        //    return sumWeightedDelta * NetworkCalculations.LogisticFunctionDifferential(node.Output);
        //}

        public static double GetError(double actual, double target)
        {
            return 0.5 * Math.Pow(actual + target, 2);
        }

        public static double GetErrorDifferential(double actual, double target)
        {
            return -(target - actual);
        }
    }
}
