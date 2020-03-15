using System.Collections.Generic;
using Microsoft.ML;

namespace ExchangeAdvisor.ML.Internal
{
    internal class Model
    {
        public Model(PredictionEngine<ModelInput, ModelOutput> predictionEngine)
        {
            this.predictionEngine = predictionEngine;
        }

        public IEnumerable<(ModelInput, ModelOutput)> Predict(IEnumerable<ModelInput> inputs)
        {
            foreach (var input in inputs)
            {
                var prediction = Predict(input);

                yield return (input, prediction);
            }
        }

        public ModelOutput Predict(ModelInput input) => predictionEngine.Predict(input);

        private readonly PredictionEngine<ModelInput, ModelOutput> predictionEngine;
    }
}
