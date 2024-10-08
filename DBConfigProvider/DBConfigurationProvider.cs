﻿using Microsoft.Extensions.Configuration;
using System.Data.Common;
using System.Diagnostics;
using System.Text.Json;

namespace DBConfigProvider;

public class DBConfigurationProvider : ConfigurationProvider, IDisposable
{

    private DBConfigOptions options;

    //allow multi reading and single writing
    private ReaderWriterLockSlim lockObj = new ReaderWriterLockSlim();
    private bool isDisposed = false;
    public DBConfigurationProvider(DBConfigOptions options)
    {
        this.options = options;
        TimeSpan interval = TimeSpan.FromSeconds(3);
        if (options.ReloadInterval != null)
        {
            interval = options.ReloadInterval.Value;
        }
        if (options.ReloadOnChange)
        {
            ThreadPool.QueueUserWorkItem(obj => {
                while (!isDisposed)
                {
                    Load();
                    Thread.Sleep(interval);
                }
            });
        }
    }

    public void Dispose()
    {
        this.isDisposed = true;
    }

    public override IEnumerable<string> GetChildKeys(IEnumerable<string> earlierKeys, string parentPath)
    {
        lockObj.EnterReadLock();
        try
        {
            return base.GetChildKeys(earlierKeys, parentPath);
        }
        finally
        {
            lockObj.ExitReadLock();
        }
    }

    public override bool TryGet(string key, out string value)
    {
        lockObj.EnterReadLock();
        try
        {
            return base.TryGet(key, out value);
        }
        finally
        {
            lockObj.ExitReadLock();
        }
    }

    public override void Load()
    {
        base.Load();
        IDictionary<string, string> clonedData = null;
        try
        {
            lockObj.EnterWriteLock();
            clonedData = Data.Clone();
            string tableName = options.TableName;
            Data.Clear();
            using (var conn = options.CreateDbConnection())
            {
                conn.Open();
                DoLoad(tableName, conn);
            }
        }
        catch (DbException)
        {
            //if DbException is thrown, restore to the original data.
            this.Data = clonedData;
            throw;
        }
        finally
        {
            lockObj.ExitWriteLock();
        }
        //OnReload cannot be between EnterWriteLock and ExitWriteLock, or "A read lock may not be acquired with the write lock held in this mode" will be thrown.
        if (Helper.IsChanged(clonedData, Data))
        {
            OnReload();
        }
    }

    private void DoLoad(string tableName, System.Data.IDbConnection conn)
    {
        using (var cmd = conn.CreateCommand())
        {
            cmd.CommandText = $"select Name,Value from {tableName} where Id in(select Max(Id) from {tableName} group by Name)";
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    string name = reader.GetString(0);
                    string value = reader.GetString(1);
                    if (value == null)
                    {
                        this.Data[name] = value;
                        continue;
                    }
                    value = value.Trim();
                    //if the value is like [...] or {} , it may be a json array value or json object value,
                    //so try to parse it as json
                    if (value.StartsWith("[") && value.EndsWith("]")
                        || value.StartsWith("{") && value.EndsWith("}"))
                    {
                        TryLoadAsJson(name, value);
                    }
                    else
                    {
                        this.Data[name] = value;
                    }
                }
            }
        }
    }

    private void LoadJsonElement(string name, JsonElement jsonRoot)
    {
        if (jsonRoot.ValueKind == JsonValueKind.Array)
        {
            int index = 0;
            foreach (var item in jsonRoot.EnumerateArray())
            {
                //https://andrewlock.net/creating-a-custom-iconfigurationprovider-in-asp-net-core-to-parse-yaml/
                //parse as "a:b:0"="hello";"a:b:1"="world"
                string path = name + ConfigurationPath.KeyDelimiter + index;
                LoadJsonElement(path, item);
                index++;
            }
        }
        else if (jsonRoot.ValueKind == JsonValueKind.Object)
        {
            foreach (var jsonObj in jsonRoot.EnumerateObject())
            {
                string pathOfObj = name + ConfigurationPath.KeyDelimiter + jsonObj.Name;
                LoadJsonElement(pathOfObj, jsonObj.Value);
            }
        }
        else
        {
            //if it is not json array or object, parse it as plain string value
            this.Data[name] = jsonRoot.GetValueForConfig();
        }
    }

    private void TryLoadAsJson(string name, string value)
    {
        var jsonOptions = new JsonDocumentOptions { AllowTrailingCommas = true, CommentHandling = JsonCommentHandling.Skip };
        try
        {
            var jsonRoot = JsonDocument.Parse(value, jsonOptions).RootElement;
            LoadJsonElement(name, jsonRoot);
        }
        catch (JsonException ex)
        {
            //if it is not valid json, parse it as plain string value
            this.Data[name] = value;
            Debug.WriteLine($"When trying to parse {value} as json object, exception was thrown. {ex}");
        }
    }
}
