using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Objeto
{
    public class Input
    {
        int _id;
        int _pino;
        string _descricao;

        public int Id { get => _id; set => _id = value; }
        public int Pino { get => _pino; set => _pino = value; }
        public string Descricao { get => _descricao; set => _descricao = value; }
    }
}
