using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace Data
{
    public class Persistencia
    {
        private static void FiltrarCampos(ref SqlCommand _comando, string[] _campos, string[] _valores)
        {
            try
            {
                if (_campos.Length != 0)
                {
                    for (int i = 0; i <= _campos.Length - 1; i++)
                    {
                        _comando.Parameters.AddWithValue(_campos[i], _valores[i]);
                    }
                }
            }
            catch
            {
                throw;
            }
        }

        public static void ExecutarComando(string _sql, string[] _campos, string[] _valores)
        {
            using (SqlConnection conexao = GetDbConnection())
            {
                SqlCommand comando = new SqlCommand
                {
                    Connection = conexao,
                    CommandText = _sql
                };

                try
                {
                    FiltrarCampos(ref comando, _campos, _valores);
                    comando.ExecuteNonQuery();
                }
                catch
                {
                    throw;
                }
                finally
                {
                    comando.Dispose();
                }
            }
        }

        public static void ExecutarComando(string _sql)
        {
            using (SqlConnection conexao = GetDbConnection())
            {
                SqlCommand comando = new SqlCommand
                {
                    Connection = conexao,
                    CommandText = _sql
                };

                try
                {
                    comando.ExecuteNonQuery();
                }
                catch
                {
                    throw;
                }
                finally
                {
                    comando.Dispose();
                }
            }
        }

        public static IDataReader ExecutarSQLReader(string _sql, string[] _campos, string[] _valores, ref SqlConnection _conexao)
        {
            IDataReader _dr = null;

            _conexao = GetDbConnection();

            SqlCommand _comando = new SqlCommand
            {
                Connection = _conexao,
                CommandText = _sql
            };

            try
            {
                FiltrarCampos(ref _comando, _campos, _valores);
                _dr = _comando.ExecuteReader();
            }
            catch
            {
                throw;
            }
            finally
            {
                _comando.Dispose();
            }

            return _dr;
        }

        //public static IDataReader ExecutarSQLReader(string _sql,ref SqlConnection _conexao)
        //{
        //    IDataReader _dr = null;
        //    _conexao = GetDbConnection();
        //    SqlCommand _comando = new SqlCommand();

        //    _comando.Connection = _conexao;
        //    _comando.CommandText = _sql;

        //    try
        //    {
        //        _dr = _comando.ExecuteReader();
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    finally
        //    {
        //        _comando.Dispose();

        //    }

        //    return _dr;
        //}


        public static IDataReader ExecutarSQLReader(string _sql, ref SqlConnection _conexao)
        {
            IDataReader _dr = null;
            _conexao = GetDbConnection();
            SqlCommand _comando = new SqlCommand
            {
                Connection = _conexao,
                CommandText = _sql
            };

            try
            {
                _dr = _comando.ExecuteReader();
            }
            catch
            {
                throw;
            }
            finally
            {
                _comando.Dispose();
            }

            return _dr;
        }

        public static int ExecutarScalar(string _sql, string[] _campos, string[] _valores)
        {
            int _total = 0;

            using (SqlConnection _conexao = GetDbConnection())
            {
                SqlCommand _comando = new SqlCommand();

                try
                {
                    _comando.Connection = _conexao;
                    _comando.CommandText = _sql;

                    FiltrarCampos(ref _comando, _campos, _valores);

                    _total = (int)_comando.ExecuteScalar();
                }
                catch
                {
                    throw;
                }
                finally
                {
                    _comando.Dispose();
                }
            }

            return _total;
        }

        private static SqlConnection GetDbConnection()
        {
            try
            {
                SqlConnection _conexao = new SqlConnection(ini.Read("BD", "GERAL"));
                _conexao.Open();

                return _conexao;
            }
            catch
            {
                throw;
            }
        }

        //    Private Shared Function GetDbConnection() As SqlConnection
        //    Try
        //        Dim conString As String = ConfigurationManager.ConnectionStrings("conexaoClienteSQLServer").ConnectionString
        //        Dim connection As SqlConnection = New SqlConnection(conString)
        //        connection.Open()
        //        Return connection
        //    Catch ex As Exception
        //        Throw ex
        //    End Try
        //End Function
    }
}
