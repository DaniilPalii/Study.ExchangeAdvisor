using System;
using System.Collections.Generic;
using System.Linq;
using ExchangeAdvisor.Domain.Values;
using Microsoft.ML;

namespace ExchangeAdvisor.ML.Internal
{
    internal class ModelBuilder
    {
        public Model Build(ICollection<Rate> historicalRates)
        {
            var trainingData = ToTrainingData(historicalRates);
            var trainingPipeline = BuildTrainingPipeline();
            mlContext.Regression.CrossValidate(trainingData, trainingPipeline, numberOfFolds: 5, ModelLearningInput.PredictableFeatureName);

            var model = trainingPipeline.Fit(trainingData);
            var modelPredictionEngine = mlContext.Model.CreatePredictionEngine<ModelPredictionInput, ModelOutput>(model);

            return new Model(modelPredictionEngine);
        }

        private IDataView ToTrainingData(ICollection<Rate> historicalRates)
        {
            if (historicalRates.Count < 2)
                throw new ArgumentException("Needs at least 2 historical rates");

            var firstRateCurrencyPair = historicalRates.First().CurrencyPair;
            if (historicalRates.Skip(1).Any(r => r.CurrencyPair != firstRateCurrencyPair))
                throw new ArgumentException("All rates should have same currency pair");

            var modelLearningInputs = historicalRates.OrderBy(r => r.Day)
                .Select(r => new ModelLearningInput(r));

            return mlContext.Data.LoadFromEnumerable(modelLearningInputs);
        }

        private IEstimator<ITransformer> BuildTrainingPipeline()
        {
            var dataProcessPipeline = mlContext.Transforms.Concatenate(FeaturesColumnName, ModelPredictionInput.InputFeatureNames);
            var trainer = mlContext.Regression.Trainers.FastTree(ModelLearningInput.PredictableFeatureName, FeaturesColumnName);

            return dataProcessPipeline.Append(trainer);
        }

        private const string FeaturesColumnName = "Features";

        private readonly MLContext mlContext = new MLContext();
    }
}
