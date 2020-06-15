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
using Utils;

namespace AlarmManager_Client
{
    public partial class frmAlerta : Form
    {
        private Historico _evento;
        
        System.Media.SoundPlayer player = null;
        bool _salvo = false;

        public frmAlerta(ref Historico _historico)
        {
            InitializeComponent();

            _evento = _historico;
         
            this.Text = _evento.Evento.Descricao;

            this.lblDataHora.Text = _evento.DataHora.ToString();
            this.lblCliente.Text = "CLIENTE: " + _evento.Central.Cliente.Nome;
            this.lblEvento.Text = "EVENTO: " + _evento.Evento.Codigo + " - " + _evento.Evento.Descricao + "\nZONA: " + _evento.Zona.Descricao + " - " + _evento.Central.Setor.Descricao + "\n--------------------------------";
            this.lblObs.Text = _evento.Central.Setor.Obs;
            
            this.cmbMotivo.DataSource = Negocio.GetMotivos();
            this.cmbMotivo.DisplayMember = "DESCRICAO";

            player = new System.Media.SoundPlayer();
            
            player.SoundLocation = ini.Read("SOM", "GERAL");

            player.PlayLooping();
        }

        
        private void btnRegistrar_Click(object sender, EventArgs e)
        {
            try
            {
                if(this.textBox1.Text.Length <= 4)
                {
                    throw new Exception("Digite algo nas observações acima de 04 caracteres");
                }

                _evento.Obs = this.textBox1.Text;
                _evento.Motivo = (Motivo)this.cmbMotivo.SelectedItem;

                Negocio.SetHistorico(_evento);

                _salvo = true;

                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Houve um erro: " + ex.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void Form2_FormClosing(object sender, FormClosingEventArgs e)
        {
            if(!this._salvo)
            {
                e.Cancel = true;
            }
            else
            {
                player.Stop();
            }
        }

        private void textBox1_Leave(object sender, EventArgs e)
        {
            var _txt = (TextBox)sender;

            _txt.Text = _txt.Text.ToUpper();
        }
    }
}
