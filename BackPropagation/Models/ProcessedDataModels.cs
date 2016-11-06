using BackPropagation.BackPropagation;

namespace BackPropagation.Models
{
    public class ProcessedDataModels
    {
        public double gamma { get; set; }
        public double epsilon { get; set; }
        public Data TrainData { get; set; }
        public Data TestData { get; set; }
        public Network NeuralNet { get; set; }
    }
}