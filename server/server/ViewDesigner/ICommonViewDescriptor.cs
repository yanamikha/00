using System.Collections.Generic;
using View;

namespace GRPCServer.ViewDesigner
{
    internal interface ICommonViewDescriptor
    {
        ColumnBuilderCollection BuilderCollection
        {
            get;
        }

        Dictionary<Statement, string> RequiredPermissions
        {
            get;
        }
    }
}