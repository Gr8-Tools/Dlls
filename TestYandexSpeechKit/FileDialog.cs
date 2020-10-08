using System;
using System.Runtime.InteropServices;

namespace TestYandexSpeechKit
{
    public enum OpenFileNameCreateType {Null = 0, Base}
    
    public static class FileDialog
    {
        public static string Open(OpenFileSettings settings)
        {
            try
            {
                OpenFileName ofn = new OpenFileName(OpenFileNameCreateType.Base)
                {
                    filter = settings.filter,
                    initialDir = settings.initialDir,
                    title = settings.title,
                    flags = 0x00080000 | 0x00001000 | 0x00000800 | 0x00000200 | 0x00000008
                };
                if (!string.IsNullOrEmpty(settings.defExt))
                    ofn.defExt = settings.defExt;

                //OFN_EXPLORER|OFN_FILEMUSTEXIST|OFN_PATHMUSTEXIST| OFN_ALLOWMULTISELECT|OFN_NOCHANGEDIR
                if (DllTest.GetOpenFileName(ofn))
                    return ofn.file;
            }catch(Exception ex){ Console.WriteLine(ex.ToString());}

            return "";
        } 
        public static string Save(OpenFileSettings settings)
        {
            try
            {
                OpenFileName ofn = new OpenFileName(OpenFileNameCreateType.Base)
                {
                    filter = settings.filter,
                    initialDir = settings.initialDir,
                    title = settings.title,
                    flags = 0x00080000 | 0x00001000 | 0x00000800 | 0x00000200 | 0x00000008
                };
                if (!string.IsNullOrEmpty(settings.defExt))
                    ofn.defExt = settings.defExt;

                //OFN_EXPLORER|OFN_FILEMUSTEXIST|OFN_PATHMUSTEXIST| OFN_ALLOWMULTISELECT|OFN_NOCHANGEDIR
                if (DllTest.GetSaveFileName(ofn))
                    return ofn.file;

            }catch(Exception ex){ Console.WriteLine(ex.ToString());}

            return "";
        } 
    }

    public struct OpenFileSettings
    {
        public string filter;
        public string initialDir;
        public string title;
        public string defExt;

        public static OpenFileSettings Base
            => new OpenFileSettings()
            {
                title = "Open file",
                filter = "All Files\0*.*\0\0",
                initialDir = $"C:\\Users\\{Environment.UserName}\\Desktop"
            };
    }
    
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public class OpenFileName {
        public int      structSize = 0;
        public IntPtr   dlgOwner = IntPtr.Zero;
        public IntPtr   instance = IntPtr.Zero;
        public String   filter = null;
        public String   customFilter = null;
        public int      maxCustFilter = 0;
        public int      filterIndex = 0;
        public String   file = null;
        public int      maxFile = 0;
        public String   fileTitle = null;
        public int      maxFileTitle = 0;
        public String   initialDir = null;
        public String   title = null;
        public int      flags = 0;
        public short    fileOffset = 0;
        public short    fileExtension = 0;
        public String   defExt = null;
        public IntPtr   custData = IntPtr.Zero;
        public IntPtr   hook = IntPtr.Zero;
        public String   templateName = null;
        public IntPtr   reservedPtr = IntPtr.Zero;
        public int      reservedInt = 0;
        public int      flagsEx = 0;
        
        public OpenFileName(){}

        public OpenFileName(OpenFileNameCreateType type)
        {
            switch (type)
            {
                case OpenFileNameCreateType.Base:
                {
                    structSize = Marshal.SizeOf(this);
                    file = new string(new char[256]);
                    maxFile = file.Length;
                    fileTitle = new string(new char[64]);
                    maxFileTitle = fileTitle.Length;
                    break;
                }
                default: break;
            }
        }
    }
    
    public class DllTest {
        [DllImport("Comdlg32.dll", SetLastError = true, ThrowOnUnmappableChar = true, CharSet = CharSet.Auto)]
        public static extern bool GetOpenFileName([In, Out] OpenFileName ofn);
        public static bool GetOpenFileName1([In, Out] OpenFileName ofn) {
            return GetOpenFileName(ofn);
        }
        [DllImport("Comdlg32.dll", SetLastError = true, ThrowOnUnmappableChar = true, CharSet = CharSet.Auto)]
        public static extern bool GetSaveFileName([In, Out] OpenFileName ofn);
    }
}