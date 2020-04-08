using System;
using ExchangeAdvisor.Domain.Values;
using ExchangeAdvisor.Domain.Values.Rate;

namespace ExchangeAdvisor.ML.Internal
{
    internal class ModelLearningInput : ModelPredictionInput
    {
        public static string PredictableFeatureName => nameof(Rate);

        public ModelLearningInput(Rate rate) : this(rate.Day, rate.Value) { }

        public ModelLearningInput(DateTime rateDay, float rate) : base(rateDay)
        {
            Rate = rate;
        }

        public float Rate { get; set; }
    }
}
