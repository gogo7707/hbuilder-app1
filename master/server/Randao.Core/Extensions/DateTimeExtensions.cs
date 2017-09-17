using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Randao.Core
{
    public static class DateTimeExtensions
    {
        /// <summary>
        ///  unix时间戳
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        public static long Epoch(this DateTime d)
        {
            return (long)(d.ToUniversalTime() - new DateTime(1970, 1, 1)).TotalSeconds;
        }

        /// <summary>
        /// unix时间戳 转 DateTime
        /// </summary>
        /// <param name="epoch"></param>
        /// <returns></returns>
        public static DateTime FromEpoch(long epoch)
        {
            var d = new DateTime(1970, 1, 1);
            d = d.AddSeconds(epoch);
            return d.ToLocalTime();
        }
    }
}
