using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BlessTheWeb.Core.Extensions
{
    public static class RavenExtensions
    {
        public static int IdValue(this string RavenDbId)
        {
            return int.Parse(RavenDbId.Substring(RavenDbId.IndexOf('/') + 1));
        }
        public static string ToRavenDbId(this int id, string ravenEntityTypeName)
        {
            return string.Format("{0}/{1}", ravenEntityTypeName, id);
        }
    }
}