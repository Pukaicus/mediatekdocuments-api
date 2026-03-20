using MediaTekDocuments.view;
using System;
using System.Windows.Forms;

namespace MediaTekDocuments
{
    static class Program
    {
        /// <summary>
        /// Point d'entrée principal de l'application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            try
            {
                Application.Run(new FrmAuthentification());
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur au démarrage de l'application :\n\n" + 
                                "Message : " + ex.Message + "\n\n" +
                                "Détails : " + ex.InnerException?.Message, 
                                "Crash de l'application", 
                                MessageBoxButtons.OK, 
                                MessageBoxIcon.Error);
            }
        }
    }
}