﻿using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace PerfIt
{
    /// <summary>
    /// Number of Operations Per Second Counter handler.
    /// </summary>
    public class NumberOfOperationsPerSecondHandler : CounterHandlerBase
    {
        private Lazy<PerformanceCounter> _counter;
        private const string TimeTakenTicksKey = "NumberOfOperationsPerSecondHandler_#_StopWatch_#_";

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="categoryName"></param>
        /// <param name="instanceName"></param>
        public NumberOfOperationsPerSecondHandler(string categoryName, string instanceName)
            : base(categoryName, instanceName)
        {
            BuildCounters();
        }

        public override string CounterType
        {
            get { return CounterTypes.NumberOfOperationsPerSecond; }
        }

        protected override void OnRequestStarting(IDictionary<string, object> contextBag, PerfItContext context)
        {
            context.Data.Add(TimeTakenTicksKey + InstanceName, Stopwatch.StartNew());
        }

        protected override void OnRequestEnding(IDictionary<string, object> contextBag, PerfItContext context)
        {
            var sw = (Stopwatch) context.Data[TimeTakenTicksKey + InstanceName];
            sw.Stop();
            _counter.Value.Increment();
        }

        protected override void BuildCounters(bool newInstanceName = false)
        {
            _counter = new Lazy<PerformanceCounter>(() => new PerformanceCounter
            {
                CategoryName = CategoryName,
                CounterName = Name,
                InstanceName = GetInstanceName(newInstanceName),
                ReadOnly = false,
                InstanceLifetime = PerformanceCounterInstanceLifetime.Process,
                RawValue = 0
            });
        }

        protected override IEnumerable<CounterCreationData> DoGetCreationData()
        {
            yield return new CounterCreationData
            {
                CounterType = PerformanceCounterType.RateOfCountsPerSecond32,
                CounterName = Name,
                CounterHelp = "# of operations / sec"
            };
        }
    }
}
