using System;
using System.Collections.Generic;
using System.Text;

namespace FootprintViewer
{
    public static class ServiceException
    {
        public static Exception IsNull()
        {
            return new Exception("Type not registered");
        }

        public static Exception IsNull<T>(T type) where T : Type
        {
            return new Exception($"Type {type} not registered");
        }
    }
}
