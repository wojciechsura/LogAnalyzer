using ConfigurationBase;
using LogAnalyzer.Configuration.Base;
using System;
using System.Xml.Serialization;

namespace LogAnalyzer.Configuration
{
    public class LogParserProfile : BaseCollectionItem
    {
        public static readonly string XML_NAME = "LogParserProfile";

        public LogParserProfile() : base(XML_NAME)
        {
            Name = new SimpleValue<string>("Name", this);
            Guid = new GuidValue("Guid", this);
            ParserUniqueName = new SimpleValue<string>("ParserUniqueName", this);
            SerializedProfile = new SimpleValue<string>("SerializedProfile", this);
        }

        public LogParserProfile(string name, Guid guid, string parserUniqueName, string serializedProfile)
            : this()
        {
            Name.Value = name;
            Guid.Value = guid;
            ParserUniqueName.Value = parserUniqueName;
            SerializedProfile.Value = serializedProfile;
        }

        public SimpleValue<string> Name { get; private set; }
        public GuidValue Guid { get; private set; }
        public SimpleValue<string> ParserUniqueName { get; private set; }
        public SimpleValue<string> SerializedProfile { get; private set; }        
    }
}