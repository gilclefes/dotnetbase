using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dotnetbase.Application.Services
{
    public static class Utils
    {
        private static Random random = new Random();

        public static string GenerateOrderNumber()
        {
            var timestamp = DateTime.UtcNow.Ticks.ToString();
            var randomString = new string(Enumerable.Repeat("ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789", 12 - timestamp.Length)
                .Select(s => s[random.Next(s.Length)]).ToArray());

            return timestamp + randomString;
        }

        public static string GenerateEmailVerificationCode()
        {
            var timestamp = DateTime.UtcNow.Ticks.ToString();
            var randomString = new string(Enumerable.Repeat("ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789", 6 - timestamp.Length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
            return timestamp + randomString;
        }

        
    }
}