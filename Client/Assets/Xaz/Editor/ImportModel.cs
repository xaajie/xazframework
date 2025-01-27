//------------------------------------------------------------
// Xaz Framework
// Feedback: qq515688254
//------------------------------------------------------------
﻿using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEditor;
using System.Linq;
using Xaz;

public class ImportModel : AssetPostprocessor 
{
    void OnPreprocessModel1()
    {
        ModelImporter importer = AssetImporter.GetAtPath(assetPath) as ModelImporter;
        if (importer != null)
        {
            var readable = false;
            var optimizeObject = true;
            var optimizeMesh = true;
            var compress = ModelImporterMeshCompression.High;
            var weldVertices = true;
            if (assetPath.Contains("Assets/Raw/Avatar/Role"))
            {
                readable = true;
                optimizeObject = false;
            }
            else if (assetPath.Contains("Assets/Raw/Map/Buildings"))
            {
                readable = true;
                weldVertices = false;
                compress = ModelImporterMeshCompression.Medium;
            	importer.importTangents = ModelImporterTangents.Import;
            }
            importer.isReadable = readable;
            importer.optimizeMesh = optimizeMesh;
            importer.meshCompression = compress;
            importer.weldVertices = weldVertices;
            importer.optimizeGameObjects = optimizeObject;
        }
    }
}
