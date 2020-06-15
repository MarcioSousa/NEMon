using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Objeto;
using System.Net;
using System.Collections;

namespace Utils
{
    public class Xml_utils
    {
        public static void XMLEscreve(Historico _historico)
        {
            try
            {
                using (XmlTextWriter _xml = new XmlTextWriter(ini.Read("EVENTOS", "GERAL") + _historico.Central.Conta + "E" + DateTime.Now.Day.ToString().PadLeft(2,'0') + DateTime.Now.Month.ToString().PadLeft(2, '0') + DateTime.Now.Year.ToString().PadLeft(2, '0') + DateTime.Now.Hour.ToString().PadLeft(2, '0') + DateTime.Now.Minute.ToString().PadLeft(2, '0') + DateTime.Now.Second.ToString().PadLeft(2, '0') + DateTime.Now.Millisecond.ToString().PadLeft(2, '0') +  ".xml", System.Text.Encoding.UTF8))
                {
                    _xml.WriteStartDocument();

                    _xml.WriteStartElement("Historcio");
                    if(_historico.Zona != null)
                    {
                        _xml.WriteElementString("ID_Zona", _historico.Zona.Id.ToString());
                        _xml.WriteElementString("Descricao_zona", _historico.Zona.Descricao);
                    }
                    else
                    {
                        _xml.WriteElementString("ID_Zona", "0");
                        _xml.WriteElementString("Descricao_zona", "---");
                    }
                    
                    _xml.WriteElementString("Data", _historico.DataHora.ToString());
                    //////////////////////////////////////////
                    _xml.WriteStartElement("Cliente");
                    _xml.WriteElementString("ID_cliente", _historico.Central.Cliente.Id.ToString());
                    _xml.WriteElementString("Nome", _historico.Central.Cliente.Nome);
                    _xml.WriteElementString("Endereco", _historico.Central.Cliente.Endereco);
                    _xml.WriteElementString("Telefone", _historico.Central.Cliente.Telefone);
                    _xml.WriteElementString("Celular", _historico.Central.Cliente.Celular);
                    _xml.WriteEndElement();
                    /////////////////////////////////////////
                    _xml.WriteStartElement("Central");
                    _xml.WriteElementString("ID_central", _historico.Central.Id.ToString());
                    _xml.WriteElementString("Modelo", _historico.Central.Modelo);
                    _xml.WriteElementString("Mac", _historico.Central.Mac);
                    _xml.WriteElementString("Conta", _historico.Central.Conta.ToString());
                    _xml.WriteEndElement();
                    //////////////////////////////////////////
                    _xml.WriteStartElement("Evento");
                    _xml.WriteElementString("ID_evento", _historico.Evento.Id.ToString());
                    _xml.WriteElementString("Codigo", _historico.Evento.Codigo);
                    _xml.WriteElementString("Descricao", _historico.Evento.Descricao);
                    _xml.WriteElementString("Prioridade", _historico.Evento.Prioridade.ToString());
                    _xml.WriteEndElement();
                    ///////////////////////////////////////////
                    _xml.WriteStartElement("Setor");
                    _xml.WriteElementString("ID_setor", _historico.Central.Setor.Id.ToString());
                    _xml.WriteElementString("nome_setor", _historico.Central.Setor.Descricao);
                    _xml.WriteElementString("Obs_setor", _historico.Central.Setor.Obs);
                    ////////////////////////////////////////////
                    _xml.WriteEndElement();
                    _xml.WriteEndDocument();
                    _xml.Close();
                }
            }
            catch
            {
                throw;
            }
        }

        public static Historico XMLLe(string _caminho)
        {
            Historico _historico = null;
            Evento _evento = null;
            Cliente _cliente = null;
            Central _central = null;
            Setor _setor;
            Input _zona;

            try
            {
                _historico = new Historico();
                _evento = new Evento();
                _cliente = new Cliente();
                _central = new Central();
                _setor = new Setor();
                _zona = new Input();
                                
                using (XmlTextReader reader = new XmlTextReader(_caminho))
                {
                    while (reader.Read())
                    {
                        if (reader.IsStartElement())
                        {
                            switch (reader.Name)
                            {
                                case "ID_Zona":
                                    _zona.Id = Convert.ToInt32(reader.ReadString());
                                    break;
                                case "Descricao_zona":
                                    _zona.Descricao = reader.ReadString();
                                    break;
                                case "Data":
                                    _historico.DataHora = Convert.ToDateTime(reader.ReadString());
                                    break;
                                case "ID_cliente":
                                    _cliente.Id = Convert.ToInt32(reader.ReadString());
                                    break;
                                case "Nome":
                                    _cliente.Nome = reader.ReadString();
                                    break;
                                case "Endereco":
                                    _cliente.Endereco = reader.ReadString();
                                    break;
                                case "Telefone":
                                    _cliente.Telefone = reader.ReadString();
                                    break;
                                case "Celular":
                                    _cliente.Celular = reader.ReadString();
                                    break;
                                case "ID_central":
                                    _central.Id = Convert.ToInt32(reader.ReadString());
                                    break;
                                case "Modelo":
                                    _central.Modelo = reader.ReadString();
                                    break;
                                case "Mac":
                                    _central.Mac = reader.ReadString();
                                    break;
                                case "Conta":
                                    _central.Conta = reader.ReadString();
                                    break;
                                case "ID_evento":
                                    _evento.Id = Convert.ToInt32(reader.ReadString());
                                    break;
                                case "Codigo":
                                    _evento.Codigo = reader.ReadString();
                                    break;
                                case "Descricao":
                                    _evento.Descricao = reader.ReadString();
                                    break;
                                case "Prioridade":
                                    _evento.Prioridade = Convert.ToInt32(reader.ReadString());
                                    break;
                                case "ID_setor":
                                    _setor.Id = Convert.ToInt32(reader.ReadString());
                                    break;
                                case "nome_setor":
                                    _setor.Descricao = reader.ReadString();
                                    break;
                                case "Obs_setor":
                                    _setor.Obs = reader.ReadString();
                                    break;
                            }
                        }
                    }
                }

                _central.Setor = _setor;
                _central.Cliente = _cliente;
                _historico.Central = _central;
                _historico.Evento = _evento;
                _historico.Zona = _zona;

            }
            catch
            {
                throw;
            }

            return _historico;
        }

        
        public static void GravarLog(string _tipo, string _info, string _app)
        {
            try
            {
                using (System.IO.StreamWriter _f = System.IO.File.AppendText(ini.Read("LOGS", "GERAL") + DateTime.Now.Day.ToString() + "-" + DateTime.Now.Month.ToString() + "-" + DateTime.Now.Year.ToString() + ".txt"))
                {
                    _f.Write(Environment.NewLine + _tipo + "| " + DateTime.Now.ToString() + " | " + _info);
                }
            }
            catch
            {
                throw;
            }
        }
    }
}
