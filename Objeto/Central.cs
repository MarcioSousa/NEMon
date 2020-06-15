using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace Objeto
{
    public class Central : Conexao
    {
        int _id;
        string _conta;
        string _mac;
        string _modelo;
        string _interface;
        Cliente _cliente;
        Setor _setor;
        List<Input> _inputs;
        
        public string Conta { get => _conta; set => _conta = value; }
        public string Mac { get => _mac; set => _mac = value; }
        public string Modelo { get => _modelo; set => _modelo = value; }
        public int Id { get => _id; set => _id = value; }
        public Cliente Cliente { get => _cliente; set => _cliente = value; }
        public string Interface { get => _interface; set => _interface = value; }
        public Setor Setor { get => _setor; set => _setor = value; }
        public List<Input> Inputs { get => _inputs; set => _inputs = value; }
    }
}
