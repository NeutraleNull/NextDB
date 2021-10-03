using System.Collections.Generic;

namespace NextDB.Settings
{
    public class Configuration
    {
        public string ServerAddress { get; set; }
        public uint Port { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        
        public int MaxRetries { get; set; }
        
        public ICollection<SqlQuery> SqlQueries { get; set; }
    }

    public class SqlQuery
    {
        public string Statement { get; set; }
        public ICollection<QueryPosOperations> QueryPosInputs { get; set; }
        public ICollection<QueryPosOperations> QueryPosOutputs { get; set; }
    }

    public class QueryPosOperations
    {
        public int ElementPos { get; set; }
        public Operation Operation { get; set; } = Operation.None;
    }

    public enum Operation
    {
        None,
        Bool,
        Null,
        Time,
        String,
        String2
    }
}