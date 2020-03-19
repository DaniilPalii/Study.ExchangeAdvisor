using System.Linq;
using ExchangeAdvisor.Domain.Services;
using ExchangeAdvisor.Domain.Values;
using Microsoft.ML;

namespace ExchangeAdvisor.ML.Internal
{
    internal class ModelBuilder
    {
        public ModelBuilder(IHistoricalRatesRepository historicalRatesRepository)
        {
            this.historicalRatesRepository = historicalRatesRepository;
        }

        public Model Build(CurrencyPair currencyPair)
        {
            var trainingData = GetTrainingData(currencyPair);
            var trainingPipeline = BuildTrainingPipeline();
            mlContext.Regression.CrossValidate(trainingData, trainingPipeline, numberOfFolds: 5, ModelLearningInput.FeatureToPredictName);

            var model = trainingPipeline.Fit(trainingData);
            var modelPredictionEngine = mlContext.Model.CreatePredictionEngine<ModelPredictionInput, ModelOutput>(model);

            return new Model(modelPredictionEngine);
        }

        private IDataView GetTrainingData(CurrencyPair currencyPair)
        {
            var modelLearningInputs = historicalRatesRepository.Get(DateRange.FromMinDate().UntilToday(), currencyPair)
                .Select(r => new ModelLearningInput(r));

            return mlContext.Data.LoadFromEnumerable(modelLearningInputs);
        }

        private IEstimator<ITransformer> BuildTrainingPipeline()
        {
            var dataProcessPipeline = mlContext.Transforms.Concatenate(FeaturesColumnName, ModelPredictionInput.InputFeatureNames);
            var trainer = mlContext.Regression.Trainers.FastTree(labelColumnName: ModelLearningInput.FeatureToPredictName, FeaturesColumnName);

            return dataProcessPipeline.Append(trainer);
        }

        private const string FeaturesColumnName = "Features";

        private readonly MLContext mlContext = new MLContext();
        private readonly IHistoricalRatesRepository historicalRatesRepository;
    }
}
