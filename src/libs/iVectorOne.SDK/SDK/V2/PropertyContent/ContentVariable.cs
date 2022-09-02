namespace iVectorOne.SDK.V2.PropertyContent
{
    public class ContentVariable
    {

        public ContentVariable()
        {

        }
        public ContentVariable(ContentVariableEnum parentGroupKey, ContentVariableEnum group, string subGroup, string key, string value, ContentVariableEnum units)
        {
            ParentGroupKey = parentGroupKey;
            Group = group;
            SubGroup = subGroup;
            Key = key;
            Value = value;
            Units = units;
        }

        public ContentVariable(ContentVariableEnum parentGroupKey, ContentVariableEnum group, string subGroup, string key, string value)
        {
            ParentGroupKey = parentGroupKey;
            Group = group;
            SubGroup = subGroup;
            Key = key;
            Value = value;
        }

        public ContentVariable(ContentVariableEnum parentGroupKey, ContentVariableEnum group, ContentVariableEnum key, string value)
        {
            ParentGroupKey = parentGroupKey;
            Group = group;
            Key = System.Enum.GetName(typeof(ContentVariableEnum), key);
            Value = value;
        }

        public ContentVariableEnum ParentGroupKey { get; set; }
        public ContentVariableEnum Group { get; set; }
        public string? SubGroup { get; set; }
        public string? Key { get; set; }
        public string? Value { get; set; }
        public ContentVariableEnum Units { get; set; }
    }
}