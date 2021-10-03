using System.Collections.Generic;

namespace NextDB.Parser
{
    public class ArmaValue
    {
        public ArmaValue(object data, string type)
        {
            Data = data;
            Type = type;
        }

        public static ArmaValue GenerateArray()
        {
            var value = new ArmaValue(string.Empty, "ARRAY")
            {
                ArrayData = new List<ArmaValue>()
            };
            return value;
        }

        public object Data { get; set; }
        public string Type { get; init; }

        public ICollection<ArmaValue> ArrayData { get; set; }

    }
}