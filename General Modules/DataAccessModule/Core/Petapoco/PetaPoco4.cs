// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PetaPoco4.cs" company="Tapmobi">
//   Copyright (c) Tapmobi. All rights reserved.
// </copyright>
// <summary>
//   The explicit columns attribute.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace PetaPoco
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data;
    using System.Data.Common;
    using System.Diagnostics;
    using System.Dynamic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Reflection.Emit;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading;

    // Poco's marked [Explicit] require all column properties to be marked
    [AttributeUsage(AttributeTargets.Class)]
    public class ExplicitColumnsAttribute : Attribute
    {
    }

    // For non-explicit pocos, causes a property to be ignored
    [AttributeUsage(AttributeTargets.Property)]
    public class IgnoreAttribute : Attribute
    {
    }

    // For explicit pocos, marks property as a column and optionally supplies column name
    [AttributeUsage(AttributeTargets.Property)]
    public class ColumnAttribute : Attribute
    {

        public ColumnAttribute()
        {
        }

        public ColumnAttribute(string name)
        {
            this.Name = name;
        }



        public string Name { get; set; }

    }

    // For explicit pocos, marks property as a result column and optionally supplies column name
    [AttributeUsage(AttributeTargets.Property)]
    public class ResultColumnAttribute : ColumnAttribute
    {

        public ResultColumnAttribute()
        {
        }

        public ResultColumnAttribute(string name)
            : base(name)
        {
        }

    }

    // Specify the table name of a poco
    [AttributeUsage(AttributeTargets.Class)]
    public class TableNameAttribute : Attribute
    {

        public TableNameAttribute(string tableName)
        {
            this.Value = tableName;
        }



        public string Value { get; private set; }

    }

    // Specific the primary key of a poco class (and optional sequence name for Oracle)
    [AttributeUsage(AttributeTargets.Class)]
    public class PrimaryKeyAttribute : Attribute
    {

        public PrimaryKeyAttribute(string primaryKey)
        {
            this.Value = primaryKey;
            this.autoIncrement = true;
        }



        public string Value { get; private set; }

        public bool autoIncrement { get; set; }

        public string sequenceName { get; set; }

    }

    [AttributeUsage(AttributeTargets.Property)]
    public class AutoJoinAttribute : Attribute
    {
    }

    // Results from paged request
    public class Page<T>
    {

        public object Context { get; set; }

        public long CurrentPage { get; set; }

        public List<T> Items { get; set; }

        public long ItemsPerPage { get; set; }

        public long TotalItems { get; set; }

        public long TotalPages { get; set; }

    }

    // Pass as parameter value to force to DBType.AnsiString
    public class AnsiString
    {

        public AnsiString(string str)
        {
            this.Value = str;
        }



        public string Value { get; private set; }

    }

    // Used by IMapper to override table bindings for an object
    public class TableInfo
    {

        public bool AutoIncrement { get; set; }

        public string PrimaryKey { get; set; }

        public string SequenceName { get; set; }

        public string TableName { get; set; }

    }

    // Optionally provide an implementation of this to Database.Mapper
    public interface IMapper
    {

        Func<object, object> GetFromDbConverter(PropertyInfo pi, Type sourceType);

        void GetTableInfo(Type t, TableInfo ti);

        Func<object, object> GetToDbConverter(Type sourceType);

        bool MapPropertyToColumn(PropertyInfo pi, ref string columnName, ref bool resultColumn);

    }

    // This will be merged with IMapper in the next major version
    public interface IMapper2 : IMapper
    {

        Func<object, object> GetFromDbConverter(Type DestType, Type SourceType);

    }

    // Database class ... this is where most of the action happens
    public partial class Database : IDisposable
    {

        private static readonly Dictionary<string, object> AutoMappers = new Dictionary<string, object>();

        private static readonly Dictionary<string, object> MultiPocoFactories = new Dictionary<string, object>();

        private static readonly ReaderWriterLockSlim RWLock = new ReaderWriterLockSlim();

        private static readonly Regex rxColumns =
            new Regex(
                @"\A\s*SELECT\s+((?:\((?>\((?<depth>)|\)(?<-depth>)|.?)*(?(depth)(?!))\)|.)*?)(?<!,\s+)\bFROM\b", 
                RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Singleline | RegexOptions.Compiled);

        private static readonly Regex rxDistinct = new Regex(
            @"\ADISTINCT\s", 
            RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Singleline | RegexOptions.Compiled);

        private static readonly Regex rxOrderBy =
            new Regex(
                @"\bORDER\s+BY\s+(?:\((?>\((?<depth>)|\)(?<-depth>)|.?)*(?(depth)(?!))\)|[\w\(\)\.])+(?:\s+(?:ASC|DESC))?(?:\s*,\s*(?:\((?>\((?<depth>)|\)(?<-depth>)|.?)*(?(depth)(?!))\)|[\w\(\)\.])+(?:\s+(?:ASC|DESC))?)*", 
                RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Singleline | RegexOptions.Compiled);

        private static readonly Regex rxParams = new Regex(@"(?<!@)@\w+", RegexOptions.Compiled);

        private static readonly Regex rxParamsPrefix = new Regex(@"(?<!@)@\w+", RegexOptions.Compiled);



        private readonly string _connectionString;

        private readonly string _providerName;

        private readonly Regex rxFrom = new Regex(
            @"\A\s*FROM\s", 
            RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase | RegexOptions.Multiline);

        private readonly Regex rxSelect = new Regex(
            @"\A\s*(SELECT|EXECUTE|CALL)\s", 
            RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase | RegexOptions.Multiline);

        private DBType _dbType = DBType.SqlServer;

        private DbProviderFactory _factory;

        private object[] _lastArgs;

        private string _lastSql;

        private string _paramPrefix = "@";

        private IDbConnection _sharedConnection;

        private int _sharedConnectionDepth;

        private IDbTransaction _transaction;

        private bool _transactionCancelled;

        private int _transactionDepth;



        public Database(IDbConnection connection)
        {
            this._sharedConnection = connection;
            this._connectionString = connection.ConnectionString;
            this._sharedConnectionDepth = 2; // Prevent closing external connection
            this.CommonConstruct();
        }

        public Database(string connectionString, string providerName)
        {
            this._connectionString = connectionString;
            this._providerName = providerName;
            this.CommonConstruct();
        }

        public Database(string connectionString, DbProviderFactory provider)
        {
            this._connectionString = connectionString;
            this._factory = provider;
            this.CommonConstruct();
        }

        public Database(string connectionStringName)
        {
            // Use first?
            if (connectionStringName == string.Empty)
            {
                connectionStringName = ConfigurationManager.ConnectionStrings[0].Name;
            }

            // Work out connection string and provider name
            var providerName = "System.Data.SqlClient";
            if (ConfigurationManager.ConnectionStrings[connectionStringName] != null)
            {
                if (!string.IsNullOrEmpty(ConfigurationManager.ConnectionStrings[connectionStringName].ProviderName))
                {
                    providerName = ConfigurationManager.ConnectionStrings[connectionStringName].ProviderName;
                }
            }
            else
            {
                throw new InvalidOperationException(
                    "Can't find a connection string with the name '" + connectionStringName + "'");
            }

            // Store factory and connection string
            this._connectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
            this._providerName = providerName;
            this.CommonConstruct();
        }



        private enum DBType
        {
            SqlServer, 

            SqlServerCE, 

            MySql, 

            PostgreSQL, 

            Oracle, 

            SQLite
        }



        public static IMapper Mapper { get; set; }

        public int CommandTimeout { get; set; }

        // Set to true to keep the first opened connection alive until this object is disposed

        public IDbConnection Connection
        {
            get
            {
                return this._sharedConnection;
            }
        }

        public bool EnableAutoSelect { get; set; }

        public bool EnableNamedParams { get; set; }

        public bool ForceDateTimesToUtc { get; set; }

        public bool KeepConnectionAlive { get; set; }

        public object[] LastArgs
        {
            get
            {
                return this._lastArgs;
            }
        }

        public string LastCommand
        {
            get
            {
                return this.FormatCommand(this._lastSql, this._lastArgs);
            }
        }

        public string LastSQL
        {
            get
            {
                return this._lastSql;
            }
        }

        public int OneTimeCommandTimeout { get; set; }



        public static string  ProcessParams(string _sql, object[] args_src, List<object> args_dest)
        {
            return rxParams.Replace(
                _sql, 
                m =>
                    {
                        var param = m.Value.Substring(1);

                        object arg_val;

                        int paramIndex;
                        if (int.TryParse(param, out paramIndex))
                        {
                            // Numbered parameter
                            if (paramIndex < 0 || paramIndex >= args_src.Length)
                            {
                                throw new ArgumentOutOfRangeException(
                                    string.Format(
                                        "Parameter '@{0}' specified but only {1} parameters supplied (in `{2}`)", 
                                        paramIndex, 
                                        args_src.Length, 
                                        _sql));
                            }

                            arg_val = args_src[paramIndex];
                        }
                        else
                        {
                            // Look for a property on one of the arguments with this name
                            var found = false;
                            arg_val = null;
                            foreach (var o in args_src)
                            {
                                var pi = o.GetType().GetProperty(param);
                                if (pi != null)
                                {
                                    arg_val = pi.GetValue(o, null);
                                    found = true;
                                    break;
                                }
                            }

                            if (!found)
                            {
                                throw new ArgumentException(
                                    string.Format(
                                        "Parameter '@{0}' specified but none of the passed arguments have a property with this name (in '{1}')", 
                                        param, 
                                        _sql));
                            }
                        }

                        // Expand collections to parameter lists
                        if ((arg_val as IEnumerable) != null && (arg_val as string) == null
                            && (arg_val as byte[]) == null)
                        {
                            var sb = new StringBuilder();
                            foreach (var i in arg_val as IEnumerable)
                            {
                                sb.Append((sb.Length == 0 ? "@" : ",@") + args_dest.Count.ToString());
                                args_dest.Add(i);
                            }

                            return sb.ToString();
                        }
                        
                        args_dest.Add(arg_val);
                        return "@" + (args_dest.Count - 1).ToString();
                    });
        }

        public static bool SplitSqlForPaging(
            string sql, out string sqlCount, out string sqlSelectRemoved, out string sqlOrderBy)
        {
            sqlSelectRemoved = null;
            sqlCount = null;
            sqlOrderBy = null;

            // Extract the columns from "SELECT <whatever> FROM"
            var m = rxColumns.Match(sql);
            if (!m.Success)
            {
                return false;
            }

            // Save column list and replace with COUNT(*)
            var g = m.Groups[1];
            sqlSelectRemoved = sql.Substring(g.Index);

            if (rxDistinct.IsMatch(sqlSelectRemoved))
            {
                sqlCount = sql.Substring(0, g.Index) + "COUNT(" + m.Groups[1].ToString().Trim() + ") "
                           + sql.Substring(g.Index + g.Length);
            }
            else
            {
                sqlCount = sql.Substring(0, g.Index) + "COUNT(*) " + sql.Substring(g.Index + g.Length);
            }

            // Look for an "ORDER BY <whatever>" clause
            m = rxOrderBy.Match(sqlCount);
            if (!m.Success)
            {
                sqlOrderBy = null;
            }
            else
            {
                g = m.Groups[0];
                sqlOrderBy = g.ToString();
                sqlCount = sqlCount.Substring(0, g.Index) + sqlCount.Substring(g.Index + g.Length);
            }

            return true;
        }

        // Abort the entire outer most transaction scope
        public void AbortTransaction()
        {
            this._transactionCancelled = true;
            if ((--this._transactionDepth) == 0)
            {
                this.CleanupTransaction();
            }
        }

        public void BeginTransaction()
        {
            this._transactionDepth++;

            if (this._transactionDepth == 1)
            {
                this.OpenSharedConnection();
                this._transaction = this._sharedConnection.BeginTransaction();
                this._transactionCancelled = false;
                this.OnBeginTransaction();
            }
        }

        public void BuildPageQueries<T>(
            long skip, long take, string sql, ref object[] args, out string sqlCount, out string sqlPage)
        {
            // Add auto select clause
            if (this.EnableAutoSelect)
            {
                sql = this.AddSelectClause<T>(sql);
            }

            // Split the SQL into the bits we need
            string sqlSelectRemoved, sqlOrderBy;
            if (!SplitSqlForPaging(sql, out sqlCount, out sqlSelectRemoved, out sqlOrderBy))
            {
                throw new Exception("Unable to parse SQL statement for paged query");
            }

            if (this._dbType == DBType.Oracle && sqlSelectRemoved.StartsWith("*"))
            {
                throw new Exception(
                    "Query must alias '*' when performing a paged query.\neg. select t.* from table t order by t.id");
            }

            // Build the SQL for the actual final result
            if (this._dbType == DBType.SqlServer || this._dbType == DBType.Oracle)
            {
                sqlSelectRemoved = rxOrderBy.Replace(sqlSelectRemoved, string.Empty);
                if (rxDistinct.IsMatch(sqlSelectRemoved))
                {
                    sqlSelectRemoved = "peta_inner.* FROM (SELECT " + sqlSelectRemoved + ") peta_inner";
                }

                sqlPage =
                    string.Format(
                        "SELECT * FROM (SELECT ROW_NUMBER() OVER ({0}) peta_rn, {1}) peta_paged WHERE peta_rn>@{2} AND peta_rn<=@{3}", 
                        sqlOrderBy == null ? "ORDER BY (SELECT NULL)" : sqlOrderBy, 
                        sqlSelectRemoved, 
                        args.Length, 
                        args.Length + 1);
                args = args.Concat(new object[] { skip, skip + take }).ToArray();
            }
            else if (this._dbType == DBType.SqlServerCE)
            {
                sqlPage = string.Format(
                    "{0}\nOFFSET @{1} ROWS FETCH NEXT @{2} ROWS ONLY", sql, args.Length, args.Length + 1);
                args = args.Concat(new object[] { skip, take }).ToArray();
            }
            else
            {
                sqlPage = string.Format("{0}\nLIMIT @{1} OFFSET @{2}", sql, args.Length, args.Length + 1);
                args = args.Concat(new object[] { take, skip }).ToArray();
            }
        }

        public void CloseSharedConnection()
        {
            if (this._sharedConnectionDepth > 0)
            {
                this._sharedConnectionDepth--;
                if (this._sharedConnectionDepth == 0)
                {
                    this.OnConnectionClosing(this._sharedConnection);
                    this._sharedConnection.Dispose();
                    this._sharedConnection = null;
                }
            }
        }

        // Complete the transaction
        public void CompleteTransaction()
        {
            if ((--this._transactionDepth) == 0)
            {
                this.CleanupTransaction();
            }
        }

        // Helper to handle named parameters from object properties

        // Create a command

        public IDbCommand CreateCommand(IDbConnection connection, string sql, params object[] args)
        {
            // Perform named argument replacements
            if (this.EnableNamedParams)
            {
                var new_args = new List<object>();
                sql = ProcessParams(sql, args, new_args);
                args = new_args.ToArray();
            }

            // Perform parameter prefix replacements
            if (this._paramPrefix != "@")
            {
                sql = rxParamsPrefix.Replace(sql, m => this._paramPrefix + m.Value.Substring(1));
            }

            sql = sql.Replace("@@", "@"); // <- double @@ escapes a single @

            // Create the command and add parameters
            var cmd = connection.CreateCommand();
            cmd.Connection = connection;
            cmd.CommandText = sql;
            cmd.Transaction = this._transaction;
            foreach (var item in args)
            {
                this.AddParam(cmd, item, this._paramPrefix);
            }

            if (this._dbType == DBType.Oracle)
            {
                cmd.GetType().GetProperty("BindByName").SetValue(cmd, true, null);
            }

            if (!string.IsNullOrEmpty(sql))
            {
                this.DoPreExecute(cmd);
            }

            return cmd;
        }

        public int Delete(string tableName, string primaryKeyName, object poco)
        {
            return this.Delete(tableName, primaryKeyName, poco, null);
        }

        public int Delete(string tableName, string primaryKeyName, object poco, object primaryKeyValue)
        {
            // If primary key value not specified, pick it up from the object
            if (primaryKeyValue == null)
            {
                var pd = PocoData.ForObject(poco, primaryKeyName);
                PocoColumn pc;
                if (pd.Columns.TryGetValue(primaryKeyName, out pc))
                {
                    primaryKeyValue = pc.GetValue(poco);
                }
            }

            // Do it
            var sql = string.Format(
                "DELETE FROM {0} WHERE {1}=@0", 
                this.EscapeTableName(tableName), 
                this.EscapeSqlIdentifier(primaryKeyName));
            return this.Execute(sql, primaryKeyValue);
        }

        public int Delete(object poco)
        {
            var pd = PocoData.ForType(poco.GetType());
            return this.Delete(pd.TableInfo.TableName, pd.TableInfo.PrimaryKey, poco);
        }

        public int Delete<T>(object pocoOrPrimaryKey)
        {
            if (pocoOrPrimaryKey.GetType() == typeof(T))
            {
                return this.Delete(pocoOrPrimaryKey);
            }

            var pd = PocoData.ForType(typeof(T));
            return this.Delete(pd.TableInfo.TableName, pd.TableInfo.PrimaryKey, null, pocoOrPrimaryKey);
        }

        public int Delete<T>(string sql, params object[] args)
        {
            var pd = PocoData.ForType(typeof(T));
            return this.Execute(
                string.Format("DELETE FROM {0} {1}", this.EscapeTableName(pd.TableInfo.TableName), sql), args);
        }

        public int Delete<T>(Sql sql)
        {
            var pd = PocoData.ForType(typeof(T));
            return
                Execute(
                    new Sql(string.Format("DELETE FROM {0}", this.EscapeTableName(pd.TableInfo.TableName))).Append(sql));
        }

        public void Dispose()
        {
            // Automatically close one open connection reference
            // (Works with KeepConnectionAlive and manually opening a shared connection)
            this.CloseSharedConnection();
        }

        public string EscapeSqlIdentifier(string str)
        {
            switch (this._dbType)
            {
                case DBType.MySql:
                    return string.Format("`{0}`", str);

                case DBType.PostgreSQL:
                    return string.Format("\"{0}\"", str);

                case DBType.Oracle:
                    return string.Format("\"{0}\"", str.ToUpperInvariant());

                default:
                    return string.Format("[{0}]", str);
            }
        }

        public string EscapeTableName(string str)
        {
            // Assume table names with "dot" are already escaped
            return str.IndexOf('.') >= 0 ? str : this.EscapeSqlIdentifier(str);
        }

        // Override this to log/capture exceptions

        // Execute a non-query command
        public int Execute(string sql, params object[] args)
        {
            try
            {
                this.OpenSharedConnection();
                try
                {
                    using (var cmd = this.CreateCommand(this._sharedConnection, sql, args))
                    {
                        var retv = cmd.ExecuteNonQuery();
                        this.OnExecutedCommand(cmd);
                        return retv;
                    }
                }
                finally
                {
                    this.CloseSharedConnection();
                }
            }
            catch (Exception x)
            {
                this.OnException(x);
                throw;
            }
        }

        public int Execute(Sql sql)
        {
            return this.Execute(sql.SQL, sql.Arguments);
        }

        // Execute and cast a scalar property
        public T ExecuteScalar<T>(string sql, params object[] args)
        {
            try
            {
                this.OpenSharedConnection();
                try
                {
                    using (var cmd = this.CreateCommand(this._sharedConnection, sql, args))
                    {
                        var val = cmd.ExecuteScalar();
                        this.OnExecutedCommand(cmd);
                        return (T)Convert.ChangeType(val, typeof(T));
                    }
                }
                finally
                {
                    this.CloseSharedConnection();
                }
            }
            catch (Exception x)
            {
                this.OnException(x);
                throw;
            }
        }

        public T ExecuteScalar<T>(Sql sql)
        {
            return this.ExecuteScalar<T>(sql.SQL, sql.Arguments);
        }

        public bool Exists<T>(object primaryKey)
        {
            return
                this.FirstOrDefault<T>(
                    string.Format(
                        "WHERE {0}=@0", this.EscapeSqlIdentifier(PocoData.ForType(typeof(T)).TableInfo.PrimaryKey)), 
                    primaryKey) != null;
        }

        // Return a typed list of pocos
        public List<T> Fetch<T>(string sql, params object[] args)
        {
            return this.Query<T>(sql, args).ToList();
        }

        public List<T> Fetch<T>(Sql sql)
        {
            return this.Fetch<T>(sql.SQL, sql.Arguments);
        }

        public List<T> Fetch<T>(long page, long itemsPerPage, string sql, params object[] args)
        {
            return this.SkipTake<T>((page - 1) * itemsPerPage, itemsPerPage, sql, args);
        }

        public List<T> Fetch<T>(long page, long itemsPerPage, Sql sql)
        {
            return this.SkipTake<T>((page - 1) * itemsPerPage, itemsPerPage, sql.SQL, sql.Arguments);
        }

        // Multi Fetch
        public List<TRet> Fetch<T1, T2, TRet>(Func<T1, T2, TRet> cb, string sql, params object[] args)
        {
            return Query(cb, sql, args).ToList();
        }

        public List<TRet> Fetch<T1, T2, T3, TRet>(Func<T1, T2, T3, TRet> cb, string sql, params object[] args)
        {
            return Query(cb, sql, args).ToList();
        }

        public List<TRet> Fetch<T1, T2, T3, T4, TRet>(Func<T1, T2, T3, T4, TRet> cb, string sql, params object[] args)
        {
            return Query(cb, sql, args).ToList();
        }

        // Multi Query

        // Multi Fetch (SQL builder)
        public List<TRet> Fetch<T1, T2, TRet>(Func<T1, T2, TRet> cb, Sql sql)
        {
            return Query(cb, sql.SQL, sql.Arguments).ToList();
        }

        public List<TRet> Fetch<T1, T2, T3, TRet>(Func<T1, T2, T3, TRet> cb, Sql sql)
        {
            return Query(cb, sql.SQL, sql.Arguments).ToList();
        }

        public List<TRet> Fetch<T1, T2, T3, T4, TRet>(Func<T1, T2, T3, T4, TRet> cb, Sql sql)
        {
            return Query(cb, sql.SQL, sql.Arguments).ToList();
        }

        // Multi Query (SQL builder)

        // Multi Fetch (Simple)
        public List<T1> Fetch<T1, T2>(string sql, params object[] args)
        {
            return this.Query<T1, T2>(sql, args).ToList();
        }

        public List<T1> Fetch<T1, T2, T3>(string sql, params object[] args)
        {
            return Query<T1, T2, T3>(sql, args).ToList();
        }

        public List<T1> Fetch<T1, T2, T3, T4>(string sql, params object[] args)
        {
            return Query<T1, T2, T3, T4>(sql, args).ToList();
        }

        // Multi Query (Simple)

        // Multi Fetch (Simple) (SQL builder)
        public List<T1> Fetch<T1, T2>(Sql sql)
        {
            return this.Query<T1, T2>(sql.SQL, sql.Arguments).ToList();
        }

        public List<T1> Fetch<T1, T2, T3>(Sql sql)
        {
            return Query<T1, T2, T3>(sql.SQL, sql.Arguments).ToList();
        }

        public List<T1> Fetch<T1, T2, T3, T4>(Sql sql)
        {
            return Query<T1, T2, T3, T4>(sql.SQL, sql.Arguments).ToList();
        }

        public T First<T>(string sql, params object[] args)
        {
            return this.Query<T>(sql, args).First();
        }

        public T First<T>(Sql sql)
        {
            return Query<T>(sql).First();
        }

        public T FirstOrDefault<T>(string sql, params object[] args)
        {
            return this.Query<T>(sql, args).FirstOrDefault();
        }

        public T FirstOrDefault<T>(Sql sql)
        {
            return Query<T>(sql).FirstOrDefault();
        }

        public string FormatCommand(IDbCommand cmd)
        {
            return this.FormatCommand(
                cmd.CommandText, (from IDataParameter parameter in cmd.Parameters select parameter.Value).ToArray());
        }

        public string FormatCommand(string sql, object[] args)
        {
            var sb = new StringBuilder();
            if (sql == null)
            {
                return string.Empty;
            }

            sb.Append(sql);
            if (args != null && args.Length > 0)
            {
                sb.Append("\n");
                for (var i = 0; i < args.Length; i++)
                {
                    sb.AppendFormat(
                        "\t -> {0}{1} [{2}] = \"{3}\"\n", this._paramPrefix, i, args[i].GetType().Name, args[i]);
                }

                sb.Remove(sb.Length - 1, 1);
            }

            return sb.ToString();
        }

        public Transaction GetTransaction()
        {
            return new Transaction(this);
        }

        public object Insert(string tableName, string primaryKeyName, object poco)
        {
            return this.Insert(tableName, primaryKeyName, true, poco);
        }

        // Insert a poco into a table.  If the poco has a property with the same name 
        // as the primary key the id of the new record is assigned to it.  Either way,
        // the new id is returned.
        public object Insert(string tableName, string primaryKeyName, bool autoIncrement, object poco)
        {
            try
            {
                this.OpenSharedConnection();
                try
                {
                    using (var cmd = this.CreateCommand(this._sharedConnection, string.Empty))
                    {
                        var pd = PocoData.ForObject(poco, primaryKeyName);
                        var names = new List<string>();
                        var values = new List<string>();
                        var index = 0;
                        foreach (var i in pd.Columns)
                        {
                            // Don't insert result columns
                            if (i.Value.ResultColumn)
                            {
                                continue;
                            }

                            // Don't insert the primary key (except under oracle where we need bring in the next sequence value)
                            if (autoIncrement && primaryKeyName != null
                                && string.Compare(i.Key, primaryKeyName, true) == 0)
                            {
                                if (this._dbType == DBType.Oracle && !string.IsNullOrEmpty(pd.TableInfo.SequenceName))
                                {
                                    names.Add(i.Key);
                                    values.Add(string.Format("{0}.nextval", pd.TableInfo.SequenceName));
                                }

                                continue;
                            }

                            names.Add(this.EscapeSqlIdentifier(i.Key));
                            values.Add(string.Format("{0}{1}", this._paramPrefix, index++));
                            this.AddParam(cmd, i.Value.GetValue(poco), this._paramPrefix);
                        }

                        cmd.CommandText = string.Format(
                            "INSERT INTO {0} ({1}) VALUES ({2})", 
                            this.EscapeTableName(tableName), 
                            string.Join(",", names.ToArray()), 
                            string.Join(",", values.ToArray()));

                        if (!autoIncrement)
                        {
                            this.DoPreExecute(cmd);
                            cmd.ExecuteNonQuery();
                            this.OnExecutedCommand(cmd);
                            return true;
                        }

                        object id;
                        switch (this._dbType)
                        {
                            case DBType.SqlServerCE:
                                this.DoPreExecute(cmd);
                                cmd.ExecuteNonQuery();
                                this.OnExecutedCommand(cmd);
                                id = this.ExecuteScalar<object>("SELECT @@@IDENTITY AS NewID;");
                                break;
                            case DBType.SqlServer:
                                cmd.CommandText += ";\nSELECT SCOPE_IDENTITY() AS NewID;";
                                this.DoPreExecute(cmd);
                                id = cmd.ExecuteScalar();
                                this.OnExecutedCommand(cmd);
                                break;
                            case DBType.PostgreSQL:
                                if (primaryKeyName != null)
                                {
                                    cmd.CommandText += string.Format(
                                        "returning {0} as NewID", this.EscapeSqlIdentifier(primaryKeyName));
                                    this.DoPreExecute(cmd);
                                    id = cmd.ExecuteScalar();
                                }
                                else
                                {
                                    id = -1;
                                    this.DoPreExecute(cmd);
                                    cmd.ExecuteNonQuery();
                                }

                                this.OnExecutedCommand(cmd);
                                break;
                            case DBType.Oracle:
                                if (primaryKeyName != null)
                                {
                                    cmd.CommandText += string.Format(
                                        " returning {0} into :newid", this.EscapeSqlIdentifier(primaryKeyName));
                                    var param = cmd.CreateParameter();
                                    param.ParameterName = ":newid";
                                    param.Value = DBNull.Value;
                                    param.Direction = ParameterDirection.ReturnValue;
                                    param.DbType = DbType.Int64;
                                    cmd.Parameters.Add(param);
                                    this.DoPreExecute(cmd);
                                    cmd.ExecuteNonQuery();
                                    id = param.Value;
                                }
                                else
                                {
                                    id = -1;
                                    this.DoPreExecute(cmd);
                                    cmd.ExecuteNonQuery();
                                }

                                this.OnExecutedCommand(cmd);
                                break;
                            case DBType.SQLite:
                                if (primaryKeyName != null)
                                {
                                    cmd.CommandText += ";\nSELECT last_insert_rowid();";
                                    this.DoPreExecute(cmd);
                                    id = cmd.ExecuteScalar();
                                }
                                else
                                {
                                    id = -1;
                                    this.DoPreExecute(cmd);
                                    cmd.ExecuteNonQuery();
                                }

                                this.OnExecutedCommand(cmd);
                                break;
                            default:
                                cmd.CommandText += ";\nSELECT @@IDENTITY AS NewID;";
                                this.DoPreExecute(cmd);
                                id = cmd.ExecuteScalar();
                                this.OnExecutedCommand(cmd);
                                break;
                        }

                        // Assign the ID back to the primary key property
                        if (primaryKeyName != null)
                        {
                            PocoColumn pc;
                            if (pd.Columns.TryGetValue(primaryKeyName, out pc))
                            {
                                pc.SetValue(poco, pc.ChangeType(id));
                            }
                        }

                        return id;
                    }
                }
                finally
                {
                    this.CloseSharedConnection();
                }
            }
            catch (Exception x)
            {
                this.OnException(x);
                throw;
            }
        }

        // Insert an annotated poco object
        public object Insert(object poco)
        {
            var pd = PocoData.ForType(poco.GetType());
            return this.Insert(pd.TableInfo.TableName, pd.TableInfo.PrimaryKey, pd.TableInfo.AutoIncrement, poco);
        }

        public bool IsNew(string primaryKeyName, object poco)
        {
            var pd = PocoData.ForObject(poco, primaryKeyName);
            object pk;
            PocoColumn pc;
            if (pd.Columns.TryGetValue(primaryKeyName, out pc))
            {
                pk = pc.GetValue(poco);
            }

#if !PETAPOCO_NO_DYNAMIC
            else if (poco.GetType() == typeof(ExpandoObject))
            {
                return true;
            }

#endif
            else
            {
                var pi = poco.GetType().GetProperty(primaryKeyName);
                if (pi == null)
                {
                    throw new ArgumentException(
                        string.Format(
                            "The object doesn't have a property matching the primary key column name '{0}'", 
                            primaryKeyName));
                }

                pk = pi.GetValue(poco, null);
            }

            if (pk == null)
            {
                return true;
            }

            var type = pk.GetType();

            if (type.IsValueType)
            {
                // Common primary key types
                if (type == typeof(long))
                {
                    return (long)pk == 0;
                }
                else if (type == typeof(ulong))
                {
                    return (ulong)pk == 0;
                }
                else if (type == typeof(int))
                {
                    return (int)pk == 0;
                }
                else if (type == typeof(uint))
                {
                    return (uint)pk == 0;
                }

                // Create a default instance and compare
                return pk == Activator.CreateInstance(pk.GetType());
            }
            else
            {
                return pk == null;
            }
        }

        public bool IsNew(object poco)
        {
            var pd = PocoData.ForType(poco.GetType());
            if (!pd.TableInfo.AutoIncrement)
            {
                throw new InvalidOperationException(
                    "IsNew() and Save() are only supported on tables with auto-increment/identity primary key columns");
            }

            return this.IsNew(pd.TableInfo.PrimaryKey, poco);
        }

        public virtual void OnBeginTransaction()
        {
        }

        public virtual void OnConnectionClosing(IDbConnection conn)
        {
        }

        public virtual IDbConnection OnConnectionOpened(IDbConnection conn)
        {
            return conn;
        }

        public virtual void OnEndTransaction()
        {
        }

        public virtual void OnException(Exception x)
        {
            Debug.WriteLine(x.ToString());
            Debug.WriteLine(this.LastCommand);
        }

        public virtual void OnExecutedCommand(IDbCommand cmd)
        {
        }

        public virtual void OnExecutingCommand(IDbCommand cmd)
        {
        }

        public void OpenSharedConnection()
        {
            if (this._sharedConnectionDepth == 0)
            {
                this._sharedConnection = this._factory.CreateConnection();
                this._sharedConnection.ConnectionString = this._connectionString;
                this._sharedConnection.Open();

                this._sharedConnection = this.OnConnectionOpened(this._sharedConnection);

                if (this.KeepConnectionAlive)
                {
                    this._sharedConnectionDepth++; // Make sure you call Dispose
                }
            }

            this._sharedConnectionDepth++;
        }

        public Page<T> Page<T>(long page, long itemsPerPage, string sql, params object[] args)
        {
            string sqlCount, sqlPage;
            this.BuildPageQueries<T>((page - 1) * itemsPerPage, itemsPerPage, sql, ref args, out sqlCount, out sqlPage);

            // Save the one-time command time out and use it for both queries
            var saveTimeout = this.OneTimeCommandTimeout;

            // Setup the paged result
            var result = new Page<T>();
            result.CurrentPage = page;
            result.ItemsPerPage = itemsPerPage;
            result.TotalItems = this.ExecuteScalar<long>(sqlCount, args);
            result.TotalPages = result.TotalItems / itemsPerPage;
            if ((result.TotalItems % itemsPerPage) != 0)
            {
                result.TotalPages++;
            }

            this.OneTimeCommandTimeout = saveTimeout;

            // Get the records
            result.Items = this.Fetch<T>(sqlPage, args);

            // Done
            return result;
        }

        public Page<T> Page<T>(long page, long itemsPerPage, Sql sql)
        {
            return this.Page<T>(page, itemsPerPage, sql.SQL, sql.Arguments);
        }

        public IEnumerable<T> Query<T>(string sql, params object[] args)
        {
            if (this.EnableAutoSelect)
            {
                sql = this.AddSelectClause<T>(sql);
            }

            this.OpenSharedConnection();
            try
            {
                using (var cmd = this.CreateCommand(this._sharedConnection, sql, args))
                {
                    IDataReader r;
                    var pd = PocoData.ForType(typeof(T));
                    try
                    {
                        r = cmd.ExecuteReader();
                        this.OnExecutedCommand(cmd);
                    }
                    catch (Exception x)
                    {
                        this.OnException(x);
                        throw;
                    }

                    var factory =
                        pd.GetFactory(
                            cmd.CommandText, 
                            this._sharedConnection.ConnectionString, 
                            this.ForceDateTimesToUtc, 
                            0, 
                            r.FieldCount, 
                            r) as Func<IDataReader, T>;
                    using (r)
                    {
                        while (true)
                        {
                            T poco;
                            try
                            {
                                if (!r.Read())
                                {
                                    yield break;
                                }

                                poco = factory(r);
                            }
                            catch (Exception x)
                            {
                                this.OnException(x);
                                throw;
                            }

                            yield return poco;
                        }
                    }
                }
            }
            finally
            {
                this.CloseSharedConnection();
            }
        }

        public IEnumerable<TRet> Query<T1, T2, TRet>(Func<T1, T2, TRet> cb, string sql, params object[] args)
        {
            return this.Query<TRet>(new[] { typeof(T1), typeof(T2) }, cb, sql, args);
        }

        public IEnumerable<TRet> Query<T1, T2, T3, TRet>(Func<T1, T2, T3, TRet> cb, string sql, params object[] args)
        {
            return this.Query<TRet>(new[] { typeof(T1), typeof(T2), typeof(T3) }, cb, sql, args);
        }

        public IEnumerable<TRet> Query<T1, T2, T3, T4, TRet>(
            Func<T1, T2, T3, T4, TRet> cb, string sql, params object[] args)
        {
            return this.Query<TRet>(new[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4) }, cb, sql, args);
        }

        public IEnumerable<TRet> Query<T1, T2, TRet>(Func<T1, T2, TRet> cb, Sql sql)
        {
            return this.Query<TRet>(new[] { typeof(T1), typeof(T2) }, cb, sql.SQL, sql.Arguments);
        }

        public IEnumerable<TRet> Query<T1, T2, T3, TRet>(Func<T1, T2, T3, TRet> cb, Sql sql)
        {
            return this.Query<TRet>(new[] { typeof(T1), typeof(T2), typeof(T3) }, cb, sql.SQL, sql.Arguments);
        }

        public IEnumerable<TRet> Query<T1, T2, T3, T4, TRet>(Func<T1, T2, T3, T4, TRet> cb, Sql sql)
        {
            return this.Query<TRet>(
                new[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4) }, cb, sql.SQL, sql.Arguments);
        }

        public IEnumerable<T1> Query<T1, T2>(string sql, params object[] args)
        {
            return this.Query<T1>(new[] { typeof(T1), typeof(T2) }, null, sql, args);
        }

        public IEnumerable<T1> Query<T1, T2, T3>(string sql, params object[] args)
        {
            return this.Query<T1>(new[] { typeof(T1), typeof(T2), typeof(T3) }, null, sql, args);
        }

        public IEnumerable<T1> Query<T1, T2, T3, T4>(string sql, params object[] args)
        {
            return this.Query<T1>(new[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4) }, null, sql, args);
        }

        // Multi Query (Simple) (SQL builder)
        public IEnumerable<T1> Query<T1, T2>(Sql sql)
        {
            return this.Query<T1>(new[] { typeof(T1), typeof(T2) }, null, sql.SQL, sql.Arguments);
        }

        public IEnumerable<T1> Query<T1, T2, T3>(Sql sql)
        {
            return this.Query<T1>(new[] { typeof(T1), typeof(T2), typeof(T3) }, null, sql.SQL, sql.Arguments);
        }

        public IEnumerable<T1> Query<T1, T2, T3, T4>(Sql sql)
        {
            return this.Query<T1>(
                new[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4) }, null, sql.SQL, sql.Arguments);
        }

        // Automagically guess the property relationships between various POCOs and create a delegate that will set them up

        // Actual implementation of the multi-poco query
        public IEnumerable<TRet> Query<TRet>(Type[] types, object cb, string sql, params object[] args)
        {
            this.OpenSharedConnection();
            try
            {
                using (var cmd = this.CreateCommand(this._sharedConnection, sql, args))
                {
                    IDataReader r;
                    try
                    {
                        r = cmd.ExecuteReader();
                        this.OnExecutedCommand(cmd);
                    }
                    catch (Exception x)
                    {
                        this.OnException(x);
                        throw;
                    }

                    var factory = this.GetMultiPocoFactory<TRet>(types, sql, r);
                    if (cb == null)
                    {
                        cb = this.GetAutoMapper(types.ToArray());
                    }

                    var bNeedTerminator = false;
                    using (r)
                    {
                        while (true)
                        {
                            TRet poco;
                            try
                            {
                                if (!r.Read())
                                {
                                    break;
                                }

                                poco = factory(r, cb);
                            }
                            catch (Exception x)
                            {
                                this.OnException(x);
                                throw;
                            }

                            if (poco != null)
                            {
                                yield return poco;
                            }
                            else
                            {
                                bNeedTerminator = true;
                            }
                        }

                        if (bNeedTerminator)
                        {
                            var poco = (TRet)(cb as Delegate).DynamicInvoke(new object[types.Length]);
                            if (poco != null)
                            {
                                yield return poco;
                            }
                            else
                            {
                                yield break;
                            }
                        }
                    }
                }
            }
            finally
            {
                this.CloseSharedConnection();
            }
        }

        public IEnumerable<T> Query<T>(Sql sql)
        {
            return this.Query<T>(sql.SQL, sql.Arguments);
        }

        public void Save(string tableName, string primaryKeyName, object poco)
        {
            if (this.IsNew(primaryKeyName, poco))
            {
                this.Insert(tableName, primaryKeyName, true, poco);
            }
            else
            {
                Update(tableName, primaryKeyName, poco);
            }
        }

        public void Save(object poco)
        {
            var pd = PocoData.ForType(poco.GetType());
            this.Save(pd.TableInfo.TableName, pd.TableInfo.PrimaryKey, poco);
        }

        public T Single<T>(object primaryKey)
        {
            return
                this.Single<T>(
                    string.Format(
                        "WHERE {0}=@0", this.EscapeSqlIdentifier(PocoData.ForType(typeof(T)).TableInfo.PrimaryKey)), 
                    primaryKey);
        }

        public T Single<T>(string sql, params object[] args)
        {
            return this.Query<T>(sql, args).Single();
        }

        public T Single<T>(Sql sql)
        {
            return Query<T>(sql).Single();
        }

        public T SingleOrDefault<T>(object primaryKey)
        {
            return
                this.SingleOrDefault<T>(
                    string.Format(
                        "WHERE {0}=@0", this.EscapeSqlIdentifier(PocoData.ForType(typeof(T)).TableInfo.PrimaryKey)), 
                    primaryKey);
        }

        public T SingleOrDefault<T>(string sql, params object[] args)
        {
            return this.Query<T>(sql, args).SingleOrDefault();
        }

        public T SingleOrDefault<T>(Sql sql)
        {
            return Query<T>(sql).SingleOrDefault();
        }

        public List<T> SkipTake<T>(long skip, long take, string sql, params object[] args)
        {
            string sqlCount, sqlPage;
            this.BuildPageQueries<T>(skip, take, sql, ref args, out sqlCount, out sqlPage);
            return this.Fetch<T>(sqlPage, args);
        }

        public List<T> SkipTake<T>(long skip, long take, Sql sql)
        {
            return this.SkipTake<T>(skip, take, sql.SQL, sql.Arguments);
        }

        public int Update(string tableName, string primaryKeyName, object poco, object primaryKeyValue)
        {
            return this.Update(tableName, primaryKeyName, poco, primaryKeyValue, null);
        }

        // Update a record with values from a poco.  primary key value can be either supplied or read from the poco
        public int Update(
            string tableName, string primaryKeyName, object poco, object primaryKeyValue, IEnumerable<string> columns)
        {
            try
            {
                this.OpenSharedConnection();
                try
                {
                    using (var cmd = this.CreateCommand(this._sharedConnection, string.Empty))
                    {
                        var sb = new StringBuilder();
                        var index = 0;
                        var pd = PocoData.ForObject(poco, primaryKeyName);
                        if (columns == null)
                        {
                            foreach (var i in pd.Columns)
                            {
                                // Don't update the primary key, but grab the value if we don't have it
                                if (string.Compare(i.Key, primaryKeyName, true) == 0)
                                {
                                    if (primaryKeyValue == null)
                                    {
                                        primaryKeyValue = i.Value.GetValue(poco);
                                    }

                                    continue;
                                }

                                // Dont update result only columns
                                if (i.Value.ResultColumn)
                                {
                                    continue;
                                }

                                // Build the sql
                                if (index > 0)
                                {
                                    sb.Append(", ");
                                }

                                sb.AppendFormat(
                                    "{0} = {1}{2}", this.EscapeSqlIdentifier(i.Key), this._paramPrefix, index++);

                                // Store the parameter in the command
                                this.AddParam(cmd, i.Value.GetValue(poco), this._paramPrefix);
                            }
                        }
                        else
                        {
                            foreach (var colname in columns)
                            {
                                var pc = pd.Columns[colname];

                                // Build the sql
                                if (index > 0)
                                {
                                    sb.Append(", ");
                                }

                                sb.AppendFormat(
                                    "{0} = {1}{2}", this.EscapeSqlIdentifier(colname), this._paramPrefix, index++);

                                // Store the parameter in the command
                                this.AddParam(cmd, pc.GetValue(poco), this._paramPrefix);
                            }

                            // Grab primary key value
                            if (primaryKeyValue == null)
                            {
                                var pc = pd.Columns[primaryKeyName];
                                primaryKeyValue = pc.GetValue(poco);
                            }
                        }

                        cmd.CommandText = string.Format(
                            "UPDATE {0} SET {1} WHERE {2} = {3}{4}", 
                            this.EscapeTableName(tableName), 
                            sb, 
                            this.EscapeSqlIdentifier(primaryKeyName), 
                            this._paramPrefix, 
                            index++);
                        this.AddParam(cmd, primaryKeyValue, this._paramPrefix);

                        this.DoPreExecute(cmd);

                        // Do it
                        var retv = cmd.ExecuteNonQuery();
                        this.OnExecutedCommand(cmd);
                        return retv;
                    }
                }
                finally
                {
                    this.CloseSharedConnection();
                }
            }
            catch (Exception x)
            {
                this.OnException(x);
                throw;
            }
        }

        public int Update(string tableName, string primaryKeyName, object poco)
        {
            return this.Update(tableName, primaryKeyName, poco, null);
        }

        public int Update(string tableName, string primaryKeyName, object poco, IEnumerable<string> columns)
        {
            return this.Update(tableName, primaryKeyName, poco, null, columns);
        }

        public int Update(object poco, IEnumerable<string> columns)
        {
            return Update(poco, null, columns);
        }

        public int Update(object poco)
        {
            return Update(poco, null, null);
        }

        public int Update(object poco, object primaryKeyValue)
        {
            return Update(poco, primaryKeyValue, null);
        }

        public int Update(object poco, object primaryKeyValue, IEnumerable<string> columns)
        {
            var pd = PocoData.ForType(poco.GetType());
            return this.Update(pd.TableInfo.TableName, pd.TableInfo.PrimaryKey, poco, primaryKeyValue, columns);
        }

        public int Update<T>(string sql, params object[] args)
        {
            var pd = PocoData.ForType(typeof(T));
            return this.Execute(
                string.Format("UPDATE {0} {1}", this.EscapeTableName(pd.TableInfo.TableName), sql), args);
        }

        public int Update<T>(Sql sql)
        {
            var pd = PocoData.ForType(typeof(T));
            return
                Execute(new Sql(string.Format("UPDATE {0}", this.EscapeTableName(pd.TableInfo.TableName))).Append(sql));
        }



        private void AddParam(IDbCommand cmd, object item, string ParameterPrefix)
        {
            // Convert value to from poco type to db type
            if (Mapper != null && item != null)
            {
                var fn = Mapper.GetToDbConverter(item.GetType());
                if (fn != null)
                {
                    item = fn(item);
                }
            }

            // Support passed in parameters
            var idbParam = item as IDbDataParameter;
            if (idbParam != null)
            {
                idbParam.ParameterName = string.Format("{0}{1}", ParameterPrefix, cmd.Parameters.Count);
                cmd.Parameters.Add(idbParam);
                return;
            }

            var p = cmd.CreateParameter();
            p.ParameterName = string.Format("{0}{1}", ParameterPrefix, cmd.Parameters.Count);
            if (item == null)
            {
                p.Value = DBNull.Value;
            }
            else
            {
                var t = item.GetType();
                if (t.IsEnum)
                {
                    // PostgreSQL .NET driver wont cast enum to int
                    p.Value = (int)item;
                }
                else if (t == typeof(Guid))
                {
                    p.Value = item.ToString();
                    p.DbType = DbType.String;
                    p.Size = 40;
                }
                else if (t == typeof(string))
                {
                    p.Size = Math.Max((item as string).Length + 1, 4000);

                    // Help query plan caching by using common size
                    p.Value = item;
                }
                else if (t == typeof(AnsiString))
                {
                    // Thanks @DataChomp for pointing out the SQL Server indexing performance hit of using wrong string type on varchar
                    p.Size = Math.Max((item as AnsiString).Value.Length + 1, 4000);
                    p.Value = (item as AnsiString).Value;
                    p.DbType = DbType.AnsiString;
                }
                else if (t == typeof(bool) && this._dbType != DBType.PostgreSQL)
                {
                    p.Value = ((bool)item) ? 1 : 0;
                }
                else if (item.GetType().Name == "SqlGeography")
                {
                    // SqlGeography is a CLR Type
                    p.GetType().GetProperty("UdtTypeName").SetValue(p, "geography", null);

                    // geography is the equivalent SQL Server Type
                    p.Value = item;
                }
                else if (item.GetType().Name == "SqlGeometry")
                {
                    // SqlGeometry is a CLR Type
                    p.GetType().GetProperty("UdtTypeName").SetValue(p, "geometry", null);

                    // geography is the equivalent SQL Server Type
                    p.Value = item;
                }
                else
                {
                    p.Value = item;
                }
            }

            cmd.Parameters.Add(p);
        }

        private string AddSelectClause<T>(string sql)
        {
            if (sql.StartsWith(";"))
            {
                return sql.Substring(1);
            }

            if (!this.rxSelect.IsMatch(sql))
            {
                var pd = PocoData.ForType(typeof(T));
                var tableName = this.EscapeTableName(pd.TableInfo.TableName);
                var cols = string.Join(
                    ", ", (from c in pd.QueryColumns select tableName + "." + this.EscapeSqlIdentifier(c)).ToArray());
                if (!this.rxFrom.IsMatch(sql))
                {
                    sql = string.Format("SELECT {0} FROM {1} {2}", cols, tableName, sql);
                }
                else
                {
                    sql = string.Format("SELECT {0} {1}", cols, sql);
                }
            }

            return sql;
        }

        private void CleanupTransaction()
        {
            this.OnEndTransaction();

            if (this._transactionCancelled)
            {
                this._transaction.Rollback();
            }
            else
            {
                this._transaction.Commit();
            }

            this._transaction.Dispose();
            this._transaction = null;

            this.CloseSharedConnection();
        }

        private void CommonConstruct()
        {
            this._transactionDepth = 0;
            this.EnableAutoSelect = true;
            this.EnableNamedParams = true;
            this.ForceDateTimesToUtc = true;

            if (this._providerName != null)
            {
                this._factory = DbProviderFactories.GetFactory(this._providerName);
            }

            var dbtype = (this._factory == null ? this._sharedConnection.GetType() : this._factory.GetType()).Name;

            // Try using type name first (more reliable)
            if (dbtype.StartsWith("MySql"))
            {
                this._dbType = DBType.MySql;
            }
            else if (dbtype.StartsWith("SqlCe"))
            {
                this._dbType = DBType.SqlServerCE;
            }
            else if (dbtype.StartsWith("Npgsql"))
            {
                this._dbType = DBType.PostgreSQL;
            }
            else if (dbtype.StartsWith("Oracle"))
            {
                this._dbType = DBType.Oracle;
            }
            else if (dbtype.StartsWith("SQLite"))
            {
                this._dbType = DBType.SQLite;
            }
            else if (dbtype.StartsWith("System.Data.SqlClient."))
            {
                this._dbType = DBType.SqlServer;
            }
                
                
                
                
                
                // else try with provider name
            else if (this._providerName.IndexOf("MySql", StringComparison.InvariantCultureIgnoreCase) >= 0)
            {
                this._dbType = DBType.MySql;
            }
            else if (this._providerName.IndexOf("SqlServerCe", StringComparison.InvariantCultureIgnoreCase) >= 0)
            {
                this._dbType = DBType.SqlServerCE;
            }
            else if (this._providerName.IndexOf("Npgsql", StringComparison.InvariantCultureIgnoreCase) >= 0)
            {
                this._dbType = DBType.PostgreSQL;
            }
            else if (this._providerName.IndexOf("Oracle", StringComparison.InvariantCultureIgnoreCase) >= 0)
            {
                this._dbType = DBType.Oracle;
            }
            else if (this._providerName.IndexOf("SQLite", StringComparison.InvariantCultureIgnoreCase) >= 0)
            {
                this._dbType = DBType.SQLite;
            }

            if (this._dbType == DBType.MySql && this._connectionString != null
                && this._connectionString.IndexOf("Allow User Variables=true") >= 0)
            {
                this._paramPrefix = "?";
            }

            if (this._dbType == DBType.Oracle)
            {
                this._paramPrefix = ":";
            }
        }

        private Func<IDataReader, object, TRet> CreateMultiPocoFactory<TRet>(Type[] types, string sql, IDataReader r)
        {
            var m = new DynamicMethod(
                "petapoco_multipoco_factory", 
                typeof(TRet), 
                new[] { typeof(MultiPocoFactory), typeof(IDataReader), typeof(object) }, 
                typeof(MultiPocoFactory));
            var il = m.GetILGenerator();

            // Load the callback
            il.Emit(OpCodes.Ldarg_2);

            // Call each delegate
            var dels = new List<Delegate>();
            var pos = 0;
            for (var i = 0; i < types.Length; i++)
            {
                // Add to list of delegates to call
                var del = this.FindSplitPoint(types[i], i + 1 < types.Length ? types[i + 1] : null, sql, r, ref pos);
                dels.Add(del);

                // Get the delegate
                il.Emit(OpCodes.Ldarg_0); // callback,this
                il.Emit(OpCodes.Ldc_I4, i); // callback,this,Index
                il.Emit(OpCodes.Callvirt, typeof(MultiPocoFactory).GetMethod("GetItem")); // callback,Delegate
                il.Emit(OpCodes.Ldarg_1); // callback,delegate, datareader

                // Call Invoke
                var tDelInvoke = del.GetType().GetMethod("Invoke");
                il.Emit(OpCodes.Callvirt, tDelInvoke); // Poco left on stack
            }

            // By now we should have the callback and the N pocos all on the stack.  Call the callback and we're done
            il.Emit(
                OpCodes.Callvirt, 
                Expression.GetFuncType(types.Concat(new[] { typeof(TRet) }).ToArray()).GetMethod("Invoke"));
            il.Emit(OpCodes.Ret);

            // Finish up
            return
                (Func<IDataReader, object, TRet>)
                m.CreateDelegate(typeof(Func<IDataReader, object, TRet>), new MultiPocoFactory { m_Delegates = dels });
        }

        private void DoPreExecute(IDbCommand cmd)
        {
            // Setup command timeout
            if (this.OneTimeCommandTimeout != 0)
            {
                cmd.CommandTimeout = this.OneTimeCommandTimeout;
                this.OneTimeCommandTimeout = 0;
            }
            else if (this.CommandTimeout != 0)
            {
                cmd.CommandTimeout = this.CommandTimeout;
            }

            // Call hook
            this.OnExecutingCommand(cmd);

            // Save it
            this._lastSql = cmd.CommandText;
            this._lastArgs = (from IDataParameter parameter in cmd.Parameters select parameter.Value).ToArray();
        }

        private Delegate FindSplitPoint(Type typeThis, Type typeNext, string sql, IDataReader r, ref int pos)
        {
            // Last?
            if (typeNext == null)
            {
                return PocoData.ForType(typeThis).GetFactory(
                    sql, this._sharedConnection.ConnectionString, this.ForceDateTimesToUtc, pos, r.FieldCount - pos, r);
            }

            // Get PocoData for the two types
            var pdThis = PocoData.ForType(typeThis);
            var pdNext = PocoData.ForType(typeNext);

            // Find split point
            var firstColumn = pos;
            var usedColumns = new Dictionary<string, bool>();
            for (; pos < r.FieldCount; pos++)
            {
                // Split if field name has already been used, or if the field doesn't exist in current poco but does in the next
                var fieldName = r.GetName(pos);
                if (usedColumns.ContainsKey(fieldName)
                    || (!pdThis.Columns.ContainsKey(fieldName) && pdNext.Columns.ContainsKey(fieldName)))
                {
                    return pdThis.GetFactory(
                        sql, 
                        this._sharedConnection.ConnectionString, 
                        this.ForceDateTimesToUtc, 
                        firstColumn, 
                        pos - firstColumn, 
                        r);
                }

                usedColumns.Add(fieldName, true);
            }

            throw new InvalidOperationException(
                string.Format("Couldn't find split point between {0} and {1}", typeThis, typeNext));
        }

        private object GetAutoMapper(Type[] types)
        {
            // Build a key
            var kb = new StringBuilder();
            foreach (var t in types)
            {
                kb.Append(t);
                kb.Append(":");
            }

            var key = kb.ToString();

            // Check cache
            RWLock.EnterReadLock();
            try
            {
                object mapper;
                if (AutoMappers.TryGetValue(key, out mapper))
                {
                    return mapper;
                }
            }
            finally
            {
                RWLock.ExitReadLock();
            }

            // Create it
            RWLock.EnterWriteLock();
            try
            {
                // Try again
                object mapper;
                if (AutoMappers.TryGetValue(key, out mapper))
                {
                    return mapper;
                }

                // Create a method
                var m = new DynamicMethod("petapoco_automapper", types[0], types, true);
                var il = m.GetILGenerator();

                for (var i = 1; i < types.Length; i++)
                {
                    var handled = false;
                    for (var j = i - 1; j >= 0; j--)
                    {
                        // Find the property
                        var candidates = from p in types[j].GetProperties() where p.PropertyType == types[i] select p;
                        if (candidates.Count() == 0)
                        {
                            continue;
                        }

                        if (candidates.Count() > 1)
                        {
                            throw new InvalidOperationException(
                                string.Format(
                                    "Can't auto join {0} as {1} has more than one property of type {0}", 
                                    types[i], 
                                    types[j]));
                        }

                        // Generate code
                        il.Emit(OpCodes.Ldarg_S, j);
                        il.Emit(OpCodes.Ldarg_S, i);
                        il.Emit(OpCodes.Callvirt, candidates.First().GetSetMethod(true));
                        handled = true;
                    }

                    if (!handled)
                    {
                        throw new InvalidOperationException(string.Format("Can't auto join {0}", types[i]));
                    }
                }

                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ret);

                // Cache it
                var del = m.CreateDelegate(Expression.GetFuncType(types.Concat(types.Take(1)).ToArray()));
                AutoMappers.Add(key, del);
                return del;
            }
            finally
            {
                RWLock.ExitWriteLock();
            }
        }

        private Func<IDataReader, object, TRet> GetMultiPocoFactory<TRet>(Type[] types, string sql, IDataReader r)
        {
            // Build a key string  (this is crap, should address this at some point)
            var kb = new StringBuilder();
            kb.Append(typeof(TRet));
            kb.Append(":");
            foreach (var t in types)
            {
                kb.Append(":");
                kb.Append(t);
            }

            kb.Append(":");
            kb.Append(this._sharedConnection.ConnectionString);
            kb.Append(":");
            kb.Append(this.ForceDateTimesToUtc);
            kb.Append(":");
            kb.Append(sql);
            var key = kb.ToString();

            // Check cache
            RWLock.EnterReadLock();
            try
            {
                object oFactory;
                if (MultiPocoFactories.TryGetValue(key, out oFactory))
                {
                    return (Func<IDataReader, object, TRet>)oFactory;
                }
            }
            finally
            {
                RWLock.ExitReadLock();
            }

            // Cache it
            RWLock.EnterWriteLock();
            try
            {
                // Check again
                object oFactory;
                if (MultiPocoFactories.TryGetValue(key, out oFactory))
                {
                    return (Func<IDataReader, object, TRet>)oFactory;
                }

                // Create the factory
                var Factory = this.CreateMultiPocoFactory<TRet>(types, sql, r);

                MultiPocoFactories.Add(key, Factory);
                return Factory;
            }
            finally
            {
                RWLock.ExitWriteLock();
            }
        }


        public class ExpandoColumn : PocoColumn
        {

            public override object ChangeType(object val)
            {
                return val;
            }

            public override object GetValue(object target)
            {
                object val = null;
                (target as IDictionary<string, object>).TryGetValue(this.ColumnName, out val);
                return val;
            }

            public override void SetValue(object target, object val)
            {
                (target as IDictionary<string, object>)[this.ColumnName] = val;
            }

        }

        public class PocoColumn
        {

            public string ColumnName;

            public PropertyInfo PropertyInfo;

            public bool ResultColumn;



            public virtual object ChangeType(object val)
            {
                return Convert.ChangeType(val, this.PropertyInfo.PropertyType);
            }

            public virtual object GetValue(object target)
            {
                return this.PropertyInfo.GetValue(target, null);
            }

            public virtual void SetValue(object target, object val)
            {
                this.PropertyInfo.SetValue(target, val, null);
            }

        }

        public class PocoData
        {

            private static readonly FieldInfo fldConverters = typeof(PocoData).GetField(
                "m_Converters", BindingFlags.Static | BindingFlags.GetField | BindingFlags.NonPublic);

            private static readonly MethodInfo fnGetValue = typeof(IDataRecord).GetMethod(
                "GetValue", new[] { typeof(int) });

            private static readonly MethodInfo fnInvoke = typeof(Func<object, object>).GetMethod("Invoke");

            private static readonly MethodInfo fnIsDBNull = typeof(IDataRecord).GetMethod("IsDBNull");

            private static readonly MethodInfo fnListGetItem =
                typeof(List<Func<object, object>>).GetProperty("Item").GetGetMethod();

            private static readonly List<Func<object, object>> m_Converters = new List<Func<object, object>>();

            private static readonly Dictionary<Type, PocoData> m_PocoDatas = new Dictionary<Type, PocoData>();

            private static ReaderWriterLockSlim RWLock = new ReaderWriterLockSlim();



            public Type type;

            private readonly Dictionary<string, Delegate> PocoFactories = new Dictionary<string, Delegate>();



            public PocoData()
            {
            }

            public PocoData(Type t)
            {
                this.type = t;
                this.TableInfo = new TableInfo();

                // Get the table name
                var a = t.GetCustomAttributes(typeof(TableNameAttribute), true);
                this.TableInfo.TableName = a.Length == 0 ? t.Name : (a[0] as TableNameAttribute).Value;

                // Get the primary key
                a = t.GetCustomAttributes(typeof(PrimaryKeyAttribute), true);
                this.TableInfo.PrimaryKey = a.Length == 0 ? "ID" : (a[0] as PrimaryKeyAttribute).Value;
                this.TableInfo.SequenceName = a.Length == 0 ? null : (a[0] as PrimaryKeyAttribute).sequenceName;
                this.TableInfo.AutoIncrement = a.Length == 0 ? false : (a[0] as PrimaryKeyAttribute).autoIncrement;

                // Call column mapper
                if (Mapper != null)
                {
                    Mapper.GetTableInfo(t, this.TableInfo);
                }

                // Work out bound properties
                var ExplicitColumns = t.GetCustomAttributes(typeof(ExplicitColumnsAttribute), true).Length > 0;
                this.Columns = new Dictionary<string, PocoColumn>(StringComparer.OrdinalIgnoreCase);
                foreach (var pi in t.GetProperties())
                {
                    // Work out if properties is to be included
                    var ColAttrs = pi.GetCustomAttributes(typeof(ColumnAttribute), true);
                    if (ExplicitColumns)
                    {
                        if (ColAttrs.Length == 0)
                        {
                            continue;
                        }
                    }
                    else
                    {
                        if (pi.GetCustomAttributes(typeof(IgnoreAttribute), true).Length != 0)
                        {
                            continue;
                        }
                    }

                    var pc = new PocoColumn();
                    pc.PropertyInfo = pi;

                    // Work out the DB column name
                    if (ColAttrs.Length > 0)
                    {
                        var colattr = (ColumnAttribute)ColAttrs[0];
                        pc.ColumnName = colattr.Name;
                        if ((colattr as ResultColumnAttribute) != null)
                        {
                            pc.ResultColumn = true;
                        }
                    }

                    if (pc.ColumnName == null)
                    {
                        pc.ColumnName = pi.Name;
                        if (Mapper != null && !Mapper.MapPropertyToColumn(pi, ref pc.ColumnName, ref pc.ResultColumn))
                        {
                            continue;
                        }
                    }

                    // Store it
                    this.Columns.Add(pc.ColumnName, pc);
                }

                // Build column list for automatic select
                this.QueryColumns = (from c in this.Columns where !c.Value.ResultColumn select c.Key).ToArray();
            }



            public Dictionary<string, PocoColumn> Columns { get; private set; }

            public string[] QueryColumns { get; private set; }

            public TableInfo TableInfo { get; private set; }



            public static PocoData ForObject(object o, string primaryKeyName)
            {
                var t = o.GetType();
#if !PETAPOCO_NO_DYNAMIC
                if (t == typeof(ExpandoObject))
                {
                    var pd = new PocoData();
                    pd.TableInfo = new TableInfo();
                    pd.Columns = new Dictionary<string, PocoColumn>(StringComparer.OrdinalIgnoreCase);
                    pd.Columns.Add(primaryKeyName, new ExpandoColumn { ColumnName = primaryKeyName });
                    pd.TableInfo.PrimaryKey = primaryKeyName;
                    pd.TableInfo.AutoIncrement = true;
                    foreach (var col in (o as IDictionary<string, object>).Keys)
                    {
                        if (col != primaryKeyName)
                        {
                            pd.Columns.Add(col, new ExpandoColumn { ColumnName = col });
                        }
                    }

                    return pd;
                }
                else
                {
#endif
                    return ForType(t);
                }
            }

            public static PocoData ForType(Type t)
            {
#if !PETAPOCO_NO_DYNAMIC
                if (t == typeof(ExpandoObject))
                {
                    throw new InvalidOperationException("Can't use dynamic types with this method");
                }

#endif

                // Check cache
                RWLock.EnterReadLock();
                PocoData pd;
                try
                {
                    if (m_PocoDatas.TryGetValue(t, out pd))
                    {
                        return pd;
                    }
                }
                finally
                {
                    RWLock.ExitReadLock();
                }

                // Cache it
                RWLock.EnterWriteLock();
                try
                {
                    // Check again
                    if (m_PocoDatas.TryGetValue(t, out pd))
                    {
                        return pd;
                    }

                    // Create it
                    pd = new PocoData(t);

                    m_PocoDatas.Add(t, pd);
                }
                finally
                {
                    RWLock.ExitWriteLock();
                }

                return pd;
            }

            // Create factory function that can convert a IDataReader record into a POCO
            public Delegate GetFactory(
                string sql, 
                string connString, 
                bool ForceDateTimesToUtc, 
                int firstColumn, 
                int countColumns, 
                IDataReader r)
            {
                // Check cache
                var key = string.Format(
                    "{0}:{1}:{2}:{3}:{4}", sql, connString, ForceDateTimesToUtc, firstColumn, countColumns);
                RWLock.EnterReadLock();
                try
                {
                    // Have we already created it?
                    Delegate factory;
                    if (this.PocoFactories.TryGetValue(key, out factory))
                    {
                        return factory;
                    }
                }
                finally
                {
                    RWLock.ExitReadLock();
                }

                // Take the writer lock
                RWLock.EnterWriteLock();

                try
                {
                    // Check again, just in case
                    Delegate factory;
                    if (this.PocoFactories.TryGetValue(key, out factory))
                    {
                        return factory;
                    }

                    // Create the method
                    var m = new DynamicMethod(
                        "petapoco_factory_" + this.PocoFactories.Count.ToString(), 
                        this.type, 
                        new[] { typeof(IDataReader) }, 
                        true);
                    var il = m.GetILGenerator();

#if !PETAPOCO_NO_DYNAMIC
                    if (this.type == typeof(object))
                    {
                        // var poco=new T()
                        il.Emit(OpCodes.Newobj, typeof(ExpandoObject).GetConstructor(Type.EmptyTypes)); // obj

                        var fnAdd = typeof(IDictionary<string, object>).GetMethod("Add");

                        // Enumerate all fields generating a set assignment for the column
                        for (var i = firstColumn; i < firstColumn + countColumns; i++)
                        {
                            var srcType = r.GetFieldType(i);

                            il.Emit(OpCodes.Dup); // obj, obj
                            il.Emit(OpCodes.Ldstr, r.GetName(i)); // obj, obj, fieldname

                            // Get the converter
                            Func<object, object> converter = null;
                            if (Mapper != null)
                            {
                                converter = Mapper.GetFromDbConverter(null, srcType);
                            }

                            if (ForceDateTimesToUtc && converter == null && srcType == typeof(DateTime))
                            {
                                converter =
                                    delegate(object src) { return new DateTime(((DateTime)src).Ticks, DateTimeKind.Utc); };
                            }

                            // Setup stack for call to converter
                            AddConverterToStack(il, converter);

                            // r[i]
                            il.Emit(OpCodes.Ldarg_0); // obj, obj, fieldname, converter?,    rdr
                            il.Emit(OpCodes.Ldc_I4, i); // obj, obj, fieldname, converter?,  rdr,i
                            il.Emit(OpCodes.Callvirt, fnGetValue); // obj, obj, fieldname, converter?,  value

                            // Convert DBNull to null
                            il.Emit(OpCodes.Dup); // obj, obj, fieldname, converter?,  value, value
                            il.Emit(OpCodes.Isinst, typeof(DBNull));

                            // obj, obj, fieldname, converter?,  value, (value or null)
                            var lblNotNull = il.DefineLabel();
                            il.Emit(OpCodes.Brfalse_S, lblNotNull); // obj, obj, fieldname, converter?,  value
                            il.Emit(OpCodes.Pop); // obj, obj, fieldname, converter?
                            if (converter != null)
                            {
                                il.Emit(OpCodes.Pop); // obj, obj, fieldname, 
                            }

                            il.Emit(OpCodes.Ldnull); // obj, obj, fieldname, null
                            if (converter != null)
                            {
                                var lblReady = il.DefineLabel();
                                il.Emit(OpCodes.Br_S, lblReady);
                                il.MarkLabel(lblNotNull);
                                il.Emit(OpCodes.Callvirt, fnInvoke);
                                il.MarkLabel(lblReady);
                            }
                            else
                            {
                                il.MarkLabel(lblNotNull);
                            }

                            il.Emit(OpCodes.Callvirt, fnAdd);
                        }
                    }
                    else
#endif
                        if (this.type.IsValueType || this.type == typeof(string) || this.type == typeof(byte[]))
                        {
                            // Do we need to install a converter?
                            var srcType = r.GetFieldType(0);
                            var converter = GetConverter(ForceDateTimesToUtc, null, srcType, this.type);

                            // "if (!rdr.IsDBNull(i))"
                            il.Emit(OpCodes.Ldarg_0); // rdr
                            il.Emit(OpCodes.Ldc_I4_0); // rdr,0
                            il.Emit(OpCodes.Callvirt, fnIsDBNull); // bool
                            var lblCont = il.DefineLabel();
                            il.Emit(OpCodes.Brfalse_S, lblCont);
                            il.Emit(OpCodes.Ldnull); // null
                            var lblFin = il.DefineLabel();
                            il.Emit(OpCodes.Br_S, lblFin);

                            il.MarkLabel(lblCont);

                            // Setup stack for call to converter
                            AddConverterToStack(il, converter);

                            il.Emit(OpCodes.Ldarg_0); // rdr
                            il.Emit(OpCodes.Ldc_I4_0); // rdr,0
                            il.Emit(OpCodes.Callvirt, fnGetValue); // value

                            // Call the converter
                            if (converter != null)
                            {
                                il.Emit(OpCodes.Callvirt, fnInvoke);
                            }

                            il.MarkLabel(lblFin);
                            il.Emit(OpCodes.Unbox_Any, this.type); // value converted
                        }
                        else
                        {
                            // var poco=new T()
                            il.Emit(
                                OpCodes.Newobj, 
                                this.type.GetConstructor(
                                    BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, 
                                    null, 
                                    new Type[0], 
                                    null));

                            // Enumerate all fields generating a set assignment for the column
                            for (var i = firstColumn; i < firstColumn + countColumns; i++)
                            {
                                // Get the PocoColumn for this db column, ignore if not known
                                PocoColumn pc;
                                if (!this.Columns.TryGetValue(r.GetName(i), out pc))
                                {
                                    continue;
                                }

                                // Get the source type for this column
                                var srcType = r.GetFieldType(i);
                                var dstType = pc.PropertyInfo.PropertyType;

                                // "if (!rdr.IsDBNull(i))"
                                il.Emit(OpCodes.Ldarg_0); // poco,rdr
                                il.Emit(OpCodes.Ldc_I4, i); // poco,rdr,i
                                il.Emit(OpCodes.Callvirt, fnIsDBNull); // poco,bool
                                var lblNext = il.DefineLabel();
                                il.Emit(OpCodes.Brtrue_S, lblNext); // poco

                                il.Emit(OpCodes.Dup); // poco,poco

                                // Do we need to install a converter?
                                var converter = GetConverter(ForceDateTimesToUtc, pc, srcType, dstType);

                                // Fast
                                var Handled = false;
                                if (converter == null)
                                {
                                    var valuegetter = typeof(IDataRecord).GetMethod(
                                        "Get" + srcType.Name, new[] { typeof(int) });
                                    if (valuegetter != null && valuegetter.ReturnType == srcType
                                        &&
                                        (valuegetter.ReturnType == dstType
                                         || valuegetter.ReturnType == Nullable.GetUnderlyingType(dstType)))
                                    {
                                        il.Emit(OpCodes.Ldarg_0); // *,rdr
                                        il.Emit(OpCodes.Ldc_I4, i); // *,rdr,i
                                        il.Emit(OpCodes.Callvirt, valuegetter); // *,value

                                        // Convert to Nullable
                                        if (Nullable.GetUnderlyingType(dstType) != null)
                                        {
                                            il.Emit(
                                                OpCodes.Newobj, 
                                                dstType.GetConstructor(new[] { Nullable.GetUnderlyingType(dstType) }));
                                        }

                                        il.Emit(OpCodes.Callvirt, pc.PropertyInfo.GetSetMethod(true)); // poco
                                        Handled = true;
                                    }
                                }

                                // Not so fast
                                if (!Handled)
                                {
                                    // Setup stack for call to converter
                                    AddConverterToStack(il, converter);

                                    // "value = rdr.GetValue(i)"
                                    il.Emit(OpCodes.Ldarg_0); // *,rdr
                                    il.Emit(OpCodes.Ldc_I4, i); // *,rdr,i
                                    il.Emit(OpCodes.Callvirt, fnGetValue); // *,value

                                    // Call the converter
                                    if (converter != null)
                                    {
                                        il.Emit(OpCodes.Callvirt, fnInvoke);
                                    }

                                    // Assign it
                                    il.Emit(OpCodes.Unbox_Any, pc.PropertyInfo.PropertyType); // poco,poco,value
                                    il.Emit(OpCodes.Callvirt, pc.PropertyInfo.GetSetMethod(true)); // poco
                                }

                                il.MarkLabel(lblNext);
                            }

                            var fnOnLoaded = RecurseInheritedTypes(
                                this.type, 
                                (x) =>
                                x.GetMethod(
                                    "OnLoaded", 
                                    BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, 
                                    null, 
                                    new Type[0], 
                                    null));
                            if (fnOnLoaded != null)
                            {
                                il.Emit(OpCodes.Dup);
                                il.Emit(OpCodes.Callvirt, fnOnLoaded);
                            }
                        }

                    il.Emit(OpCodes.Ret);

                    // Cache it, return it
                    var del = m.CreateDelegate(Expression.GetFuncType(typeof(IDataReader), this.type));
                    this.PocoFactories.Add(key, del);
                    return del;
                }
                finally
                {
                    RWLock.ExitWriteLock();
                }
            }



            private static void AddConverterToStack(ILGenerator il, Func<object, object> converter)
            {
                if (converter != null)
                {
                    // Add the converter
                    var converterIndex = m_Converters.Count;
                    m_Converters.Add(converter);

                    // Generate IL to push the converter onto the stack
                    il.Emit(OpCodes.Ldsfld, fldConverters);
                    il.Emit(OpCodes.Ldc_I4, converterIndex);
                    il.Emit(OpCodes.Callvirt, fnListGetItem); // Converter
                }
            }

            private static Func<object, object> GetConverter(
                bool forceDateTimesToUtc, PocoColumn pc, Type srcType, Type dstType)
            {
                Func<object, object> converter = null;

                // Get converter from the mapper
                if (Mapper != null)
                {
                    if (pc != null)
                    {
                        converter = Mapper.GetFromDbConverter(pc.PropertyInfo, srcType);
                    }
                    else
                    {
                        var m2 = Mapper as IMapper2;
                        if (m2 != null)
                        {
                            converter = m2.GetFromDbConverter(dstType, srcType);
                        }
                    }
                }

                // Standard DateTime->Utc mapper
                if (forceDateTimesToUtc && converter == null && srcType == typeof(DateTime)
                    && (dstType == typeof(DateTime) || dstType == typeof(DateTime?)))
                {
                    converter = delegate(object src) { return new DateTime(((DateTime)src).Ticks, DateTimeKind.Utc); };
                }

                // Forced type conversion including integral types -> enum
                if (converter == null)
                {
                    if (dstType.IsEnum && IsIntegralType(srcType))
                    {
                        if (srcType != typeof(int))
                        {
                            converter = delegate(object src) { return Convert.ChangeType(src, typeof(int), null); };
                        }
                    }
                    else if (!dstType.IsAssignableFrom(srcType))
                    {
                        converter = delegate(object src) { return Convert.ChangeType(src, dstType, null); };
                    }
                }

                return converter;
            }

            private static bool IsIntegralType(Type t)
            {
                var tc = Type.GetTypeCode(t);
                return tc >= TypeCode.SByte && tc <= TypeCode.UInt64;
            }

            private static T RecurseInheritedTypes<T>(Type t, Func<Type, T> cb)
            {
                while (t != null)
                {
                    var info = cb(t);
                    if (info != null)
                    {
                        return info;
                    }

                    t = t.BaseType;
                }

                return default(T);
            }

        }

        private class MultiPocoFactory
        {

            public List<Delegate> m_Delegates;



            public Delegate GetItem(int index)
            {
                return this.m_Delegates[index];
            }

        }

        // Member variables
    }

    // Transaction object helps maintain transaction depth counts
    public class Transaction : IDisposable
    {

        private Database _db;



        public Transaction(Database db)
        {
            this._db = db;
            this._db.BeginTransaction();
        }



        public virtual void Complete()
        {
            this._db.CompleteTransaction();
            this._db = null;
        }

        public void Dispose()
        {
            if (this._db != null)
            {
                this._db.AbortTransaction();
            }
        }

    }

    // Simple helper class for building SQL statments
    public class Sql
    {

        private readonly object[] _args;

        private readonly string _sql;

        private object[] _argsFinal;

        private Sql _rhs;

        private string _sqlFinal;



        public Sql()
        {
        }

        public Sql(string sql, params object[] args)
        {
            this._sql = sql;
            this._args = args;
        }



        public static Sql Builder
        {
            get
            {
                return new Sql();
            }
        }

        public object[] Arguments
        {
            get
            {
                this.Build();
                return this._argsFinal;
            }
        }

        public string SQL
        {
            get
            {
                this.Build();
                return this._sqlFinal;
            }
        }



        public Sql Append(Sql sql)
        {
            if (this._rhs != null)
            {
                this._rhs.Append(sql);
            }
            else
            {
                this._rhs = sql;
            }

            return this;
        }

        public Sql Append(string sql, params object[] args)
        {
            return this.Append(new Sql(sql, args));
        }

        public Sql From(params object[] tables)
        {
            return this.Append(new Sql("FROM " + string.Join(", ", (from x in tables select x.ToString()).ToArray())));
        }

        public Sql GroupBy(params object[] columns)
        {
            return
                this.Append(new Sql("GROUP BY " + string.Join(", ", (from x in columns select x.ToString()).ToArray())));
        }

        public SqlJoinClause InnerJoin(string table)
        {
            return this.Join("INNER JOIN ", table);
        }

        public SqlJoinClause LeftJoin(string table)
        {
            return this.Join("LEFT JOIN ", table);
        }

        public Sql OrderBy(params object[] columns)
        {
            return
                this.Append(new Sql("ORDER BY " + string.Join(", ", (from x in columns select x.ToString()).ToArray())));
        }

        public Sql Select(params object[] columns)
        {
            return this.Append(
                new Sql("SELECT " + string.Join(", ", (from x in columns select x.ToString()).ToArray())));
        }

        public Sql Where(string sql, params object[] args)
        {
            return this.Append(new Sql("WHERE (" + sql + ")", args));
        }



        private static bool Is(Sql sql, string sqltype)
        {
            return sql != null && sql._sql != null
                   && sql._sql.StartsWith(sqltype, StringComparison.InvariantCultureIgnoreCase);
        }

        private void Build()
        {
            // already built?
            if (this._sqlFinal != null)
            {
                return;
            }

            // Build it
            var sb = new StringBuilder();
            var args = new List<object>();
            this.Build(sb, args, null);
            this._sqlFinal = sb.ToString();
            this._argsFinal = args.ToArray();
        }

        private void Build(StringBuilder sb, List<object> args, Sql lhs)
        {
            if (!string.IsNullOrEmpty(this._sql))
            {
                // Add SQL to the string
                if (sb.Length > 0)
                {
                    sb.Append("\n");
                }

                var sql = Database.ProcessParams(this._sql, this._args, args);

                if (Is(lhs, "WHERE ") && Is(this, "WHERE "))
                {
                    sql = "AND " + sql.Substring(6);
                }

                if (Is(lhs, "ORDER BY ") && Is(this, "ORDER BY "))
                {
                    sql = ", " + sql.Substring(9);
                }

                sb.Append(sql);
            }

            // Now do rhs
            if (this._rhs != null)
            {
                this._rhs.Build(sb, args, this);
            }
        }

        private SqlJoinClause Join(string JoinType, string table)
        {
            return new SqlJoinClause(this.Append(new Sql(JoinType + table)));
        }


        public class SqlJoinClause
        {

            private readonly Sql _sql;



            public SqlJoinClause(Sql sql)
            {
                this._sql = sql;
            }



            public Sql On(string onClause, params object[] args)
            {
                return this._sql.Append("ON " + onClause, args);
            }

        }
    }
}