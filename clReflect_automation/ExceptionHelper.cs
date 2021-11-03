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
            
            errorMessage.Append(string.Format("[ {0:yyyy-MM-dd hh:mm s} ] Application Name", DateTime.Now));
            errorMessage.Append("Module : " + e.Source);
            errorMessage.Append('\n');
            if (e != null)
            {
                errorMessage.Append(e.ToString());
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
