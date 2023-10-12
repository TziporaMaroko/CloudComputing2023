using Microsoft.ML.Data;

namespace ZeldaWebsite.Models
{
    public class FlavourPrediction
    {
        
        [ColumnName("PredictedLabel")]
        public string Flavor { get; set; }

        public float Score { get; set; }
    }
}
