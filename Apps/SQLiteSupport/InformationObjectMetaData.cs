using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore.Scaffolding.Metadata;

namespace SQLite
{
    
}

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

    [ReadOnly(true)]
    [DebuggerDisplay("Metadata: {SemanticDomain}.{ObjectType}.{ObjectID}")]
    [Table("InformationObjectMetaData")]
    public class InformationObjectMetaData
    {
        public InformationObjectMetaData()
        {
            ID = Guid.NewGuid().ToString();
            MD5 = String.Empty;
            ETag = String.Empty;
        }

        [Column()]
        [ScaffoldColumn(true)]
        [Editable(false)]
        [Key]
        public string ID { get; set; }

        [Column]
        [ScaffoldColumn(true)]
        [Editable(false)]
        public string SemanticDomain { get; set; }

        [Column]
        [ScaffoldColumn(true)]
        [Editable(false)]
        public string ObjectType { get; set; }

        [Column]
        [ScaffoldColumn(true)]
        [Editable(false)]
        public string ObjectID { get; set; }

        [Column]
        [ScaffoldColumn(true)]
        [Editable(false)]
        public string MD5 { get; set; }

        [Column]
        [ScaffoldColumn(true)]
        [Editable(false)]
        public string ETag { get; set; }


        [Column]
        [ScaffoldColumn(true)]
        [Editable(false)]
        public string LastWriteTime { get; set; }

        [Column]
        [ScaffoldColumn(true)]
        [Editable(false)]
        public long FileLength { get; set; }

        [Column]
        [ScaffoldColumn(true)]
        [Editable(false)]
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
[ETag] TEXT NOT NULL,
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