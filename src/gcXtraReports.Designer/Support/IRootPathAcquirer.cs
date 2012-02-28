using System.Linq;

namespace GeniusCode.XtraReports.Designer.Support
{
    public interface IRootPathAcquirer
    {
        string AcquireRootPath(string defaultValue);
    }
}