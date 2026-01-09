
namespace CodeStandardsChecker.Utilities
{
    public class PrefixMapper
    {
        public static readonly System.Collections.Generic.Dictionary<System.String, System.String> RegionPrefixes = new System.Collections.Generic.Dictionary<System.String, System.String>
        {
            { "class declarations", "m" },
            { "properties", "prp" },
            // etc...
        };

        public static readonly System.Collections.Generic.Dictionary<System.String, System.String> TypePrefixes = new System.Collections.Generic.Dictionary<System.String, System.String>
        {
            { "System.Guid", "guid" },
            { "System.Guid?", "guid" },
            { "System.String", "str" },
            { "System.String?", "str" },
            { "System.Int32", "int" },
            { "cApplicationFunctions", "obj" },
            { "cApplicationFunctions?", "obj" },
            { "BaseWebProjectMvc80.structCustomButton", "sru" },
            { "BaseWebProjectMvc80.structCustomButton?", "sru" },
            { "BaseWebProjectMvc80.structRecordHistory", "sru" },
            { "BaseWebProjectMvc80.structRecordHistory?", "sru" },
            { "Framework80.structPageInformation", "sru" },
            { "Framework80.structPageInformation?", "sru" },
            { "BaseWebProjectMvc80.enumBootStrapButtonClass", "enm" },
            { "BaseWebProjectMvc80.enumBootStrapButtonClass?", "enm" },
            //{ "BaseWebProjectMvc80.structRecordHistory", "sru" },
            //{ "BaseWebProjectMvc80.structRecordHistory?", "sru" },
            //{ "BaseWebProjectMvc80.structRecordHistory", "sru" },
            //{ "BaseWebProjectMvc80.structRecordHistory?", "sru" },
            //{ "BaseWebProjectMvc80.structRecordHistory", "sru" },
            //{ "BaseWebProjectMvc80.structRecordHistory?", "sru" },
            //{ "BaseWebProjectMvc80.structRecordHistory", "sru" },
            //{ "BaseWebProjectMvc80.structRecordHistory?", "sru" },

            
            // etc...
        };
    }
}
