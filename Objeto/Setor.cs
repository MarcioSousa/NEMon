using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Objeto
{
    public class Setor
    {
        int _id;
        string _descricao;
        string _obs;

        public int Id { get => _id; set => _id = value; }
        public string Descricao { get => _descricao; set => _descricao = value; }
        public string Obs { get => _obs; set => _obs = value; }
    }
}
