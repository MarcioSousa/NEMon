using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Objeto
{
    public class Usuario
    {
        int _id;
        string _nome;
        string _login;
        string _senha;
        int _nivel;

        public int Id { get => _id; set => _id = value; }
        public string Nome { get => _nome; set => _nome = value; }
        public string Login { get => _login; set => _login = value; }
        public int Nivel { get => _nivel; set => _nivel = value; }
        public string Senha { get => _senha; set => _senha = value; }
    }
}
