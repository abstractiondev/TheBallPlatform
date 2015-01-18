using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using SQLite.TheBall.Payments;

namespace SQLiteSupport
{
    public static class FileSystemSync
    {
        public static FileInfo[] GetFileInfos(string pathRoot)
        {
            DirectoryInfo rootDir = new DirectoryInfo(pathRoot);
            var result = rootDir.GetFiles("*", SearchOption.AllDirectories);
            return result;
        }

        public static InformationObjectMetaData[] GetMetaDatas(string pathRoot)
        {
            var fileInfos = GetFileInfos(pathRoot);
            return GetMetaDatas(pathRoot, fileInfos);
        }
        
        public static InformationObjectMetaData[] GetMetaDatas(string pathRoot, FileInfo[] fileInfos)
        {
            if (!pathRoot.EndsWith("\\"))
                pathRoot += "\\";
            var result = fileInfos.Where(fi => fi.Extension == "").Select(fi =>
            {
                string relativePath = fi.FullName.Replace(pathRoot, "");
                string[] components = relativePath.Split('\\');
                string semanticDomain = components[0];
                string objectType = components[1];
                string objectID = components[2];
                return new InformationObjectMetaData
                {
                    SemanticDomain = semanticDomain,
                    ObjectType = objectType,
                    ObjectID = objectID,
                    FileLength = fi.Length,
                    LastWriteTime = fi.LastWriteTimeUtc.ToString("s"),
                    CurrentStoragePath = relativePath
                };
            }).ToArray();
            return result;
        }


    }
}
