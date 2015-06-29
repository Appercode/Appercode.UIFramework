using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Appercode.UI
{
    public static class PerfomanceLogger
    {
        private static List<Tuple<DateTime, string>> eventsList = new List<Tuple<DateTime, string>>();

        public static void Log(string info)
        {
            eventsList.Add(new Tuple<DateTime, string>(DateTime.Now, info));
        }

        public static void Print()
        {
#if __ANDROID__
            var saveDirictory = global::Android.OS.Environment.ExternalStorageDirectory.AbsolutePath + "/Appercodelogs/";
            if (!Directory.Exists(saveDirictory))
            {
                Directory.CreateDirectory(saveDirictory);
            }
            
            var filePath = saveDirictory + "log" + DateTime.Now + ".txt";

            StringBuilder sb = new StringBuilder();
            eventsList.ForEach((x) =>
                {
                    sb.AppendLine(x.Item1.ToString("hh:mm:ss.fffffff") + " " + x.Item2);
                });


            try
            {
                using (StreamWriter outfile = new StreamWriter(filePath))
                {
                    outfile.Write(sb.ToString());
                }
            }
            catch (Exception e)
            {
                 System.Diagnostics.Debug.WriteLine(e.Message);
            }

#endif
        }
    }
}