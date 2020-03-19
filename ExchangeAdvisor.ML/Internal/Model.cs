using System.Collections.Generic;
using Microsoft.ML;

namespace ExchangeAdvisor.ML.Internal
{
    internal class Model
    {
        public Model(PredictionEngine<ModelPredictionInput, ModelOutput> predictionEngine)
        {
            this.predictionEngine = predictionEngine;
        }

        public IEnumerable<(ModelPredictionInput, ModelOutput)> Predict(IEnumerable<ModelPredictionInput> inputs)
        {
            foreach (var input in inputs)
            {
                var prediction = Predict(input);

                yield return (input, prediction);
            }
        }

        public ModelOutput Predict(ModelPredictionInput input) => predictionEngine.Predict(input);

        private readonly PredictionEngine<ModelPredictionInput, ModelOutput> predictionEngine;
    }
}
