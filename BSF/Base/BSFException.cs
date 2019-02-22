using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BSF.Extensions;

namespace BSF.Base
{
    public class BSFException:Exception
    {
        public string BSFMessage = "";
        public List<Exception> BSFExceptionList = new List<Exception>();
        public BSFException(string message):base(message)
        {
            BSFMessage += message.NullToEmpty();
        }

        public BSFException(string message,Exception exception):base(message,exception)
        {
            if(exception != null)
            {
                BSFMessage += message + exception.Message.NullToEmpty();
                BSFExceptionList.Add(exception);
            }
        }
    }
}
