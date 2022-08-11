namespace iVectorOne.Search.Models
{
    using System;
    using Intuitive.Helpers.Extensions;
    using Intuitive.Helpers.Net;

    /// <summary>
    /// A class to track the third party request time
    /// </summary>
    /// <seealso cref="Intuitive.Net.WebRequests.RequestTime" />
    public partial class ThirdPartyRequestTime : RequestTime
    {
        /// <summary>
        /// The d pre process start time
        /// </summary>
        private DateTime preProcessStartTime;

        /// <summary>
        /// The d pre process stop time
        /// </summary>
        private DateTime preProcessStopTime;

        /// <summary>
        /// The d post process start time
        /// </summary>
        private DateTime postProcessStartTime;

        /// <summary>
        /// The d post process stop time
        /// </summary>
        private DateTime postProcessStopTime;

        /// <summary>
        /// The d store start time
        /// </summary>
        private DateTime storeStartTime;

        /// <summary>
        /// The d store stop time
        /// </summary>
        private DateTime storeStopTime;

        /// <summary>
        /// The d total start time
        /// </summary>
        private DateTime totalStartTime;

        /// <summary>
        /// The d total stop time
        /// </summary>
        private DateTime totalStopTime;

        public ThirdPartyRequestTime() { }

        public ThirdPartyRequestTime(string source)
        {
            Source = source;
        }

        /// <summary>Gets or sets the source.</summary>
        public string Source { get; set; } = string.Empty;

        /// <summary>Gets or sets the total results.</summary>
        public int TotalResults { get; set; }

        /// <summary>Gets or sets the request count.</summary>
        public int RequestCount { get; set; }

        /// <summary>Gets or sets the total time.</summary>
        public decimal TotalTime { get; set; }

        /// <summary>Gets or sets the store time.</summary>
        public decimal StoreTime { get; set; }

        /// <summary>Gets or sets the post process time.</summary>
        public decimal PostProcessTime { get; set; }

        /// <summary>Gets or sets the pre process time.</summary>
        public decimal PreProcessTime { get; set; }

        /// <summary>
        /// Sets the times.
        /// </summary>
        public void SetTimes()
        {
            // total time
            if (this.TotalTime == 0m)
            {
                if (this.totalStartTime.IsEmptyDate())
                {
                    this.TotalTime = 0m;
                }
                else if (this.totalStopTime.IsEmptyDate())
                {
                    this.TotalTime = (DateTime.Now.Subtract(this.totalStartTime).TotalMilliseconds / 1000d).ToSafeDecimal();
                }
                else
                {
                    this.TotalTime = (this.totalStopTime.Subtract(this.totalStartTime).TotalMilliseconds / 1000d).ToSafeDecimal();
                }
            }

            // pre process time
            if (this.PreProcessTime == 0m)
            {
                if (this.preProcessStartTime.IsEmptyDate())
                {
                    this.PreProcessTime = 0m;
                }
                else if (this.preProcessStopTime.IsEmptyDate())
                {
                    this.PreProcessTime = (DateTime.Now.Subtract(this.preProcessStartTime).TotalMilliseconds / 1000d).ToSafeDecimal();
                }
                else
                {
                    this.PreProcessTime = (this.preProcessStopTime.Subtract(this.preProcessStartTime).TotalMilliseconds / 1000d).ToSafeDecimal();
                }
            }

            // post process time
            if (this.PostProcessTime == 0m)
            {
                if (this.postProcessStartTime.IsEmptyDate())
                {
                    this.PostProcessTime = 0m;
                }
                else if (this.postProcessStopTime.IsEmptyDate())
                {
                    this.PostProcessTime = (DateTime.Now.Subtract(this.postProcessStartTime).TotalMilliseconds / 1000d).ToSafeDecimal();
                }
                else
                {
                    this.PostProcessTime = (this.postProcessStopTime.Subtract(this.postProcessStartTime).TotalMilliseconds / 1000d).ToSafeDecimal();
                }
            }

            // store time
            if (this.StoreTime == 0m)
            {
                if (this.storeStartTime.IsEmptyDate())
                {
                    this.StoreTime = 0m;
                }
                else if (this.storeStopTime.IsEmptyDate())
                {
                    this.StoreTime = (DateTime.Now.Subtract(this.storeStartTime).TotalMilliseconds / 1000d).ToSafeDecimal();
                }
                else
                {
                    this.StoreTime = (this.storeStopTime.Subtract(this.storeStartTime).TotalMilliseconds / 1000d).ToSafeDecimal();
                }
            }
        }

        /// <summary>
        /// Starts the pre process timer.
        /// </summary>
        public void StartPreProcessTimer()
        {
            this.preProcessStartTime = DateTime.Now;
        }

        /// <summary>
        /// Stops the pre process timer.
        /// </summary>
        public void StopPreProcessTimer()
        {
            this.preProcessStopTime = DateTime.Now;
        }

        /// <summary>
        /// Starts the post process timer.
        /// </summary>
        public void StartPostProcessTimer()
        {
            this.postProcessStartTime = DateTime.Now;
        }

        /// <summary>
        /// Stops the post process timer.
        /// </summary>
        public void StopPostProcessTimer()
        {
            this.postProcessStopTime = DateTime.Now;
        }

        /// <summary>
        /// Starts the store timer.
        /// </summary>
        public void StartStoreTimer()
        {
            this.storeStartTime = DateTime.Now;
        }

        /// <summary>
        /// Stops the store timer.
        /// </summary>
        public void StopStoreTimer()
        {
            this.storeStopTime = DateTime.Now;
        }

        /// <summary>
        /// Starts the total timer.
        /// </summary>
        public void StartTotalTimer()
        {
            this.totalStartTime = DateTime.Now;
        }

        /// <summary>
        /// Stops the total timer.
        /// </summary>
        public void StopTotalTimer()
        {
            this.totalStopTime = DateTime.Now;
        }
    }
}