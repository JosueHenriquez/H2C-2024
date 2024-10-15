﻿using PTC2024.View.Facturacion;
using System;
using System.Text;
using PTC2024.Model.DAO.BillsDAO;
using System.Data;
using System.Windows.Forms;
using PTC2024.View.BillsViews;
using PTC2024.View.Reporting;
using System.Security.Cryptography;
using PTC2024.Controller.Helper;
using System.Drawing;
using PTC2024.Model.DAO.EmployeesDAO;
using PTC2024.Model.DTO.EmployeesDTO;
using System.Drawing.Printing;
using System.IO;
using PTC2024.Model.DTO.BillsDTO;
using PTC2024.View.Reporting.Bills;
using PTC2024.View.formularios.inicio;
using PTC2024.Model.DAO.HelperDAO;
using QRCoder;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Drawing.Imaging;
using System.Diagnostics; // Para abrir el PDF después de generarlo



namespace PTC2024.Controller.BillsController
{
    internal class ControllerBills
    {
        FrmBills objFormBills;
        Form currentForm;
        StartMenu objStartMenu;
        int disabledBillId;

        public ControllerBills(FrmBills View)
        {
            objFormBills = View;
            objFormBills.Load += new EventHandler(LoadDataBills);
            objFormBills.btnNewBills.Click += new EventHandler(AddBills);
            objFormBills.cmsPrintBill.Click += new EventHandler(printBills);
            objFormBills.cmsOverrideBill.Click += new EventHandler(OverrideBills);
            objFormBills.cmsRectifyBill.Click += new EventHandler(Rectify);
            objFormBills.txtSearchB.KeyDown += new KeyEventHandler(SearchBills);
            objFormBills.dgvBills.CellMouseDown += new DataGridViewCellMouseEventHandler(objFormBills_CellMouseDown);
            objFormBills.dgvBills.SelectionChanged += new EventHandler(dgvBills_SelectionChanged);
            objFormBills.cbEfectivo.Click += new EventHandler(CheckboxFiltersMethodCash);
            objFormBills.cbCheque.Click += new EventHandler(CheckboxFiltersMethodPayCheck);
            objFormBills.cbCriptomoneda.Click += new EventHandler(CheckboxFiltersMethodCryptocurrency);
            objFormBills.cbEmitida.Click += new EventHandler(CheckboxFiltersStatusIssued);
            objFormBills.cbRectificada.Click += new EventHandler(CheckboxFiltersStatusRectify);
            objFormBills.cbPagada.Click += new EventHandler(CheckboxFiltersStatusPay);
            objFormBills.cbAnulada.Click += new EventHandler(CheckboxFiltersStatusOverride);
            objFormBills.cbPendiente.Click += new EventHandler(CheckboxFiltersStatusDue);
            disabledBillId = -1;
        }
        /// <summary>
        /// Metodo para refrescar tanto valores de la datagrid como formulario en general
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void LoadDataBills(object sender, EventArgs e)
        {
            ChargeData();
            if (Properties.Settings.Default.darkMode == true)
            {
                objFormBills.BackColor = Color.FromArgb(18, 18, 18);
                objFormBills.lblTitle.ForeColor = Color.White;
                objFormBills.lblSubTitle.ForeColor = Color.White;
                objFormBills.lblPayMeth.ForeColor = Color.White;
                objFormBills.lblStatus.ForeColor = Color.White;
                objFormBills.lblCash.ForeColor = Color.White;
                objFormBills.lblCheque.ForeColor = Color.White;
                objFormBills.lblCrypto.ForeColor = Color.White;
                objFormBills.lblEm.ForeColor = Color.White;
                objFormBills.lblRec.ForeColor = Color.White;
                objFormBills.lblPaid.ForeColor = Color.White;
                objFormBills.lblCanceled.ForeColor = Color.White;
                objFormBills.lblPen.ForeColor = Color.White;
                //objFormBills.txtSearchB.FillColor = Color.FromArgb(26, 32, 161);
                //objFormBills.txtSearchB.BorderColorActive = Color.FromArgb(26, 32, 161);
                //objFormBills.txtSearchB.OnIdleState.BorderColor = Color.FromArgb(26, 32, 161);
                //objFormBills.txtSearchB.BorderColorIdle = Color.FromArgb(26, 32, 161);
                //objFormBills.btnNewBills.OnIdleState.FillColor = Color.FromArgb(26, 32, 161);
                //objFormBills.btnNewBills.OnIdleState.BorderColor = Color.FromArgb(26, 32, 161);
                //objFormBills.btnNewBills.IdleFillColor = Color.FromArgb(26, 32, 161);
                //objFormBills.btnNewBills.IdleBorderColor = Color.FromArgb(26, 32, 161);
                objFormBills.dgvBills.BackgroundColor = Color.FromArgb(45, 45, 45);
                objFormBills.dgvBills.HeaderBackColor = Color.LightSlateGray;
                objFormBills.dgvBills.GridColor = Color.FromArgb(45, 45, 45);
                objFormBills.dgvBills.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.LightSlateGray;
                objFormBills.dgvBills.ColumnHeadersDefaultCellStyle.BackColor = Color.LightSlateGray;
            }
        }
        public void ChargeData()
        {
            DAOBills dAOBills = new DAOBills();
            DataSet ds = dAOBills.Bills();
            //Llenando el datagridview
            objFormBills.dgvBills.DataSource = ds.Tables["viewBill"];
            objFormBills.dgvBills.Columns[0].DividerWidth = 1;
            objFormBills.dgvBills.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            objFormBills.dgvBills.Columns[2].Visible = false;
            objFormBills.dgvBills.Columns[3].Visible = false;
            objFormBills.dgvBills.Columns[9].Visible = false;
            //objFormBills.dgvBills.Columns[12].Visible = false;
            objFormBills.dgvBills.Columns[13].Visible = false;
            objFormBills.dgvBills.Columns[14].Visible = false;
            objFormBills.dgvBills.Columns[18].Visible = false;
            objFormBills.dgvBills.Columns[19].Visible = false;

        }
        //filtracion por busqueda
        public void SearchBills(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                Search();

            }
            if (objFormBills.txtSearchB.Text == string.Empty)
            {
                ChargeData();
            }
        }

        public void Search()
        {
            DAOBills dAOBills = new DAOBills();

            /*Aca se le da valor al atributo de la clase*/
            dAOBills.SearchB1 = objFormBills.txtSearchB.Text;

            /*Se captura la respuesta de l metodo SearchData y se le agrega su respectivo parametro*/
            DataSet respuesta = dAOBills.SearchData(dAOBills.SearchB1);
            /*Se le dice al DataGridView lo que tiene que mostrar*/
            objFormBills.dgvBills.DataSource = respuesta.Tables["viewBill"];
        }
        //Filtracion por checkBox
        public void CheckboxFiltersMethodCash(object senderl, EventArgs e)
        {

            //Creamos objeto del dAOBills
            DAOBills dAOBills = new DAOBills();
            //Creamos una variable string que dependerá de que checkbox esta activado
            string Method;
            if (objFormBills.cbEfectivo.Checked == true)
            {
                Method = objFormBills.cbEfectivo.Tag.ToString();
                objFormBills.cbCheque.Checked = false;
                objFormBills.cbCriptomoneda.Checked = false;
                //Creamos un dataset para capturar el que nos va a devolver el método en el DAO, y le enviamos la variable string que este tiene como parámetro
                DataSet ds = dAOBills.CheckboxFiltersMethod(Method);
                //Le damos el valor al datagrid
                objFormBills.dgvBills.DataSource = ds.Tables["viewBill"];

            }
            else
            {
                ChargeData();
            }
        }

        public void CheckboxFiltersMethodPayCheck(object senderl, EventArgs e)
        {

            //Creamos objeto del dAOBills
            DAOBills dAOBills = new DAOBills();
            //Creamos una variable string que dependerá de que checkbox esta activado
            string Method;
            if (objFormBills.cbCheque.Checked == true)
            {
                Method = objFormBills.cbCheque.Tag.ToString();
                objFormBills.cbEfectivo.Checked = false;
                objFormBills.cbCriptomoneda.Checked = false;
                //Creamos un dataset para capturar el que nos va a devolver el método en el DAO, y le enviamos la variable string que este tiene como parámetro
                DataSet ds = dAOBills.CheckboxFiltersMethod(Method);
                //Le damos el valor al datagrid
                objFormBills.dgvBills.DataSource = ds.Tables["viewBill"];

            }
            else
            {
                ChargeData();
            }
        }

        public void CheckboxFiltersMethodCryptocurrency(object senderl, EventArgs e)
        {

            //Creamos objeto del dAOBills
            DAOBills dAOBills = new DAOBills();
            //Creamos una variable string que dependerá de que checkbox esta activado
            string Method;
            if (objFormBills.cbCriptomoneda.Checked == true)
            {
                Method = objFormBills.cbCriptomoneda.Tag.ToString();
                objFormBills.cbCheque.Checked = false;
                objFormBills.cbEfectivo.Checked = false;
                //Creamos un dataset para capturar el que nos va a devolver el método en el DAO, y le enviamos la variable string que este tiene como parámetro
                DataSet ds = dAOBills.CheckboxFiltersMethod(Method);
                //Le damos el valor al datagrid
                objFormBills.dgvBills.DataSource = ds.Tables["viewBill"];

            }
            else
            {
                ChargeData();
            }
        }

        public void CheckboxFiltersStatusIssued(object senderl, EventArgs e)
        {
            //Creamos objeto del dAOBills
            DAOBills dAOBills = new DAOBills();
            //Creamos una variable string que dependerá de que checkbox esta activado
            string status;
            if (objFormBills.cbEmitida.Checked == true)
            {
                status = objFormBills.cbEmitida.Tag.ToString();
                objFormBills.cbRectificada.Checked = false;
                objFormBills.cbPagada.Checked = false;
                objFormBills.cbAnulada.Checked = false;
                objFormBills.cbPendiente.Checked = false;
                //Creamos un dataset para capturar el que nos va a devolver el método en el DAO, y le enviamos la variable string que este tiene como parámetro
                DataSet ds = dAOBills.CheckboxFiltersStatus(status);
                //Le damos el valor al datagrid
                objFormBills.dgvBills.DataSource = ds.Tables["viewBill"];

            }
            else
            {
                ChargeData();
            }
        }

        public void CheckboxFiltersStatusRectify(object senderl, EventArgs e)
        {
            //Creamos objeto del dAOBills
            DAOBills dAOBills = new DAOBills();
            //Creamos una variable string que dependerá de que checkbox esta activado
            string status;
            if (objFormBills.cbRectificada.Checked == true)
            {
                status = objFormBills.cbRectificada.Tag.ToString();
                objFormBills.cbEmitida.Checked = false;
                objFormBills.cbPagada.Checked = false;
                objFormBills.cbAnulada.Checked = false;
                objFormBills.cbPendiente.Checked = false;
                //Creamos un dataset para capturar el que nos va a devolver el método en el DAO, y le enviamos la variable string que este tiene como parámetro
                DataSet ds = dAOBills.CheckboxFiltersStatus(status);
                //Le damos el valor al datagrid
                objFormBills.dgvBills.DataSource = ds.Tables["viewBill"];

            }
            else
            {
                ChargeData();
            }
        }

        public void CheckboxFiltersStatusPay(object senderl, EventArgs e)
        {
            //Creamos objeto del dAOBills
            DAOBills dAOBills = new DAOBills();
            //Creamos una variable string que dependerá de que checkbox esta activado
            string status;
            if (objFormBills.cbPagada.Checked == true)
            {
                status = objFormBills.cbPagada.Tag.ToString();
                objFormBills.cbEmitida.Checked = false;
                objFormBills.cbRectificada.Checked = false;
                objFormBills.cbAnulada.Checked = false;
                objFormBills.cbPendiente.Checked = false;
                //Creamos un dataset para capturar el que nos va a devolver el método en el DAO, y le enviamos la variable string que este tiene como parámetro
                DataSet ds = dAOBills.CheckboxFiltersStatus(status);
                //Le damos el valor al datagrid
                objFormBills.dgvBills.DataSource = ds.Tables["viewBill"];

            }
            else
            {
                ChargeData();
            }
        }

        public void CheckboxFiltersStatusOverride(object senderl, EventArgs e)
        {
            //Creamos objeto del dAOBills
            DAOBills dAOBills = new DAOBills();
            //Creamos una variable string que dependerá de que checkbox esta activado
            string status;
            if (objFormBills.cbAnulada.Checked == true)
            {
                status = objFormBills.cbAnulada.Tag.ToString();
                objFormBills.cbEmitida.Checked = false;
                objFormBills.cbRectificada.Checked = false;
                objFormBills.cbPagada.Checked = false;
                objFormBills.cbPendiente.Checked = false;
                //Creamos un dataset para capturar el que nos va a devolver el método en el DAO, y le enviamos la variable string que este tiene como parámetro
                DataSet ds = dAOBills.CheckboxFiltersStatus(status);
                //Le damos el valor al datagrid
                objFormBills.dgvBills.DataSource = ds.Tables["viewBill"];

            }
            else
            {
                ChargeData();
            }
        }

        public void CheckboxFiltersStatusDue(object senderl, EventArgs e)
        {
            //Creamos objeto del dAOBills
            DAOBills dAOBills = new DAOBills();
            //Creamos una variable string que dependerá de que checkbox esta activado
            string status;
            if (objFormBills.cbPendiente.Checked == true)
            {
                status = objFormBills.cbAnulada.Tag.ToString();
                objFormBills.cbEmitida.Checked = false;
                objFormBills.cbRectificada.Checked = false;
                objFormBills.cbPagada.Checked = false;
                objFormBills.cbAnulada.Checked = false;
                //Creamos un dataset para capturar el que nos va a devolver el método en el DAO, y le enviamos la variable string que este tiene como parámetro
                DataSet ds = dAOBills.CheckboxFiltersStatus(status);
                //Le damos el valor al datagrid
                objFormBills.dgvBills.DataSource = ds.Tables["viewBill"];

            }
            else
            {
                ChargeData();
            }
        }

        /// <summary>
        /// Método para imprimir la factura convirtiendo la a PDF
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void printBills(object sender, EventArgs e)
        {
            // Método que se encarga de manejar el evento Click del botón cmsPrintBill
            if (objFormBills.dgvBills.CurrentRow != null)
            {
                // Capturar el IdBill de la factura seleccionada en el DataGridView
                int billId = Convert.ToInt32(objFormBills.dgvBills.CurrentRow.Cells["N°"].Value);

                // Generar el PDF de la factura con el IdBill seleccionado
                GenerateBillPDF(billId);
            }
            else
            {
                MessageBox.Show("Por favor, selecciona una factura para imprimir.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        // Método para generar el PDF de una factura
        public void GenerateBillPDF(int idBill)
        {
            try
            {
                DAOBills dAOBills = new DAOBills();
                DataSet dsBill = dAOBills.GetBillDetails(idBill);

                if (dsBill != null && dsBill.Tables["viewBill"].Rows.Count > 0)
                {
                    DataRow billRow = dsBill.Tables["viewBill"].Rows[0];

                    // Obtener un directorio temporal para almacenar el PDF
                    string tempFilePath = Path.Combine(Path.GetTempPath(), $"Bill_{idBill}.pdf");

                    Document doc = new Document();
                    PdfWriter.GetInstance(doc, new FileStream(tempFilePath, FileMode.Create));
                    doc.Open();

                    // Fuentes para los textos
                    var titleFont = iTextSharp.text.FontFactory.GetFont("Arial", 18, iTextSharp.text.Font.BOLD, BaseColor.RED);
                    var regularFont = iTextSharp.text.FontFactory.GetFont("Arial", 12, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                    var boldFont = iTextSharp.text.FontFactory.GetFont("Arial", 12, iTextSharp.text.Font.BOLD, BaseColor.BLACK);

                    // Título del documento
                    doc.Add(new Paragraph("FACTURA", titleFont));
                    doc.Add(new Paragraph(" "));

                    // Datos principales
                    doc.Add(new Paragraph($"Número de Factura: {billRow["N°"]}", boldFont));
                    doc.Add(new Paragraph($"Razón Social: {billRow["Razon Social"]}", regularFont));
                    doc.Add(new Paragraph($"NIT: {billRow["NIT"]}", regularFont));
                    doc.Add(new Paragraph($"NRC: {billRow["NRC"]}", regularFont));
                    doc.Add(new Paragraph($"Cliente: {billRow["Cliente"]}", regularFont));
                    doc.Add(new Paragraph($"DUI del Cliente: {billRow["DUI"]}", regularFont));
                    doc.Add(new Paragraph($"Teléfono del Cliente: {billRow["Télefono"]}", regularFont));
                    doc.Add(new Paragraph($"Email del Cliente: {billRow["Email"]}", regularFont));
                    doc.Add(new Paragraph(" "));

                    // Detalles del servicio
                    doc.Add(new Paragraph("Detalles del Servicio:", boldFont));
                    doc.Add(new Paragraph($"Servicio: {billRow["Servicios"]}", regularFont));
                    doc.Add(new Paragraph($"Descuento: {billRow["Descuento"]}%", regularFont));
                    doc.Add(new Paragraph($"Subtotal: ${billRow["Subtotal"]}", regularFont));
                    doc.Add(new Paragraph($"Total a Pagar: ${billRow["Total"]}", regularFont));
                    doc.Add(new Paragraph($"Método de Pago: {billRow["Método de Pago"]}", regularFont));
                    doc.Add(new Paragraph(" "));

                    // Fechas
                    doc.Add(new Paragraph($"Fecha de Emisión: {billRow["Fecha de emisión"]}", regularFont));
                    doc.Add(new Paragraph($"Fecha Inicio del Servicio: {billRow["Fecha inicio"]}", regularFont));
                    doc.Add(new Paragraph($"Fecha Fin del Servicio: {billRow["Fecha fin"]}", regularFont));
                    doc.Add(new Paragraph(" "));

                    // Encargado y estado
                    doc.Add(new Paragraph($"Encargado: {billRow["Encargado"]}", regularFont));
                    doc.Add(new Paragraph($"Estado de la Factura: {billRow["Estado"]}", regularFont));

                    // Generar el código QR basado en los datos de la factura
                    string qrData = $"Factura N°: {billRow["N°"]}\n" +
                                    $"Razón Social: {billRow["Razon Social"]}\n" +
                                    $"Cliente: {billRow["Cliente"]}\n" +
                                    $"Total a Pagar: ${billRow["Total"]}\n" +
                                    $"Fecha de Emisión: {billRow["Fecha de emisión"]}";

                    using (MemoryStream msQrCode = new MemoryStream())
                    {
                        // Generar el código QR usando QRCoder
                        QRCodeGenerator qrGenerator = new QRCodeGenerator();
                        QRCodeData qrCodeData = qrGenerator.CreateQrCode(qrData, QRCodeGenerator.ECCLevel.Q);
                        QRCode qrCode = new QRCode(qrCodeData);

                        using (Bitmap qrCodeImage = qrCode.GetGraphic(20)) // Ajusta la escala del código QR aquí
                        {
                            // Guardar el código QR como imagen en memoria
                            qrCodeImage.Save(msQrCode, ImageFormat.Png);
                        }

                        // Convertir el stream en una imagen que iTextSharp pueda usar
                        iTextSharp.text.Image qrImage = iTextSharp.text.Image.GetInstance(msQrCode.ToArray());
                        qrImage.ScaleToFit(100f, 100f); // Ajusta el tamaño del QR 
                        qrImage.Alignment = Element.ALIGN_RIGHT;

                        // Añadir el código QR al PDF
                        doc.Add(qrImage);
                    }

                    // Cerrar el documento PDF
                    doc.Close();

                    // Abrir el PDF en el navegador predeterminado o visor de PDF
                    Process.Start(new ProcessStartInfo(tempFilePath)
                    {
                        UseShellExecute = true // Esto asegurará que se abra con el programa predeterminado del sistema
                    });
                }
                else
                {
                    MessageBox.Show("No se encontraron datos para la factura seleccionada.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al generar el PDF: " + ex.Message);
            }
        }



        /// <summary>
        /// Método para abrir formulario "Agregar factura"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void AddBills(object sender, EventArgs e)
        {
            FrmAddBills newBill = new FrmAddBills(2);
            newBill.ShowDialog();
            ChargeData();
        }

        /// <summary>
        /// Método para rectificar factura, cargando los datos y luego estos se puedan modificar nuevamente
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Rectify(object sender, EventArgs e)
        {
            FrmAddBills rectifyBill = new FrmAddBills(1);
            rectifyBill.ShowDialog();
            ChargeData();


        }

        /// <summary>
        /// Método para anular facturas
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void OverrideBills(object sender, EventArgs e)
        {
            int row = objFormBills.dgvBills.CurrentRow.Index;
            if (row < 0)
            {
                MessageBox.Show("Por favor, seleccione una factura para anular.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int idBill = int.Parse(objFormBills.dgvBills[0, row].Value.ToString());
            FrmOverrideBill openForm = new FrmOverrideBill();
            ControllerOverride controller = new ControllerOverride(openForm);

            openForm.ShowDialog();

            if (controller.ConfirmValue == 1)
            {
                DAOBills daoBills = new DAOBills();
                DataSet ds = daoBills.over(idBill.ToString());
                // Deshabilitar visualmente la fila y marcarla como solo lectura
                StartMenu startMenu = new StartMenu(SessionVar.Username);
                startMenu.snackBar.Show(startMenu, $"Factura anulada", Bunifu.UI.WinForms.BunifuSnackbar.MessageTypes.Success, 3000, null, Bunifu.UI.WinForms.BunifuSnackbar.Positions.TopRight);
                disabledBillId = idBill;
                DisableRow(idBill);
                SetRowReadOnly(idBill);
                DAOInitialView daoInitial = new DAOInitialView();
                daoInitial.ActionType = "Se anuló una factura";
                daoInitial.TableName = "Facturas";
                daoInitial.ActionBy = SessionVar.Username;
                daoInitial.ActionDate = DateTime.Now;
                int auditAnswer = daoInitial.InsertAudit();
                if (auditAnswer != 1)
                {
                    objStartMenu.snackBar.Show(objStartMenu, $"La auditoria no pudo ser registrada", Bunifu.UI.WinForms.BunifuSnackbar.MessageTypes.Success, 3000, null, Bunifu.UI.WinForms.BunifuSnackbar.Positions.BottomRight);
                }
            }
            else
            {
                StartMenu startMenu = new StartMenu(SessionVar.Username);
                startMenu.snackBar.Show(startMenu, $"Contraseña de administrador incorrecta", Bunifu.UI.WinForms.BunifuSnackbar.MessageTypes.Error, 3000, null, Bunifu.UI.WinForms.BunifuSnackbar.Positions.TopRight);
                startMenu.snackBar.Show(startMenu, $"Operación cancelada", Bunifu.UI.WinForms.BunifuSnackbar.MessageTypes.Error, 3000, null, Bunifu.UI.WinForms.BunifuSnackbar.Positions.TopRight);
            }
            ChargeData();
        }

        /// <summary>
        /// Deshabilita visualmente la fila de la datagrid 
        /// </summary>
        /// <param name="idBill"></param>
        public void DisableRow(int idBill)
        {
            foreach (DataGridViewRow row in objFormBills.dgvBills.Rows)
            {
                var cellValue = row.Cells["N°"].Value;

                if (cellValue != null && int.TryParse(cellValue.ToString(), out int cellValueAsInt) && cellValueAsInt == idBill)
                {
                    row.DefaultCellStyle.BackColor = Color.LightGray;
                    row.DefaultCellStyle.ForeColor = Color.DarkGray;
                    row.DefaultCellStyle.SelectionBackColor = Color.LightGray;
                    row.DefaultCellStyle.SelectionForeColor = Color.DarkGray;
                    ChargeData();
                    break;
                }
            }
            ChargeData();
        }
        /// <summary>
        /// Método que se utliza pa una fila especifica de dgvBills y la marca como solo lectura sin posibilidad de editarla
        /// </summary>
        /// <param name="idBill"></param>
    public void SetRowReadOnly(int idBill)
    {
            foreach (DataGridViewRow row in objFormBills.dgvBills.Rows)
        {
            var cellValue = row.Cells["N°"].Value;
                //Valor no nulo se compara el idBill
            if (cellValue != null && int.TryParse(cellValue.ToString(), out int cellValueAsInt) && cellValueAsInt == idBill)
            {
                    //se establece como solo lectura
                row.ReadOnly = true;
                    //detiene la búsqueda para deshabilitar la fila segun id
                    break;
            }
        }
    }
        /// <summary>
        /// Método para indicar cuando el usuario haga clic en la celda de dgvBills
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
    public void objFormBills_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
    {
            //Verifica si la fila es valida
        if (e.RowIndex >= 0)
        {
            DataGridViewRow row = objFormBills.dgvBills.Rows[e.RowIndex];
                //Obtiene el valor de la celda en la columna N° 
            var cellValue = row.Cells["N°"].Value;
                //Si valor no nulo lo compara con el metodo disableBillId
            if (cellValue != null && int.TryParse(cellValue.ToString(), out int cellValueAsInt) && cellValueAsInt == disabledBillId)
            {
            }
        }
    }

        /// <summary>
        /// Maneja el evento cuando la selección de las filas de dgvBills cambia
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
    public void dgvBills_SelectionChanged(object sender, EventArgs e)
    {
        foreach (DataGridViewRow row in objFormBills.dgvBills.SelectedRows)
        {
            var cellValue = row.Cells["N°"].Value;

            if (cellValue != null && int.TryParse(cellValue.ToString(), out int cellValueAsInt))
            {
                if (cellValueAsInt == disabledBillId)
                {
                    row.Selected = false;
                }
            }
        }
    }

    }
}
