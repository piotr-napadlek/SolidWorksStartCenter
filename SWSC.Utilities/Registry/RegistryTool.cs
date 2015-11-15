using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;
using System.Xml.Serialization;
using System.Security.AccessControl;
using System.Diagnostics;
using System.IO;
using System.Threading;
using WNApplications.SWSC.Data;

namespace WNApplications.SWSC.Utilities
{
    public class RegistryTool  //class for getting and setting Windows registry entries
    {

        private const string RegHKCU = "HKEY_CURRENT_USER\\";

        //singleton implementation
        private static RegistryTool _instance;

        private RegistryTool() { }

        public static RegistryTool GetInstance()
        {
            if (_instance==null)
            {
                _instance = new RegistryTool();
            }
            return _instance;
        }

        //TODO: implement transactions on registry manipulation
        //this does not threaten the security of eg SolidWorks being corrupted
        //but just for the matter of better coding

        private List<RegistryEntry> ParseSelectedKey(RegistryKey keyToParse, bool parseSubkeys, string remString="")
        {
            List<RegistryEntry> tempRegEntries = new List<RegistryEntry>();

            string[] values = keyToParse.GetValueNames();

            foreach (string value in values)
            {
                tempRegEntries.Add(new RegistryEntry(keyToParse.Name.Replace(remString,"") , value, keyToParse.GetValue(value), keyToParse.GetValue(value).GetType().ToString()));
            }

            if (parseSubkeys)
            {
                foreach (string subkey in keyToParse.GetSubKeyNames())
                {
                    if (subkey != "Setup")
                    {
                        tempRegEntries.AddRange(ParseSelectedKey(keyToParse.OpenSubKey(subkey, false), true, remString));
                    }
                }
            }
            
            return tempRegEntries;
        }

        public List<RegistryEntry> ParseSWSettings(SWVersion version, List<string> keysList)
        {
            List<RegistryEntry> regEntryList = new List<RegistryEntry>();

            foreach (var key in keysList)
            {
                RegistryKey baseKey = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry64);
                RegistryKey rootKeyToExtract = baseKey.OpenSubKey(key);

                regEntryList.AddRange(ParseSelectedKey(rootKeyToExtract, true, RegHKCU + version.RegPath)); 
            }

            return regEntryList;
        }

        public bool WriteSWRegistrySettings(SWVersion version, List<RegistryEntry> regEntries)
        {
            try
            {
                RegistryKey baseKey = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry64);
                RegistryKey regKey = baseKey.OpenSubKey(version.RegPath + "\\User Interface");
                regKey.DeleteSubKeyTree("CommandManager",false);
            }
            catch (Exception ex)
            {
                Debug.Print(ex.Message);
                ClearCMToolbar(version);
            }

            regEntries.ForEach(regEnt => Registry.SetValue(RegHKCU + version.RegPath + regEnt.KeyName, regEnt.EntryName, regEnt.EntryValueAsObject));

            return true;
        }

        public static void ClearCMToolbar(SWVersion version)
        {
            File.WriteAllText("C:\\Temp\\reg.reg", @"Windows Registry Editor Version 5.00

[-HKEY_CURRENT_USER\" + version.RegPath + @"\User Interface\CommandManager]");
            //so that toolbars in CommandManager mode are not corrupted

            Process regProc = new Process();
            regProc.StartInfo.FileName = "regedit";
            regProc.StartInfo.Arguments = "/s C:\\Temp\\reg.reg";
            regProc.Start();
            regProc.WaitForExit();
        }

        public void OverrideSWSettings(SWVersion version, SWSetting setting)
        {
            string baseKey = RegHKCU + version.RegPath + "\\";

            string pdmPartPath=null;
            string pdmAssyPath=null;
            string pdmDrwPath=null;
            string bomTable=null;
            string weldTable=null;
            string bendTable=null;

            if (Directory.Exists(setting.PdmTemplateDirectory))
            {
                pdmPartPath = Directory.GetFiles(setting.PdmTemplateDirectory, "*.prtdot", SearchOption.AllDirectories).FirstOrDefault();
                pdmAssyPath = Directory.GetFiles(setting.PdmTemplateDirectory, "*.asmdot", SearchOption.AllDirectories).FirstOrDefault();
                pdmDrwPath = Directory.GetFiles(setting.PdmTemplateDirectory, "*.drwdot", SearchOption.AllDirectories).FirstOrDefault(); 
            }
            if (Directory.Exists(setting.TableTemplateDirectory))
            {
                bomTable = Directory.GetFiles(setting.TableTemplateDirectory, "*.sldbomtbt", SearchOption.AllDirectories).FirstOrDefault();
                weldTable = Directory.GetFiles(setting.TableTemplateDirectory, "*.sldwldtbt", SearchOption.AllDirectories).FirstOrDefault();
                bendTable = Directory.GetFiles(setting.TableTemplateDirectory, "*.sldbndtbt", SearchOption.AllDirectories).FirstOrDefault(); 
            }
                   
            if (!String.IsNullOrWhiteSpace(pdmAssyPath))
                Registry.SetValue(baseKey + "Document Templates", "Default Assy Template", pdmAssyPath);
            if (!String.IsNullOrWhiteSpace(pdmPartPath))
                Registry.SetValue(baseKey + "Document Templates", "Default Part Template", pdmPartPath);
            if (!String.IsNullOrWhiteSpace(pdmDrwPath))
                Registry.SetValue(baseKey + "Document Templates", "Default Draw Template", pdmDrwPath);

            if (!String.IsNullOrWhiteSpace(bomTable))
                Registry.SetValue(baseKey + "Drawings", "Last BOM Template Path", bomTable);
            if (!String.IsNullOrWhiteSpace(weldTable))
                Registry.SetValue(baseKey + "Drawings", "Last Weldment Template Path", weldTable);
            if (!String.IsNullOrWhiteSpace(bendTable))
                Registry.SetValue(baseKey + "Drawings", "Last BendTable Template Path", bendTable);

            if (Directory.Exists(setting.PdmTemplateDirectory))
            {
                Registry.SetValue(baseKey + "ExtReferences", "Document Template Folders", setting.PdmTemplateDirectory);
            }
            if (Directory.Exists(setting.TableTemplateDirectory))
            {
                Registry.SetValue(baseKey + "ExtReferences", "Bend Table Template Folder", setting.TableTemplateDirectory);
                Registry.SetValue(baseKey + "ExtReferences", "BOM Template Folders", setting.TableTemplateDirectory);
                Registry.SetValue(baseKey + "ExtReferences", "Weld Table Template Folder", setting.TableTemplateDirectory);
                Registry.SetValue(baseKey + "ExtReferences", "Revision Table Template Folders", setting.TableTemplateDirectory);
                Registry.SetValue(baseKey + "ExtReferences", "Punch Table Template Folder", setting.TableTemplateDirectory);
            }

            
            if (!String.IsNullOrWhiteSpace(setting.DrwTemplates?.FirstOrDefault()?.SlddrtPath) && File.Exists(setting.DrwTemplates?.FirstOrDefault()?.SlddrtPath))
            {
                Registry.SetValue(baseKey + "ExtReferences", "Sheet Format Folders", Directory.GetParent(setting.DrwTemplates?.FirstOrDefault()?.SlddrtPath).FullName); 
            }

            if (Directory.Exists(setting.WorkPath))
            {
                Registry.SetValue(baseKey + "General", "Last user path", setting.WorkPath);
                Registry.SetValue(baseKey + "ExtReferences", "Document Folders", setting.WorkPath); 
            }
        }

    }
}
