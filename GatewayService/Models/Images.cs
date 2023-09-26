namespace GatewayAPI.Models
{
    public class Result2
    {
        public List<Tag> tags { get; set; }
    }

    public class RootImages
    {
        public Result2 result { get; set; }
        public Status status { get; set; }
    }

    public class Status
    {
        public string text { get; set; }
        public string type { get; set; }
    }

    public class Tag
    {
        public double confidence { get; set; }
        public Tag2 tag { get; set; }
    }

    public class Tag2
    {
        public string en { get; set; }
    }


}
