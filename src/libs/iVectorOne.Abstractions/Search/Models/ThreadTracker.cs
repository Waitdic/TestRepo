namespace iVectorOne.Search.Models
{
    using System;
    using System.Threading;
    using Intuitive;
    using Intuitive.Helpers.Extensions;
    using Intuitive.Helpers.Net;

    /// <summary>
    /// A thread tracker
    /// </summary>
    public class ThreadTracker
    {
        /// <summary>
        /// The total requests to send
        /// </summary>
        private int totalRequestsToSend = 0;

        /// <summary>
        /// The requests sent
        /// </summary>
        private int requestsSent = 0;

        /// <summary>
        /// The total responses received
        /// </summary>
        private int totalResponsesReceived = 0;

        /// <summary>
        /// Gets or sets the d request start time.
        /// </summary>
        /// <value>
        /// The d request start time.
        /// </value>
        private DateTime requestStartTime;

        /// <summary>
        /// The thread exceptions
        /// </summary>
        private int threadExceptions = 0;

        /// <summary>
        /// Gets or sets a value indicating whether [b this thread exception].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [b this thread exception]; otherwise, <c>false</c>.
        /// </value>
        private bool thisThreadException = false;

        /// <summary>
        /// The request timeouts
        /// </summary>
        private int requestTimeouts = 0;

        /// <summary>
        /// Gets or sets a value indicating whether [b thread cancelled].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [b thread cancelled]; otherwise, <c>false</c>.
        /// </value>
        private bool threadCancelled  = false;

        /// <summary>
        /// Gets or sets a value indicating whether [transform started].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [transform started]; otherwise, <c>false</c>.
        /// </value>
        public bool TransformStarted { get; set; } = false;

        /// <summary>
        /// Gets or sets the request limit.
        /// </summary>
        /// <value>
        /// The request limit.
        /// </value>
        public int RequestLimit { get; set; } = 10;

        /// <summary>
        /// Gets or sets the thread time.
        /// </summary>
        /// <value>
        /// The thread time.
        /// </value>
        public ThirdPartyRequestTime ThreadTime { get; set; } = new ThirdPartyRequestTime();

        /// <summary>
        /// Gets a value indicating whether [all responses received].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [all responses received]; otherwise, <c>false</c>.
        /// </value>
        public bool AllResponsesReceived
        {
            get
            {
                return this.totalResponsesReceived == this.totalRequestsToSend;
            }
        }

        /// <summary>
        /// Gets a value indicating whether [all threads threw exceptions].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [all threads threw exceptions]; otherwise, <c>false</c>.
        /// </value>
        public bool AllThreadsThrewExceptions
        {
            get
            {
                return this.thisThreadException || (this.totalRequestsToSend > 0 && this.threadExceptions >= this.totalRequestsToSend);
            }
        }

        /// <summary>
        /// Gets a value indicating whether [main thread time out].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [main thread time out]; otherwise, <c>false</c>.
        /// </value>
        public bool MainThreadTimeOut
        {
            get
            {
                return this.threadCancelled || (this.totalRequestsToSend > 0 && this.requestTimeouts >= this.totalRequestsToSend);
            }
        }

        /// <summary>
        /// All the responses returned.
        /// </summary>
        public void AllResponsesReturned()
        {
            if (this.requestTimeouts == 0)
            {
                this.ThreadTime.Timeout = false;
            }
        }

        /// <summary>
        /// Threads the started.
        /// </summary>
        /// <param name="source">The source.</param>
        public void ThreadStarted(string source)
        {
            this.ThreadTime.Source = source;
            this.ThreadTime.StartPreProcessTimer();
            this.ThreadTime.StartTotalTimer();
        }

        /// <summary>
        /// Requests the process started.
        /// </summary>
        /// <param name="requestsToSend">The requests to send.</param>
        public void RequestProcessStarted(int requestsToSend)
        {
            int requestLimit = this.RequestLimit;
            if (requestLimit > requestsToSend)
            {
                this.totalRequestsToSend = requestsToSend;
            }
            else
            {
                this.totalRequestsToSend = requestLimit;
            }

            this.ThreadTime.RequestCount = this.totalRequestsToSend;
            this.ThreadTime.StopPreProcessTimer();
            this.ThreadTime.StartRequestTimer();
            this.requestStartTime = DateTime.Now;
        }

        /// <summary>
        /// Requests the sent.
        /// </summary>
        public void RequestSent()
        {
            Interlocked.Increment(ref this.requestsSent);
        }

        /// <summary>
        /// Records the error when sending request.
        /// </summary>
        public void RecordErrorWhenSendingRequest()
        {
            this.ThreadTime.Timeout = false;
            Interlocked.Decrement(ref this.totalRequestsToSend);
            this.thisThreadException = true;
        }

        /// <summary>
        /// Records the error when processing response.
        /// </summary>
        public void RecordErrorWhenProcessingResponse()
        {
            this.ThreadTime.Timeout = false;
            this.thisThreadException = true;
        }

        /// <summary>
        /// Requests the received.
        /// </summary>
        /// <param name="request">The o request.</param>
        /// <param name="exceptionThrownByThirdParty">if set to <c>true</c> [b exception thrown by third party].</param>
        public void RequestReceived(Request request, bool exceptionThrownByThirdParty)
        {
            Interlocked.Increment(ref this.totalResponsesReceived);
            if (exceptionThrownByThirdParty || request.Exception & !request.TimeOut)
            {
                Interlocked.Increment(ref this.threadExceptions);
            }
            else if (request.TimeOut)
            {
                Interlocked.Increment(ref this.requestTimeouts);
            }

            if (this.AllResponsesReceived)
            {
                this.ThreadTime.TotalResponseTime = (DateTime.Now.Subtract(this.requestStartTime).TotalMilliseconds / 1000d).ToSafeDecimal();
                this.ThreadTime.StartPostProcessTimer();
            }
        }

        /// <summary>
        /// Inserts the into SQL started.
        /// </summary>
        public void InsertIntoSqlStarted()
        {
            this.ThreadTime.StopPostProcessTimer();
            this.ThreadTime.StartStoreTimer();
        }

        /// <summary>
        /// Inserts the into SQL finished.
        /// </summary>
        /// <param name="resultCount">The result count.</param>
        public void InsertIntoSqlFinished(int resultCount)
        {
            this.ThreadTime.StopStoreTimer();
            this.ThreadTime.TotalResults = resultCount;
        }

        /// <summary>
        /// Threads the cancelled.
        /// </summary>
        public void ThreadCancelled()
        {
            this.threadCancelled = true;
        }

        /// <summary>
        /// Threads the finished.
        /// </summary>
        public void ThreadFinished()
        {
            if (!this.MainThreadTimeOut && this.requestTimeouts == 0)
            {
                this.ThreadTime.Timeout = false;
            }

            this.ThreadTime.Exception = this.AllThreadsThrewExceptions;

            // Only set these times when we have all the requests - The set times is also done in the main thread so will get set at some point
            if (this.AllResponsesReceived)
            {
                this.ThreadTime.StopTotalTimer();

                // clear up any still open timers just in case
                this.ThreadTime.SetTimes();
            }
        }
    }
}
