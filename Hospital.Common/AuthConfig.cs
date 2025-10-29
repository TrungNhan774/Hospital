using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.Common
{
    public static class AuthConfig
    {
        public const string CookieName = ".Hospital.SharedAuth";
        public const string LoginPath = "/Account/Login";
        public const string AccessDeniedPath = "/Account/AccessDenied";
        public const string AppName = "HospitalAuthShared";
        public static readonly string KeysPath = @"D:\HospitalSharedKeys";
    }
}
