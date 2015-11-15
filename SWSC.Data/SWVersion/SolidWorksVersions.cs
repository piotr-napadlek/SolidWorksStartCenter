using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WNApplications.SWSC.Data
{
    public class SolidWorksVersions
    {

        //class created by Piotr Napadłek 25.08.2015
        //gets the list of SolidWorks installations
        //with connected install paths and registry root entries

        private SWVersion[] sldWorksVersions;


        public SWVersion[] SWInstallations
        {
            get
            {
                return sldWorksVersions;
            }
            set
            {
                sldWorksVersions = value;
            }
        }


        public SolidWorksVersions()
        {
            string strRootConfigKey = @"SOFTWARE\SolidWorks";
            string strRootUninstallKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall";

            List<string> ListSWVersions = new List<string>();

            List<SWVersion> structSWVersionsList = new List<SWVersion>();


            #region RegistryParser

            RegistryKey baseKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64);

            RegistryKey RootConfigKey = baseKey.OpenSubKey(strRootConfigKey);
            RegistryKey RootUninstallKey = baseKey.OpenSubKey(strRootUninstallKey);


            string[] strRootConfigKeySubkeys = RootConfigKey.GetSubKeyNames();

            foreach (string subKey in strRootConfigKeySubkeys)
            {
                if (subKey.ToLower().Contains("solidworks"))
                {
                    ListSWVersions.Add(subKey);
                }
            }

            string[] sKN = RootUninstallKey.GetSubKeyNames();

            foreach (string subKey in sKN)
            {
                try
                {
                    RegistryKey rk = baseKey.OpenSubKey(strRootUninstallKey + "\\" + subKey, false);

                    structSWVersionsList.Add(new SWVersion(rk.GetValue("DisplayName").ToString(), rk.GetValue("InstallLocation").ToString()));

                    // Console.WriteLine(rk.GetValue("DisplayName").ToString());

                }
                catch (Exception e)
                {
                    Debug.Print(e.Message);
                }

            }

            #endregion

            FilterSWElements(ListSWVersions, structSWVersionsList);

            this.SWInstallations = structSWVersionsList.ToArray();

            MatchRegistryRootPaths(ListSWVersions.ToArray());
            FindExecutables();
        }

        private void MatchRegistryRootPaths(string[] regRootPaths)
        {
            foreach (string path in regRootPaths)
            {
                for (int i = 0; i < this.SWInstallations.Length; i++)
                {
                    if (this.SWInstallations[i].DisplayName.ToLower().Contains(path.ToLower()))
                        this.SWInstallations[i].RegPath = @"SOFTWARE\SolidWorks\" + path;
                }
            }
        }

        private void FilterSWElements(List<string> key, List<SWVersion> filtered)
        {
            bool delFlag = true;
            List<SWVersion> RemList = new List<SWVersion>();

            foreach (SWVersion vers in filtered)
            {
                foreach (string tempKey in key)
                {
                    if (vers.DisplayName.ToLower().Contains(tempKey.ToLower()))
                    {
                        delFlag = false;
                    }
                }

                if (vers.DisplayName.ToLower().Contains("resources"))
                    delFlag = true;

                if (delFlag == true)
                    RemList.Add(vers);

                delFlag = true;
            }

            foreach (SWVersion vers in RemList)
            {
                filtered.Remove(vers);
            }

        }

        private void FindExecutables()
        {
            for (int i = 0; i < SWInstallations.Length; i++)
            {
                SWInstallations[i].ExePath = SWInstallations[i].InstallLocation + @"SLDWORKS.exe";
                if (!(System.IO.File.Exists(SWInstallations[i].ExePath)))
                {
                    SWInstallations[i].DisplayName += " (errornous installation: SLDWORKS.exe not found)";
                    SWInstallations[i].ExePath = "(missing)";
                }
            }
        }
    }
}
