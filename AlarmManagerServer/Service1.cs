using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using Objeto;
using Banco;
using Utils;
using System.Net.Sockets;

namespace AlarmManagerServer
{
    public partial class Service1 : ServiceBase
    {
        byte[] _keep;

        BackgroundWorker _server;
        BackgroundWorker _processaStream;
        BackgroundWorker _mataConexaoPresa;
        List<Conexao> _centrais;

        public Service1()
        {
            InitializeComponent();

            //ack
            _keep = new byte[1];
            _keep[0] = 0xFE;

            //colecao de centrais conctada
            _centrais = new List<Conexao>();

            _server = new BackgroundWorker();
            _server.DoWork += _server_DoWork;
            _server.RunWorkerCompleted += _server_RunWorkerCompleted;
            _server.WorkerSupportsCancellation = true;

            _mataConexaoPresa = new BackgroundWorker();
            _mataConexaoPresa.DoWork += _mataConexaoPresa_DoWork;
            _mataConexaoPresa.RunWorkerCompleted += _mataConexaoPresa_RunWorkerCompleted;
            _mataConexaoPresa.WorkerSupportsCancellation = true;
        }

        
        /// <summary>
        /// Elimina as conexões que nao responderam ao keepAlive em tempo habil
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _mataConexaoPresa_DoWork(object sender, DoWorkEventArgs e)
        {
            while(!((BackgroundWorker)sender).CancellationPending)
            {
                try
                {
                    for (int i = _centrais.Count - 1; i >= 0; i--)
                    {
                        ///verifica se a central está ha tempo X em minutos sem interagir com o servidor
                        ///caso sim, a conexao é finalizada.
                        ///o objeto é excluso da lista de conexoes
                        if ((DateTime.Now - _centrais[i].UltimoMovimento).TotalMinutes >= Convert.ToInt32(ini.Read("KEEPALIVE", "GERAL")))
                        {
                            _centrais[i].ClientIP.Client.Close();
                            _centrais.Remove(_centrais[i]);
                        }
                    }
                }
                catch
                {
                    throw;
                }
                

                System.Threading.Thread.Sleep(1000);
            }
        }

        private void _mataConexaoPresa_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //grava log de erro
            if (e.Error != null)
            {
                try
                {
                    Utils.Xml_utils.GravarLog("Erro", e.Error.Message, "");
                }
                catch
                {

                }
            }
        }

        private void _processaStream_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //grava log de erro
            if (e.Error != null)
            {
                try
                {
                    Utils.Xml_utils.GravarLog("Erro", e.Error.Message, "");
                }
                catch
                {

                }
            }
        }

        //finalizado a trhead principal
        private void _server_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //grava log de erro
            if (e.Error != null)
            {
                try
                {
                    Utils.Xml_utils.GravarLog("Erro", e.Error.Message, "");
                }
                catch
                {

                }
            }
            
        }

        protected override void OnStart(string[] args)
        {
            _server.RunWorkerAsync();
            _mataConexaoPresa.RunWorkerAsync();
        }

        protected override void OnStop()
        {
            _mataConexaoPresa.CancelAsync();
            _server.CancelAsync();
        }
        
        /// <summary>
        /// Inicia o servidor
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _server_DoWork(object sender, DoWorkEventArgs e)
        {
            int porta = Convert.ToInt32(ini.Read("PORTA", "GERAL"));
            IPAddress _ip = IPAddress.Parse(ini.Read("SERVIDOR", "GERAL"));

            TcpListener _tcp = new TcpListener(_ip, porta);
            _tcp.Start();
            
            var _bgw = (BackgroundWorker)sender;

            while (!_bgw.CancellationPending)
            {
                try
                {
                    TcpClient _cli = _tcp.AcceptTcpClient();

                    _processaStream = new BackgroundWorker();

                    _processaStream.DoWork += _processaStream_DoWork;
                    _processaStream.RunWorkerCompleted += _processaStream_RunWorkerCompleted;

                    _processaStream.RunWorkerAsync(_cli);
                }
                catch
                {
                    throw;
                }
                finally
                {
                    //Utils.Xml_utils.GravarLog("Info", "Sistema finalizado as " + DateTime.Now.ToString(), "");
                    e.Cancel = true;
                }
            }
        }

        /// <summary>
        /// Thread responsavel por receber e tratar as informações recebidas pela central
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _processaStream_DoWork(object sender, DoWorkEventArgs e)
        {
            TcpClient _client = (TcpClient)e.Argument;
            NetworkStream _ns = null;
            var _objeto = (BackgroundWorker)sender;

            byte[] _pacoteTratado;

            ////////////////////////////////////////////////////////////////////////
            ///Independente da conexão em que o servidor receba, sempre será tratado o tempo de keepAlive do host,
            ///isso previne possiveis DDoS adivindos da internet.
            Central _central = new Central();
            _central.ClientIP = _client;
            _central.UltimoMovimento = DateTime.Now;

            _centrais.Add(_central);
            ////////////////////////////////////////////////////////////////////////

            while (_client.Connected)
            {
                try
                {
                    ////////////////////////////////////////////////////////////////
                    ///Recebe o stream do Client
                    _ns = _client.GetStream();
                    ///////////////////////////////////////////////////////////////

                    ///////////////////////////////////////////////////////////////
                    ///Le o stream
                    byte[] _recebe = new byte[_client.ReceiveBufferSize];
                    _ns.Read(_recebe, 0, _client.ReceiveBufferSize);
                    ///////////////////////////////////////////////////////////////
                    
                    ///////////////////////////////////////////////////////////////
                    ///Faz o tratamento do pacote recebido
                    _pacoteTratado = ContabilizaArray(_recebe);
                    ///////////////////////////////////////////////////////////////
                    
                    ///Verifica se o pacote é válido
                    if (_pacoteTratado.Length > 0)
                    {
                        ///Verifica se é keepalive
                        if (_pacoteTratado[0] == 0xF7)
                        {
                            Utils.Xml_utils.GravarLog("Info", "KEEPALIVE " + ((System.Net.IPEndPoint)_central.ClientIP.Client.RemoteEndPoint).Address.ToString() + " " + _central.Conta, "");

                            ////////////////////////////////////////////////////////////////
                            ///primeiramente verifica se existe um cliente cadastrado com essa central
                            ///caso nao houver, o tempo de ultimo keepalive irá se esgotar e a central
                            ///será desconectada
                            if(_central.Cliente != null)
                            {
                                ///////////////////////////////////////////////////////////
                                _central.UltimoMovimento = DateTime.Now;
                                ///////////////////////////////////////////////////////////
                                
                                ///////////////////////////////////////////////////////////
                                ///update no banco
                                Negocio.UpdateIPCentral(_central);
                                ///////////////////////////////////////////////////////////
                                
                                ///////////////////////////////////////////////////////////
                                ///envia um ACK
                                _ns.Write(_keep, 0, _keep.Length);
                                ///////////////////////////////////////////////////////////
                            }
                        }
                        ///Verifica se é conexao
                        else if (_pacoteTratado[1] == 0x94 && _pacoteTratado[_pacoteTratado.Length - 1] == Calculate(_pacoteTratado))
                        {
                            ///////////////////////////////////////////////////////////////
                            ///Caso seja uma conexao real de dispositivos utilizando o protocolo ContactID
                            CentralFactory(ref _central, _pacoteTratado);
                            ///////////////////////////////////////////////////////////////

                            ///////////////////////////////////////////////////////////////
                            ///atualiza o banco de dados com o novo IP e o tempo de interação atualizado
                            Negocio.UpdateIPCentral(_central);
                            ///////////////////////////////////////////////////////////////

                            ///////////////////////////////////////////////////////////////
                            ///envia o pacote ACK para a central
                            _ns.Write(_keep, 0, _keep.Length);
                            ///////////////////////////////////////////////////////////////
                        }
                        ///Verifica se é evento
                        else if (_pacoteTratado[1] == 0xB0 && _pacoteTratado[_pacoteTratado.Length - 1] == Calculate(_pacoteTratado))
                        {
                            ////////////////////////////////////////////////////////////////////
                            ///cria um objeto do tipo evento e gera um XML com as informações do disparo
                            ///primeiramente verifica-se se a central é cadastrada com um cliente válido
                            ///aí sim o evento é criado, caso contrario a central nao recebe o ACK e o XML nao é criado
                            Historico _historico = EventoFactory(ref _central, _pacoteTratado);
                            if (_historico.Central.Cliente != null)
                            {
                                Utils.Xml_utils.XMLEscreve(_historico);

                                /// envia o pacote ACK para a central
                                _ns.Write(_keep, 0, _keep.Length);
                            }
                            ////////////////////////////////////////////////////////////////////                            
                        }
                        ///Evento com data e hora
                        else if (_pacoteTratado[1] == 0xB4 && _pacoteTratado[_pacoteTratado.Length - 1] == Calculate(_pacoteTratado))
                        {
                            ///////////////////////////////////////////////////////////////
                            ///envia o pacote ACK para a central
                            _ns.Write(_keep, 0, _keep.Length);
                            ///////////////////////////////////////////////////////////////
                        }
                        ///Evento com indice
                        else if (_pacoteTratado[1] == 0xB5 && _pacoteTratado[_pacoteTratado.Length - 1] == Calculate(_pacoteTratado))
                        {
                            ///////////////////////////////////////////////////////////////
                            ///envia o pacote ACK para a central
                            _ns.Write(_keep, 0, _keep.Length);
                            ///////////////////////////////////////////////////////////////
                        }
                        ///Solicitacao de data e hora
                        else if (_pacoteTratado[1] == 0x80 && _pacoteTratado[_pacoteTratado.Length - 1] == Calculate(_pacoteTratado))
                        {
                            ///////////////////////////////////////////////////////////////
                            ///envia o pacote ACK para a central
                            _ns.Write(_keep, 0, _keep.Length);
                            ///////////////////////////////////////////////////////////////
                        }
                    }


                }
                catch
                {
                    throw;
                }
                
                System.Threading.Thread.Sleep(100);
            }

            _objeto.DoWork -= _processaStream_DoWork;
            _objeto.Dispose();
        }

        /// <summary>
        /// Trata o pacote e remove os bytes nulos
        /// </summary>
        /// <param name="_stream"></param>
        /// <returns></returns>
        static private byte[] ContabilizaArray(byte[] _stream)
        {
            int i = _stream.Length - 1;
            byte[] bar;

            try
            {
                while (_stream[i] == 0)
                {
                    --i;

                    if (i < 0)
                    {
                        //pacote inválido
                        bar = new byte[0];
                        return bar;
                    }
                }

                bar = new byte[i + 1];
                Array.Copy(_stream, bar, i + 1);
            }
            catch
            {
                throw;
            }


            return bar;
        }

        /// <summary>
        /// Calcula o checksum do pacote recebido pela central.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private byte Calculate(byte[] data)
        {
            byte xor = 0;

            //obtem o penultimo byte pra calcular o checksum
            for (int i = 0; i <= data.Length - 2; i++)
            {
                xor ^= data[i];
            }

            xor ^= 0xFF;

            return xor;
        }


        private Historico EventoFactory(ref Central _central, byte[] _stream)
        {       
            Evento _evento;
            Historico _historico;
            Input _input = null;

            string _queryEvento;
            int _zona;

            try
            {
                ///Fazendo busca pelo MAC da centrsl
                //Negocio.GetCentralByMAC(ref _central);

                //evento
                _queryEvento = _stream[9].ToString("X").Replace("A", "0");
                _queryEvento += _stream[10].ToString("X").Replace("A", "0");
                _queryEvento += _stream[11].ToString("X").Replace("A", "0");
                _queryEvento += _stream[12].ToString("X").Replace("A", "0");

                _evento = Negocio.GetEventoByCodigo(_queryEvento);

                //zona
                _zona = Convert.ToInt32(_stream[17].ToString("X").Replace("A", "0"));

                if(_evento.ExibeZona == 1)
                {
                    _input = _central.Inputs.Where(z => z.Pino == _zona).FirstOrDefault();
                }
                
                _historico = new Historico
                {
                    Evento = _evento,
                    Central = _central,
                    Zona = _input,
                    DataHora = DateTime.Now
                };
            }
            catch
            {
                throw;
            }


            return _historico;
        }

        private void CentralFactory(ref Central _central, byte[] _stream)
        {
            
            string _queryMAC;
            string _conta;

            _conta = _stream[3].ToString("X");
            _conta += _stream[4].ToString("X");

            _queryMAC = _stream[5].ToString("X").PadLeft(2,'0');
            _queryMAC += _stream[6].ToString("X").PadLeft(2, '0');
            _queryMAC += _stream[7].ToString("X").PadLeft(2, '0');

            _central.Mac = _queryMAC;
            _central.Conta = _conta;

            try
            {
                
                Negocio.GetCentralByMAC(ref _central);

                if (_stream[2] == 0x45)
                {
                    _central.Interface = "ETHERNET";
                }
                else if (_stream[2] == 0x47)
                {
                    _central.Interface = "GPRS 1";
                }
                else
                {
                    _central.Interface = "GPRS 2";
                }
            }
            catch
            {
                throw;
            }

            //return _central;
        }
    }
}
