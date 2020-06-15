using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace Objeto
{
    public abstract class Conexao
    {
        TcpClient _clientIP;
        DateTime _ultimoMovimento;

        public TcpClient ClientIP { get => _clientIP; set => _clientIP = value; }
        public DateTime UltimoMovimento { get => _ultimoMovimento; set => _ultimoMovimento = value; }
    }
}
