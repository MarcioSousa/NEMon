using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Objeto
{
    public class Evento
    {
        int _id;
        string _codigo;
        string _descricao;
        int _prioridade;
        string _zona;
        int _exibeZona;
        
        public int Id { get => _id; set => _id = value; }
        public string Codigo { get => _codigo; set => _codigo = value; }
        public string Descricao { get => _descricao; set => _descricao = value; }
        public int Prioridade { get => _prioridade; set => _prioridade = value; }
        public string Zona { get => _zona; set => _zona = value; }
        public int ExibeZona { get => _exibeZona; set => _exibeZona = value; }
    }
}
