using System;
using System.Linq;
using ExchangeAdvisor.Domain.Helpers;
using ExchangeAdvisor.Domain.Values.Rate;
using Microsoft.ML;

namespace ExchangeAdvisor.ML.Internal
{
    internal class ModelBuilder
    {
        public Model Build(RateCollectionBase history)
        {
            var trainingData = ToTrainingData(history);
            var trainingPipeline = BuildTrainingPipeline();
            mlContext.Regression.CrossValidate(
                trainingData,
                trainingPipeline,
                numberOfFolds: 5,
                ModelLearningInput.PredictableFeatureName);

            var model = trainingPipeline.Fit(trainingData);
            var predictionEngine = mlContext.Model.CreatePredictionEngine<ModelPredictionInput, ModelOutput>(model);

            return new Model(predictionEngine);
        }

        private IDataView ToTrainingData(RateCollectionBase history)
        {
            if (!history.Rates.HasAtLeast(count: 2))
                throw new ArgumentException(message: "Needs at least 2 historical rates");

            var modelLearningInputs = history.Rates.OrderBy(r => r.Day).Select(r => new ModelLearningInput(r));

            return mlContext.Data.LoadFromEnumerable(modelLearningInputs);
        }

        private IEstimator<ITransformer> BuildTrainingPipeline()
        {
            var dataProcessPipeline = mlContext.Transforms.Concatenate(
                FeaturesColumnName,
                ModelPredictionInput.InputFeatureNames);
            var trainer = mlContext.Regression.Trainers.FastTree(ModelLearningInput.PredictableFeatureName);

            return dataProcessPipeline.Append(trainer);
        }

        private readonly MLContext mlContext = new MLContext();
        private const string FeaturesColumnName = "Features";
    }
}