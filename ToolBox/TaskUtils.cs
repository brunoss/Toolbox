using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ToolBox
{
    public class TaskUtils
    {
        public static int Sum(int[] values, CancellationToken token)
        {
            int acc = 0;
            for (int i = 0; i < values.Length; ++i)
            {
                acc += values[i];
                token.ThrowIfCancellationRequested(); //you may wish to do this verification less often, doing this every iteration will reduce performance
                Thread.Sleep(100); //just simulate a longer task so you may know that it may be canceled before completing. 
            }
            return acc;
        }


        public static int Sum(int[] values, CancellationToken token, TimeSpan timeout)
        {
            int now = Environment.TickCount;
            int acc = 0;
            for (int i = 0; i < values.Length; ++i)
            {
                acc += values[i];
                token.ThrowIfCancellationRequested(); //you may wish to do this verification less often, doing this every iteration will reduce performance
                Thread.Sleep(100); //just simulate a longer task so you may know that it may be canceled before completing. 
            }
            return acc;
        }
    }
}
