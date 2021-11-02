using net.r_eg.Conari;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace clReflect_automation
{
    class DLLHelper
    {
        public static void LoadDLLToConariL(ref ConariL conariL, in string dllPath)
        {
            if (conariL == null)
            {
                conariL = new ConariL(dllPath, CallingConvention.Cdecl);
            }
        }

        public static void UnLoadDLLFromConariL(ref ConariL conariL)
        {
            if (conariL != null)
            {
                conariL.Dispose();
                conariL = null;
            }
        }
    }
}
