using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace clReflect_automation
{
    class ExceptionHelper
    {
        private static Mutex ExceptionHelperMutex = new Mutex();

        public static void ShowExceptionMessageBox(Exception e)
        {
            ExceptionHelperMutex.WaitOne();

            StringBuilder errorMessage = new StringBuilder();

            errorMessage.Append(string.Format("[ {0:yyyy-MM-dd hh:mm s} ] Application Name", DateTime.Now));
            errorMessage.Append("Module : " + e.Source);
            errorMessage.Append('\n');
            if (e != null)
            {
                errorMessage.Append(e.ToString());
            }

            MessageBox.Show(errorMessage.ToString(), "Exception!!!!"); // fails here

            ExceptionHelperMutex.ReleaseMutex();
        }

        public static void ShowExceptionMessageBox(UnhandledExceptionEventArgs unhandledExceptionEventArgs)
        {
            if(unhandledExceptionEventArgs != null)
            {
                if(unhandledExceptionEventArgs.ExceptionObject != null)
                {
                    Exception e = unhandledExceptionEventArgs.ExceptionObject as Exception;
                    if(e != null)
                    {
                        ShowExceptionMessageBox(e);
                    }
                }
            }
        }

        public static void ShowExceptionMessageBox(ThreadExceptionEventArgs threadExceptionEventArgs)
        {
            if (threadExceptionEventArgs != null)
            {
                if (threadExceptionEventArgs.Exception != null)
                {
                    ShowExceptionMessageBox(threadExceptionEventArgs.Exception);
                }
            }
        }
        
    }
}
