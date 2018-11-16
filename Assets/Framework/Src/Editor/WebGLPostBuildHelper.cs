using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
using System.IO;
using Ionic.Zlib;

public class WebGLPostBuildHelper
{
    private const string FILE_FORMAT_ORIGINAL = "{0}.asm.framework.unityweb";
    private const string FILE_FORMAT_DECOMPRESSED = "{0}.asm.js";
    private const string UNITY_LOADER_HEADER = "UnityWeb Compressed Content (gzip)";

    [PostProcessBuildAttribute(1)]
    public static void OnPostprocessBuild(BuildTarget target, string pathToBuiltProject)
    {
        if (target != BuildTarget.WebGL) {
            return;
        }

        SerializedObject projectSettings = new SerializedObject(UnityEditor.AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/ProjectSettings.asset")[0]);
        WebGLCompressionFormat compressionFormat = (WebGLCompressionFormat) projectSettings.FindProperty("webGLCompressionFormat").intValue;

        if (Debug.isDebugBuild) {
            compressionFormat = WebGLCompressionFormat.Disabled;
        }

        string dataDir = pathToBuiltProject + "/Build/";
        string dataFileName = pathToBuiltProject.Substring(pathToBuiltProject.LastIndexOf("/") + 1);
        string dataPath = dataDir + dataFileName;
        string filePath = string.Format(FILE_FORMAT_DECOMPRESSED, dataPath);

        Logger.Message("Updating WebGL build with retina hotfix...");

        if (compressionFormat == WebGLCompressionFormat.Disabled) {
            RenameFile(dataPath, FILE_FORMAT_ORIGINAL, FILE_FORMAT_DECOMPRESSED);
        } else if (compressionFormat == WebGLCompressionFormat.Gzip) {
            DecompressFile(dataPath, FILE_FORMAT_ORIGINAL, FILE_FORMAT_DECOMPRESSED);
        } else if (compressionFormat == WebGLCompressionFormat.Brotli) {
            Logger.Error("Unable to decompress Brotli builds, skipping retina hotfix.");
            return;
        }

        ReplaceRetinaCalculations(filePath);

        if (compressionFormat == WebGLCompressionFormat.Disabled) {
            RenameFile(dataPath, FILE_FORMAT_DECOMPRESSED, FILE_FORMAT_ORIGINAL);
        } else if (compressionFormat == WebGLCompressionFormat.Gzip) {
            CompressFile(dataPath, FILE_FORMAT_DECOMPRESSED, FILE_FORMAT_ORIGINAL);
        }

        Logger.Message("Completed updating WebGL build.");
    }

    private static void RenameFile(string path, string original, string rename)
    {
        string fileOriginal = string.Format(original, path);
        string fileRenamed = string.Format(rename, path);

        File.Move(fileOriginal, fileRenamed);
    }

    private static void DecompressFile(string path, string original, string target)
    {
        string fileOriginal = string.Format(original, path);
        FileInfo file = new FileInfo(fileOriginal);

        using (FileStream originalFileStream = file.OpenRead()) {
            string decompressedFileName = string.Format(target, path);

            using (FileStream decompressedFileStream = File.Create(decompressedFileName)) {
                using (GZipStream decompressionStream = new GZipStream(originalFileStream, CompressionMode.Decompress)) {
                    CopyFromStreamToStream(decompressionStream, decompressedFileStream);
                }
            }
        }

        File.Delete(fileOriginal);
    }

    private static void CompressFile(string path, string original, string target)
    {
        string fileOriginal = string.Format(original, path);
        FileInfo file = new FileInfo(fileOriginal);

        using (FileStream originalFileStream = file.OpenRead()) {
            if ((File.GetAttributes(file.FullName) & FileAttributes.Hidden) != FileAttributes.Hidden & file.Extension != ".gz") {
                string compressedFileName = string.Format(target, path);

                using (FileStream compressedFileStream = File.Create(compressedFileName)) {
                    using (GZipStream compressionStream = new GZipStream(compressedFileStream, CompressionMode.Compress)) {
                        compressionStream.FileName = compressedFileName.Substring(compressedFileName.LastIndexOf("/") + 1);
                        compressionStream.Comment = UNITY_LOADER_HEADER;

                        CopyFromStreamToStream(originalFileStream, compressionStream);
                    }
                }
            }
        }

        File.Delete(fileOriginal);
    }

    private static void ReplaceRetinaCalculations(string filePath)
    {
        string clientX = "e.clientX - rect.left";
        string clientY = "e.clientY - rect.top";

        if (!Debug.isDebugBuild) {
            clientX = clientX.Replace(" ", "");
            clientY = clientY.Replace(" ", "");
        }

        string content = File.ReadAllText(filePath);

        content = content.Replace("e.screenX", "(e.screenX*(window.resolutionScalingFactor||1))");
        content = content.Replace("e.screenY", "(e.screenY*(window.resolutionScalingFactor||1))");
        content = content.Replace(clientX, "(e.clientX-rect.left)*(window.resolutionScalingFactor||1)");
        content = content.Replace(clientY, "(e.clientY-rect.top)*(window.resolutionScalingFactor||1)");

        File.WriteAllText(filePath, content);
    }

    private static void CopyFromStreamToStream(Stream input, Stream output)
    {
        byte[] buffer = new byte[4096];
        int bytesRead;

        while ((bytesRead = input.Read(buffer, 0, buffer.Length)) > 0) {
            output.Write(buffer, 0, bytesRead);
        }
    }
}
