﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using log4net;
using Plus.Core;
using Plus.HabboHotel.Cache.Type;
using Plus.HabboHotel.Users;

namespace Plus.HabboHotel.Cache.Process
{
    internal sealed class ProcessComponent
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(ProcessComponent));

        /// <summary>
        /// ThreadPooled Timer.
        /// </summary>
        private Timer _timer;

        /// <summary>
        /// Prevents the timer from overlapping itself.
        /// </summary>
        private bool _timerRunning;

        /// <summary>
        /// Checks if the timer is lagging behind (server can't keep up).
        /// </summary>
#pragma warning disable CS0414
        private bool _timerLagging;

        /// <summary>
        /// Enable/Disable the timer WITHOUT disabling the timer itself.
        /// </summary>
        private bool _disabled;

        /// <summary>
        /// Used for disposing the ProcessComponent safely.
        /// </summary>
        private readonly AutoResetEvent _resetEvent = new(true);

        /// <summary>
        /// How often the timer should execute.
        /// </summary>
        private const int RuntimeInSec = 1200;

        /// <summary>
        /// Initializes the ProcessComponent.
        /// </summary>
        public void Init()
        {
            _timer = new Timer(Run, null, RuntimeInSec * 1000, RuntimeInSec * 1000);
        }

        /// <summary>
        /// Called for each time the timer ticks.
        /// </summary>
        /// <param name="state"></param>
        public void Run(object state)
        {
            try
            {
                if (_disabled)
                    return;

                if (_timerRunning)
                {
                    _timerLagging = true;
                    return;
                }

                _resetEvent.Reset();

                // BEGIN CODE
                List<UserCache> cacheList = PlusEnvironment.GetGame().GetCacheManager().GetUserCache().ToList();
                if (cacheList.Count > 0)
                {
                    foreach (UserCache cache in cacheList)
                    {
                        try
                        {
                            if (cache == null)
                                continue;

                            if (cache.IsExpired())
                                PlusEnvironment.GetGame().GetCacheManager().TryRemoveUser(cache.Id, out _);
                        }
                        catch (Exception e)
                        {
                            ExceptionLogger.LogException(e);
                        }
                    }
                }

                List<Habbo> cachedUsers = PlusEnvironment.GetUsersCached().ToList();
                if (cachedUsers.Count > 0)
                {
                    foreach (Habbo data in cachedUsers)
                    {
                        try
                        {
                            if (data == null)
                                continue;

                            Habbo temp = null;

                            if (data.CacheExpired())
                                PlusEnvironment.RemoveFromCache(data.Id, out temp);

                            temp?.Dispose();
                        }
                        catch (Exception e)
                        {
                            ExceptionLogger.LogException(e);
                        }
                    }
                }
                // END CODE

                // Reset the values
                _timerRunning = false;
                _timerLagging = false;

                _resetEvent.Set();
            }
            catch (Exception e)
            {
                ExceptionLogger.LogException(e);
            }
        }

        /// <summary>
        /// Stops the timer and disposes everything.
        /// </summary>
        public void Dispose()
        {
            // Wait until any processing is complete first.
            try
            {
                _resetEvent.WaitOne(TimeSpan.FromMinutes(5));
            }
            catch
            {
            } // give up

            // Set the timer to disabled
            _disabled = true;

            // Dispose the timer to disable it.
            try
            {
                _timer?.Dispose();
            }
            catch
            {
            }

            // Remove reference to the timer.
            _timer = null;
        }
    }
}