#r "nuget: Microsoft.ML.Probabilistic"
#r "nuget: Microsoft.ML.Probabilistic.Compiler"
#r "nuget: Microsoft.ML.Probabilistic.Learners"

using System.Runtime.CompilerServices;
using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.ML.Probabilistic.Models;
using Microsoft.ML.Probabilistic.Distributions;


Variable<bool> rainVariable = Variable.Bernoulli(0.2).Named("Rain");
Variable<bool> sprinklerVariable = Variable<bool>.New<bool>().Named("Sprinkler");
Variable<bool> wetGrassVariable = Variable<bool>.New<bool>().Named("wetGrass");

using (Variable.If(rainVariable))
{
    sprinklerVariable = Variable.Bernoulli(0.01);   
}
using (Variable.IfNot(rainVariable))
{
    sprinklerVariable = Variable.Bernoulli(0.6); 
}

InferenceEngine engine = new InferenceEngine();
engine.Infer(sprinklerVariable);


var cause = engine.Infer<Bernoulli>(sprinklerVariable);
Console.WriteLine($"Sprinkler: {cause.GetProbTrue()}");