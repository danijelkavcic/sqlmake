using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Resources;
using System.Reflection;

namespace SQLMakeTest
{
        public class ResourceHelper
        {
            private static ResourceManager resourceManager;

            static ResourceHelper()
            {
                resourceManager = new ResourceManager("SQLMakeTest.Properties.Resources", Assembly.GetExecutingAssembly());
            }

            public static string GetResourceString(string name)
            {
                if (resourceManager == null)
                {
                    throw new Exception("ResourceHelper: Resource manager not set!");
                }

                return resourceManager.GetString(name);
            }

            public static string Format(string resourceStringName, object arg)
            {
                return String.Format(GetResourceString(resourceStringName), arg);
            }

            public static string Format(string resourceStringName, object arg0, object arg1)
            {
                return String.Format(GetResourceString(resourceStringName), arg0, arg1);
            }

            public static string Format(string resourceStringName, object arg0, object arg1, object arg2)
            {
                return String.Format(GetResourceString(resourceStringName), arg0, arg1, arg2);
            }

            public static string Format(string resourceStringName, object[] args)
            {
                return String.Format(GetResourceString(resourceStringName), args);
            }
        }

}
