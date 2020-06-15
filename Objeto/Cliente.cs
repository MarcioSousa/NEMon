using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Objeto
{
    public class Cliente
    {
        int _id;
        string _nome;
        string _endereco;
        string _telefone;
        string _celular;
        List<Central> _centrais;

        public int Id { get => _id; set => _id = value; }
        public string Nome { get => _nome; set => _nome = value; }
        public string Endereco { get => _endereco; set => _endereco = value; }
        public string Telefone { get => _telefone; set => _telefone = value; }
        public string Celular { get => _celular; set => _celular = value; }
        public List<Central> Centrais { get => _centrais; set => _centrais = value; }
    }
}
