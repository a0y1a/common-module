using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KeKeSoftPlatform.Common;
using EntityFramework.Extensions;
using System.Linq.Expressions;
using System.Reflection;
using System.Data.Entity;
using System.Data.SqlClient;
using KeKeSoftPlatform.WebExtension;


namespace KeKeSoftPlatform.Core
{
    public static class DbExtension
    {
        public static Pager<TEntity> Page<TEntity>(this IQueryable<TEntity> query, int pageNum, int pageSize = Pager.DEFAULT_PAGE_SIZE) where TEntity : class
        {
            var queryCount = query.FutureCount();
            var data = query.Skip((pageNum - 1) * pageSize).Take(pageSize).Future();

            var pager = new Pager<TEntity>
            {
                ItemCount = queryCount.Value,
                PageNum = pageNum,
                PageSize = pageSize,
                Data = data.ToList()
            };
            return pager;
        }

        #region 备份还原
        public static void Backup(this Database database, string fullPath)
        {
            database.ExecuteSqlCommand(TransactionalBehavior.DoNotEnsureTransaction, "use master;backup database @0 to disk=@1;",
                new SqlParameter
                {
                    ParameterName = "@0",
                    Value = database.Connection.Database
                }, new SqlParameter
                {
                    ParameterName = "@1",
                    Value = fullPath
                });
        }

        public static void Restore(this Database database, string fullPath)
        {
            database.ExecuteSqlCommand(TransactionalBehavior.DoNotEnsureTransaction, 
                                            @"USE [master]
                                                DECLARE @spid NVARCHAR(20)   
                                                DECLARE cDblogin CURSOR  
                                                FOR
                                                    SELECT CAST(spid AS VARCHAR(20)) AS spid
                                                    FROM   MASTER..sysprocesses
                                                    WHERE  dbid = DB_ID(@dbname)

                                                                                            OPEN   cDblogin  
                                                                                            FETCH   NEXT   FROM   cDblogin   
                                                INTO   @spid   
                                                WHILE @@fetch_status = 0
                                                BEGIN
                                                    IF @spid <> @@SPID
                                                        EXEC ('kill   ' + @spid)
    
                                                    FETCH NEXT FROM cDblogin INTO @spid
                                                END       
                                                CLOSE   cDblogin   
                                                DEALLOCATE   cDblogin  
                                                RESTORE DATABASE @dbname  FROM DISK 
                                                = @fullPath 
                                                WITH REPLACE",
                                                new SqlParameter { ParameterName = "dbname", Value = database.Connection.Database }, 
                                                new SqlParameter { ParameterName = "fullPath", Value = fullPath });
        }
        #endregion
    }
}
