using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Objeto;
using Banco;
using System.Data;
using System.Data.SqlClient;
using Data;

namespace Banco
{
    public class Negocio
    {
        public static Evento GetEventoByCodigo(string _codigo)
        {
            Evento _evento = null;

            SqlDataReader _dr = null;
            SqlConnection _conexao = null;

            string[] _campoSelect = new string[1];
            string[] _campoValor = new string[1];

            _campoSelect[0] = "@CODIGO";
            _campoValor[0] = _codigo;

            try
            {
                string _select = "SELECT ID, CODIGO, DESCRICAO, PRIORIDADE, EXIBE_ZONA FROM EVENTO WHERE CODIGO = @CODIGO";

                _dr = (SqlDataReader)Persistencia.ExecutarSQLReader(_select, _campoSelect, _campoValor, ref _conexao);

                if (_dr.HasRows)
                {
                    while (_dr.Read())
                    {
                        _evento = new Evento
                        {
                            Id = (int)_dr["ID"],
                            Codigo = (string)_dr["CODIGO"],
                            Descricao = (string)_dr["DESCRICAO"],
                            Prioridade = (int)_dr["PRIORIDADE"],
                            ExibeZona = (int)_dr["EXIBE_ZONA"]
                        };
                    }
                }
                else
                {
                    _evento = new Evento
                    {
                        Id = 0,
                        Codigo = _codigo,
                        Descricao = "EVENTO NAO CADASTRADO",
                        Prioridade = 0,
                        ExibeZona = 0
                    };
                }

            }
            catch
            {
                throw;
            }
            finally
            {
                if (_conexao.State == System.Data.ConnectionState.Open)
                {
                    _conexao.Close();
                }
            }

            return _evento;
        }

        //public static List<Historico> GetHistorico()
        //{
        //    Historico _historico = null;
        //    Central _central = null;
        //    Cliente _cliente = null;
        //    Motivo _motivo = null;
        //    Evento _evento = null;

        //    List<Historico> _historicos = new List<Historico>();

        //    SqlDataReader _dr = null;
        //    SqlConnection _conexao = null;

        //    try
        //    {
        //        string _select = "select c.id as 'id_central', c.modelo, c.conta, c.mac, e.id as 'id_evento', e.codigo, e.descricao as 'evento_descricao', e.prioridade, u.id as 'id_cliente', u.nome, u.endereco, h.id as 'id_evento', h.zona, m.id as 'id_motivo', m.descricao as 'descricao_motivo', h.data_evento, h.obs from historico h " +
        //                         "left outer join central c on h.central = c.id left outer join cliente u on u.CENTRAL = c.ID " +
        //                         "left outer join evento e on h.evento = e.ID left outer join motivo_disparo m on h.motivo = m.id " +
        //                         "order by h.data_evento";

        //        _dr = (SqlDataReader)Persistencia.ExecutarSQLReader(_select, ref _conexao);


        //        if (_dr.HasRows)
        //        {
        //            while (_dr.Read())
        //            {
        //                _central = new Central
        //                {
        //                    Id = (int)_dr["id_central"],
        //                    Modelo = (string)_dr["modelo"],
        //                    Conta = (string)_dr["conta"],
        //                    Mac = (string)_dr["mac"]
        //                };

        //                _evento = new Evento
        //                {
        //                    Id = (int)_dr["id_evento"],
        //                    Codigo = (string)_dr["codigo"],
        //                    Descricao = (string)_dr["evento_descricao"],
        //                    Prioridade = (int)_dr["prioridade"]
        //                };

        //                _cliente = new Cliente
        //                {
        //                    Id = (int)_dr["id_cliente"],
        //                    Nome = (string)_dr["nome"],
        //                    Endereco = (string)_dr["endereco"]
        //                };

        //                _motivo = new Motivo();
        //                if (_dr["id_motivo"] != DBNull.Value)
        //                {
        //                    _motivo.Descricao = (string)_dr["descricao_motivo"];
        //                }
        //                else
        //                {
        //                    _motivo.Descricao = "";
        //                }


        //                _historico = new Historico
        //                {
        //                    Id = (int)_dr["id_evento"],
        //                    Obs = (string)_dr["obs"],
        //                    DataHora = (DateTime)_dr["data_evento"],
        //                    Zona = (string)_dr["zona"]
        //                };


        //                _central.Cliente = _cliente;

        //                _historico.Central = _central;
        //                _historico.Motivo = _motivo;
        //                _historico.Evento = _evento;


        //                _historicos.Add(_historico);
        //            }
        //        }
        //    }
        //    catch
        //    {
        //        throw;
        //    }
        //    finally
        //    {
        //        if (_conexao.State == System.Data.ConnectionState.Open)
        //        {
        //            _conexao.Close();
        //        }
        //    }

        //    return _historicos;
        //}

        public static List<Motivo> GetMotivos()
        {
            Motivo _motivo = null;
            List<Motivo> _motivos = new List<Motivo>();

            SqlDataReader _dr = null;
            SqlConnection _conexao = null;

            try
            {
                string _select = "SELECT ID, DESCRICAO FROM MOTIVO_DISPARO";
                _dr = (SqlDataReader)Persistencia.ExecutarSQLReader(_select, ref _conexao);

                if (_dr.HasRows)
                {
                    while (_dr.Read())
                    {
                        _motivo = new Motivo()
                        {
                            Id = (int)_dr["ID"],
                            Descricao = (string)_dr["DESCRICAO"]
                        };

                        _motivos.Add(_motivo);
                    }
                }
            }
            catch
            {
                throw;
            }
            finally
            {
                if (_conexao.State == System.Data.ConnectionState.Open)
                {
                    _conexao.Close();
                }
            }

            return _motivos;
        }


        public static void UpdateIPCentral(Central _central)
        {
            try
            {
                string[] _camposInsert = new string[4];
                string[] _valorInsert = new string[4];

                _camposInsert[0] = "@ENDERECO";
                _camposInsert[1] = "@ULTIMA_INTERACAO";
                _camposInsert[2] = "@INTERFACE";
                _camposInsert[3] = "@MAC";

                _valorInsert[0] = ((System.Net.IPEndPoint)_central.ClientIP.Client.RemoteEndPoint).Address.ToString();
                _valorInsert[1] = _central.UltimoMovimento.ToString();
                _valorInsert[2] = _central.Interface;
                _valorInsert[3] = _central.Mac;

                string _update = "UPDATE CENTRAL SET ENDERECO = @ENDERECO, ULTIMA_INTERACAO =  Convert(datetime, @ULTIMA_INTERACAO, 103), INTERFACE_CONEXAO = @INTERFACE WHERE MAC = @MAC";

                Persistencia.ExecutarComando(_update, _camposInsert, _valorInsert);
            }
            catch
            {
                throw;
            }
        }


        public static void GetCentralByMAC(ref Central _central)
        {
            //List<Usuario> _usuarios = new List<Usuario>();
            //Central _central = null;
            Cliente _cliente = null;
            Setor _setor;

            SqlDataReader _dr = null;
            SqlConnection _conexao = null;

            string[] _campoSelect = new string[1];
            string[] _campoValor = new string[1];

            _campoSelect[0] = "@MAC";
            _campoValor[0] = _central.Mac;

            try
            {

                string _select = "SELECT C.ID AS 'ID_CENTRAL', C.MODELO, C.CONTA, U.ID AS 'ID_CLIENTE', C.ULTIMA_INTERACAO, U.NOME, U.ENDERECO, U.TELEFONE, U.CELULAR, S.ID as 'id_setor', S.DESCRICAO as 'descricao_setor', S.OBS as 'obs_setor' FROM CENTRAL C " +
                                 "LEFT JOIN CLIENTE U ON C.CLIENTE = U.ID  " +
                                 "LEFT JOIN SETOR S ON C.SETOR = S.ID " +
                                 "WHERE C.MAC = @MAC ";

                _dr = (SqlDataReader)Persistencia.ExecutarSQLReader(_select, _campoSelect, _campoValor, ref _conexao);

                if (_dr.HasRows)
                {
                    while (_dr.Read())
                    {
                        _central.Id = (int)_dr["ID_CENTRAL"];
                        _central.Modelo = (string)_dr["MODELO"];
                        //_central.Conta = (string)_dr["CONTA"];
                        //_central.Mac = (string)_dr["MAC"];
                        //_central.UltimoMovimento = (DateTime)_dr["ULTIMA_INTERACAO"];

                        _cliente = new Cliente
                        {
                            Id = (int)_dr["ID_CLIENTE"],
                            Nome = (string)_dr["NOME"],
                            Endereco = (string)_dr["ENDERECO"],
                            Telefone = (string)_dr["TELEFONE"],
                            Celular = (string)_dr["CELULAR"]
                        };

                        _setor = new Setor
                        {
                            Id = (int)_dr["id_setor"],
                            Descricao = (string)_dr["descricao_setor"],
                            Obs = (string)_dr["obs_setor"]
                        };

                        _central.Setor = _setor;
                        _central.Cliente = _cliente;
                        _central.Inputs = GetInputsByCentral(_central.Id.ToString());
                    }
                }
                else
                {
                    SetCentral(_central.Mac, _central.Conta);
                }

            }
            catch
            {
                throw;
            }
            finally
            {
                if (_conexao.State == System.Data.ConnectionState.Open)
                {
                    _conexao.Close();
                }
            }

            //return _central;

        }

        public static List<Input> GetInputsByCentral(string _id)
        {
            Input _input = null;
            List<Input> _inputs = new List<Input>();

            SqlDataReader _dr = null;
            SqlConnection _conexao = null;

            string[] _campoSelect = new string[1];
            string[] _campoValor = new string[1];

            _campoSelect[0] = "@CENTRAL";
            _campoValor[0] = _id;

            try
            {
                string _select = "SELECT ID, PINO, DESCRICAO FROM INPUT WHERE CENTRAL = @CENTRAL";

                _dr = (SqlDataReader)Persistencia.ExecutarSQLReader(_select, _campoSelect, _campoValor, ref _conexao);

                if (_dr.HasRows)
                {
                    while (_dr.Read())
                    {
                        _input = new Input
                        {
                            Id = (int)_dr["ID"],
                            Pino = (int)_dr["PINO"],
                            Descricao = (string)_dr["DESCRICAO"]
                        };

                        _inputs.Add(_input);
                    }
                }
            }
            catch
            {
                throw;
            }

            return _inputs;
        }

        /// <summary>
        /// Gravar a central na tentativa de conexão caso nao haja cadastro
        /// </summary>
        /// <param name="_mac"></param>
        public static void SetCentral(string _mac, string _conta)
        {
            string[] _camposInsert = new string[5];
            string[] _valorInsert = new string[5];

            _camposInsert[0] = "@MODELO";
            _camposInsert[1] = "@CONTA";
            _camposInsert[2] = "@MAC";
            _camposInsert[3] = "@SETOR";
            _camposInsert[4] = "@CLIENTE";

            _valorInsert[0] = "";
            _valorInsert[1] = _conta;
            _valorInsert[2] = _mac;
            _valorInsert[3] = "0";
            _valorInsert[4] = "0";

            string _insert = "INSERT INTO CENTRAL(MODELO, CONTA, MAC, SETOR, CLIENTE) VALUES(@MODELO, @CONTA, @MAC, @SETOR, @CLIENTE)";

            try
            {
                Persistencia.ExecutarComando(_insert, _camposInsert, _valorInsert);
            }
            catch
            {
                throw;
            }

        }

        public static List<Central> GetCentralByCliente(string _id)
        {
            //List<Usuario> _usuarios = new List<Usuario>();
            Central _central = null;
            Setor _setor;

            List<Central> _centrais = new List<Central>();

            SqlDataReader _dr = null;
            SqlConnection _conexao = null;

            string[] _camposSelect = new string[1];
            string[] _valorSelect = new string[1];

            _camposSelect[0] = "@CLIENTE";
            _valorSelect[0] = _id;

            try
            {

                string _select = "select amt.id as 'id_central', amt.modelo, amt.conta, amt.mac, amt.endereco, amt.interface_conexao, s.id as 'id_setor', s.descricao, s.obs " +
                                 "from central amt left outer join setor s on amt.setor = s.ID " +
                                 "where amt.CLIENTE = @CLIENTE";

                _dr = (SqlDataReader)Persistencia.ExecutarSQLReader(_select, _camposSelect, _valorSelect, ref _conexao);

                if (_dr.HasRows)
                {
                    while (_dr.Read())
                    {
                        _central = new Central
                        {
                            Id = (int)_dr["ID_CENTRAL"],
                            Modelo = (string)_dr["MODELO"],
                            Conta = (string)_dr["CONTA"],
                            Mac = (string)_dr["MAC"],
                            Interface = (string)_dr["INTERFACE_CONEXAO"],
                            
                        };

                        _setor = new Setor();

                        if (_dr["id_setor"] != DBNull.Value)
                        {
                            _setor.Id = (int)_dr["id_setor"];
                        }

                        if (_dr["descricao"] != DBNull.Value)
                        {
                            _setor.Descricao = (string)_dr["descricao"];
                        }

                        if (_dr["obs"] != DBNull.Value)
                        {
                            _setor.Obs = (string)_dr["obs"];
                        }

                        _central.Setor = _setor;
                        
                        _centrais.Add(_central);
                    }
                }

            }
            catch
            {
                throw;
            }
            finally
            {
                if (_conexao.State == System.Data.ConnectionState.Open)
                {
                    _conexao.Close();
                }
            }

            return _centrais;
        }

        public static List<Central> GetCentrais()
        {
            //List<Usuario> _usuarios = new List<Usuario>();
            Central _central = null;
            Cliente _cliente = null;
            Setor _setor;

            List<Central> _centrais = new List<Central>();

            SqlDataReader _dr = null;
            SqlConnection _conexao = null;

            try
            {

                string _select = "SELECT C.ID AS 'ID_CENTRAL', C.MODELO, C.CONTA, C.MAC, C.INTERFACE_CONEXAO, U.ID AS 'ID_CLIENTE', C.ULTIMA_INTERACAO, U.NOME, U.ENDERECO, U.TELEFONE, U.CELULAR, S.ID as 'id_setor', S.DESCRICAO as 'descricao_setor', S.OBS as 'obs_setor' FROM CENTRAL C " +
                                 "LEFT OUTER JOIN CLIENTE U ON C.CLIENTE = U.ID  " +
                                 "LEFT OUTER JOIN SETOR S ON C.SETOR = S.ID ORDER BY C.CONTA";

                _dr = (SqlDataReader)Persistencia.ExecutarSQLReader(_select, ref _conexao);

                if (_dr.HasRows)
                {
                    while (_dr.Read())
                    {
                        _central = new Central
                        {
                            Id = (int)_dr["ID_CENTRAL"],
                            Modelo = (string)_dr["MODELO"],
                            Conta = (string)_dr["CONTA"],
                            Mac = (string)_dr["MAC"],
                            UltimoMovimento = (DateTime)_dr["ULTIMA_INTERACAO"],
                            Interface = (string)_dr["INTERFACE_CONEXAO"]
                        };

                        _cliente = new Cliente();

                        if (_dr["ID_CLIENTE"] != DBNull.Value)
                        {
                            _cliente.Id = (int)_dr["ID_CLIENTE"];
                        }

                        if (_dr["NOME"] != DBNull.Value)
                        {
                            _cliente.Nome = (string)_dr["NOME"];
                        }

                        if (_dr["ENDERECO"] != DBNull.Value)
                        {
                            _cliente.Endereco = (string)_dr["ENDERECO"];
                        }

                        if (_dr["TELEFONE"] != DBNull.Value)
                        {
                            _cliente.Telefone = (string)_dr["TELEFONE"];
                        }

                        if (_dr["CELULAR"] != DBNull.Value)
                        {
                            _cliente.Celular = (string)_dr["CELULAR"];
                        }

                        _setor = new Setor();

                        if (_dr["id_setor"] != DBNull.Value)
                        {
                            _setor.Id = (int)_dr["id_setor"];
                        }

                        if (_dr["descricao_setor"] != DBNull.Value)
                        {
                            _setor.Descricao = (string)_dr["descricao_setor"];
                        }

                        if (_dr["obs_setor"] != DBNull.Value)
                        {
                            _setor.Obs = (string)_dr["obs_setor"];
                        }

                        _central.Setor = _setor;
                        _central.Cliente = _cliente;

                        _centrais.Add(_central);
                    }
                }

            }
            catch
            {
                throw;
            }
            finally
            {
                if (_conexao.State == System.Data.ConnectionState.Open)
                {
                    _conexao.Close();
                }
            }

            return _centrais;
        }

        public static List<Cliente> GetClientes()
        {
            Cliente _cliente = null;
            List<Cliente> _clientes = new List<Cliente>();

            SqlDataReader _dr = null;
            SqlConnection _conexao = null;

            try
            {
                string _select = "SELECT ID, NOME, ENDERECO, TELEFONE, CELULAR FROM CLIENTE";
                _dr = (SqlDataReader)Persistencia.ExecutarSQLReader(_select, ref _conexao);

                if (_dr.HasRows)
                {
                    while (_dr.Read())
                    {
                        _cliente = new Cliente
                        {
                            Id = (int)_dr["ID"],
                            Nome = (string)_dr["NOME"],
                            Endereco = (string)_dr["ENDERECO"],
                            Telefone = (string)_dr["TELEFONE"],
                            Celular = (string)_dr["CELULAR"]
                        };

                        _clientes.Add(_cliente);
                    }
                }
            }
            catch
            {
                throw;
            }

            return _clientes;
        } 


        public static void SetCliente(Cliente _cliente)
        {
            try
            {
                string[] _camposInsert = new string[4];
                string[] _valorInsert = new string[4];

                _camposInsert[0] = "@NOME";
                _camposInsert[1] = "@ENDERECO";
                _camposInsert[2] = "@TELEFONE";
                _camposInsert[3] = "@CELULAR";

                _valorInsert[0] = _cliente.Nome;
                _valorInsert[1] = _cliente.Endereco;
                _valorInsert[2] = _cliente.Telefone;
                _valorInsert[3] = _cliente.Celular;

                string _insert = "INSERT INTO CLIENTE VALUES(@NOME, @ENDERECO, @TELEFONE, @CELULAR)";

                Persistencia.ExecutarComando(_insert, _camposInsert, _valorInsert);
            }
            catch
            {
                throw;
            }
        }

        //grava historico
        public static void SetHistorico(Historico _historico)
        {
            try
            {
                string[] _camposInsert = new string[7];
                string[] _valorInsert = new string[7];

                _camposInsert[0] = "@EVENTO";
                _camposInsert[1] = "@OBS";
                _camposInsert[2] = "@MOTIVO";
                _camposInsert[3] = "@DATA_EVENTO";
                _camposInsert[4] = "@USUARIO";
                _camposInsert[5] = "@ZONA";
                _camposInsert[6] = "@CENTRAL";

                _valorInsert[0] = _historico.Evento.Id.ToString();
                _valorInsert[1] = _historico.Obs;

                if (_historico.Motivo != null)
                {
                    _valorInsert[2] = _historico.Motivo.Id.ToString();
                }
                else
                {
                    _valorInsert[2] = DBNull.Value.ToString();
                }

                _valorInsert[3] = _historico.DataHora.ToString();

                _valorInsert[4] = _historico.Usuario.Id.ToString();

                if (_historico.Zona.Id.ToString() == "0")
                {
                    _valorInsert[5] = DBNull.Value.ToString();
                }
                else
                {
                    _valorInsert[5] = _historico.Zona.Id.ToString();
                }

                _valorInsert[6] = _historico.Central.Id.ToString();

                string _insert = "INSERT INTO HISTORICO VALUES(@EVENTO, @OBS, @MOTIVO, Convert(datetime, @DATA_EVENTO, 103), @USUARIO, @ZONA, @CENTRAL)";

                Persistencia.ExecutarComando(_insert, _camposInsert, _valorInsert);
            }
            catch
            {
                throw;
            }
        }

        public static Usuario LoginUsuario(string _nome, string _senha)
        {
            Usuario _usuario = null;

            SqlDataReader _dr = null;
            SqlConnection _conexao = null;

            string[] _campoSelect = new string[2];
            string[] _campoValor = new string[2];

            _campoSelect[0] = "@USUARIO";
            _campoSelect[1] = "@SENHA";

            _campoValor[0] = _nome;
            _campoValor[1] = _senha;

            try
            {
                string _select = "SELECT ID, NOME, NOME_LOGIN, NIVEL FROM USUARIO WHERE NOME_LOGIN = @USUARIO AND SENHA = @SENHA";
                _dr = (SqlDataReader)Persistencia.ExecutarSQLReader(_select, _campoSelect, _campoValor, ref _conexao);

                if (_dr.HasRows)
                {
                    while (_dr.Read())
                    {
                        _usuario = new Usuario
                        {
                            Id = (int)_dr["ID"],
                            Nome = (string)_dr["NOME"],
                            Login = (string)_dr["NOME_LOGIN"],
                            Nivel = (int)_dr["NIVEL"]
                        };
                    }
                }
            }
            catch
            {
                throw;
            }

            return _usuario;
        }
    }
}
