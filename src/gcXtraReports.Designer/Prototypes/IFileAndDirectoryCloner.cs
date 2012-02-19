using System.Linq;

namespace GeniusCode.XtraReports.Designer.Prototypes
{
    public interface IFileAndDirectoryCloner
    {
        void Clone(string sourcePath, string destinationPath);
    }
}
