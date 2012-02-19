using System.Linq;

namespace GeniusCode.XtraReports.Designer.Prototypes
{
    public interface IDynamicDllLoader
    {
        void LoadDllsInDirectory(string path);
    }
}