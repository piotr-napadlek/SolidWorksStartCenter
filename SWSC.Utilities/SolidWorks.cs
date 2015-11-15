using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SldWorks;
using System.Diagnostics;
using System.Timers;
using WNApplications.SWSC.Data;

namespace WNApplications.SWSC.Utilities
{
    public class SolidWorks
    {
        private SldWorks.SldWorks _instance;
        private Process swProcess;
        private Dictionary<string, object> _ROT;
        private Timer rotTimer;
        private SWVersion swVersion;

        public event Action InstanceStarted;
        public event Action ProcessStarted;
        public event Action ProcessEnded;
        public event Action SWIdle;

        /// <summary>
        /// Creates an instance of SolidWorks class
        /// </summary>
        /// <param name="version">Information about a version in SWVersion class format.</param>
        public SolidWorks(SWVersion version)
        {
            rotTimer = new Timer();
            rotTimer.Elapsed += UpdateROT;
            _ROT = ROTHelper.GetActiveObjectList(String.Empty);
            _instance = null;
            swProcess = null;
            swVersion = version;
            ProcessEnded += ProcessHasEnded;
        }

        /// <summary>
        /// Starts a SolidWorks execution process
        /// </summary>
        public void StartRunningProcess()
        {
            swProcess = Process.Start(swVersion.ExePath);
            if (swProcess != null)
            {
                swProcess.Exited += OnProcessExit;
                if (ProcessStarted != null)
                {
                    ProcessStarted();
                }
            }
            swProcess.EnableRaisingEvents = true;

            rotTimer.Interval = 1000;
            rotTimer.Start();
        }

        /// <summary>
        /// Represents a working instance of SolidWorks Application
        /// </summary>
        public SldWorks.SldWorks Instance
        {
            get
            {
                return _instance;
            }
            set
            {
                _instance = value;
                if(_instance!=null && InstanceStarted!=null)
                {
                    InstanceStarted();
                }
            }
        }

        private Dictionary<string, object> ROT
        {
            get
            {
                return _ROT;
            }

            set
            {
                _ROT = value;
                FindInstanceByProcess();
            }
        }

        private void FindInstanceByProcess()
        {
            if(ROT!=null) //searching for SW instances with matching processID
            {
                foreach(var rotItem in ROT)
                {
                    if(rotItem.Value is SldWorks.SldWorks)
                    {
                        SldWorks.SldWorks tempSworks = (SldWorks.SldWorks)rotItem.Value;
                        if(tempSworks.GetProcessID()==swProcess.Id)
                        {
                            Instance = tempSworks;
                            Instance.OnIdleNotify += SolidWorksIdle;
                            rotTimer.Stop();
                            if (InstanceStarted!=null)
                            {
                                InstanceStarted();
                            }
                        }
                    }
                }
            }
        }

        private int SolidWorksIdle()
        {
            if (SWIdle != null)
            {
                SWIdle();
                return 1;
            }
            return 0;
        }

        private void UpdateROT(object sender, ElapsedEventArgs e)
        {
            Dictionary<string, object> tempROT = ROTHelper.GetActiveObjectList(String.Empty);

            //deep comparison of ROT
            if(tempROT==null)
            {
                ROT = null;
                return;
            }

            if (tempROT.Count == ROT.Count)
            {
                for (int i = 0; i < tempROT.Count; i++)
                {
                    if (tempROT.ElementAt(i).Key != ROT.ElementAt(i).Key)
                    {
                        ROT = tempROT;
                    }
                }
            }
            else
            {
                ROT = tempROT;
            }
        }

        private void OnProcessExit(object sender, EventArgs e)
        {
            rotTimer.Stop();
            Instance = null;
            swProcess = null;
            if(ProcessEnded!=null)
            {
                ProcessEnded();
            }
        }

        private void ProcessHasEnded()
        {
            Debug.Print("SW EXITED!");
        }
    }
}
