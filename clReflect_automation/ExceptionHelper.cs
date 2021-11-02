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
        public static void ShowExceptionMessageBox(Exception e)
        {
            StringBuilder errorMessage = new StringBuilder();

            if (e != null)
            {
                errorMessage.Append("Exception Message : ");
                errorMessage.Append(e.Message);
                errorMessage.Append('\n');
            }

            if (e.InnerException != null)
            {
                errorMessage.Append("InnerException Message : ");
                errorMessage.Append(e.InnerException.Message);
                errorMessage.Append('\n');
            }

            if (e != null)
            {
                errorMessage.Append("StackTrace : ");
                errorMessage.Append(e.StackTrace);
                errorMessage.Append('\n');
            }

            MessageBox.Show(errorMessage.ToString(), "Exception!!!!"); // fails here
        }

        public static void ShowExceptionMessageBox(UnhandledExceptionEventArgs unhandledExceptionEventArgs)
        {
            StringBuilder errorMessage = new StringBuilder();

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
            StringBuilder errorMessage = new StringBuilder();

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
