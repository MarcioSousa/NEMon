using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Objeto;
using Banco;

namespace NEMon
{
    public partial class frmPopUp : Form
    {
        private Historico _evento;

        public frmPopUp(ref Historico _historico)
        {
            InitializeComponent();

            _evento = _historico;
            _evento.DataHora = DateTime.Now;
            
            this.lblDataHora.Text = _evento.DataHora.ToString();
            this.lblCliente.Text = "CLIENTE: " + _evento.Central.Cliente.Nome;
            this.lblEvento.Text =  "EVENTO:\n" + _evento.Evento.Codigo + " - " + _evento.Evento.Descricao ;

            this.cmbMotivo.DataSource = Negocio.GetMotivos();
            this.cmbMotivo.DisplayMember = "DESCRICAO";
        }

        private void btnRegistrar_Click(object sender, EventArgs e)
        {
            try
            {
                _evento.Obs = this.textBox1.Text;
                _evento.Motivo = (Motivo)this.cmbMotivo.SelectedItem;

                Negocio.SetHistorico(_evento);

                MessageBox.Show("Evento salvo!", "Sucesso");

                this.Close();
            }
            catch(Exception ex)
            {
                MessageBox.Show("Houve um erro: " + ex.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                this.Close();
            }
        }
    }
}
