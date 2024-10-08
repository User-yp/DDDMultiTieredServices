﻿using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using StackExchange.Redis;
using Timer = System.Timers.Timer;
using System.Collections.Concurrent;
using System.Net;

namespace CacheServices.RedisService;

public class RedisService : IRedisService
{
    private readonly ConnectionMultiplexer _conn;
    private readonly IDatabase _db;

    public RedisService(IOptionsMonitor<RedisOptions> options) : this(options.CurrentValue)
    {
    }

    public RedisService(RedisOptions options)
    {
        var connectionString = options.ConnectionString;
        _conn = ConnectionMultiplexer.Connect(connectionString);

        var dbNumber = options.DbNumber;
        _db = _conn.GetDatabase(dbNumber);
    }


    #region String

    public async Task<bool> StringSetAsync<T>(string key, T value) =>
        await _db.StringSetAsync(key, value.ToRedisValue());

    public async Task<bool> StringSetAsync<T>(string key, T value, TimeSpan timeSpan) =>
        await _db.StringSetAsync(key, value.ToRedisValue(), timeSpan);

    public async Task<T> StringGetAsync<T>(string key)
        //where T : class 
        => (await _db.StringGetAsync(key)).ToObject<T>();

    public async Task<double> StringIncrementAsync(string key, int value = 1) =>
        await _db.StringIncrementAsync(key, value);

    public async Task<double> StringDecrementAsync(string key, int value = 1) =>
        await _db.StringDecrementAsync(key, value);

    #endregion

    #region List

    public async Task<long> EnqueueAsync<T>(string key, T value) =>
        await _db.ListRightPushAsync(key, value.ToRedisValue());

    public async Task<long> EnqueueorCreateAsync<T>(string key, T value)
    {
        if (!await KeyExistsAsync(key))
            return await EnqueueAsync(key, value);
        else
            return await _db.ListRightPushAsync(key, value.ToRedisValue());
    }
    public async Task<T> DequeueAsync<T>(string key) where T : class =>
        (await _db.ListLeftPopAsync(key)).ToObject<T>();

    public async Task<IEnumerable<T>> PeekRangeAsync<T>(string key, long start = 0, long stop = -1)
        where T : class =>
        (await _db.ListRangeAsync(key, start, stop)).ToObjects<T>();

    #endregion

    #region Set

    public async Task<bool> SetAddAsync<T>(string key, T value) =>
        await _db.SetAddAsync(key, value.ToRedisValue());

    public async Task<long> SetRemoveAsync<T>(string key, IEnumerable<T> values) =>
        await _db.SetRemoveAsync(key, values.ToRedisValues());

    public async Task<IEnumerable<T>> SetMembersAsync<T>(string key) where T : class =>
        (await _db.SetMembersAsync(key)).ToObjects<T>();

    public async Task<bool> SetContainsAsync<T>(string key, T value) =>
        await _db.SetContainsAsync(key, value.ToRedisValue());

    #endregion

    #region ZSet

    public async Task<bool> SortedSetAddAsync(string key, string member, double score) =>
        await _db.SortedSetAddAsync(key, member, score);

    public async Task<long> SortedSetRemoveAsync(string key, IEnumerable<string> members) =>
        await _db.SortedSetRemoveAsync(key, members.ToRedisValues());

    public async Task<double> SortedSetIncrementAsync(string key, string member, double value) =>
        await _db.SortedSetIncrementAsync(key, member, value);

    public async Task<double> SortedSetDecrementAsync(string key, string member, double value) =>
        await _db.SortedSetDecrementAsync(key, member, value);

    public async Task<ConcurrentDictionary<string, double>> SortedSetRangeByRankWithScoresAsync(string key,
        long start = 0,
        long stop = -1,
        Order order = Order.Ascending) =>
        (await _db.SortedSetRangeByRankWithScoresAsync(key, start, stop, order)).ToConcurrentDictionary();

    public async Task<ConcurrentDictionary<string, double>> SortedSetRangeByScoreWithScoresAsync(string key,
        double start = double.NegativeInfinity, double stop = double.PositiveInfinity,
        Exclude exclude = Exclude.None, Order order = Order.Ascending, long skip = 0, long take = -1) =>
        (await _db.SortedSetRangeByScoreWithScoresAsync(key, start, stop, exclude, order, skip, take))
        .ToConcurrentDictionary();

    #endregion

    #region Hash

    public async Task<ConcurrentDictionary<string, string>> HashGetAsync(string key) =>
        (await _db.HashGetAllAsync(key)).ToConcurrentDictionary();

    public async Task<ConcurrentDictionary<string, string>> HashGetFieldsAsync(string key, IEnumerable<string> fields) =>
        (await _db.HashGetAsync(key, fields.ToRedisValues())).ToConcurrentDictionary(fields);

    public async Task HashSetAsync(string key, ConcurrentDictionary<string, string> entries)
    {
        var val = entries.ToHashEntries();
        if (val != null)
            await _db.HashSetAsync(key, val);
    }

    public async Task HashSetAsync(string key, ConcurrentDictionary<string, string> entries, TimeSpan timeSpan)
    {
        var val = entries.ToHashEntries();
        if (val != null)
            await _db.HashSetAsync(key, val);
        await _db.KeyExpireAsync(key, timeSpan);
    }

    public async Task HashSetFieldsAsync(string key, ConcurrentDictionary<string, string> fields)
    {
        if (fields == null || fields.IsEmpty)
            return;

        var hs = await HashGetAsync(key);
        foreach (var field in fields)
        {
            //if(!hs.ContainsKey(field.Key))

            //    continue;

            hs[field.Key] = field.Value;
        }

        await HashSetAsync(key, hs);
    }

    public async Task HashSetFieldsAsync(string key, ConcurrentDictionary<string, string> fields, TimeSpan timeSpan)
    {
        if (fields == null || fields.IsEmpty)
            return;

        var hs = await HashGetAsync(key);
        foreach (var field in fields)
        {
            //if(!hs.ContainsKey(field.Key))

            //    continue;

            hs[field.Key] = field.Value;
        }
        await HashSetAsync(key, hs);
        await _db.KeyExpireAsync(key, timeSpan);
    }

    public async Task HashSetorCreateFieldsAsync(string key, ConcurrentDictionary<string, string> fields)
    {
        if (!await KeyExistsAsync(key))
            await HashSetAsync(key, fields);
        else
        {
            if (fields == null || fields.IsEmpty)
                return;

            var hs = await HashGetAsync(key);
            foreach (var field in fields)
            {
                //if(!hs.ContainsKey(field.Key))

                //    continue;

                hs[field.Key] = field.Value;
            }
            await HashSetAsync(key, hs);
        }
    }

    public async Task HashSetorCreateFieldsAsync(string key, ConcurrentDictionary<string, string> fields, TimeSpan timeSpan)
    {
        if (!await KeyExistsAsync(key))
            await HashSetAsync(key, fields);
        else
        {
            if (fields == null || fields.IsEmpty)
                return;

            var hs = await HashGetAsync(key);
            foreach (var field in fields)
            {
                /*if (!hs.ContainsKey(field.Key))
                    continue;*/

                hs[field.Key] = field.Value;
            }
            await HashSetAsync(key, hs);
            await _db.KeyExpireAsync(key, timeSpan);
        }
    }

    public async Task<bool> HashFieldsExistsAsync(string key, IEnumerable<string> fields)
    {
        if (!await KeyExistsAsync(key))
            return false;
        var dic = await HashGetFieldsAsync(key, fields);
        foreach (var field in fields)
        {
            if (dic[field] == null)
                return false;
        }
        return true;
    }
    public async Task<bool> HashDeleteAsync(string key) =>
        await KeyDeleteAsync(new string[] { key }) > 0;

    public async Task<bool> HashDeleteFieldsAsync(string key, IEnumerable<string> fields)
    {
        if (fields == null || !fields.Any())
            return false;

        var success = true;
        foreach (var field in fields)
        {
            if (!await _db.HashDeleteAsync(key, field))
                success = false;
        }
        return success;
    }

    #endregion

    #region Key

    public IEnumerable<string> GetAllKeys() =>
        _conn.GetEndPoints().Select(endPoint => _conn.GetServer(endPoint))
            .SelectMany(server => server.Keys().ToStrings());

    public IEnumerable<string> GetAllKeys(EndPoint endPoint) =>
        _conn.GetServer(endPoint).Keys().ToStrings();

    public async Task<bool> KeyExistsAsync(string key) =>
        await _db.KeyExistsAsync(key);

    public async Task<long> KeyDeleteAsync(IEnumerable<string> keys) =>
        await _db.KeyDeleteAsync(keys.Select(k => (RedisKey)k).ToArray());

    public async Task<bool> KeyDeleteAsync(string key) =>
        await _db.KeyDeleteAsync(key);

    public async Task<bool> DeleteAllKeyAsync()
    {
        var keys = GetAllKeys();
        if (keys == null || !keys.Any())
            return false;
        foreach (var key in keys)
        {
            if (!await KeyDeleteAsync(key))
                return false;
        }
        return true;
    }

    public async Task<bool> KeyExpireAsync(string key, TimeSpan? expiry) =>
        await _db.KeyExpireAsync(key, expiry);

    public async Task<bool> KeyExpireAsync(string key, DateTime? expiry) =>
        await _db.KeyExpireAsync(key, expiry);

    #endregion

    #region Advanced

    public async Task<long> PublishAsync(string channel, string msg) =>
        await _conn.GetSubscriber().PublishAsync(channel, msg);

    public async Task SubscribeAsync(string channel, Action<string, string> handler) =>
        await _conn.GetSubscriber().SubscribeAsync(channel, (chn, msg) => handler(chn, msg));

    public Task ExecuteBatchAsync(params Action[] operations) =>
        Task.Run(() =>
        {
            var batch = _db.CreateBatch();

            foreach (var operation in operations)
                operation();

            batch.Execute();
        });


    public async Task<(bool, object)> LockExecuteAsync(string key, string value, Delegate del,
        TimeSpan expiry, params object[] args)
    {
        if (!await _db.LockTakeAsync(key, value, expiry))
            return (false, null);

        try
        {
            return (true, del.DynamicInvoke(args));
        }
        finally
        {
            _db.LockRelease(key, value);
        }
    }


    public bool LockExecute(string key, string value, Delegate del, out object result, TimeSpan expiry,
        int timeout = 0, params object[] args)
    {
        result = null;
        if (!GetLock(key, value, expiry, timeout))
            return false;

        try
        {
            result = del.DynamicInvoke(args);
            return true;
        }
        finally
        {
            _db.LockRelease(key, value);
        }
    }

    public bool LockExecute(string key, string value, Action action, TimeSpan expiry, int timeout = 0)
    {
        return LockExecute(key, value, action, out var _, expiry, timeout);
    }

    public bool LockExecute<T>(string key, string value, Action<T> action, T arg, TimeSpan expiry, int timeout = 0)
    {
        return LockExecute(key, value, action, out var _, expiry, timeout, arg);
    }

    public bool LockExecute<T>(string key, string value, Func<T> func, out T result, TimeSpan expiry,
        int timeout = 0)
    {
        result = default;
        if (!GetLock(key, value, expiry, timeout))
            return false;
        try
        {
            result = func();
            return true;
        }
        finally
        {
            _db.LockRelease(key, value);
        }
    }

    public bool LockExecute<T, TResult>(string key, string value, Func<T, TResult> func, T arg, out TResult result,
        TimeSpan expiry, int timeout = 0)
    {
        result = default;
        if (!GetLock(key, value, expiry, timeout))
            return false;
        try
        {
            result = func(arg);
            return true;
        }
        finally
        {
            _db.LockRelease(key, value);
        }
    }

    private bool GetLock(string key, string value, TimeSpan expiry, int timeout)
    {
        using var waitHandle = new AutoResetEvent(false);
        var timer = new Timer(1000);
        timer.Elapsed += (s, e) =>
        {
            if (!_db.LockTake(key, value, expiry))
                return;
            try
            {
                waitHandle.Set();
                timer.Stop();
            }
            catch
            {
            }
        };
        timer.Start();


        if (timeout > 0)
            waitHandle.WaitOne(timeout);
        else
            waitHandle.WaitOne();

        timer.Stop();
        timer.Close();
        timer.Dispose();

        return _db.LockQuery(key) == value;
    }


    #endregion
}

public static class StackExchangeRedisExtension
{
    public static IEnumerable<string> ToStrings(this IEnumerable<RedisKey> keys)
    {
        var redisKeys = keys as RedisKey[] ?? keys.ToArray();
        return !redisKeys.Any() ? null : redisKeys.Select(k => (string)k);
    }

    public static RedisValue ToRedisValue<T>(this T value)
    {
        if (value == null)
            return RedisValue.Null;

        return value switch
        {
            ValueType => value.ToString(),
            string s => s,
            _ => JsonConvert.SerializeObject(value)
        };
    }


    public static RedisValue[] ToRedisValues<T>(this IEnumerable<T> values)
    {
        var enumerable = values as T[] ?? values.ToArray();
        return !enumerable.Any() ? null : enumerable.Select(v => v.ToRedisValue()).ToArray();
    }

    public static T ToObject<T>(this RedisValue value)
    //where T : class
    {
        if (!value.HasValue)
            return default;

        if (typeof(T).IsSubclassOf(typeof(ValueType)) || typeof(T) == typeof(string))
            return (T)Convert.ChangeType(value.ToString(), typeof(T));

        return JsonConvert.DeserializeObject<T>(value.ToString());
    }

    public static IEnumerable<T> ToObjects<T>(this IEnumerable<RedisValue> values) where T : class
    {
        var redisValues = values as RedisValue[] ?? values.ToArray();
        return !redisValues.Any() ? null : redisValues.Select(v => v.ToObject<T>());
    }

    public static HashEntry[] ToHashEntries(this ConcurrentDictionary<string, string> entries)
    {
        if (entries == null || !entries.Any())
            return null;

        var es = new HashEntry[entries.Count];
        for (var i = 0; i < entries.Count; i++)
        {
            var name = entries.Keys.ElementAt(i);
            var value = entries[name];
            es[i] = new HashEntry(name, value);
        }

        return es;
    }

    public static ConcurrentDictionary<string, string> ToConcurrentDictionary(this IEnumerable<HashEntry> entries)
    {
        var hashEntries = entries as HashEntry[] ?? entries.ToArray();
        if (!hashEntries.Any())
            return null;


        var dict = new ConcurrentDictionary<string, string>();
        foreach (var entry in hashEntries)
            dict[entry.Name] = entry.Value;

        return dict;
    }

    public static ConcurrentDictionary<string, string> ToConcurrentDictionary(this RedisValue[] hashValues,
        IEnumerable<string> fields)
    {
        var enumerable = fields as string[] ?? fields.ToArray();
        if (hashValues == null || !hashValues.Any() || !enumerable.Any())
            return null;

        var dict = new ConcurrentDictionary<string, string>();
        for (var i = 0; i < enumerable.Count(); i++)
            dict[enumerable.ElementAt(i)] = hashValues[i];

        return dict;
    }

    public static ConcurrentDictionary<string, double> ToConcurrentDictionary(
        this IEnumerable<SortedSetEntry> entries)
    {
        var sortedSetEntries = entries as SortedSetEntry[] ?? entries.ToArray();
        if (!sortedSetEntries.Any())
            return null;
        var dict = new ConcurrentDictionary<string, double>();
        foreach (var entry in sortedSetEntries)
            dict[entry.Element] = entry.Score;

        return dict;
    }
}
