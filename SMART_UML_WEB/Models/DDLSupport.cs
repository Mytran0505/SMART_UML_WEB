namespace SMART_UML_WEB.Models
{
    public class DDLSupport
    {
        // Dictionary to map common words to SQL data types
        public Dictionary<string, string> dataTypes = new Dictionary<string, string>
        {
            { "mã", "INT" },
            { "tên", "VARCHAR(100)" },
            { "text", "VARCHAR(255)" },
            { "số", "INT" },
            { "decimal", "DECIMAL(10, 2)" },
            { "ngày", "DATE" },
            { "giờ", "TIME" },
            { "ngày_giờ", "DATETIME" },
            { "float", "FLOAT" },
            { "int", "INT" }
        };

        // Function to determine the SQL data type based on the attribute name
        public string GetSqlDataType(string word)
        {
            word = word.ToLower();
            if (word.Contains("số"))
                return dataTypes["số"];
            if (word.Contains("ngày"))
                return dataTypes["ngày"];
            if (word.Contains("mã"))
                return dataTypes["mã"];
            if (word.Contains("tên"))
                return dataTypes["tên"];
            return dataTypes["text"];
        }
    }
}
