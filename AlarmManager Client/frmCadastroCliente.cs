using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Banco;
using Objeto;
using System.Windows.Forms;


namespace AlarmManager_Client
{
    public partial class frmCadastroCliente : Form
    {
        bool _novo;

        public frmCadastroCliente()
        {
            InitializeComponent();
        }

        private void PreencheGridClientes()
        {
            try
            {
                var _clientes = Negocio.GetClientes();

                foreach(var _cli in _clientes)
                {
                    dataGridView1.Rows.Add(_cli.Id, _cli.Nome, _cli.Endereco, _cli.Telefone, _cli.Celular);
                }
            }
            catch
            {
                throw;
            }
        }

        private void PreencheGridCentrais(string _id)
        {
            try
            {
                this.dataGridView2.Rows.Clear();

                var _centrais = Negocio.GetCentralByCliente(_id);

                foreach(var _central in _centrais)
                {
                    dataGridView2.Rows.Add(_central.Id, _central.Modelo, _central.Conta, _central.Mac, _central.Setor.Descricao, _central.Interface);
                }
            }
            catch
            {
                throw;
            }
        }

        private void btnGravar_Click(object sender, EventArgs e)
        {
            var _cliente = new Cliente
            {
                Nome = txtNome.Text,
                Endereco = txtEndereco.Text,
                Telefone = txtTelefone.Text,
                Celular = txtCelular.Text
            };

            try
            {
                if(_novo)
                {
                    Negocio.SetCliente(_cliente);
                }
                
                MessageBox.Show("Cliente cadastrado com sucesso");
            }
            catch(Exception ex)
            {
                frmException _ex = new frmException(ex);
                _ex.ShowDialog();
            }
            
        }

        private void btnSair_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnNovo_Click(object sender, EventArgs e)
        {
            this._novo = true;

            this.txtID.Text = "";
            this.txtNome.Text = "";
            this.txtEndereco.Text = "";
            this.txtTelefone.Text = "";
            this.txtCelular.Text = "";
        }

        private void frmCadastroCliente_Load(object sender, EventArgs e)
        {
            try
            {
                PreencheGridClientes();
            }
            catch(Exception ex)
            {
                frmException _ex = new frmException(ex);
                _ex.ShowDialog();
            }
        }

        private void dataGridView1_CellDoubleClick_1(object sender, DataGridViewCellEventArgs e)
        {
            var _dgv = (DataGridView)sender;

            try
            {
                if (_dgv.RowCount > 0)
                {
                    _novo = false;

                    this.txtID.Text = _dgv[0, _dgv.CurrentRow.Index].Value.ToString();
                    this.txtNome.Text = _dgv[1, _dgv.CurrentRow.Index].Value.ToString();
                    this.txtEndereco.Text = _dgv[2, _dgv.CurrentRow.Index].Value.ToString();
                    this.txtTelefone.Text = _dgv[3, _dgv.CurrentRow.Index].Value.ToString();
                    this.txtCelular.Text = _dgv[4, _dgv.CurrentRow.Index].Value.ToString();

                    PreencheGridCentrais(this.txtID.Text);
                }
            }
            catch(Exception ex)
            {
                frmException _ex = new frmException(ex);
                _ex.ShowDialog();
            }
        }

        private void txtNome_Leave(object sender, EventArgs e)
        {
            var _txt = (TextBox)sender;
            _txt.Text = _txt.Text.ToUpper();
        }
    }
}
