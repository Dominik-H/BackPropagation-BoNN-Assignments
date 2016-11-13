using System.ComponentModel.DataAnnotations;

namespace BackPropagation.Models
{
    public class InputDataModels
    {
        [Required]
        [Range(0, 100)]
        [Display(Name = "Peecentage of Train Data")]
        public int trainPercent { get; set; }
        //[Required]
        //[Range(0, 100)]
        //[Display(Name = "Peecentage of Test Data")]
        //public int testPercent { get; set; }
        [Required]
        [Display(Name = "Normalize Data")]
        public bool normData { get; set; }
        [Required]
        [Range(1, 5)]
        [Display(Name = "Number of Hidden Layers")]
        public int numHidden { get; set; }
        [Required]
        [Range(0d, 1d)]
        [Display(Name = "Learning Rate Gamma")]
        public double gamma { get; set; }
        [Required]
        [Range(0d, 1d)]
        [Display(Name = "Maximum Error")]
        public double epsilon { get; set; }
        [Required]
        [Range(0d, 1d)]
        [Display(Name = "Momentum")]
        public double momentum { get; set; }
        [Required]
        [Range(0, 20000)]
        [Display(Name = "Max number of iterations")]        
        public int numIter { get; set; }
    }
}