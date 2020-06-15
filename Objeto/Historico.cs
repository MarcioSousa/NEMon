using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Objeto
{
    public class Historico : IDisposable
    {
        // Flag: Has Dispose already been called?
        bool disposed = false;

        int id;
        Evento _evento;
        Central _central;
        string _obs;
        //string _zona;
        Input _zona;
        Motivo _motivo;
        DateTime _dataHora;
        Usuario _usuario;

        public int Id { get => id; set => id = value; }
        public Evento Evento { get => _evento; set => _evento = value; }
        public Central Central { get => _central; set => _central = value; }
        public string Obs { get => _obs; set => _obs = value; }
        public Input Zona { get => _zona; set => _zona = value; }
        public Motivo Motivo { get => _motivo; set => _motivo = value; }
        public DateTime DataHora { get => _dataHora; set => _dataHora = value; }
        public Usuario Usuario { get => _usuario; set => _usuario = value; }

        public void Dispose()
        {
            // Dispose of unmanaged resources.
            Dispose(true);
            // Suppress finalization.
            GC.SuppressFinalize(this);
        }

        // Protected implementation of Dispose pattern.
        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            disposed = true;
        }
    }
}
