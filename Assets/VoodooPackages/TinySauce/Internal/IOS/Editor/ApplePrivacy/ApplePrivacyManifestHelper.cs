using System.IO;
using System.Text;

namespace Voodoo.Tiny.Sauce.Internal.Editor.ApplePrivacy
{
    public class ApplePrivacyManifestHelper
    {
        
        private const string MANIFEST_TEMPLATE_PATH = "Assets/VoodooPackages/TinySauce/Internal/IOS/Editor/ApplePrivacy/Template/PrivacyInfo.xcprivacy";
        private const string MANIFEST_DESTINATION_PATH_FOLDER = "Assets/Plugins/iOS/";
        private const string MANIFEST_FILE_NAME = "PrivacyInfo.xcprivacy";
        public static string MergeManifests(string sourcePath, string destPath)
        {
            Plist sourcePlist = new Plist(sourcePath);
            Plist destPlist = new Plist(destPath);
            Plist.MergePlist(destPlist, sourcePlist);
            return destPlist.ToXmlString();
        }
        
        public static void SaveXmlContentToFile(string content, string filepath)
        {
            FileMode mode = FileMode.OpenOrCreate;
            if (File.Exists(filepath))
                mode = FileMode.Truncate;
            
            using FileStream stream = new FileStream(filepath, mode);
            var utf8Encoding = new UTF8Encoding(false);
            using var w = new StreamWriter(stream, utf8Encoding, 1024, true);
            w.Write(content);
        }

        public static void ProcessManifest()
        {
            //Create folder if doesnt exist
            if (!File.Exists(MANIFEST_DESTINATION_PATH_FOLDER)) {
                Directory.CreateDirectory(MANIFEST_DESTINATION_PATH_FOLDER);
            }

            string manifestDestinationPath = MANIFEST_DESTINATION_PATH_FOLDER + MANIFEST_FILE_NAME;
            
            if (File.Exists(manifestDestinationPath)) {
                //If file exist merge the manifest
                string finalManifestContent = MergeManifests(MANIFEST_TEMPLATE_PATH, manifestDestinationPath);
                SaveXmlContentToFile(finalManifestContent, manifestDestinationPath);
            } else {
                //If file doesnt exist
                File.Copy(MANIFEST_TEMPLATE_PATH, manifestDestinationPath);
            }
        }
    }
}