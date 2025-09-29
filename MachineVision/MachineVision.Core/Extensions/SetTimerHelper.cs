﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MachineVision.Core.Extensions
{
    public static class SetTimerHelper
    {
        /// <summary>
        /// 函数调用计时
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public static double SetTimer(this Action action)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            action();
            stopwatch.Stop();
            return stopwatch.ElapsedMilliseconds;
        }

        /// <summary>
        /// 函数调用计时-异步
        /// </summary>
        /// <param name="func"></param>
        /// <param name="exception"></param>
        /// <returns></returns>
        public static async Task<double> SetTimerAsync(this Func<Task> func, Action<Exception> exception = null)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            try
            {
                await func();
            }
            catch (Exception ex)
            {
                //异常可以绑定外部一个回调函数进行处理
                exception?.Invoke(ex);
            }
            finally
            {
                stopwatch.Stop();
            }
            return stopwatch.ElapsedMilliseconds;
        }
    }
}
