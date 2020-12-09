using System;
using System.IO;
using System.Linq;
using CodeGeneration.Utils;
using JetBrains.Annotations;
using TreeGraph;
using UnityEngine;

namespace CodeGeneration
{
    public class BaseGenerator: Tree<string>
    {
        /// <summary>
        /// Check if current version file exists
        /// </summary>
        protected bool CurrentFileExists<T>(InheritorInfo<T> iVersionEntity, out string fileName)
        {
            fileName = DefaultGenerationParams.GetFullFileName(iVersionEntity);
            return File.Exists(fileName);
        }
        
        /// <summary>
        /// Create new version file and return FileStream 
        /// </summary>
        [CanBeNull]
        protected FileStream CreateFile(string fileName, bool removeOnExist)
        {
            if (File.Exists(fileName))
            {
                if (removeOnExist)
                    File.Delete(fileName);
                else
                    return null;
            }

            try
            {
                return File.Create(fileName);
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
                return null;
            }   
        }
        
        /// <summary>
        /// Returns generated code from string (order: parent->child, left-brother->rightBrother) 
        /// </summary>
        protected string GenerateCode()
        {
            return RootNodes.OfType<BaseGeneratorNode>()
                .Aggregate("", 
                    (current, node) => current + node.GetCode() + Environment.NewLine);
        }
    }
}