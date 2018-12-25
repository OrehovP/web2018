using RDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Movies
{
    public class RCaller
    {
        private REngine engine;

        public RCaller(string scriptPath)
        {
            REngine.SetEnvironmentVariables(
                @"C:/Program Files/R/R-3.4.4/bin/x64",
                @"C:/Program Files/R/R-3.4.4"
            );
            engine = REngine.GetInstance();
            engine.Initialize();
            engine.Evaluate($"source('{scriptPath}')");
        }

        public SymbolicExpression GetPrediction(int budget, double vote)
        {
            var expression = $"revenue.prediction({budget}, {vote})";
            return engine.Evaluate(expression);
        }
    }
}
