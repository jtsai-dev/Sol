using System;
using System.Threading;

namespace JT.Infrastructure.Timer
{
    // TimeoutListener timeout = new TimeoutListener();
    // timeout.TimeSpan = new TimeSpan(0, 0, 0, 2);
    // timeout.Action = () =>
    // {
    //     Console.WriteLine("线程操作开始");
    //     //模拟超时流程
    //     System.Threading.Thread.Sleep(1000);
    //     Console.WriteLine("线程操作结束");
    // };
    // bool isTimeout = timeout.InvokeAction();
    // if (isTimeout)
    // {
    //     Console.WriteLine("执行超时");
    // }
    // else
    // {
    //     Console.WriteLine("未超时");
    // }

    public class TimeoutListener
    {
        private ManualResetEvent timeoutObject;
        private bool isTimeout;
        public Action Action { get; set; }
        public TimeSpan TimeSpan { get; set; }
        public Exception Exception { get; private set; }
        public TimeoutListener()
        {
            //初始状态为 停止
            this.timeoutObject = new ManualResetEvent(true);
        }
        /// <summary>
        /// Invoke the action and return it's timeout or not
        /// </summary>
        /// <returns>isTimeout or not</returns>
        public bool InvokeAction()
        {
            if (this.Action == null)
            {
                return false;
            }
            this.timeoutObject.Reset();
            this.isTimeout = true;
            this.Action.BeginInvoke(CallBackAsync, null);
            // 等待 信号Set
            if (!this.timeoutObject.WaitOne(TimeSpan, false))
            {
                this.isTimeout = true;
            }
            return this.isTimeout;
        }
        ///<summary>
        /// 异步委托 回调函数
        ///</summary>
        ///<param name="result"></param>
        private void CallBackAsync(IAsyncResult result)
        {
            try
            {
                this.Action.EndInvoke(result);
                this.isTimeout = false;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                //Logger.Error(ex);
                Exception = ex;
                this.isTimeout = true;
            }
            finally
            {
                this.timeoutObject.Set();
            }
        }
    }
}