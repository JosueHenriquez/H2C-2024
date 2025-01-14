﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using PTC2024.Model.DAO.MaintenanceDAO;
using PTC2024.View.Maintenance;

namespace PTC2024.Controller.MaintenanceController
{
    internal class ControllerBanks
    {
        FrmBanks objBanks;
        public ControllerBanks(FrmBanks View)
        {
            objBanks = View;
            objBanks.Load += new EventHandler(InitialCharge);
            objBanks.btnAddBank.Click += new EventHandler(NewBank);
            objBanks.cmsDeleteBank.Click += new EventHandler(DeleteBank);
            objBanks.btnGoBack.Click += new EventHandler(CloseForm);
            objBanks.txtBank.MouseDown += new MouseEventHandler(DisableContextMenu);
            objBanks.txtBank.TextChanged += new EventHandler(OnlyLettersBank);
        }

        public void InitialCharge(object sender, EventArgs e)
        {
            LoadDataGridBanks();
            if(Properties.Settings.Default.darkMode == true)
            {
                objBanks.BackColor = Color.FromArgb(30, 30, 30);
                objBanks.lblTitle.ForeColor = Color.White;
                objBanks.lblText.ForeColor = Color.White;
                objBanks.dgvBanks.BackgroundColor = Color.FromArgb(45, 45, 45);
                objBanks.dgvBanks.HeaderBackColor = Color.LightSlateGray;
                objBanks.dgvBanks.GridColor = Color.FromArgb(45, 45, 45);
                objBanks.dgvBanks.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.LightSlateGray;
                objBanks.dgvBanks.ColumnHeadersDefaultCellStyle.BackColor = Color.LightSlateGray;
            }
        }

        public void CloseForm(object sender, EventArgs e)
        {
            objBanks.Close();
        }

        public void LoadDataGridBanks()
        {
            //Creamos objeto del daoBanks
            DAOBanks daoBanks = new DAOBanks();
            //Creamos un dataset que recogerá lo que nos manda el método del DTO
            DataSet ds = daoBanks.GetBanks();
            //Le damos valor al datasource del datagrid
            objBanks.dgvBanks.DataSource = ds.Tables["viewBanks"];
        }

        public void NewBank(object sender, EventArgs e)
        {
            //Verificación de campos vacíos
            if (!(string.IsNullOrEmpty(objBanks.txtBank.Text)))
            {
                //Creamos objeto del dao
                DAOBanks daoBanks = new DAOBanks();
                //damos valor al getter
                daoBanks.Bank = objBanks.txtBank.Text.Trim();
                //ejecutamos el método del dao
                int returnedAnswer = daoBanks.AddBank();

                //Validamos la respuesta que nos retornaron
                if (returnedAnswer == 1)
                {
                    MessageBox.Show("El banco se agregó correctamente", "Proceso completado", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("El banco no pudo ser agregado", "Proceso fallido", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                LoadDataGridBanks();
                objBanks.txtBank.Clear();

            }
            else
            {
                MessageBox.Show("Llene el campo para agregar un nuevo banco", "Campo Vacío", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        public void DeleteBank(object sender, EventArgs e)
        {
            //Creamos objeto de la clase DAO
            DAOBanks daoBanks = new DAOBanks();
            //Creamos la variable con la que sabremos que registro esta seleccionado en el datagrid
            int row = objBanks.dgvBanks.CurrentRow.Index;
            int idBank = int.Parse(objBanks.dgvBanks[0, row].Value.ToString());
            //Confirmación por parte del usuario
            if (MessageBox.Show("¿Está seguro que desea eliminar este banco?", "Confirmación", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                //Damos valor al getter del dao
                daoBanks.IdBank = int.Parse(objBanks.dgvBanks[0, row].Value.ToString());
                //ejecutamos el método del dao
                int returnedAnswer = daoBanks.DeleteBank();
                //validamos la respuesta devuelta
                if (returnedAnswer == 1)
                {
                    MessageBox.Show("El banco se eliminó correctamente", "Proceso completado", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    objBanks.snack.Show(objBanks, "Este banco no se puede eliminar debido a que algun empleado pertenece a este banco", Bunifu.UI.WinForms.BunifuSnackbar.MessageTypes.Error, 3000, null, Bunifu.UI.WinForms.BunifuSnackbar.Positions.BottomCenter);
                }
                LoadDataGridBanks();
            }

        }

        private void DisableContextMenu(object sender, MouseEventArgs e)
        {
            // Desactiva el menú contextual al hacer clic derecho
            if (e.Button == MouseButtons.Right)
            {
                ((Bunifu.UI.WinForms.BunifuTextBox)sender).ContextMenu = new ContextMenu();  // Asigna un menú vacío
            }
        }

        public void OnlyLettersBank(object sender, EventArgs e)
        {
            // Obtener la posición actual del cursor
            int cursorPosition = objBanks.txtBank.SelectionStart;

            // Filtrar el texto para que solo queden letras y espacios
            string text = new string(objBanks.txtBank.Text
                                       .Where(c => char.IsLetter(c) || char.IsWhiteSpace(c))
                                       .ToArray());

            // Actualizar el contenido del TextBox con el texto filtrado
            objBanks.txtBank.Text = text;

            // Restaurar la posición del cursor
            objBanks.txtBank.SelectionStart = cursorPosition;
        }
    }
}
