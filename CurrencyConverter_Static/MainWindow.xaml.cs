using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CurrencyConverter_Static
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        SqlConnection con = new SqlConnection();      //Create Object for SqlConnection
        SqlCommand cmd = new SqlCommand();            //Create Object for SqlCommand
        SqlDataAdapter da = new SqlDataAdapter();     //Created Object for SqlDataAdapter

        private int CurrencyId = 0;         //Declare Currency with int DataType and Assign Value 0
        private double FromAmount = 0;      //Declare FromAmount with double DataType and Assign Value 0
        private double ToAmount = 0;        //Declare ToAmount with double DataType and Assign Valie 0

        public MainWindow()
        {
            InitializeComponent();
            BindCurrency();
            BindCurrency();
            GetData();
        }
        //CRUD

        public void mycon()
        {
            String Conn = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;   //Database Connection String
            con = new SqlConnection(Conn);
            con.Open(); //Connection Open
        }

        private void BindCurrency()
        {
            //DataTable dtCurrency = new DataTable();
            //dtCurrency.Columns.Add("Text");
            //dtCurrency.Columns.Add("Value");

            //Add rows in the Database with text and value
            //dtCurrency.Rows.Add("--SELECT--", 0);
            //dtCurrency.Rows.Add("ZAR", 1);
            //dtCurrency.Rows.Add("USD", 18.577789);
            //dtCurrency.Rows.Add("EUR", 21.082839);
            //dtCurrency.Rows.Add("POUND", 24.800229);
            //dtCurrency.Rows.Add("INR", 0.217629);
            //dtCurrency.Rows.Add("AUD", 11.890262);
            //dtCurrency.Rows.Add("CAD", 13.387423);
            //dtCurrency.Rows.Add("SGD", 14.122394);
            //dtCurrency.Rows.Add("CHF", 22.441976);
            //dtCurrency.Rows.Add("MYR", 4.245746);
            //dtCurrency.Rows.Add("JPY", 0.129768);

            mycon();
            //Create an object for DataTable
            DataTable dt = new DataTable();
            //Write query to get data from Currency_Master table
            cmd = new SqlCommand("select Id, CurrencyName from Currency_Master", con);
            //CommandType define which type of command we use for write a query
            cmd.CommandType = CommandType.Text;

            //It is accepting a parameter that contains the command text of the object's selectedCommand property
            da = new SqlDataAdapter(cmd);
            da.Fill(dt);

            //Create an object for DataRow
            DataRow newRow = dt.NewRow();
            //Assign a value to Id column
            newRow["Id"] = 0;
            //Assign value to CurrencyName column
            newRow["CurrencyName"] = "--SELECT--";

            //Insert a new row in dt with the data at a 0 position
            dt.Rows.InsertAt(newRow, 0);

            //dt is not null and rows count greater than 0
            if(dt != null && dt.Rows.Count > 0)
            {
                //Assign the datatable data to from currency combobox using ItemSource property.
                cmbFromCurrency.ItemsSource = dt.DefaultView;

                //Assign the datatable data to currency combobox using ItemSource property.
                cmbToCurrency.ItemsSource = dt.DefaultView;
            }
            con.Close();

            cmbFromCurrency.DisplayMemberPath = "CurrencyName";
            cmbFromCurrency.SelectedValuePath = "Id";
            cmbFromCurrency.SelectedValue = 0;

            cmbToCurrency.DisplayMemberPath = "CurrencyName";
            cmbToCurrency.SelectedValuePath = "Id";
            cmbToCurrency.SelectedValue = 0;
        }

        private void Convert_Click(object sender, RoutedEventArgs e)
        {
            //Create a variable as ConvertedValue with double data type to store currency converted value
            double ConvertedValue;

            //Check amount textbox is Null or Blank
            if (txtCurrency.Text == null || txtCurrency.Text.Trim() == "")
            {
                //If amount textbox is Null or Blank it will show the below message box   
                MessageBox.Show("Please Enter Currency", "Information", MessageBoxButton.OK, MessageBoxImage.Information);

                //After clicking on message box OK sets the Focus on amount textbox
                txtCurrency.Focus();
                return;
            }
            //Else if the currency from is not selected or it is default text --SELECT--
            else if (cmbFromCurrency.SelectedValue == null || cmbFromCurrency.SelectedIndex == 0)
            {
                //It will show the message
                MessageBox.Show("Please Select Currency From", "Information", MessageBoxButton.OK, MessageBoxImage.Information);

                //Set focus on From Combobox
                cmbFromCurrency.Focus();
                return;
            }
            //Else if Currency To is not Selected or Select Default Text --SELECT--
            else if (cmbToCurrency.SelectedValue == null || cmbToCurrency.SelectedIndex == 0)
            {
                //It will show the message
                MessageBox.Show("Please Select Currency To", "Information", MessageBoxButton.OK, MessageBoxImage.Information);

                //Set focus on To Combobox
                cmbToCurrency.Focus();
                return;
            }
            //If From and To Combobox selected values are same
            if (cmbFromCurrency.Text == cmbToCurrency.Text)
            {
                //The amount textbox value set in ConvertedValue.
                //double.parse is used to convert datatype String To Double.
                //Textbox text have string and ConvertedValue is double datatype
                ConvertedValue = double.Parse(txtCurrency.Text);

                //Show in label converted currency and converted currency name.
                // and ToString("N3") is used to place 000 after after the(.)
                lblCurrency.Content = cmbToCurrency.Text + " " + ConvertedValue.ToString("N3");
            }
            else
            {

                //Calculation for currency converter is From Currency value multiply(*) 
                // with amount textbox value and then the total is divided(/) with To Currency value
                ConvertedValue = (double.Parse(cmbFromCurrency.SelectedValue.ToString()) * 
                    double.Parse(txtCurrency.Text)) / 
                    double.Parse(cmbToCurrency.SelectedValue.ToString());

                //Show in label converted currency and converted currency name.
                lblCurrency.Content = cmbToCurrency.Text + " " + ConvertedValue.ToString("N3");
            }
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            ClearControls();
        }


        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void ClearControls()
        {
            txtCurrency.Text = string.Empty;
            if (cmbFromCurrency.Items.Count > 0)
            {
                cmbFromCurrency.SelectedIndex = 0;
            }
            if (cmbToCurrency.Items.Count > 0)
            {
                cmbToCurrency.SelectedIndex = 0;
            }
            lblCurrency.Content = "";
            txtCurrency.Focus();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (txtAmount.Text == null || txtAmount.Text.Trim() == "")
                {
                    MessageBox.Show("Please enter amount", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                    txtAmount.Focus();
                    return;
                }
                else if (txtCurrencyName.Text == null || txtCurrencyName.Text.Trim() == "")
                {
                    MessageBox.Show("Please enter currency name", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                    txtCurrencyName.Focus();
                    return;
                }
                else
                {
                    if (CurrencyId > 0)       //Code for Update button. Here Check CurrencyId greater than zerothan it is go fo update
                    {
                        if (MessageBox.Show("Are you sure you want to update?", "Information", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)  //Show Confirmation Message
                        {
                            mycon();
                            DataTable dt = new DataTable();
                            cmd = new SqlCommand("UPDATE Currency_Master SET Amount = @Amount, CurrencyName = @CurrencyName WHERE Id = @Id", con);  //Update Query Record updated using Id
                            cmd.CommandType = CommandType.Text;
                            cmd.Parameters.AddWithValue("@Id", CurrencyId);
                            cmd.Parameters.AddWithValue("@Amount", txtAmount.Text);
                            cmd.Parameters.AddWithValue("@CurrencyName", txtCurrencyName.Text);
                            cmd.ExecuteNonQuery();
                            con.Close();

                            MessageBox.Show("Data updated successfully", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    }
                    else     //Save Button Code
                    {
                        if (MessageBox.Show("Are you sure you want to save?", "Information", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                        {
                            mycon();
                            cmd = new SqlCommand("INSERT INTO Currency_Master(Amount, CurrencyName) VALUES(@Amount, @CurrencyName)", con);
                            cmd.CommandType = CommandType.Text;
                            cmd.Parameters.AddWithValue("@Amount", txtAmount.Text);
                            cmd.Parameters.AddWithValue("@CurrencyName", txtCurrencyName.Text);
                            cmd.ExecuteNonQuery();
                            con.Close();

                            MessageBox.Show("Data saved successfully", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    }
                    ClearMaster();
                }
            }catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ClearMaster()  //This Method is used to clear all the input which the user entered in currency master tab
        {
            try
            {
                txtAmount.Text = string.Empty;
                txtCurrencyName.Text = string.Empty;
                btnSave.Content = "Save";
                GetData();
                CurrencyId = 0;
                BindCurrency();
                txtAmount.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)  // Cancel button click event
        {
            try
            {
                ClearMaster();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void GetData()
        {
            mycon();   //mycon() method is used for connect with database and open database connection
            DataTable dt = new DataTable();   //Create Datatable object
            cmd = new SqlCommand("SELECT * FROM Currency_Master", con);
            cmd.CommandType = CommandType.Text;
            da = new SqlDataAdapter(cmd);
            da.Fill(dt);
            if (dt != null && dt.Rows.Count > 0)   //dt is not null and rows count greater than 0
            {
                dgvCurrency.ItemsSource = dt.DefaultView;   //Assign DataTable data to dgvCurrency using ItemSource Property.
            }
            else
            {
                dgvCurrency.ItemsSource = null;
            }
            con.Close();  //Database connection Colse
        }


        private void dgvCurrency_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            try
            {
                DataGrid grd = (DataGrid)sender;   //Create object for DataGrid
                DataRowView row_selected = grd.CurrentItem as DataRowView;  //Create object for DataRowView

                if (row_selected != null)   //row_selected is not null
                {
                    if (dgvCurrency.Items.Count > 0)
                    {
                        if (grd.SelectedCells.Count > 0)
                        {
                            CurrencyId = Int32.Parse(row_selected["Id"].ToString());

                            if (grd.SelectedCells[0].Column.DisplayIndex == 0)   //DisplayIndex is equal to zero than it is Edit cell
                            {
                                txtAmount.Text = row_selected["Amount"].ToString();
                                txtCurrencyName.Text = row_selected["CurrencyName"].ToString();
                                btnSave.Content = "Update";   //Change save button text Save to Update
                            }
                            if (grd.SelectedCells[0].Column.DisplayIndex == 1)   //DisplayIndex is equal to one than it is Delete cell
                            {
                                if (MessageBox.Show("Are you sure you want to delete?", "Information", 
                                    MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)   //Show confirmation dialogue box
                                {
                                    mycon();
                                    DataTable dt = new DataTable();
                                    cmd = new SqlCommand("DELETE FROM Currency_Master WHERE Id = @Id", con);
                                    cmd.CommandType = CommandType.Text;
                                    cmd.Parameters.AddWithValue("@Id", CurrencyId);
                                    cmd.ExecuteNonQuery();
                                    con.Close();

                                    MessageBox.Show("Data deleted successfully", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                                    ClearMaster();
                                }
                            }
                        }
                    }
                }
            }catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
