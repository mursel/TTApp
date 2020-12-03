using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using Dapper;
using Dapper.Contrib;
using Dapper.Contrib.Extensions;
using DbProvider.Model;

namespace DbProvider.Pomocna
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private RepositoryResult<T> repositoryResult;

        public Repository()
        {
            repositoryResult = new RepositoryResult<T>();
        }

        public async Task<RepositoryResult<T>> AddAsync(T entity)
        {
            try
            {
                using (var connection = DatabaseManager.GetConnection())
                {
                    await connection.OpenAsync();
                    using (var transakcija = connection.BeginTransaction())
                    {
                        var index = await connection.InsertAsync<T>(entity, transakcija);
                        repositoryResult.Entity = await connection.GetAsync<T>(index, transakcija);
                        transakcija.Commit();
                    }
                }
            }
            catch (SqlException se)
            {
                repositoryResult.Result = new HResultStatus(se, HResultStatus.ResultStatusCodes.SQL_EXCEPTION);
            }
            catch(SqlTypeException ste)
            {
                repositoryResult.Result = new HResultStatus(ste, HResultStatus.ResultStatusCodes.SQL_EXCEPTION);
            }
            catch (ApplicationException ae)
            {
                repositoryResult.Result = new HResultStatus(ae, HResultStatus.ResultStatusCodes.APPLICATION_ERROR);
            }
            return repositoryResult;
        }

        public async Task<RepositoryResult<T>> GetByIdAsync(int Id)
        {
            try
            {
                using (var connection = DatabaseManager.GetConnection())
                {
                    await connection.OpenAsync();
                    repositoryResult.Entity = await connection.GetAsync<T>(Id);
                }

            }
            catch (ApplicationException ae)
            {
                repositoryResult.Result = new HResultStatus(ae, HResultStatus.ResultStatusCodes.APPLICATION_ERROR);
            }
            return repositoryResult;
        }

        public async Task<RepositoryResult<T>> RemoveAsync(T entity)
        {
            repositoryResult = new RepositoryResult<T>();
            try
            {
                using (var connection = DatabaseManager.GetConnection())
                {
                    await connection.OpenAsync();
                    using (var transaction = connection.BeginTransaction())
                    {
                        await connection.DeleteAsync<T>(entity, transaction);
                        transaction.Commit();
                    }                    
                }
            }
            catch (InvalidOperationException ioe)
            {
                repositoryResult.Result = new HResultStatus(ioe, HResultStatus.ResultStatusCodes.APPLICATION_ERROR);
            }
            catch (SqlException se)
            {
                repositoryResult.Result = new HResultStatus(se, HResultStatus.ResultStatusCodes.SQL_EXCEPTION);
            }
            catch (ApplicationException ae)
            {
                repositoryResult.Result = new HResultStatus(ae, HResultStatus.ResultStatusCodes.APPLICATION_ERROR);
            }
            return repositoryResult;
        }

        public async Task<RepositoryResult<T>> UpdateAsync(T entity)
        {
            try
            {
                using (var connection = DatabaseManager.GetConnection())
                {
                    await connection.OpenAsync();
                    var transaction = connection.BeginTransaction();
                    await connection.UpdateAsync<T>(entity, transaction);
                    transaction.Commit();
                }
            }
            catch (SqlException se)
            {
                repositoryResult.Result = new HResultStatus(se, HResultStatus.ResultStatusCodes.SQL_EXCEPTION);
            }
            catch (ApplicationException ae)
            {
                repositoryResult.Result = new HResultStatus(ae, HResultStatus.ResultStatusCodes.APPLICATION_ERROR);
            }
            return repositoryResult;
        }

        public async Task<RepositoryResult<T>> GetListAsync()
        {
            try
            {
                using (var connection = DatabaseManager.GetConnection())
                {
                    await connection.OpenAsync();
                    repositoryResult.EntityCollection = await connection.GetAllAsync<T>();
                }
            }
            catch (Exception e)
            {
                repositoryResult.Result = new HResultStatus(e, HResultStatus.ResultStatusCodes.SQL_EXCEPTION);
            }
            //catch (SqlException se)
            //{
            //    repositoryResult.Result = new HResultStatus(se, HResultStatus.ResultStatusCodes.SQL_EXCEPTION);
            //}
            return repositoryResult;
        }

        public async Task<RepositoryResult<T>> GetListAsync(Func<T, bool> expression)
        {
            try
            {
                using (var connection = DatabaseManager.GetConnection())
                {
                    await connection.OpenAsync();
                    var list = await connection.GetAllAsync<T>();
                    repositoryResult.EntityCollection = list.Where(expression).ToList();
                }
            }
            catch (SqlException se)
            {
                repositoryResult.Result = new HResultStatus(se, HResultStatus.ResultStatusCodes.SQL_EXCEPTION);
            }
            catch (ApplicationException ae)
            {
                repositoryResult.Result = new HResultStatus(ae, HResultStatus.ResultStatusCodes.APPLICATION_ERROR);
            }
            return repositoryResult;
        }



        public async Task<RepositoryResult<T>> UpdateAllAsync(IEnumerable<T> entiteti)
        {
            try
            {
                using (var connection = DatabaseManager.GetConnection())
                {
                    await connection.OpenAsync();
                    var transaction = connection.BeginTransaction();
                    await connection.UpdateAsync(entiteti, transaction);
                    transaction.Commit();
                }
            }
            catch (SqlException se)
            {
                repositoryResult.Result = new HResultStatus(se, HResultStatus.ResultStatusCodes.SQL_EXCEPTION);
            }
            catch (ApplicationException ae)
            {
                repositoryResult.Result = new HResultStatus(ae, HResultStatus.ResultStatusCodes.APPLICATION_ERROR);
            }
            return repositoryResult;
        }

        //public async Task<RepositoryResult<T>> QueryMultipleByTAsync(string query, int Id)
        //{
        //    try
        //    {
        //        using (var connection = DatabaseManager.GetConnection())
        //        {
        //            await connection.OpenAsync();
        //            var gridReader = await connection.QueryAsync<T>()
        //            if (gridReader != null)
        //                repositoryResult.EntityCollection = await gridReader.ReadAsync<T>();
        //        }
        //    }
        //    catch (SqlException se)
        //    {
        //        repositoryResult.Result = new HResultStatus(se, HResultStatus.ResultStatusCodes.SQL_EXCEPTION);
        //    }
        //    catch (ApplicationException ae)
        //    {
        //        repositoryResult.Result = new HResultStatus(ae, HResultStatus.ResultStatusCodes.APPLICATION_ERROR);
        //    }
        //    return repositoryResult;
        //}

        public async Task<bool> UpdateCustomFieldsAsync(string tableName, string fieldName, string criteria, string value)
        {
            try
            {
                using (var connection = DatabaseManager.GetConnection())
                {
                    await connection.OpenAsync();

                    var transaction = connection.BeginTransaction();

                    await connection.ExecuteAsync("UPDATE " + tableName + " SET " + fieldName + " = " + value + " WHERE " + criteria, transaction);

                    transaction.Commit();
                }
            }
            catch (SqlException se)
            {
                repositoryResult.Result = new HResultStatus(se, HResultStatus.ResultStatusCodes.SQL_EXCEPTION);
                return false;
            }
            catch (ApplicationException ae)
            {
                repositoryResult.Result = new HResultStatus(ae, HResultStatus.ResultStatusCodes.APPLICATION_ERROR);
                return false;
            }
            return true;
        }

        public async Task<RepositoryResult<T>> AddBulkAsync(DataTable dataTable)
        {
            try
            {
                using (var cn = new SqlConnection(DatabaseManager.ConnectionString))
                {
                    await cn.OpenAsync();
                    var transaction = cn.BeginTransaction();
                    using (var sqlBulkCopy = new SqlBulkCopy(cn, SqlBulkCopyOptions.Default, transaction))
                    {
                        sqlBulkCopy.DestinationTableName = dataTable.TableName;
                        sqlBulkCopy.BatchSize = 5000;        // velicina serije (tuning je potreban za pravu velicinu)
                        sqlBulkCopy.BulkCopyTimeout = 40;   // standardno vrijeme je 30 sekundi
                        await sqlBulkCopy.WriteToServerAsync(dataTable);
                        transaction.Commit();
                        repositoryResult.Result = new HResultStatus(null, HResultStatus.ResultStatusCodes.SUCCESS);
                    }
                }
            }
            catch(Exception exc) when(exc is InvalidOperationException || exc is ApplicationException)
            {
                repositoryResult.Result = new HResultStatus(exc, HResultStatus.ResultStatusCodes.APPLICATION_ERROR);
            }
            catch(SqlException se)
            {
                repositoryResult.Result = new HResultStatus(se, HResultStatus.ResultStatusCodes.SQL_EXCEPTION);
            }

            return repositoryResult;
        }
    }
}
