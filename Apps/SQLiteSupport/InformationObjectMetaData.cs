using System;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace SQLiteSupport
{
    [Flags]
    public enum SerializationType
    {
        Undefined = 0,
        XML = 1,
        JSON = 2,
        XML_AND_JSON = XML | JSON
    }

    [Table]
    public class InformationObjectMetaData
    {
        public InformationObjectMetaData()
        {
            ID = Guid.NewGuid().ToString();
            MD5 = String.Empty;
        }

        [Column(IsPrimaryKey = true)]
        public string ID { get; set; }

        [Column]
        public string SemanticDomain { get; set; }

        [Column]
        public string ObjectType { get; set; }

        [Column]
        public string ObjectID { get; set; }

        [Column]
        public string MD5 { get; set; }

        [Column]
        public string LastWriteTime { get; set; }

        [Column]
        public long FileLength { get; set; }

        [Column]
        public SerializationType SerializationType { get; set; }

        public ChangeAction CurrentChangeAction { get; set; }
        public string CurrentStoragePath { get; set; }

        public static string[] GetMetaDataTableCreateSQLs()
        {
            return new string[]
            {
                @"
CREATE TABLE IF NOT EXISTS InformationObjectMetaData(
[ID] TEXT NOT NULL PRIMARY KEY, 
[SemanticDomain] TEXT NOT NULL, 
[ObjectType] TEXT NOT NULL, 
[ObjectID] TEXT NOT NULL,
[MD5] TEXT NOT NULL,
[LastWriteTime] TEXT NOT NULL,
[FileLength] INTEGER NOT NULL,
[SerializationType] INTEGER NOT NULL
)",
                @"
CREATE UNIQUE INDEX IF NOT EXISTS ObjectIX ON InformationObjectMetaData (
SemanticDomain, 
ObjectType, 
ObjectID
)"
            };
        }



    }
}