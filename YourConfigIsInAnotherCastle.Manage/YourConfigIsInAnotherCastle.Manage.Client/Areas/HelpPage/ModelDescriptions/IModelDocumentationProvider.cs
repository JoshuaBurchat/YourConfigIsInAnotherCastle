using System;
using System.Reflection;

namespace YourConfigIsInAnotherCastle.Manage.Client.Areas.HelpPage.ModelDescriptions
{
    public interface IModelDocumentationProvider
    {
        string GetDocumentation(MemberInfo member);

        string GetDocumentation(Type type);
    }
}