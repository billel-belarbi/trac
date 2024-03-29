﻿using System;
using System.IO;
using System.Linq;

using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

using System.Windows;

using TNT.check_in_out;
using TNT.login;
using TNT.syncro;

namespace TNT
{
    public partial class  Menu : Form
    {
        public String id_util;
        public exp_dest expedition;
        public reception.recpt recepte;

        public Menu()
        {
            InitializeComponent();
            

            
        }

        private void Enleve_Click(object sender, EventArgs e)
        {
            expedition = new exp_dest();
            expedition.expediteur();
            expedition.Show();
        }

        private void check_Click(object sender, EventArgs e)
        {
            //menu_in_out in_out = new menu_in_out();
            //in_out.Show();
        }

        private void ferme_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Recept_Click(object sender, EventArgs e)
        {
            recepte = new TNT.reception.recpt();
            recepte.Show();
        }
        private void pBStock_Click_1(object sender, EventArgs e)
        {
            expedition = new exp_dest();
            expedition.expediteur();
            expedition.Show();
        }

        private void pBEnd_Click_1(object sender, EventArgs e)
        {
            if (traitement_authentification.sync != 3)
            {
                MessageBox.Show("Veilliez Cloturé la tourné d'abord!!");
            }
            else
            {
                
                    // Do something
                    traitement_authentification.sync = 0;
                    this.Close();
                
            }
         

                        
        }

        private void pBDelivery_Click_1(object sender, EventArgs e)
        {
            recepte = new TNT.reception.recpt();
            recepte.Show();
        }

        private void pBSyncPCversPDA_Click(object sender, EventArgs e)
        {

        }

        private void pBSyncPCversPDA_Click_1(object sender, EventArgs e)
        {
            syncro.syncro syncro = new TNT.syncro.syncro();
            syncro.Show();
            this.Close();
        }

        private void pBSync_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            string location = ConfigurationManager.GetChemin<string>("server");
            MessageBox.Show("la location est: " + location.ToString());
        }

        private void Menu_Load(object sender, EventArgs e)
        {

            int syncro = traitement_authentification.sync;
            if (syncro == 2)
            {
                lab_recep.Enabled = true;
                pB_recep.Enabled = true;

                lab_enlev.Enabled = true;
                pB_enlev.Enabled = true;
            } if (syncro == 0)
            {
                MessageBox.Show("Veuliez etablir la synchronisation d'abord !!");
            }
        }


        private void pictureBox1_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;

            traitement_upload up = new traitement_upload();
            up.exporte_fichier();
            int repense =up.upload();
            if (repense == 1) 
            {
                //up.backup_enlev();
                up.backup_enlev();
                up.backup_recept();
            }

            
            int rep1= up.copy_signature("My Documents\\signature\\enlevement", "My Documents\\signature\\copy\\enlevement");
            int rep2= up.copy_signature("My Documents\\signature\\reception", "My Documents\\signature\\copy\\reception");
            
            
            if (traitement_authentification.sync == -1)
            {
                traitement_authentification.sync = 2;
            }
            else { traitement_authentification.sync = 3; }

            lab_recep.Enabled = false;
            pB_recep.Enabled = false;

            lab_enlev.Enabled = false;
            pB_enlev.Enabled = false;

            pBSyncPCversPDA.Enabled = false;
            lab_sync.Enabled = false;

            Cursor.Current = Cursors.Default;
        }

    }
}