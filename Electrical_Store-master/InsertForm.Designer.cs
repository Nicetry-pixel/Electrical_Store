namespace Electrical_Store
{
    partial class InsertForm
    {
        private System.ComponentModel.IContainer components = null;
        protected override void Dispose(bool disposing) { if (disposing && (components != null)) components.Dispose(); base.Dispose(disposing); }

        private void InitializeComponent()
        {
            this.panelUser = new System.Windows.Forms.Panel();
            this.lblUserFIO = new System.Windows.Forms.Label();
            this.txtUserFIO = new System.Windows.Forms.TextBox();
            this.lblUserLogin = new System.Windows.Forms.Label();
            this.txtUserLogin = new System.Windows.Forms.TextBox();
            this.lblUserPass = new System.Windows.Forms.Label();
            this.txtUserPass = new System.Windows.Forms.TextBox();
            this.lblUserEmail = new System.Windows.Forms.Label();
            this.txtUserEmail = new System.Windows.Forms.TextBox();
            this.lblUserPhone = new System.Windows.Forms.Label();
            this.txtUserPhone = new System.Windows.Forms.TextBox();
            this.lblUserRole = new System.Windows.Forms.Label();
            this.cmbUserRole = new System.Windows.Forms.ComboBox();

            this.panelProduct = new System.Windows.Forms.Panel();
            this.lblProdSKU = new System.Windows.Forms.Label();
            this.txtProdSKU = new System.Windows.Forms.TextBox();
            this.lblProdName = new System.Windows.Forms.Label();
            this.txtProdName = new System.Windows.Forms.TextBox();
            this.lblProdCategory = new System.Windows.Forms.Label();
            this.cmbProdCategory = new System.Windows.Forms.ComboBox();
            this.lblProdBrand = new System.Windows.Forms.Label();
            this.txtProdBrand = new System.Windows.Forms.TextBox();
            this.lblProdPrice = new System.Windows.Forms.Label();
            this.txtProdPrice = new System.Windows.Forms.TextBox();
            this.lblProdRetail = new System.Windows.Forms.Label();
            this.txtProdRetail = new System.Windows.Forms.TextBox();

            this.panelStock = new System.Windows.Forms.Panel();
            this.lblStockProduct = new System.Windows.Forms.Label();
            this.cmbStockProduct = new System.Windows.Forms.ComboBox();
            this.lblStockQty = new System.Windows.Forms.Label();
            this.txtStockQty = new System.Windows.Forms.TextBox();
            this.lblStockMin = new System.Windows.Forms.Label();
            this.txtStockMin = new System.Windows.Forms.TextBox();
            this.lblStockLoc = new System.Windows.Forms.Label();
            this.txtStockLoc = new System.Windows.Forms.TextBox();

            this.panelOrder = new System.Windows.Forms.Panel();
            this.lblOrdNumber = new System.Windows.Forms.Label();
            this.txtOrdNumber = new System.Windows.Forms.TextBox();
            this.lblOrdCust = new System.Windows.Forms.Label();
            this.txtOrdCust = new System.Windows.Forms.TextBox();
            this.lblOrdPhone = new System.Windows.Forms.Label();
            this.txtOrdPhone = new System.Windows.Forms.TextBox();
            this.lblOrdStatus = new System.Windows.Forms.Label();
            this.cmbOrdStatus = new System.Windows.Forms.ComboBox();
            this.lblOrdTotal = new System.Windows.Forms.Label();
            this.txtOrdTotal = new System.Windows.Forms.TextBox();

            this.btnSave = new System.Windows.Forms.Button();
            this.btnExit = new System.Windows.Forms.Button();

            // --- panelUser ---
            this.panelUser.Controls.Add(this.cmbUserRole); this.panelUser.Controls.Add(this.lblUserRole);
            this.panelUser.Controls.Add(this.txtUserPhone); this.panelUser.Controls.Add(this.lblUserPhone);
            this.panelUser.Controls.Add(this.txtUserEmail); this.panelUser.Controls.Add(this.lblUserEmail);
            this.panelUser.Controls.Add(this.txtUserPass); this.panelUser.Controls.Add(this.lblUserPass);
            this.panelUser.Controls.Add(this.txtUserLogin); this.panelUser.Controls.Add(this.lblUserLogin);
            this.panelUser.Controls.Add(this.txtUserFIO); this.panelUser.Controls.Add(this.lblUserFIO);
            this.panelUser.Dock = System.Windows.Forms.DockStyle.Top; this.panelUser.Height = 250;

            this.lblUserFIO.Text = "ФИО:"; this.lblUserFIO.Location = new System.Drawing.Point(20, 20);
            this.txtUserFIO.Location = new System.Drawing.Point(120, 20); this.txtUserFIO.Width = 200;
            this.lblUserLogin.Text = "Логин:"; this.lblUserLogin.Location = new System.Drawing.Point(20, 60);
            this.txtUserLogin.Location = new System.Drawing.Point(120, 60); this.txtUserLogin.Width = 200;
            this.lblUserPass.Text = "Пароль:"; this.lblUserPass.Location = new System.Drawing.Point(20, 100);
            this.txtUserPass.Location = new System.Drawing.Point(120, 100); this.txtUserPass.Width = 200;
            this.lblUserEmail.Text = "Email:"; this.lblUserEmail.Location = new System.Drawing.Point(20, 140);
            this.txtUserEmail.Location = new System.Drawing.Point(120, 140); this.txtUserEmail.Width = 200;
            this.lblUserPhone.Text = "Телефон:"; this.lblUserPhone.Location = new System.Drawing.Point(20, 180);
            this.txtUserPhone.Location = new System.Drawing.Point(120, 180); this.txtUserPhone.Width = 200;
            this.lblUserRole.Text = "Роль:"; this.lblUserRole.Location = new System.Drawing.Point(20, 220);
            this.cmbUserRole.Location = new System.Drawing.Point(120, 220); this.cmbUserRole.Width = 200; this.cmbUserRole.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;

            // --- panelProduct ---
            this.panelProduct.Controls.Add(this.txtProdRetail); this.panelProduct.Controls.Add(this.lblProdRetail);
            this.panelProduct.Controls.Add(this.txtProdPrice); this.panelProduct.Controls.Add(this.lblProdPrice);
            this.panelProduct.Controls.Add(this.txtProdBrand); this.panelProduct.Controls.Add(this.lblProdBrand);
            this.panelProduct.Controls.Add(this.cmbProdCategory); this.panelProduct.Controls.Add(this.lblProdCategory);
            this.panelProduct.Controls.Add(this.txtProdName); this.panelProduct.Controls.Add(this.lblProdName);
            this.panelProduct.Controls.Add(this.txtProdSKU); this.panelProduct.Controls.Add(this.lblProdSKU);
            this.panelProduct.Dock = System.Windows.Forms.DockStyle.Top; this.panelProduct.Height = 250;

            this.lblProdSKU.Text = "Артикул:"; this.lblProdSKU.Location = new System.Drawing.Point(20, 20);
            this.txtProdSKU.Location = new System.Drawing.Point(120, 20); this.txtProdSKU.Width = 200;
            this.lblProdName.Text = "Название:"; this.lblProdName.Location = new System.Drawing.Point(20, 60);
            this.txtProdName.Location = new System.Drawing.Point(120, 60); this.txtProdName.Width = 200;
            this.lblProdCategory.Text = "Категория:"; this.lblProdCategory.Location = new System.Drawing.Point(20, 100);
            this.cmbProdCategory.Location = new System.Drawing.Point(120, 100); this.cmbProdCategory.Width = 200; this.cmbProdCategory.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.lblProdBrand.Text = "Бренд:"; this.lblProdBrand.Location = new System.Drawing.Point(20, 140);
            this.txtProdBrand.Location = new System.Drawing.Point(120, 140); this.txtProdBrand.Width = 200;
            this.lblProdPrice.Text = "Закупка:"; this.lblProdPrice.Location = new System.Drawing.Point(20, 180);
            this.txtProdPrice.Location = new System.Drawing.Point(120, 180); this.txtProdPrice.Width = 200;
            this.lblProdRetail.Text = "Розница:"; this.lblProdRetail.Location = new System.Drawing.Point(20, 220);
            this.txtProdRetail.Location = new System.Drawing.Point(120, 220); this.txtProdRetail.Width = 200;

            // --- panelStock ---
            this.panelStock.Controls.Add(this.txtStockLoc); this.panelStock.Controls.Add(this.lblStockLoc);
            this.panelStock.Controls.Add(this.txtStockMin); this.panelStock.Controls.Add(this.lblStockMin);
            this.panelStock.Controls.Add(this.txtStockQty); this.panelStock.Controls.Add(this.lblStockQty);
            this.panelStock.Controls.Add(this.cmbStockProduct); this.panelStock.Controls.Add(this.lblStockProduct);
            this.panelStock.Dock = System.Windows.Forms.DockStyle.Top; this.panelStock.Height = 250;

            this.lblStockProduct.Text = "Товар:"; this.lblStockProduct.Location = new System.Drawing.Point(20, 20);
            this.cmbStockProduct.Location = new System.Drawing.Point(120, 20); this.cmbStockProduct.Width = 200; this.cmbStockProduct.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.lblStockQty.Text = "Кол-во:"; this.lblStockQty.Location = new System.Drawing.Point(20, 60);
            this.txtStockQty.Location = new System.Drawing.Point(120, 60); this.txtStockQty.Width = 200;
            this.lblStockMin.Text = "Мин. остаток:"; this.lblStockMin.Location = new System.Drawing.Point(20, 100);
            this.txtStockMin.Location = new System.Drawing.Point(120, 100); this.txtStockMin.Width = 200;
            this.lblStockLoc.Text = "Ячейка:"; this.lblStockLoc.Location = new System.Drawing.Point(20, 140);
            this.txtStockLoc.Location = new System.Drawing.Point(120, 140); this.txtStockLoc.Width = 200;

            // --- panelOrder ---
            this.panelOrder.Controls.Add(this.txtOrdTotal); this.panelOrder.Controls.Add(this.lblOrdTotal);
            this.panelOrder.Controls.Add(this.cmbOrdStatus); this.panelOrder.Controls.Add(this.lblOrdStatus);
            this.panelOrder.Controls.Add(this.txtOrdPhone); this.panelOrder.Controls.Add(this.lblOrdPhone);
            this.panelOrder.Controls.Add(this.txtOrdCust); this.panelOrder.Controls.Add(this.lblOrdCust);
            this.panelOrder.Controls.Add(this.txtOrdNumber); this.panelOrder.Controls.Add(this.lblOrdNumber);
            this.panelOrder.Dock = System.Windows.Forms.DockStyle.Top; this.panelOrder.Height = 250;

            this.lblOrdNumber.Text = "Номер заказа:"; this.lblOrdNumber.Location = new System.Drawing.Point(20, 20);
            this.txtOrdNumber.Location = new System.Drawing.Point(120, 20); this.txtOrdNumber.Width = 200;
            this.lblOrdCust.Text = "ФИО Клиента:"; this.lblOrdCust.Location = new System.Drawing.Point(20, 60);
            this.txtOrdCust.Location = new System.Drawing.Point(120, 60); this.txtOrdCust.Width = 200;
            this.lblOrdPhone.Text = "Телефон:"; this.lblOrdPhone.Location = new System.Drawing.Point(20, 100);
            this.txtOrdPhone.Location = new System.Drawing.Point(120, 100); this.txtOrdPhone.Width = 200;
            this.lblOrdStatus.Text = "Статус:"; this.lblOrdStatus.Location = new System.Drawing.Point(20, 140);
            this.cmbOrdStatus.Location = new System.Drawing.Point(120, 140); this.cmbOrdStatus.Width = 200; this.cmbOrdStatus.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.lblOrdTotal.Text = "Сумма:"; this.lblOrdTotal.Location = new System.Drawing.Point(20, 180);
            this.txtOrdTotal.Location = new System.Drawing.Point(120, 180); this.txtOrdTotal.Width = 200;

            // --- Buttons ---
            this.btnSave.Text = "Изменить"; this.btnSave.Location = new System.Drawing.Point(100, 320); this.btnSave.Size = new System.Drawing.Size(100, 30);
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            this.btnExit.Text = "Выход"; this.btnExit.Location = new System.Drawing.Point(220, 320); this.btnExit.Size = new System.Drawing.Size(100, 30);
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);

            // CreateForm
            this.ClientSize = new System.Drawing.Size(400, 400);
            this.Controls.Add(this.btnExit); this.Controls.Add(this.btnSave);
            this.Controls.Add(this.panelOrder); this.Controls.Add(this.panelStock); this.Controls.Add(this.panelProduct); this.Controls.Add(this.panelUser);
            this.Name = "CreateForm"; this.Text = "Добавление";
        }

        private System.Windows.Forms.Panel panelUser, panelProduct, panelStock, panelOrder;
        private System.Windows.Forms.Label lblUserFIO, lblUserLogin, lblUserPass, lblUserEmail, lblUserPhone, lblUserRole;
        private System.Windows.Forms.TextBox txtUserFIO, txtUserLogin, txtUserPass, txtUserEmail, txtUserPhone;
        private System.Windows.Forms.ComboBox cmbUserRole;
        private System.Windows.Forms.Label lblProdSKU, lblProdName, lblProdCategory, lblProdBrand, lblProdPrice, lblProdRetail;
        private System.Windows.Forms.TextBox txtProdSKU, txtProdName, txtProdBrand, txtProdPrice, txtProdRetail;
        private System.Windows.Forms.ComboBox cmbProdCategory;
        private System.Windows.Forms.Label lblStockProduct, lblStockQty, lblStockMin, lblStockLoc;
        private System.Windows.Forms.ComboBox cmbStockProduct;
        private System.Windows.Forms.TextBox txtStockQty, txtStockMin, txtStockLoc;
        private System.Windows.Forms.Label lblOrdNumber, lblOrdCust, lblOrdPhone, lblOrdStatus, lblOrdTotal;
        private System.Windows.Forms.TextBox txtOrdNumber, txtOrdCust, txtOrdPhone, txtOrdTotal;
        private System.Windows.Forms.ComboBox cmbOrdStatus;
        private System.Windows.Forms.Button btnSave, btnExit;
    }
}