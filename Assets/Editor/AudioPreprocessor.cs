using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class AudioPreprocessor : AssetPostprocessor
{
    private const int _bigFile = 5242880;
    private const int _shortFile = 204800;
    private void OnPreprocessAudio()
    {
        AudioImporter audioImporter = (AudioImporter)assetImporter;
        var audioSettings = audioImporter.defaultSampleSettings;
        var file = new FileInfo(assetPath);

        audioSettings.compressionFormat = AudioCompressionFormat.Vorbis;
        audioSettings.quality = 0.75f;
        
        if (assetPath.Contains("mono"))
        {
            audioImporter.forceToMono = true;
        }
        if (file.Length > _bigFile)
        {
            audioSettings.loadType = AudioClipLoadType.Streaming;
            audioImporter.loadInBackground = true;
        }
        else if (file.Length > _shortFile)
        {
            audioSettings.loadType = AudioClipLoadType.CompressedInMemory;
        }
        else
        {
            audioSettings.loadType = AudioClipLoadType.DecompressOnLoad;
        }

        audioImporter.SetOverrideSampleSettings("Standalone", audioSettings);
    }
}
