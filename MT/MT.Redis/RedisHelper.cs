using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;
using MT.Common.Json;
namespace MT.Redis
{
    public class RedisHelper
    {
        private Lazy<ConnectionMultiplexer> lazyConnection;

        public RedisHelper(string ConnectionString= "localhost,abortConnect=false")
        {
            lazyConnection = new Lazy<ConnectionMultiplexer>(() => { return ConnectionMultiplexer.Connect(ConnectionString); });
        } 
    
        public IDatabase RedisDB
        {
            get
            {
                return lazyConnection.Value.GetDatabase();
            }
        }
         
        public bool Set<T>(string Key,T t,TimeSpan ts ) {

            var tstr = JsonConvert.SerializeObject(t);  
            return RedisDB.StringSet(Key,tstr, ts);
        }
        public bool Set<T>(string Key, T t )
        {

            var tstr = JsonConvert.SerializeObject(t); 
            return RedisDB.StringSet(Key, tstr);
        }

        public T Get<T>(string Key) where T : class
        {
            var strValue = RedisDB.StringGet(Key).ToString();

            if (!string.IsNullOrEmpty(strValue))
            {
                if (JsonHelper.IsJson(strValue))
                {
                    return JsonConvert.DeserializeObject<T>(strValue);
                }
                return strValue as T;
            }
            return null;

        }

        //public static string ListKeyName = "MessageList";
        //public void HomeController()
        //{
        //   var db = Connection.GetDatabase();
        //    if (db.IsConnected(ListKeyName) && (!db.KeyExists(ListKeyName) || !db.KeyType(ListKeyName).Equals(RedisType.List)))
        //    {
        //        //Add sample data.
        //        db.KeyDelete(ListKeyName);
        //        //Push data from the left
        //        db.ListLeftPush(ListKeyName, "TestMsg1");
        //        db.ListLeftPush(ListKeyName, "TestMsg2");
        //        db.ListLeftPush(ListKeyName, "TestMsg3");
        //        db.ListLeftPush(ListKeyName, "TestMsg4");
        //    }

 

        //}

    }

   
}
