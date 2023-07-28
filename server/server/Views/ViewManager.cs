using GRPCServer.ViewDesigner;
using System.Collections.Generic;
using View;

namespace GRPCServer.Views
{
    internal class ViewManager
    {
        protected internal static Dictionary<ACommonView, string> RegisteredViews { get; set; }

        public ViewManager()
        {
            RegisteredViews = new Dictionary<ACommonView, string>()
            {
                { new PermissionGroupsView(),   "Групи дозволів"},
                { new PermissionsView(),        "Дозволи"},
                { new PositionsView(),          "Посади"},
                { new QualificationTypesView(), "Типи кваліфікацій"},
                { new UsersView(),              "Користувачі" },
                { new UserGroupsView(),         "Групи користувачів"},
                { new UserGroupPermissionsView(),   "Дозволи по відношенню до груп користувачів"},
            };
        }

        internal static List<RegisteredView> GetRegisteredViewNames
        {
            get
            {
                List<RegisteredView> rv = new List<RegisteredView>();

                foreach (KeyValuePair<ACommonView, string> keyValue in RegisteredViews)
                {
                    RegisteredView registeredView = new RegisteredView()
                    {
                        DisplayName = keyValue.Value,
                        VName = GetRegisteredViewByKey(keyValue.Value).Name,
                    };

                    rv.Add(registeredView);
                }

                return rv;
            }
        }

        internal static ACommonView GetRegisteredViewByName(string name)
        {
            ACommonView tmp = null;

            foreach (KeyValuePair<ACommonView, string> keyValue in RegisteredViews)
            {
                if (keyValue.Key.Name == name)
                {
                    tmp = keyValue.Key;
                    break;
                }
            }
            return tmp;
        }

        internal static ACommonView GetRegisteredViewByKey(string key)
        {
            ACommonView tmp = null;

            foreach (KeyValuePair<ACommonView, string> keyValue in RegisteredViews)
            {
                if (keyValue.Value == key)
                {
                    tmp = keyValue.Key;
                    break;
                }
            }
            return tmp;
        }
    }
}