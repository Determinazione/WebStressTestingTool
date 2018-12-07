using System.Collections.Generic;

namespace GSMediator.StressTest.Generator
{
    public class AccountGenerator
    {
        private static readonly Queue<string> _accountQueue = new Queue<string>();
        private const string PREFIX = "AA";

        public AccountGenerator()
        {
            // 初始化的時候，放入指定的帳號
            for(int i = 0; i < Program.MultiTaskTimes; i++)
            {
                _accountQueue.Enqueue(PREFIX + i);
            }
        }

        /// <summary>
        /// Dequeues the account.
        /// </summary>
        /// <returns></returns>
        public string DequeueAccount()
        {
            lock (_accountQueue)
            {
                return _accountQueue.Dequeue();
            }
        }
    }
}
