namespace GranTurismoDataConsole
{
    public class Link
    {
        public int Id { get; set; }
        public string? Href { get; set; }

        public bool Created { get; set; }
        public DataType Type { get; set; }

    }

    public enum DataType
    {
        Car,
        Circuit
    }
}
