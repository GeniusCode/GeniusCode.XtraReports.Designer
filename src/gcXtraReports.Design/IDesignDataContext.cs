using System.Linq;

namespace GeniusCode.XtraReports.Design
{
    public interface IDesignDataContext
    {
        IDesignDataRepository DesignDataRepository { get; }
        IDesignReportMetadataAssociationRepository DesignDataDefinitionRepository { get; }
    }
}