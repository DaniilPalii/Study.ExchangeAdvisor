using System.Linq;
using Microsoft.ML;

namespace ExchangeAdvisor.ML.Internal
{
    internal class ModelBuilder
    {
        public Model Build()
        {
            var trainingDataView = mlContext.Data.LoadFromTextFile<ModelInput>(
                path: @"C:\Code\ExchangeAdvisor\MLSource\Exchange rate history.tsv",
                hasHeader: true,
                separatorChar: '\t',
                allowQuoting: true,
                allowSparse: false);

            var trainingPipeline = BuildTrainingPipeline();
            mlContext.Regression.CrossValidate(trainingDataView, trainingPipeline, numberOfFolds: 5, labelColumnName: "Rate");

            var model = trainingPipeline.Fit(trainingDataView);
            var modelPredictionEngine = mlContext.Model.CreatePredictionEngine<ModelInput, ModelOutput>(model);

            return new Model(modelPredictionEngine);
        }

        private IEstimator<ITransformer> BuildTrainingPipeline()
        {
            var dataProcessPipeline = mlContext.Transforms.Categorical.OneHotEncoding(
                new[]
                {
                    new InputOutputColumnPair("Base currency", "Base currency"),
                    new InputOutputColumnPair("Comparing currency", "Comparing currency")
                })
                .Append(mlContext.Transforms.Concatenate(
                    "Features",
                    new[]
                    {
                        "Base currency",
                        "Comparing currency",
                        "Year",
                        "Month",
                        "Day",
                        "Absolute day number",
                        "Day of week",
                        "Day of year"
                    }));
            var trainer = mlContext.Regression.Trainers.FastTree(labelColumnName: "Rate", featureColumnName: "Features");

            return dataProcessPipeline.Append(trainer);
        }

        private readonly MLContext mlContext = new MLContext(seed: 1);
    }
}
