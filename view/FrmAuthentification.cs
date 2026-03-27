using System;
using System.Windows.Forms;
using MediaTekDocuments.model;
using MediaTekDocuments.controller;

namespace MediaTekDocuments.view
{
    public partial class FrmAuthentification : Form
    {
        private FrmMediatekController controller;

        public FrmAuthentification()
        {
            InitializeComponent();
            this.controller = new FrmMediatekController();
        }

        /// <summary>
        /// Événement du bouton Se Connecter
        /// </summary>
        private void btnSeConnecter_Click(object sender, EventArgs e)
        {
            string login = txtLogin.Text;
            string pwd = txtPassword.Text;

            Utilisateur utilisateur = controller.Authentification(login, pwd);

            if (utilisateur != null)
            {
                FrmMediatek frmMediatek = new FrmMediatek(utilisateur);
                
                frmMediatek.FormClosed += (s, args) => Application.Exit(); 
                
                frmMediatek.Show();
                
                this.Hide(); 
            }
        }
    }
}