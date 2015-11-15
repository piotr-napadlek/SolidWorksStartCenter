using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WNApplications.SWSC.Data
{
    public interface IDataManager
    {
        List<SWSetting> GetSettingsData();

        void UpdateSettingsData(List<SWSetting> swSettings);

        SWVersion[] GetSolidWorksVersions();
    }
}
