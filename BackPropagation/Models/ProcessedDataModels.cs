using BackPropagation.BackPropagation;

namespace BackPropagation.Models
{
    public class ProcessedDataModels
    {
        public double gamma { get; set; }
        public double epsilon { get; set; }
        public double momentum { get; set; }
        public int maxIter { get; set; }
        public Network NeuralNet { get; set; }
    }
}