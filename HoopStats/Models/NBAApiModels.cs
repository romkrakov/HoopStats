namespace HoopStats.Models
{
    public class NBAApiResponse
    {
        public List<NBAResultSet> ResultSets { get; set; } = new List<NBAResultSet>();
    }

    public class NBAResultSet
    {
        public string Name { get; set; } = "";
        public string[] Headers { get; set; } = Array.Empty<string>();
        public object[][] RowSet { get; set; } = Array.Empty<object[]>();
    }
}