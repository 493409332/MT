using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using MT.Common.JsonUtility;
using System.Diagnostics;
using MT.Common.ConfigUtility;
using log4net.Repository;
using log4net.Config;
using System.IO;
using log4net;
using MT.Common.Log4Utility;
using System.Threading.Tasks;

namespace MT.Redis
{
    /// <summary>
    /// redis帮助类 注意集群无法同时操作多个key
    /// </summary>
    public class RedisHelper
    {
        #region redis初始化
        /// <summary>
        /// redis配置
        /// </summary>
        class RedisConfig
        {
            public string Type { get; set; }
            public string ConnectionString { get; set; }
            public string ConnectionStrings { get; set; }
        }

        public Lazy<ConnectionMultiplexer> lazyConnection = null;
        //redis链接字符串 
        private static readonly RedisConfig RedisConnectionString = ConfigurationHelper.GetConfiguration<RedisConfig>("redis.json", ConfigurationType.JSON);

        public static readonly RedisHelper Instance =new RedisHelper();

        public RedisHelper()
        {
            if (RedisConnectionString.Type.Equals("cluster"))
            {
                LazyRedis(RedisConnectionString.ConnectionStrings);
            }
            else
            {
                LazyRedis(RedisConnectionString.ConnectionString);
            }
        }

        public void LazyRedis(string ConnectionString)
        {
            lazyConnection = new Lazy<ConnectionMultiplexer>(() => { return RedisConnect(ConnectionString); });

        }
        public ConnectionMultiplexer RedisConnect(string ConnectionString)
        {
            var connect = ConnectionMultiplexer.Connect(ConnectionString);
            connect.ConnectionFailed += MuxerConnectionFailed;
            connect.ConnectionRestored += MuxerConnectionRestored;
            connect.ErrorMessage += MuxerErrorMessage;
            connect.ConfigurationChanged += MuxerConfigurationChanged;
            connect.HashSlotMoved += MuxerHashSlotMoved;
            connect.InternalError += MuxerInternalError;
            return connect;
        }

        public void LazyRedis(params string[] ConnectionString)
        {

#if !DEBUG
            foreach (var item in ConnectionString)
            {
                lazyConnection = new Lazy<ConnectionMultiplexer>(() => { return RedisConnect(item); });
                if (RedisDB.IsConnected(""))
                    return;
            }
#endif
#if DEBUG

            foreach (var item in ConnectionString)
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();
                lazyConnection = new Lazy<ConnectionMultiplexer>(() => { return ConnectionMultiplexer.Connect(item); });

                sw.Stop();
                TimeSpan ts2 = sw.Elapsed;
                sw.Restart();




                if (RedisDB.IsConnected(""))
                {
                    sw.Stop();
                    ts2 = sw.Elapsed;
                    return;
                }
                sw.Stop();
                ts2 = sw.Elapsed;
            }
#endif
        }

        /// <summary>
        /// redis连接接口
        /// </summary>
        public IDatabase RedisDB
        {
            get
            {
                return Instance.lazyConnection.Value.GetDatabase();
            }
        }
        #endregion

        #region 事件

        /// <summary>
        /// 配置更改时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void MuxerConfigurationChanged(object sender, EndPointEventArgs e)
        {
            Log4Helper.Info("Configuration changed: " + e.EndPoint);
        }

        /// <summary>
        /// 发生错误时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void MuxerErrorMessage(object sender, RedisErrorEventArgs e)
        {
            Log4Helper.Error("ErrorMessage: " + e.Message);
        }

        /// <summary>
        /// 重新建立连接之前的错误
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void MuxerConnectionRestored(object sender, ConnectionFailedEventArgs e)
        {
            Log4Helper.Error("ConnectionRestored: " + e.EndPoint);
        }

        /// <summary>
        /// 连接失败 ， 如果重新连接成功你将不会收到这个通知
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void MuxerConnectionFailed(object sender, ConnectionFailedEventArgs e)
        {
            Log4Helper.Error("重新连接：Endpoint failed: " + e.EndPoint + ", " + e.FailureType + (e.Exception == null ? "" : (", " + e.Exception.Message)));
        }

        /// <summary>
        /// 更改集群
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void MuxerHashSlotMoved(object sender, HashSlotMovedEventArgs e)
        {
            Log4Helper.Info("HashSlotMoved:NewEndPoint" + e.NewEndPoint + ", OldEndPoint" + e.OldEndPoint);
        }

        /// <summary>
        /// redis类库错误
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void MuxerInternalError(object sender, InternalErrorEventArgs e)
        {
            Log4Helper.Error("InternalError:Message" + e.Exception.Message);

        }



        #endregion 事件

        #region 同步方法

        /// <summary>
        /// 设置
        /// </summary>
        /// <typeparam name="T">泛型类</typeparam>
        /// <param name="Key">redis key</param>
        /// <param name="t">保存值</param>
        /// <param name="ts">过期时间</param>
        /// <returns></returns>
        public bool Set<T>(string Key, T t, TimeSpan? ts = default(TimeSpan?))
        { 

            var tstr = typeof(T).Name == typeof(string).Name? t as string:JsonConvert.SerializeObject(t);
            return RedisDB.StringSet(Key, tstr, ts);
        }
        /// <summary>
        /// 保存多个key value 集群无法使用改方法
        /// </summary>
        /// <param name="keyValues">键值对</param>
        /// <returns></returns>
        public bool Set(List<KeyValuePair<RedisKey, RedisValue>> keyValues)
        { 
            return RedisDB.StringSet(keyValues.ToArray());
        }


        /// <summary>
        /// 获取一个Key的对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Key">Key</param>
        /// <returns></returns>
        public T Get<T>(string Key) where T : class
        {
            var redisValue = RedisDB.StringGet(Key);
            var strValue = redisValue.ToString();

            if (!redisValue.IsNullOrEmpty)
            {
                if (JsonHelper.IsJson(strValue))
                {
                    return JsonConvert.DeserializeObject<T>(strValue);
                }
                return strValue as T;
            }
            return null;

        }
        /// <summary>
        /// 获取多个Key
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="KeyList">Redis Key集合</param>
        /// <returns></returns>
        public List<T> Get<T>(List<string> KeyList) where T : class
        {
            var quer=  KeyList.Select(redisKey => (RedisKey)redisKey).ToArray();
            var RedisValues = RedisDB.StringGet(quer);
            List<T> list = new List<T>();
            foreach (var item in RedisValues)
            {
                var strvalue = item.ToString();
                if (!item.IsNullOrEmpty)
                {
                    if (JsonHelper.IsJson(strvalue))
                    {
                        list.Add(JsonConvert.DeserializeObject<T>(strvalue));
                    }
                    else if (strvalue is T)
                    {
                        list.Add(strvalue as T);
                    }
                }
            }
            if (list.Count > 0)
            {
                return list;
            }
            return null;
        }
        /// <summary>
        /// 为数字增长val
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="val">可以为负</param>
        /// <returns>增长后的值</returns>
        public double Increment(string key, double val = 1)
        {
            return RedisDB.StringIncrement(key, val);
        }
        /// <summary>
        /// 为数字减少val
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="val">可以为负</param>
        /// <returns>减少后的值</returns>
        public double Decrement(string key, double val = 1)
        {
            return RedisDB.StringDecrement(key, val);
        }
        #endregion

        #region 异步方法

        /// <summary>
        /// 保存一个对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">Redis Key</param>
        /// <param name="value">保存的值</param>
        /// <param name="expiry">过期时间</param>
        /// <returns></returns>
        public async Task<bool> SetAsync<T>(string key, T t, TimeSpan? expiry = default(TimeSpan?))
        {
            var tstr = typeof(T).Name == typeof(string).Name ? t as string : JsonConvert.SerializeObject(t);
            return await RedisDB.StringSetAsync(key, tstr, expiry);
        }

        /// <summary>
        /// 保存多个key value
        /// </summary>
        /// <param name="keyValues">键值对</param>
        /// <returns></returns>
        public async Task<bool> SetAsync(List<KeyValuePair<RedisKey, RedisValue>> keyValues)
        {
            List<KeyValuePair<RedisKey, RedisValue>> newkeyValues =
                keyValues.Select(p => new KeyValuePair<RedisKey, RedisValue>(p.Key, p.Value)).ToList();
            return await RedisDB.StringSetAsync(newkeyValues.ToArray());
        }


        /// <summary>
        /// 获取多个Key
        /// </summary>
        /// <param name="listKey">Redis Key集合</param>
        /// <returns></returns>
        public async Task<List<T>> GetAsync<T>(List<string> listKey) where T : class
        {
            var RedisValues = await RedisDB.StringGetAsync(listKey.Select(redisKey => (RedisKey)redisKey).ToArray());
            List<T> list = new List<T>();
            foreach (var item in RedisValues)
            {
                var strvalue = item.ToString();
                if (!item.IsNullOrEmpty)
                {
                    if (JsonHelper.IsJson(strvalue))
                    {
                        list.Add(JsonConvert.DeserializeObject<T>(strvalue));
                    }
                    else if (strvalue is T)
                    {
                        list.Add(strvalue as T);
                    }
                }
            }
            if (list.Count > 0)
            {
                return list;
            }
            return null;
        }

        /// <summary>
        /// 获取一个Key的对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Key">Key</param>
        /// <returns></returns>
        public async Task<T> GetAsync<T>(string Key) where T : class
        {
            var redisValue = await RedisDB.StringGetAsync(Key);
            var strValue = redisValue.ToString();
            if (!redisValue.IsNullOrEmpty)
            {
                if (JsonHelper.IsJson(strValue))
                {
                    return JsonConvert.DeserializeObject<T>(strValue);
                }
                return strValue as T;
            }
            return null;

        }


        /// <summary>
        /// 为数字增长val
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="val">可以为负</param>
        /// <returns>增长后的值</returns>
        public async Task<double> IncrementAsync(string key, double val = 1)
        {
            return await RedisDB.StringIncrementAsync(key, val);
        }
        /// <summary>
        /// 为数字减少val
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="val">可以为负</param>
        /// <returns>减少后的值</returns>
        public async Task<double> DecrementAsync(string key, double val = 1)
        {
            return await RedisDB.StringDecrementAsync(key, val);
        }

        #endregion

        #region Hash
        #region 同步方法
        /// <summary>
        /// 判断某个数据是否已经被缓存
        /// </summary>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <returns></returns>
        public bool HashExists(string key, string dataKey)
        { 
            return RedisDB.HashExists(key, dataKey);
        }

        /// <summary>
        /// 存储数据到hash表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public bool HashSet<T>(string key, string dataKey, T t)
        { 
            var tstr = typeof(T).Name == typeof(string).Name ? t as string : JsonConvert.SerializeObject(t);
            return RedisDB.HashSet(key, dataKey, tstr); 
        }

        /// <summary>
        /// 移除hash中的某值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <returns></returns>
        public bool HashDelete(string key, string dataKey)
        { 
            return RedisDB.HashDelete(key, dataKey);
        }

        /// <summary>
        /// 移除hash中的多个值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="dataKeys"></param>
        /// <returns></returns>
        public long HashDelete(string key, List<RedisValue> dataKeys)
        { 
            return RedisDB.HashDelete(key, dataKeys.ToArray());
        }

        /// <summary>
        /// 从hash表获取数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <returns></returns>
        public T HashGet<T>(string key, string dataKey)
        { 
            string value = RedisDB.HashGet(key, dataKey);
            return JsonConvert.DeserializeObject<T>(value); 
        }

        /// <summary>
        /// 为数字增长val
        /// </summary>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <param name="val">可以为负</param>
        /// <returns>增长后的值</returns>
        public double HashIncrement(string key, string dataKey, double val = 1)
        { 
            return RedisDB.HashIncrement(key, dataKey, val);
        }

        /// <summary>
        /// 为数字减少val
        /// </summary>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <param name="val">可以为负</param>
        /// <returns>减少后的值</returns>
        public double HashDecrement(string key, string dataKey, double val = 1)
        { 
            return RedisDB.HashDecrement(key, dataKey, val);
        }

        /// <summary>
        /// 获取hashkey所有Redis key
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public List<T> HashKeys<T>(string key)
        {
            RedisValue[] values = RedisDB.HashKeys(key);
            return ConvetList<T>(values); 
        }
        #endregion

        #region 异步方法
        /// <summary>
        /// 判断某个数据是否已经被缓存
        /// </summary>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <returns></returns>
        public async Task<bool> HashExistsAsync(string key, string dataKey)
        {
            return await RedisDB.HashExistsAsync(key, dataKey);
        }

        /// <summary>
        /// 存储数据到hash表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public async Task<bool> HashSetAsync<T>(string key, string dataKey, T t)
        { 
            var tstr = typeof(T).Name == typeof(string).Name ? t as string : JsonConvert.SerializeObject(t);
            return await RedisDB.HashSetAsync(key, dataKey, tstr);
        }

        /// <summary>
        /// 移除hash中的某值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <returns></returns>
        public async Task<bool> HashDeleteAsync(string key, string dataKey)
        {
            return await RedisDB.HashDeleteAsync(key, dataKey);
        }

        /// <summary>
        /// 移除hash中的多个值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="dataKeys"></param>
        /// <returns></returns>
        public async Task<long>  HashDeleteAsync(string key, List<RedisValue> dataKeys)
        {
            return await RedisDB.HashDeleteAsync(key, dataKeys.ToArray());
        }

        /// <summary>
        /// 从hash表获取数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <returns></returns>
        public async Task<T>  HashGetAsync<T>(string key, string dataKey)
        {
            string value = await RedisDB.HashGetAsync(key, dataKey);
            return  JsonConvert.DeserializeObject<T>(value);
        }

        /// <summary>
        /// 为数字增长val
        /// </summary>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <param name="val">可以为负</param>
        /// <returns>增长后的值</returns>
        public async Task<double>   HashIncrementAsync(string key, string dataKey, double val = 1)
        {
            return await RedisDB.HashIncrementAsync(key, dataKey, val);
        }

        /// <summary>
        /// 为数字减少val
        /// </summary>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <param name="val">可以为负</param>
        /// <returns>减少后的值</returns>
        public async Task<double> HashDecrementAsync(string key, string dataKey, double val = 1)
        {
            return await RedisDB.HashDecrementAsync(key, dataKey, val);
        }

        /// <summary>
        /// 获取hashkey所有Redis key
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<List<T>> HashKeysAsync<T>(string key)
        {
            RedisValue[] values = await RedisDB.HashKeysAsync(key);
            return ConvetList<T>(values);
        }

        #endregion
        #endregion

        #region List

        #region 同步方法

        /// <summary>
        /// 移除指定ListId的内部List的值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public long ListRemove<T>(string key, T t)
        {
            var tstr = typeof(T).Name == typeof(string).Name ? t as string : JsonConvert.SerializeObject(t);
            return RedisDB.ListRemove(key,tstr);
        }

        /// <summary>
        /// 获取指定key的List
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public List<T> ListRange<T>(string key)
        { 
            var values = RedisDB.ListRange(key);
            return ConvetList<T>(values); 
        }

        /// <summary>
        /// 入队
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public long ListRightPush<T>(string key, T t)
        {
            var tstr = typeof(T).Name == typeof(string).Name ? t as string : JsonConvert.SerializeObject(t);
            return RedisDB.ListRightPush(key, tstr);
        }

        /// <summary>
        /// 出队
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public T ListRightPop<T>(string key)
        { 
            var value = RedisDB.ListRightPop(key);
            return JsonConvert.DeserializeObject<T>(value); 
        }

        /// <summary>
        /// 入栈
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public long ListLeftPush<T>(string key, T t)
        {
            var tstr = typeof(T).Name == typeof(string).Name ? t as string : JsonConvert.SerializeObject(t);
            return RedisDB.ListLeftPush(key, tstr);
        }

        /// <summary>
        /// 出栈
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public T ListLeftPop<T>(string key)
        { 
                var value = RedisDB.ListLeftPop(key);
                return JsonConvert.DeserializeObject<T>(value); 
        }

        /// <summary>
        /// 获取集合中的数量
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public long ListLength(string key)
        { 
            return RedisDB.ListLength(key);
        }

        #endregion 同步方法

        #region 异步方法

        /// <summary>
        /// 移除指定ListId的内部List的值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public async Task<long> ListRemoveAsync<T>(string key, T t)
        {
            var tstr = typeof(T).Name == typeof(string).Name ? t as string : JsonConvert.SerializeObject(t);
            return await RedisDB.ListRemoveAsync(key,tstr);
        }

        /// <summary>
        /// 获取指定key的List
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<List<T>> ListRangeAsync<T>(string key)
        {
             
            var values = await RedisDB.ListRangeAsync(key);
            return ConvetList<T>(values);
        }

        /// <summary>
        /// 入队
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public async Task<long> ListRightPushAsync<T>(string key, T t)
        {
            var tstr = typeof(T).Name == typeof(string).Name ? t as string : JsonConvert.SerializeObject(t);
            return await RedisDB.ListRightPushAsync(key,tstr);
        }

        /// <summary>
        /// 出队
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<T> ListRightPopAsync<T>(string key)
        {
             
            var value = await RedisDB.ListRightPopAsync(key);
            return JsonConvert.DeserializeObject<T>(value);
        }

        /// <summary>
        /// 入栈
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public async Task<long> ListLeftPushAsync<T>(string key, T t)
        {
            var tstr = typeof(T).Name == typeof(string).Name ? t as string : JsonConvert.SerializeObject(t);
            return await RedisDB.ListLeftPushAsync(key, tstr);
        }

        /// <summary>
        /// 出栈
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<T> ListLeftPopAsync<T>(string key)
        {
             
            var value = await RedisDB.ListLeftPopAsync(key);
            return JsonConvert.DeserializeObject<T>(value);
        }

        /// <summary>
        /// 获取集合中的数量
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<long> ListLengthAsync(string key)
        {
             
            return await RedisDB.ListLengthAsync(key);
        }

        #endregion 异步方法

        #endregion List

        #region SortedSet 有序集合

        #region 同步方法

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="score"></param>
        public bool SortedSetAdd<T>(string key, T t, double score)
        {
            var tstr = typeof(T).Name == typeof(string).Name ? t as string : JsonConvert.SerializeObject(t);
            return RedisDB.SortedSetAdd(key,tstr, score);
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public bool SortedSetRemove<T>(string key, T t)
        {
            var tstr = typeof(T).Name == typeof(string).Name ? t as string : JsonConvert.SerializeObject(t);
            return RedisDB.SortedSetRemove(key, tstr);
        }

        /// <summary>
        /// 获取全部
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public List<T> SortedSetRangeByRank<T>(string key)
        { 
                var values = RedisDB.SortedSetRangeByRank(key);
                return ConvetList<T>(values);
          
        }

        /// <summary>
        /// 获取集合中的数量
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public long SortedSetLength(string key)
        {
            
            return RedisDB.SortedSetLength(key);
        }

        #endregion 同步方法

        #region 异步方法

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="score"></param>
        public async Task<bool> SortedSetAddAsync<T>(string key, T t, double score)
        {
            var tstr = typeof(T).Name == typeof(string).Name ? t as string : JsonConvert.SerializeObject(t);
            return await RedisDB.SortedSetAddAsync(key, tstr, score);
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public async Task<bool> SortedSetRemoveAsync<T>(string key, T t)
        {
            var tstr = typeof(T).Name == typeof(string).Name ? t as string : JsonConvert.SerializeObject(t);
            return await RedisDB.SortedSetRemoveAsync(key,tstr);
        }

        /// <summary>
        /// 获取全部
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<List<T>> SortedSetRangeByRankAsync<T>(string key)
        {
            
            var values = await RedisDB.SortedSetRangeByRankAsync(key);
            return ConvetList<T>(values);
        }

        /// <summary>
        /// 获取集合中的数量
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<long> SortedSetLengthAsync(string key)
        {
            
            return await RedisDB.SortedSetLengthAsync(key);
        }

        #endregion 异步方法

        #endregion SortedSet 有序集合

        #region key

        /// <summary>
        /// 删除单个key
        /// </summary>
        /// <param name="key">redis key</param>
        /// <returns>是否删除成功</returns>
        public bool KeyDelete(string key)
        { 
            return RedisDB.KeyDelete(key);
        }

        /// <summary>
        /// 删除多个key
        /// </summary>
        /// <param name="keys">rediskey</param>
        /// <returns>成功删除的个数</returns>
        public long KeyDelete(List<string> keys)
        { 
            return RedisDB.KeyDelete(ConvertRedisKeys(keys));
        }

        /// <summary>
        /// 判断key是否存储
        /// </summary>
        /// <param name="key">redis key</param>
        /// <returns></returns>
        public bool KeyExists(string key)
        {
            
            return RedisDB.KeyExists(key);
        }

        /// <summary>
        /// 重新命名key
        /// </summary>
        /// <param name="key">就的redis key</param>
        /// <param name="newKey">新的redis key</param>
        /// <returns></returns>
        public bool KeyRename(string key, string newKey)
        {
            
            return RedisDB.KeyRename(key, newKey);
        }

        /// <summary>
        /// 设置Key的时间
        /// </summary>
        /// <param name="key">redis key</param>
        /// <param name="expiry"></param>
        /// <returns></returns>
        public bool KeyExpire(string key, TimeSpan? expiry = default(TimeSpan?))
        {
            
            return RedisDB.KeyExpire(key, expiry);
        }

        #endregion key

        #region 发布订阅

        /// <summary>
        /// Redis发布订阅  订阅
        /// </summary>
        /// <param name="subChannel"></param>
        /// <param name="handler"></param>
        public void Subscribe(string subChannel, Action<RedisChannel, RedisValue> handler = null)
        {
            ISubscriber sub = lazyConnection.Value.GetSubscriber();
            sub.Subscribe(subChannel, (channel, message) =>
            {
                if (handler == null)
                { 
                    Log4Helper.Info(subChannel + " 订阅收到消息：" + message);
                }
                else
                {
                    handler(channel, message);
                }
            });
        }

        /// <summary>
        /// Redis发布订阅  发布
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="channel"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public long Publish<T>(string channel, T t)
        {
            ISubscriber sub = lazyConnection.Value.GetSubscriber();
            var tstr = typeof(T).Name == typeof(string).Name ? t as string : JsonConvert.SerializeObject(t);
            return sub.Publish(channel, tstr);
        }

        /// <summary>
        /// Redis发布订阅  取消订阅
        /// </summary>
        /// <param name="channel"></param>
        public void Unsubscribe(string channel)
        {
            ISubscriber sub = lazyConnection.Value.GetSubscriber();
            sub.Unsubscribe(channel);
        }

        /// <summary>
        /// Redis发布订阅  取消全部订阅
        /// </summary>
        public void UnsubscribeAll()
        {
            ISubscriber sub = lazyConnection.Value.GetSubscriber();
            sub.UnsubscribeAll();
        }

        #endregion 发布订阅
         
        #region 辅助方法
        public string CustomKey;


        private List<T> ConvetList<T>(RedisValue[] values)
        {
            List<T> result = new List<T>();
            foreach (var item in values)
            {
                var model = JsonConvert.DeserializeObject<T>(item);
                result.Add(model);
            }
            return result;
        }



        private RedisKey[] ConvertRedisKeys(List<string> redisKeys)
        {
            return redisKeys.Select(redisKey => (RedisKey)redisKey).ToArray();
        }
        public ITransaction CreateTransaction()
        {
            return RedisDB.CreateTransaction();
        }

    
        public IServer GetServer(string hostAndPort)
        {
            return lazyConnection.Value.GetServer(hostAndPort);
        }
        #endregion 辅助方法

    } 
}
