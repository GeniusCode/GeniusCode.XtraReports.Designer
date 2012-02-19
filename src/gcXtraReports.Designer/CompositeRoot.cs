using System.Linq;
using Autofac;
using DevExpress.XtraReports.UserDesigner;
using GeniusCode.XtraReports.Designer.Messaging;
using GeniusCode.XtraReports.Designer.Popups;
using GeniusCode.XtraReports.Designer.Support;

namespace GeniusCode.XtraReports.Designer
{
    public class CompositeRoot
    {
        public static CompositeRoot Instance { get; private set; }

        private readonly IContainer _container;

        private CompositeRoot(IContainer container)
        {
            _container = container;
        }

        public static void Init (IContainer container)
        {
            Instance = new CompositeRoot(container);
        }

        public XRDesignForm GetDesignForm()
        {
            return _container.Resolve<XRMessagingDesignForm>();
        }

        public DesignDataContext2 GetDesignDataContext()
        {
            return _container.Resolve<DesignDataContext2>();
        }

        public  ActionMessageHandler GetActionMessageHandler()
        {
            return _container.Resolve<ActionMessageHandler>();
        }

        public DebugMessageHandler GetDebugMessageHandler()
        {
            return _container.Resolve<DebugMessageHandler>();
        }
    }
}