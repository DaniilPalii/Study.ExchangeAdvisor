using System;
using ExchangeAdvisor.Domain.Values;

namespace ExchangeAdvisor.ML.Internal
{
    internal class ModelLearningInput : ModelPredictionInput
    {
        public static string FeatureToPredictName => nameof(Rate);

        public ModelLearningInput(Rate rate) : this(rate.Day, rate.Value) { }

        public ModelLearningInput(DateTime rateDay, float rate) : base(rateDay)
        {
            Rate = rate;
        }

        public float Rate { get; set; }
    }
}
