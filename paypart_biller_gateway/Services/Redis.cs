using System;
using System.Collections.Generic;
using System.Threading.Tasks;
//using StackExchange.Redis;
using paypart_biller_gateway.Models;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Caching.Distributed;
using System.Threading;

namespace paypart_biller_gateway.Services
{
    public class Redis
    {
        IOptions<Settings> settings;
        IDistributedCache redis;
        public delegate void SetBiller(string key, Biller billers);
        public delegate void SetBillers(string key, IEnumerable<Biller> billers);

        public Redis(IOptions<Settings> _settings, IDistributedCache _redis)
        {
            settings = _settings;
            redis = _redis;
        }
        public async Task<Biller> getbiller(string key, CancellationToken ctx)
        {
            Biller billers = new Biller();
            try
            {
                var biller = await redis.GetStringAsync(key, ctx);
                billers = JsonHelper.fromJson<Biller>(biller);
            }
            catch (Exception)
            {

                //throw;
            }
            return billers;
        }

        public async Task<List<Biller>> getbillers(string key, CancellationToken ctx)
        {
            List<Biller> billers = new List<Biller>();
            try
            {
                var biller = await redis.GetStringAsync(key, ctx);
                billers = JsonHelper.fromJson<List<Biller>>(biller);
            }
            catch (Exception)
            {

                //throw;
            }
            return billers;
        }

        public async void setbiller(string key,Biller billers)
        {
            try
            {
                var biller = await redis.GetStringAsync(key);
                if (!string.IsNullOrEmpty(biller))
                {
                    redis.Remove(key);
                }
                string value = JsonHelper.toJson(billers);

                await redis.SetStringAsync(key,value);
            }
            catch (Exception)
            {

                //throw;
            }

        }
        public async Task setbillerAsync(string key, Biller billers,CancellationToken cts)
        {
            try
            {
                var biller = await redis.GetStringAsync(key);
                if (!string.IsNullOrEmpty(biller))
                {
                    redis.Remove(key);
                }
                string value = JsonHelper.toJson(billers);

                await redis.SetStringAsync(key, value, cts);
            }
            catch (Exception ex)
            {

                //throw;
            }

        }
        public async Task setbillers(string key, List<Biller> billers,CancellationToken cts)
        {
            try
            {
                var biller = await redis.GetStringAsync(key);
                if (!string.IsNullOrEmpty(biller))
                {
                    redis.Remove(key);
                }
                string value = JsonHelper.toJson(billers);

                await redis.SetStringAsync(key, value, cts);
            }
            catch (Exception)
            {

                //throw;
            }

        }
    }
}
