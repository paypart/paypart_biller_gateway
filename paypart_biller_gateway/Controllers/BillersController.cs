using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using paypart_biller_gateway.Models;
using paypart_biller_gateway.Services;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Caching.Distributed;
using System.Net;
using System.Threading;
using Microsoft.AspNetCore.Authorization;
using System.IO;

namespace paypart_biller_gateway.Controllers
{
    [Authorize]
    [Produces("application/json")]
    [Route("api/billers/[action]")]
    public class BillersController : Controller
    {
        private readonly IBillerMongoRepository billerMongoRepo;
        private readonly IBillerSqlServerRepository billerSqlRepo;

        IOptions<Settings> settings;
        IDistributedCache cache;

        public BillersController(IOptions<Settings> _settings, IBillerMongoRepository _billerMongoRepo
            , IBillerSqlServerRepository _billerSqlRepo, IDistributedCache _cache)
        {
            settings = _settings;
            billerMongoRepo = _billerMongoRepo;
            billerSqlRepo = _billerSqlRepo;
            cache = _cache;
        }

        [HttpGet()]
        [ProducesResponseType(typeof(List<Biller>), 200)]
        [ProducesResponseType(typeof(BillerError), 400)]
        [ProducesResponseType(typeof(BillerError), 500)]
        public async Task<IActionResult> getallbillers()
        {
            List<Biller> billers = null;
            BillerError e = new BillerError();

            CancellationTokenSource cts;
            cts = new CancellationTokenSource();
            cts.CancelAfter(settings.Value.redisCancellationToken);

            Redis redis = new Redis(settings, cache);
            string key = "all_billers";

            //validate request
            if (!ModelState.IsValid)
            {
                var modelErrors = new List<BillerError>();
                var eD = new List<string>();
                foreach (var modelState in ModelState.Values)
                {
                    foreach (var modelError in modelState.Errors)
                    {
                        eD.Add(modelError.ErrorMessage);
                    }
                }
                e.error = ((int)HttpStatusCode.BadRequest).ToString();
                e.errorDetails = eD;

                return BadRequest(e);
            }

            //check redis cache for details
            try
            {
                billers = await redis.getbillers(key, cts.Token);

                if (billers != null && billers.Count > 0)
                {
                    return CreatedAtAction("getallbillers", billers);
                }
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
            }

            //Get Billers from Sql
            try
            {
                billers = await billerSqlRepo.GetAllBillers();
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
            }

            //Write to Redis
            try
            {
                if (billers != null && billers.Count > 0)
                    await redis.setbillers(key, billers, cts.Token);
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
            }
            return CreatedAtAction("getallbillers", billers);
        }


        [HttpGet("{category_id}")]
        [ProducesResponseType(typeof(List<Biller>), 200)]
        [ProducesResponseType(typeof(BillerError), 400)]
        [ProducesResponseType(typeof(BillerError), 500)]
        public async Task<IActionResult> getbillers(int category_id)
        {
            List<Biller> billers = null;
            BillerError e = new BillerError();

            CancellationTokenSource cts;
            cts = new CancellationTokenSource();
            cts.CancelAfter(settings.Value.redisCancellationToken);

            Redis redis = new Redis(settings, cache);
            string key = "billers_of_category_" + category_id;

            //validate request
            if (!ModelState.IsValid)
            {
                var modelErrors = new List<BillerError>();
                var eD = new List<string>();
                foreach (var modelState in ModelState.Values)
                {
                    foreach (var modelError in modelState.Errors)
                    {
                        eD.Add(modelError.ErrorMessage);
                    }
                }
                e.error = ((int)HttpStatusCode.BadRequest).ToString();
                e.errorDetails = eD;

                return BadRequest(e);
            }

            //check redis cache for details
            try
            {
                billers = await redis.getbillers(key, cts.Token);

                if (billers != null && billers.Count > 0)
                {
                    return CreatedAtAction("getbillers", billers);
                }
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
            }

            //Get billers from Mongo
            try
            {
                //billers = await billerMongoRepo.GetBillers(category_id.ToString());
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
            }

            //Get Billers from Sql
            try
            {
                billers = await billerSqlRepo.GetBillers(category_id.ToString());
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
            }

            //Write to Redis
            try
            {
                if (billers != null && billers.Count > 0)
                    await redis.setbillers(key, billers, cts.Token);
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
            }
            return CreatedAtAction("getbillers", billers);
        }

        [HttpPost]
        [ProducesResponseType(typeof(Biller), 200)]
        [ProducesResponseType(typeof(BillerError), 400)]
        [ProducesResponseType(typeof(BillerError), 500)]
        public async Task<IActionResult> addbiller([FromBody]Biller biller)
        {
            Biller _biller = null;

            BillerError e = new BillerError();
            Redis redis = new Redis(settings, cache);

            CancellationTokenSource cts;
            cts = new CancellationTokenSource();
            cts.CancelAfter(settings.Value.redisCancellationToken);

            biller.created_on = DateTime.Now;
            biller.status = (int)Status.Active;
            biller.billercontact.status = (int)Status.Active;

            //validate request
            if (!ModelState.IsValid)
            {
                var modelErrors = new List<BillerError>();
                var eD = new List<string>();
                foreach (var modelState in ModelState.Values)
                {
                    foreach (var modelError in modelState.Errors)
                    {
                        eD.Add(modelError.ErrorMessage);
                    }
                }
                e.error = ((int)HttpStatusCode.BadRequest).ToString();
                e.errorDetails = eD;

                return BadRequest(e);
            }

            //produce on kafka
            try
            {
                //string billerText = JsonHelper.toJson(_biller);
                //var config = new Dictionary<string, object> { { "bootstrap.servers", settings.Value.brokerList } };
                //using (var producer = new Producer<Null, string>(config, null, new StringSerializer(Encoding.UTF8)))
                //{
                //    var deliveryReport = await producer.ProduceAsync(settings.Value.addBillerTopic, null, billerText);
                //    var result = deliveryReport.Value;
                //    producer.Flush(TimeSpan.FromSeconds(1));

                //}
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
            }
            //return _biller;


            //Add to mongo
            try
            {
                //_biller = await billerMongoRepo.AddBiller(biller);
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
            }

            //Add Biller to sql server
            try
            {
                if (biller.id == 0)
                {
                    _biller = await billerSqlRepo.AddBiller(biller);
                }
                else if (biller.id > 0)
                {
                    _biller = await billerSqlRepo.UpdateBiller(biller);
                }
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);

            }

            //Add to redis
            try
            {
                if (_biller != null)
                {
                    string key = "billers_with_id_" + _biller.id;

                    await redis.setbillerAsync(key, _biller, cts.Token);
                }

            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
            }

            return CreatedAtAction("addbiller", biller);

        }

		[HttpPost("{logo}")]
        [ProducesResponseType(typeof(bool), 200)]
        [ProducesResponseType(typeof(BillerError), 400)]
        [ProducesResponseType(typeof(BillerError), 500)]
        public async Task<IActionResult> uploadlogo(string logo)
        {
            bool isUploaded = false;

            try
            {
                long size = 0;
                Console.Write("size before checking reqiuest.form.files " + size);
                var files = Request.Form.Files;
                Console.Write(files.Count);
                foreach (var file in files)
                {
                    //var filename = ContentDispositionHeaderValue
                    //                .Parse(file.ContentDisposition)
                    //                .FileName
                    //                .Trim('"');
                    string filename = settings.Value.hostingEnv + $@"\uploads\biller-logo\{logo}";

                    Console.Write("filemane: " + filename);
                    //filename = settings.Value.uploadPath + @"\" + logo;
                    size += file.Length;

                    Console.Write("size after checking reqiuest.form.files " + size);
                    using (FileStream fs = System.IO.File.Create(filename))
                    {
                        Console.Write("before file.copytoasync");
                        await file.CopyToAsync(fs);
                        Console.Write("after file.copytoasync");
                        fs.Flush();
                        isUploaded = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.Write(ex.ToString());
            }
            return CreatedAtAction("uploadlogo", isUploaded); 
        }
    }
}