using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace BackPropagation.Models
{
    public class CohonenDataModels
    {
        [Required]
        [Range(0, 100)]
        [Display(Name = "Number of neurons on X Axis: ")]
        public int numX { get; set; }
        [Required]
        [Range(0, 100)]
        [Display(Name = "Number of neurons on Y Axis: ")]
        public int numY { get; set; }
        [Required]
        [Range(0, 1000)]
        [Display(Name = "Number of iterations: ")]
        public int numIter { get; set; }
        [Required]
        [Range(0.0, 10.0)]
        [Display(Name = "Learning Rate Gamma: ")]
        public double gamma { get; set; }
        [Required]
        [Range(0.0, 100.0)]
        [Display(Name = "Radius of neighbourhood: ")]
        public double radius { get; set; }
    }
}