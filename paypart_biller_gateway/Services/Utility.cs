using Microsoft.Extensions.Options;
using paypart_biller_gateway.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace paypart_biller_gateway.Services
{
    public enum Status
    {
        Pending,
        Active,
        InActive
    }
    public class Utility
    {
        private readonly IOptions<Settings> settings;
        public Utility(IOptions<Settings> _settings)
        {
            settings = _settings;
        }
        public async Task<IEnumerable<Biller>> getBillers(string key, CancellationToken ctx)
        {
            IEnumerable<Biller> billers = new List<Biller>();
            try
            {
                //billers = await 
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
            }
            return billers;
        }
    }
}
