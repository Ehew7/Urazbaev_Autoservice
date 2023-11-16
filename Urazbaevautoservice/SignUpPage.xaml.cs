using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

namespace Urazbaevautoservice
{
    /// <summary>
    /// Логика взаимодействия для SignUpPage.xaml
    /// </summary>
    public partial class SignUpPage : Page
    {
        private Service _currentService = new Service();
       
        public SignUpPage(Service SelectedService)
        {
            InitializeComponent();
            if (SelectedService != null)
            {
                this._currentService = SelectedService;
               
            }

            DataContext = _currentService;

            var _currentClient = Urazbaev_autoserviceEntities.GetContext().Client.ToList();

            ComboClient.ItemsSource = _currentClient;
        }

        private ClientService _currentClientService = new ClientService();
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder();

            if (ComboClient.SelectedItem == null)
                errors.AppendLine("Укажите ФИО клиента");

            if (StartDate.Text == "")
                errors.AppendLine("Укажите дату услуги");

            if (TBStart.Text == "")
                errors.AppendLine("Укажите время начала услуги");

            if (errors.Length > 0)
            {
                MessageBox.Show(errors.ToString());
                return;
            }

            _currentClientService.ClientID = ComboClient.SelectedIndex + 1;
            _currentClientService.ServiceID = _currentService.ID;
            _currentClientService.StartTime = Convert.ToDateTime(StartDate.Text + " " + TBStart.Text);

            if (_currentClientService.ID == 0)
                Urazbaev_autoserviceEntities.GetContext().ClientService.Add(_currentClientService);
            //Urazbaev_autoserviceEntities.GetContext().SaveChanges();

            try
            {
                Urazbaev_autoserviceEntities.GetContext().SaveChanges();
                MessageBox.Show("информация сохранена");
                Manager.MainFrame.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

        private void TBStart_TextChanged(object sender, TextChangedEventArgs e)
        {
            string s = TBStart.Text;

            if (s.Length < 3 || s.Length > 5 || !s.Contains(':'))
                TBEnd.Text = "";
            else
            {
                string[] start = s.Split(new char[] { ':' });
                try
                {

                    if (Convert.ToInt32(start[0].ToString()) >= 0 && Convert.ToInt32(start[0].ToString()) <= 23 && Convert.ToInt32(start[1].ToString()) >= 0 && Convert.ToInt32(start[1].ToString()) <= 59 && start[1].Length == 2)
                    {
                        int startHour = Convert.ToInt32(start[0].ToString()) * 60;
                        int startMin = Convert.ToInt32(start[1].ToString());

                        int sum = startHour + startMin + _currentService.DurationInSeconds;

                        string EndHour = (sum / 60 % 24).ToString();
                        string EndMin = (sum % 60).ToString();
                        if (Convert.ToInt32(EndMin) / 10 == 0)
                        {
                            EndMin = '0' + EndMin;
                        }
                        s = EndHour.ToString() + ":" + EndMin;
                        TBEnd.Text = s;
                    }
                    else
                    {
                        TBEnd.Text = "";
                    }
                }
                catch
                {
                    TBEnd.Text = "";
                }

            }
        }

        private void TBStart_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // Проверяем, что вводится только цифра
            if (!char.IsDigit(e.Text, e.Text.Length - 1))
            {
                e.Handled = true;
                return;
            }

            // Получаем текущее значение текста в TextBox
            string currentValue = ((TextBox)sender).Text;
            if (currentValue.Length == 0)
            {
                int hours1 = Convert.ToInt32(e.Text);
                TBStart.Clear();
                if (hours1 > 2)
                {
                    currentValue = "";
                    currentValue = "0" + (hours1).ToString();
                    TBStart.Text = "";

                    TBStart.Text = currentValue;
                    e.Handled = true;
                }
            }
            if (currentValue.Length == 1)
            {
                if (currentValue[0] == '2')
                {
                    int hours2 = Convert.ToInt32(e.Text);
                    Console.WriteLine(hours2);
                    if (hours2 > 3)
                    {
                        e.Handled = true; // Игнорируем ввод
                        return;
                    }
                }

            }

            if (currentValue.Length == 3)
            {
                int minute = Convert.ToInt32(e.Text);
                if (minute > 5)
                {
                    e.Handled = true; // Игнорируем ввод
                    return;
                }
            }

            // Если введено 2 цифры и следующий символ не ":", добавляем ":"
            if (currentValue.Length == 2 && e.Text != ":")
            {
                currentValue += ":";
            }

            // Если введено 5 символов (формат "hh:mm"), то не даем вводить больше
            if (currentValue.Length > 5)
            {
                e.Handled = true;
                return;
            }

        // Обновляем значение текста в TextBox
        ((TextBox)sender).Text = currentValue;
            ((TextBox)sender).SelectionStart = ((TextBox)sender).Text.Length;
        }
    }
}
