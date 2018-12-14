using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace YouTubson
{
    class Ini
    {
        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);

        private String ArqIni;

        public Ini()
        {
            ArqIni = Directory.GetCurrentDirectory() + @"\YouTubson.ini";
        }

        private String Le(String Campo)
        {
            StringBuilder temp = new StringBuilder(255);
            GetPrivateProfileString("Config", Campo, "", temp, 255, ArqIni);
            return temp.ToString();
        }

        public void setData()
        {
            DateTime Data = DateTime.Now;
            string sData = Data.ToShortDateString();
            WritePrivateProfileString("Config", "Data", sData, ArqIni);
        }

        public DateTime getData()
        {
            string sData = Le("Data");
            DateTime Data;
            if (sData==string.Empty)
            {
                Data = DateTime.Now.AddDays(-1);
            } else
            {
                Data = Convert.ToDateTime(sData);
            }            
            return Data;
        }

    }
}
