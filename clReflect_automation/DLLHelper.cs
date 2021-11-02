using net.r_eg.Conari;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace clReflect_automation
{
    class DLLHelper
    {
        public static void LoadDLLToConariL(ref ConariL conariL, in string dllPath)
        {
            if (conariL == null)
            {
                try
                {
                    Console.WriteLine("Try Load DLL : {0}", dllPath);
                    Console.Out.Flush();

                    conariL = new ConariL(dllPath, CallingConvention.Cdecl);

                    Console.WriteLine("Load DLL Success : {0}", dllPath);
                }
                catch(Exception e)
                {
                    throw new Exception(String.Format("Fail to LoadDLL ( DllPath : {0}, ErrorMessage : {1}, ErrorMessage ( Inner ) : {1} )", dllPath, e.Message, e.InnerException.Message));
                }
            }
        }

        public static void UnLoadDLLFromConariL(ref ConariL conariL)
        {
            if (conariL != null)
            {
                try
                {
                    conariL.Dispose();
                    conariL = null;
                }
                catch (Exception e)
                {
                    throw new Exception(String.Format("Fail to ULoadDLL (  ErrorMessage : {1}, ErrorMessage ( Inner ) : {1} )", e.Message, e.InnerException.Message));
                }
            }
        }
    }
}
