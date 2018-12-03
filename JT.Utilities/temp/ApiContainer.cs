using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;

namespace JT.Infrastructure
{
    /*****************************************************
    * Author         JT
    * LastModified   20160531
    * Description    
    * Dependencies   Log4Net
    *******************************************************/

    public class ApiContainer
    {
        private HttpClient _client;

        #region timeOut
        private TimeSpan _defaultTimeout = new TimeSpan(0, 3, 0);
        public TimeSpan DefaultTimeout
        {
            get { return _defaultTimeout; }
            set { value = _defaultTimeout; }
        }
        #endregion

        #region retry setting
        private int _retryTick = 5;
        public int RetryTick
        {
            get { return _retryTick; }
            set { value = _retryTick; }
        }

        private bool _isAutoRetry = true;
        public bool IsAutoRetry
        {
            get { return _isAutoRetry; }
            set { value = _isAutoRetry; }
        }
        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="baseAddress">base address of Uniform Resource Identifier (URI) of the
        /// Internet resource used when sending requests</param>
        /// <param name="accessToken"></param>
        public ApiContainer(string baseAddress, string accessToken)
        {
            _client = new HttpClient();
            _client.Timeout = _defaultTimeout;
            _client.BaseAddress = new Uri(baseAddress);
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            if (string.IsNullOrEmpty(accessToken))
                throw new ArgumentNullException("accessToken");

            var byteArray = System.Text.Encoding.ASCII.GetBytes(accessToken);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
        }
        public ApiContainer(string baseAddress)
        {
            _client = new HttpClient();
            _client.Timeout = _defaultTimeout;
            _client.BaseAddress = new Uri(baseAddress);
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public ApiResult<T> Get<T>(string requestUri)
        {
            return TryCatch<T>(() =>
            {
                return _client.GetAsync(requestUri).Result;
            });
        }

        public ApiResult<T> Get<T>(string requestUri, object param)
        {
            return Post<T>(requestUri, param);
        }

        public ApiResult<object> Post(string requestUri, object param)
        {
            return TryCatch<object>(() =>
            {
                //_client.PostAsync(requestUri, new StringContent(param));
                return _client.PostAsJsonAsync(requestUri, param).Result;
            });
        }

        public ApiResult<T> Post<T>(string requestUri, object param)
        {
            return TryCatch<T>(() =>
            {
                return _client.PostAsJsonAsync(requestUri, param).Result;
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="requestUri">The Uri the request is sent to</param>
        /// <param name="path">A absolute path for the file</param>
        /// <returns>The absolute path for the saved file in server</returns>
        public ApiResult<string> UploadFile(string requestUri, string path)
        {
            return TryCatch<string>(() =>
            {
                using (var form = new MultipartFormDataContent())
                {
                    using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read))
                    {
                        using (var fileContent = new StreamContent(stream))
                        {
                            fileContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                            {
                                FileName = path,
                                DispositionType = DispositionTypeNames.Attachment,
                            };

                            form.Add(fileContent);

                            return _client.PostAsync(requestUri, form).Result;
                        }
                    }
                }
            });
        }


        private ApiResult<T> ApiResultHandler<T>(HttpResponseMessage response)
        {
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return response.Content.ReadAsAsync<ApiResult<T>>().Result;
            }
            else
            {
                return new ApiResult<T>()
                {
                    IsSuccess = false,
                    StatusCode = response.StatusCode,
                    Message = string.Format("{0}: {1}", response.StatusCode.ToString(), response.Content),
                };
            }
        }
        //private ApiResult<object> ApiResultHandler(HttpResponseMessage response)
        //{
        //    return ApiResultHandler<object>(response);

        //    //if (response.StatusCode == System.Net.HttpStatusCode.OK)
        //    //{
        //    //    return response.Content.ReadAsAsync<ApiResult>().Result;
        //    //}
        //    //else
        //    //{
        //    //    return new ApiResult()
        //    //    {
        //    //        IsSuccess = false,
        //    //        StatusCode = response.StatusCode,
        //    //        Message = string.Format("{0}: {1}", response.StatusCode.ToString(), response.Content),
        //    //    };
        //    //}
        //}

        //private ApiResult TryCatch(Func<HttpResponseMessage> func)
        //{
        //    try
        //    {
        //        var result = ApiResultHandler(func.Invoke());
        //        if (!result.IsSuccess 
        //            && result.StatusCode != System.Net.HttpStatusCode.NotFound 
        //            && _retryTick > 0)
        //        {
        //            result = ApiResultHandler(func.Invoke());
        //            _retryTick--;
        //        }

        //        return result;
        //    }
        //    catch (AggregateException ex)
        //    {
        //        Logger.Log4Net.Error(ex);
        //        return new ApiResult()
        //        {
        //            IsSuccess = false,
        //            Message = string.Join(" ", ex.InnerExceptions.Select(p => p.Message))
        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        Logger.Log4Net.Error(ex);
        //        return new ApiResult()
        //        {
        //            IsSuccess = false,
        //            Message = ex.Message
        //        };
        //    }
        //}
        private ApiResult<T> TryCatch<T>(Func<HttpResponseMessage> func)
        {
            try
            {
                var result = ApiResultHandler<T>(func.Invoke());
                if (!result.IsSuccess
                    && result.StatusCode != System.Net.HttpStatusCode.NotFound
                    && _retryTick > 0)
                {
                    result = ApiResultHandler<T>(func.Invoke());
                    _retryTick--;
                }

                return result;
            }
            catch (AggregateException ex)
            {
                //Logger.Error(ex);
                return new ApiResult<T>()
                {
                    IsSuccess = false,
                    Message = string.Join(" ", ex.InnerExceptions.Select(p => p.Message))
                };
            }
            catch (Exception ex)
            {
                //Logger.Error(ex);
                return new ApiResult<T>()
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }
    }


    #region model
    public class ApiResult<T>
    {
        private bool _isSuccess = true;
        public bool IsSuccess
        {
            get
            {
                return _isSuccess;
            }
            set
            {
                _isSuccess = value;
            }
        }
        private System.Net.HttpStatusCode _statusCode = System.Net.HttpStatusCode.OK;
        public System.Net.HttpStatusCode StatusCode
        {
            get { return _statusCode; }
            set { _statusCode = value; }
        }
        public string Message { get; set; }
        public T Data { get; set; }
    }
    //public class ApiResult : ApiResult<object>
    //{

    //}
    #endregion
}
