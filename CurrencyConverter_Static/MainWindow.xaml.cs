using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Http;
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
using Newtonsoft.Json;

namespace CurrencyConverter_Static
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //this class initializes the components
        //gather data from an API (for 
        //most accuarate return rates). It returns 
        //the currency Name with it's value
        public class Root
        {
            //it is important to use the same
            //names as the properties in the
            //Json root (found near the bottom of
            //https://docs.openexchangerates.org/reference/api-introduction
            public Rate rates { get; set; }
            public long timestamp;
            public string license;
        }

        //this class is used to store all real-time rates
        //from the API
        public class Rate
        {
            public double INR { get; set; }
            public double JPY { get; set; }
            public double USD { get; set; }
            public double NZD { get; set; }
            public double EUR { get; set; }
            public double CAD { get; set; }
            public double ISK { get; set; }
            public double PHP { get; set; }
            public double DKK { get; set; }
            public double CZK { get; set; }
        }
        //variable for setting currency rate values
        Root val = new Root();

        //Create an object for SqlConnection        
        SqlConnection con = new SqlConnection();

        //Create an object for SqlCommand
        SqlCommand cmd = new SqlCommand();

        //Create object for SqlDataAdapter
        SqlDataAdapter da = new SqlDataAdapter();

        //Declare CurrencyId with int data type and assign value as 0.
        private int CurrencyId = 0;

        //Declare FromAmount with double data type and assign value 0.
        private double FromAmount = 0;

        //Declare ToAmount with double data type and assign value 0.
        private double ToAmount = 0;



        public MainWindow()
        {
            InitializeComponent();
            ClearControls();
            //this should be written here to be executed anytime
            GetValue();
        }

        //sets val to asynchronously be set to a currency rate value
        private async void GetValue()
        {
            val = await GetData<Root>("https://openexchangerates.org/api/latest.json?app_id=86e7e5dfb29b49b6a51be510b412a689"); //API Link
            BindCurrency();
        }

        //this will gather the data from the web using our Root class
        //(made on line 32)

        //Task is used here so that we have our result be of type 
        //<Root> (so that we can do whatever we want with it, like
        //for example assigning the result to a specific currency rate)
        public static async Task<Root> GetData<T>(string url)
        {
            var myRoot = new Root();
            //always use try and catch when gathering data from the 
            //internet, something can always go wrong
            try
            {
                //HttpClient class provides a base class for sending/receiving the HTTP requests/responses from a URL.
                using (var client = new HttpClient())
                {
                    //The timespan to wait before the request times out.
                    client.Timeout = TimeSpan.FromMinutes(1);

                    //HttpResponseMessage is a way of returning a message/data from your action.
                    HttpResponseMessage response = await client.GetAsync(url);

                    // The response contains information like:
                    // - Status code (200, 404, 500, etc.)
                    // - Headers
                    // - The actual data returned by the server
                    //
                    // Here we check if the server says everything worked (200 OK).
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        // The data returned by the server lives inside 'response.Content'
                        //
                        // 'Content' is the BODY of the response — the actual data,
                        // usually JSON when calling an API.
                        //
                        // ReadAsStringAsync() converts that data into a normal C# string.
                        // 'await' is used again because reading data also takes time.
                        //
                        // This is also called Serialization
                        var ResponseString = await response.Content.ReadAsStringAsync();


                        // We turn the C# string into a C# object by deserializing it,
                        // making it easier to access it's properties and use the 
                        // data straight from the web as an object
                        var ResponseObject = JsonConvert.DeserializeObject<Root>(ResponseString);
                        // MessageBox.Show("Rates: " + ResponseObject.license, "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                        return ResponseObject;  //Return API responce
                    }
                    return myRoot;
                }
            }
            catch
            {
                return myRoot;
            }
        }

        public void myconnection()
        {
            //Database connection string
            String Conn = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            con = new SqlConnection(Conn);
            //opening the connection lets us start running SQL
            //commands such as creating data and putting it
            //into the database, getting data out of it,
            //updating the data, or deleting it

            //CRUD (stands for create, read, update, update, delete)
            con.Open(); //Connection Open
        }

        private void BindCurrency()
        {
            //Create an object for DataTable.
            //we create this for C# to understand
            //what we are trying to do with the
            //data table. We need to do that
            //since we need C# to give that 
            //information to the database
            DataTable dt = new DataTable();
            //Write query to get data from Currency_Master table

            dt.Columns.Add("Text");

            dt.Columns.Add("Value");
            

            //Insert a new row in dt with the data at a 0 position
            dt.Rows.Add("--SELECT--", 0);
            dt.Rows.Add("INR", val.rates.INR);
            dt.Rows.Add("USD", val.rates.USD);
            dt.Rows.Add("NZD", val.rates.NZD);
            dt.Rows.Add("JPY", val.rates.JPY);
            dt.Rows.Add("EUR", val.rates.EUR);
            dt.Rows.Add("CAD", val.rates.CAD);
            dt.Rows.Add("ISK", val.rates.ISK);
            dt.Rows.Add("PHP", val.rates.PHP);
            dt.Rows.Add("DKK", val.rates.DKK);
            dt.Rows.Add("CZK", val.rates.CZK);

            
            //Assign the datatable data to from currency combobox using ItemSource property.
            cmbFromCurrency.ItemsSource = dt.DefaultView;
           

            //this sets the Text column (and it's respected rows) as what is displayed
            cmbFromCurrency.DisplayMemberPath = "Text";
            //this sets the Value colummn (and it's respected rows) as values
            //representing what Text is selected
            cmbFromCurrency.SelectedValuePath = "Value";
            //this sets the text "--SELECT--" to be
            //displayed on the box
            cmbFromCurrency.SelectedValue = 0;

            cmbToCurrency.ItemsSource = dt.DefaultView;
            cmbToCurrency.DisplayMemberPath = "Text";
            cmbToCurrency.SelectedValuePath = "Value";
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
                ConvertedValue = (double.Parse(cmbToCurrency.SelectedValue.ToString()) *

                    double.Parse(txtCurrency.Text)) / double.Parse(cmbFromCurrency.SelectedValue.ToString());


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
            //this regular expression makes it so that it ONLY accept numbers
            //between 0-9
            Regex regex = new Regex("^[0-9]+");
            //we implement it to see our event's text, if it
            //matches our regex, and that the event is only handled
            //if it matches our regex
            e.Handled = !regex.IsMatch(e.Text);
        }

        //we keep this as a seperate method just in case we
        //want to reimplement it in another method
        private void ClearControls()
        {
            txtCurrency.Text = string.Empty;
            if (cmbFromCurrency.Items.Count > 0)
                cmbFromCurrency.SelectedIndex = 0;
            if (cmbToCurrency.Items.Count > 0)
                cmbToCurrency.SelectedIndex = 0;
            lblCurrency.Content = "";
            txtCurrency.Focus();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            //When using databases, something can always go wrong. That
            //is why it is always a good practice to put Events
            //in a try catch block with a generic error message
            //in case something goes wrong
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
                {   //Edit time and set that record Id in CurrencyId variable.
                    //Code to Update. If CurrencyId greater than zero than it is go for update.
                    if (CurrencyId > 0)
                    {
                        //Show the confirmation message
                        if (MessageBox.Show("Are you sure you want to update ?", "Information", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                        {
                            myconnection();
                            DataTable dt = new DataTable();

                            //Update Query Record update using Id
                            cmd = new SqlCommand("UPDATE Currency_Master SET Amount = @Amount, CurrencyName = @CurrencyName WHERE Id = @Id", con);
                            cmd.CommandType = CommandType.Text;
                            cmd.Parameters.AddWithValue("@Id", CurrencyId);
                            cmd.Parameters.AddWithValue("@Amount", txtAmount.Text);
                            cmd.Parameters.AddWithValue("@CurrencyName", txtCurrencyName.Text);
                            cmd.ExecuteNonQuery();
                            con.Close();

                            MessageBox.Show("Data updated successfully", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    }
                    // Code to Save
                    else
                    {
                        if (MessageBox.Show("Are you sure you want to save ?", "Information", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                        {
                            myconnection();
                            //Insert query to Save data in the table
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
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        //Method is used to clear all the input which the user entered in currency master tab
        private void ClearMaster()
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

        public void GetData()
        {

            //Method is used for connect with database and open database connection
            myconnection();

            //Create Datatable object
            DataTable dt = new DataTable();

            //Write SQL query to get the data from database table. Query written in double quotes and after comma provide connection.
            cmd = new SqlCommand("SELECT * FROM Currency_Master", con);

            //CommandType define which type of command will execute like Text, StoredProcedure, TableDirect.
            cmd.CommandType = CommandType.Text;

            //It is accept a parameter that contains the command text of the object's SelectCommand property.
            da = new SqlDataAdapter(cmd);

            //The DataAdapter serves as a bridge between a DataSet and a data source for retrieving and saving data. 
            //The fill operation then adds the rows to destination DataTable objects in the DataSet
            da.Fill(dt);

            //dt is not null and rows count greater than 0
            if (dt != null && dt.Rows.Count > 0)
                //Assign DataTable data to dgvCurrency using item source property.
                dgvCurrency.ItemsSource = dt.DefaultView;
            else
                dgvCurrency.ItemsSource = null;

            //Database connection close
            con.Close();
        }


        private void dgvCurrency_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            try
            {
                //Create object for DataGrid
                DataGrid grd = (DataGrid)sender;

                //Create an object for DataRowView
                DataRowView row_selected = grd.CurrentItem as DataRowView;

                //If row_selected is not null
                if (row_selected != null)
                {
                    //dgvCurrency items count greater than zero
                    if (dgvCurrency.Items.Count > 0)
                    {
                        if (grd.SelectedCells.Count > 0)
                        {
                            //Get selected row id column value and set it to the CurrencyId variable
                            CurrencyId = Int32.Parse(row_selected["Id"].ToString());

                            //DisplayIndex is equal to zero in the Edited cell
                            if (grd.SelectedCells[0].Column.DisplayIndex == 0)
                            {
                                //Get selected row amount column value and set to amount textbox
                                txtAmount.Text = row_selected["Amount"].ToString();

                                //Get selected row CurrencyName column value and set it to CurrencyName textbox
                                txtCurrencyName.Text = row_selected["CurrencyName"].ToString();
                                btnSave.Content = "Update";     //Change save button text Save to Update
                            }

                            //DisplayIndex is equal to one in the deleted cell
                            if (grd.SelectedCells[0].Column.DisplayIndex == 1)
                            {
                                //Show confirmation dialog box
                                if (MessageBox.Show("Are you sure you want to delete ?", "Information", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                                {
                                    myconnection();
                                    DataTable dt = new DataTable();

                                    //Execute delete query to delete record from table using Id
                                    cmd = new SqlCommand("DELETE FROM Currency_Master WHERE Id = @Id", con);
                                    cmd.CommandType = CommandType.Text;

                                    //CurrencyId set in @Id parameter and send it in delete statement
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
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
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
    }
}
