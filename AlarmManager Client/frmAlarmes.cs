using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using Utils;
using Objeto;
using Banco;

namespace AlarmManager_Client
{
    public partial class Form1 : Form
    {
        FileSystemWatcher _watcher;

        private delegate void SafeListView(Color _cor);

        BackgroundWorker _bgwPreencheLista;
        BackgroundWorker _bgwCarregaXMLS;

        Usuario _usuario;

        Form _login;

        bool _logoff;

        public Form1(Usuario usuario, Form _frmLogin)
        {
            InitializeComponent();

            _bgwPreencheLista = new BackgroundWorker();
            _bgwPreencheLista.DoWork += _bgwPreencheLista_DoWork;
            _bgwPreencheLista.WorkerSupportsCancellation = true;

            _bgwCarregaXMLS = new BackgroundWorker();
            _bgwCarregaXMLS.DoWork += _bgwCarregaXMLS_DoWork;
            _bgwCarregaXMLS.WorkerSupportsCancellation = true;
            _bgwCarregaXMLS.RunWorkerCompleted += _bgwCarregaXMLS_RunWorkerCompleted;

            CheckForIllegalCrossThreadCalls = false;

            _usuario = usuario;

            this.toolStripStatusLabel1.Text = "USUARIO: " + _usuario.Nome + " | DATA/HORA LOGIN: " + DateTime.Now.ToString() + " | NIVEL: " + ((_usuario.Nivel == 1)?"ADMINISTRADOR":"USUARIO");

            this._login = _frmLogin;

        }

        private void _bgwCarregaXMLS_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if(e.Error != null)
            {
                MessageBox.Show("Erro ao carregar XML de disparo: " + e.Error.Message);
            }
            else
            {
                _watcher = new FileSystemWatcher
                {
                    Path = ini.Read("EVENTOS", "GERAL"),
                    NotifyFilter = NotifyFilters.FileName,
                    Filter = ("*.xml"),

                    EnableRaisingEvents = true
                };

                _watcher.Created += _watcher_Created;
            }
        }

        private void _bgwCarregaXMLS_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                foreach (var _arquivo in System.IO.Directory.GetFiles(ini.Read("EVENTOS", "GERAL"), "*.xml"))
                {
                    var _historico = Xml_utils.XMLLe(_arquivo);
                    _historico.Usuario = _usuario;

                    TrataAlarme(_historico);

                    System.IO.File.Delete(_arquivo);
                }

            }
            catch
            {
                throw;
            }
        }

        private void _bgwPreencheLista_DoWork(object sender, DoWorkEventArgs e)
        {
            while(!((BackgroundWorker)sender).CancellationPending)
            {
                var _clientes = Negocio.GetCentrais();
                this.listView1.Items.Clear();

                for(int i = 0; i <= _clientes.Count -1; i++)
                {
                    this.listView1.Items.Add(new ListViewItem(new string[] { ((_clientes[i].Conta) != "")?_clientes[i].Conta:"----" , ((_clientes[i].Setor.Descricao) != null)?_clientes[i].Setor.Descricao:"----", _clientes[i].Interface, _clientes[i].Mac}));

                    if((DateTime.Now - _clientes[i].UltimoMovimento).TotalMinutes > Convert.ToInt32(ini.Read("KEEPALIVE", "GERAL")))
                    {
                        this.listView1.Items[i].ForeColor = Color.Red;
                    }
                    else
                    {
                        this.listView1.Items[i].ForeColor = Color.Green;
                    }
                }

                System.Threading.Thread.Sleep(60000);
            }
        }

        private void _watcher_Created(object sender, FileSystemEventArgs e)
        {
            Historico _historico = null;

            System.Threading.Thread.Sleep(300);

            try
            {
                _historico = Xml_utils.XMLLe(e.FullPath);
                _historico.Usuario = _usuario;

                TrataAlarme(_historico);

                File.Delete(e.FullPath);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void TrataAlarme(Historico _historico)
        {
            try
            {
                if (_historico.Evento.Prioridade == 1)
                {
                    var _frmPop = new frmAlerta(ref _historico);
                    this.Invoke(new Action(() => _frmPop.ShowDialog()));
                }
                //evento irrelevante
                else
                {
                    _historico.Obs = "Salvo automaticamente";
                    Negocio.SetHistorico(_historico);
                }

                //limpa grid
                if (dataGridView1.RowCount == 20)
                {
                    dataGridView1.Rows.Clear();
                }

                this.dataGridView1.Rows.Add(_historico.Central.Setor.Descricao + " - " + _historico.Central.Conta.ToString(), _historico.Central.Setor.Obs, _historico.Evento.Codigo + " - " + _historico.Evento.Descricao, _historico.Zona.Descricao, _historico.DataHora, _historico.Evento.Prioridade, _historico.Obs);
            }
            catch
            {
                throw;
            }
        }

        private void dataGridView1_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            var _dgv = (DataGridView)sender;

            try
            {
                string _valor = _dgv.Rows[e.RowIndex].Cells[5].Value.ToString();

                if (_valor == "1")
                {
                    _dgv.Rows[e.RowIndex].DefaultCellStyle.ForeColor = Color.Red;
                }
                else
                {
                    _dgv.Rows[e.RowIndex].DefaultCellStyle.ForeColor = Color.Blue;
                }

                dataGridView1.Sort(this.dataGridView1.Columns[4], ListSortDirection.Descending);
                
            }
            catch
            {
                throw;
            }
        }

        private void sairToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        private void logoffToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _logoff = true;

            _login.Visible = true;
            this.Close();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            _bgwPreencheLista.RunWorkerAsync();
            _bgwCarregaXMLS.RunWorkerAsync();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if(!this._logoff)
            {
                Environment.Exit(0);
            }
        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            var _dgv = ((DataGridView)sender);
            this.txtInfo.Text = "";

            ///Verifica a prioridade
            if(_dgv[5, _dgv.CurrentRow.Index].Value.ToString() == "1")
            {
                this.txtInfo.ForeColor = Color.Red;
            }
            else
            {
                this.txtInfo.ForeColor = Color.Blue;
            }

            ///Setor e data/hora
            this.txtInfo.Text = _dgv[0, _dgv.CurrentRow.Index].Value.ToString() + (char)32 + _dgv[4, _dgv.CurrentRow.Index].Value.ToString() +  Environment.NewLine;
            this.txtInfo.Text += "---------------------" + Environment.NewLine;

            ///Evento
            this.txtInfo.Text += _dgv[2, _dgv.CurrentRow.Index].Value.ToString() + (char)32 + "ZONA: " + _dgv[3, _dgv.CurrentRow.Index].Value.ToString() + Environment.NewLine;

            this.txtInfo.Text += _dgv[6, _dgv.CurrentRow.Index].Value.ToString();

        }

        private void clientesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmCadastroCliente _cadCli = new frmCadastroCliente();
            _cadCli.Show();
        }
    }
}
