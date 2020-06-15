using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using Objeto;
using Banco;

namespace NEMon
{
    public partial class Form1 : Form
    {
        byte[] _keep;

        BackgroundWorker _server;
        BackgroundWorker _processaStream;
                                   
        bool _continua;

        public Form1()
        {
            InitializeComponent();

            //ack
            _keep = new byte[1];
            _keep[0] = 0xFE;

            _continua = true;

            _server = new BackgroundWorker();
            _server.DoWork += _server_DoWork;
            _server.RunWorkerCompleted += _server_RunWorkerCompleted;

        }
        
        private void _processaStream_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //grava log de erro
            if(e.Error != null)
            {
                Utils.Xml_utils.GravarLog("Erro", e.Error.Message, "");
            }
        }
                
        //finalizado a trhead principal
        private void _server_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //grava log de erro
            if (e.Error != null)
            {
                Utils.Xml_utils.GravarLog("Erro", e.Error.Message, "");
            }
        }

        //começa a ouvir na porta e receber os streams
        private void _server_DoWork(object sender, DoWorkEventArgs e)
        {
            int porta = 9009;
            IPAddress _ip = IPAddress.Parse("192.168.0.5");

            TcpListener _tcp = new TcpListener(_ip, porta);
            _tcp.Start();

            while (_continua)
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
            }
        }

        
        private void _processaStream_DoWork(object sender, DoWorkEventArgs e)
        {
            TcpClient _central = (TcpClient)e.Argument;
            var _objeto = (BackgroundWorker)sender;

            while (_central.Connected)
            {
                try
                {
                    NetworkStream _ns = _central.GetStream();

                    byte[] _recebe = new byte[_central.ReceiveBufferSize];
                    _ns.Read(_recebe, 0, _central.ReceiveBufferSize);

                    //verifica se a conexão é válida e manda um ACK
                    if (AnalisaStream(ContabilizaArray(_recebe), _central))
                    {
                        //envia o pacote ACK para a central
                        _ns.Write(_keep, 0, _keep.Length);
                    }
                }
                catch
                {
                    throw;
                }
                finally
                {
                    _objeto.DoWork -= _processaStream_DoWork;
                    _objeto.Dispose();
                }
            }
        }

        private bool AnalisaStream(byte[] _stream, TcpClient _client)
        {
            bool _valido = false;

            try
            {
                //pacote invalido
                if (_stream.Length == 0)
                {
                    return true;
                }
                //ack
                else if (_stream[0] == 0xF7)
                {
                    return true;
                }
                //conexao
                else if (_stream[1] == 0x94 && _stream[_stream.Length - 1] == Calculate(_stream))
                {
                    var _central = CentralFactory(_stream, _client);

                    Utils.Xml_utils.XMLConexao(_central);
                }
                //eventos
                else if (_stream[1] == 0xB0 && _stream[_stream.Length - 1] == Calculate(_stream))
                {
                    Historico _historico = EventoFactory(_stream);

                    Utils.Xml_utils.XMLEvento(_historico);

                    _valido = true;
                }
                //data e hora no evento
                else if (_stream[1] == 0xB4 && _stream[_stream.Length - 1] == Calculate(_stream))
                {
                    _valido = true;
                }
            }
            catch
            {
                throw;
            }
            

            return _valido;
        }

                
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

        private void Form1_Load(object sender, EventArgs e)
        {
            _server.RunWorkerAsync();
        }

        private byte Calculate(byte[] data)
        {
            byte xor = 0;

            //obtem o penultimo byte pra calcular o checksum
            for(int i = 0; i <= data.Length - 2; i++)
            {
                xor ^= data[i];
            }

            xor ^= 0xFF;

            return xor;
        }

        private Historico EventoFactory(byte[] _stream)
        {
            Central _central;
            Evento _evento;
            Historico _historico;

            string _queryConta;
            string _queryEvento;
            string _zona;

            try
            {
                //conta
                _queryConta = _stream[3].ToString("X").Replace("A", "0");
                _queryConta += _stream[4].ToString("X").Replace("A", "0");
                _queryConta += _stream[5].ToString("X").Replace("A", "0");
                _queryConta += _stream[6].ToString("X").Replace("A", "0");

                _central = Negocio.GetCentralByConta(_queryConta);

                //evento
                _queryEvento = _stream[9].ToString("X").Replace("A", "0");
                _queryEvento += _stream[10].ToString("X").Replace("A", "0");
                _queryEvento += _stream[11].ToString("X").Replace("A", "0");
                _queryEvento += _stream[12].ToString("X").Replace("A", "0");

                _evento = Negocio.GetEventoByCodigo(_queryEvento);

                //zona
                _zona = _stream[17].ToString("X").Replace("A", "0");

                _historico = new Historico
                {
                    Evento = _evento,
                    Central = _central,
                    Zona = _zona,
                    DataHora = DateTime.Now
                };
            }
            catch
            {
                throw;
            }

          
            return _historico;
        }

        private Central CentralFactory(byte[] _stream, TcpClient _clientTcp)
        {
            string _queryConta;
            
            Central _central = null;
            
            //conta
            _queryConta = _stream[3].ToString("X").Replace("A", "0").PadLeft(2, '0');
            _queryConta += _stream[4].ToString("X").Replace("A", "0");

            try
            {
                _central = Negocio.GetCentralByConta(_queryConta);

                if (_central != null)
                {
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
                        _central.Interface = "GPRS 1";
                    }

                    _central.ClientIP = _clientTcp;
                }
            }
            catch
            {
                throw;
            }
                                     
            return _central;
        }

        private void dataGridView1_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            var _dgv = (DataGridView)sender;

            string _valor = _dgv.Rows[e.RowIndex].Cells[5].Value.ToString();

            if (_valor == "1")
            {
                _dgv.Rows[e.RowIndex].DefaultCellStyle.ForeColor = Color.Red;
            }
            else
            {
                _dgv.Rows[e.RowIndex].DefaultCellStyle.ForeColor = Color.Blue;
            }
        
    }
}
