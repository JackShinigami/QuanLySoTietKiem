using BUS_QLSTK;
using DTO_QLSTK;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Shapes;

namespace GUI_QLSTK
{
    /// <summary>
    /// Interaction logic for ManageConfigWindow.xaml
    /// </summary>
    public partial class ManageConfigWindow : Window, INotifyPropertyChanged
    {
        public long CurrentAmountMinimum { get; set; }
        public long InputAmountMinimum { get; set; }

        public int CurrentDayMinimum { get; set; }
        public int InputDayMinimum { get; set; }

        public double CurrentInterestRate { get; set; }
        public double NewInterestRate { get; set; }

        public bool IsLoading { get; set; }

        Business business = Business.Instance;

        public ManageConfigWindow()
        {
            InitializeComponent();
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            progressBar.Visibility = Visibility.Visible;
            IsLoading = true;
            var list = new List<LoaiTietKiem>();
            try
            {
                await Task.Run(() =>
                {
                    list = getListPeriod();
                    CurrentAmountMinimum = business.get_SoTienGuiToiThieu();
                    CurrentDayMinimum = business.get_NgayGuiToiThieu();
                    IsLoading = false;
                });

            }
            catch (Exception ex)
            {
                errorTextBlock.Text = ex.Message;
            }
            while (IsLoading)
            {
                // wait for loading to finish
            }
            interestRateComboBox.ItemsSource = list;
            periodTypeComboBox.ItemsSource = list;
            this.DataContext = this;
            progressBar.Visibility = Visibility.Hidden;
        }

        #region Period
        private List<LoaiTietKiem> getListPeriod()
        {

            List<LoaiTietKiem> list = business.getList_LoaiTietKiem();
            list.Add(new LoaiTietKiem { Kyhan = -1, Laisuat = 0 });
            list.Sort(new Comparison<LoaiTietKiem>((x, y) => x.Kyhan.CompareTo(y.Kyhan)));
            return list;
        }

        private void periodSaveButton_Click(object sender, RoutedEventArgs e)
        {
            successTextBlock.Text = "";
            errorTextBlock.Text = "";

            progressBar.Visibility = Visibility.Visible;
            IsLoading = true;
            bool addSuccess = false;

            if (periodTypeTextBox.Text.IsNullOrEmpty())
            {
                errorTextBlock.Text = "Vui lòng nhập kì hạn cần thêm";
                return;
            }

            // Add
            int period = 0;
            try
            {
                if (!periodTypeTextBox.Text.IsNullOrEmpty())
                {
                    try
                    {
                        period = int.Parse(periodTypeTextBox.Text);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Kỳ hạn không hợp lệ");
                    }

                    var list = new List<LoaiTietKiem>();
                    Task.Run(() =>
                    {
                        business.add_LoaiTietKiem(period, 0);
                        list = getListPeriod();
                        addSuccess = true;
                        IsLoading = false;
                    });

                    while (IsLoading)
                    {
                        // wait for loading to finish
                    }
                    periodTypeComboBox.ItemsSource = list;
                    interestRateComboBox.ItemsSource = list;
                    throw new Exception("Thêm thành công, vui lòng điều chỉnh lãi suất của kì hạn mới");

                }
            }
            catch (Exception ex)
            {
                if (addSuccess)
                {
                    successTextBlock.Text = ex.Message;
                } else
                {
                    errorTextBlock.Text = ex.Message;
                }
            }

            progressBar.Visibility = Visibility.Hidden; 

        }


        private void periodDeleteButton_Click(object sender, RoutedEventArgs e)
        {
            progressBar.Visibility = Visibility.Visible;
            IsLoading = true;
            successTextBlock.Text = "";
            errorTextBlock.Text = "";
            
            bool removeSuccess = false;
            // Remove
            var periodType = periodTypeComboBox.SelectedItem as LoaiTietKiem;
            int period = 0;
            if (periodType != null && periodType.Kyhan != -1)
            {
                try
                {
                    period = periodType!.Kyhan;

                    var list = new List<LoaiTietKiem>();

                    Task.Run(() =>
                    {
                        business.delete_LoaiTietKiem(periodType);
                        list = business.getList_LoaiTietKiem();
                        removeSuccess = true;
                        IsLoading = false;
                    });

                    while (IsLoading)
                    {
                        // wait for loading to finish
                    }
                    periodTypeComboBox.ItemsSource = list;
                    interestRateComboBox.ItemsSource = list;

                    throw new Exception("Xóa thành công");
                }

                catch (Exception ex)
                {
                    if (removeSuccess)
                    {
                        successTextBlock.Text = ex.Message;
                    } else
                    {
                        errorTextBlock.Text = ex.Message;
                    }
                }
            } else
            {
                   errorTextBlock.Text = "Vui lòng chọn kì hạn cần xóa";
            }

            progressBar.Visibility = Visibility.Hidden;
        }
        #endregion

        #region Minimum Amount
        private void minAmountSaveButton_Click(object sender, RoutedEventArgs e)
        {
            successTextBlock.Text = "";
            errorTextBlock.Text = "";

            progressBar.Visibility = Visibility.Visible;
            IsLoading = true;
            bool success = false;

            LongToVndConverter longToVnd = new LongToVndConverter();

            try
            {
                if (minAmountTextBox.Text.IsNullOrEmpty())
                {
                    throw new Exception("Vui lòng nhập số tiền gửi tối thiểu");
                }

                Task.Run(() =>
                {
                    business.update_SoTienGuiToiThieu(InputAmountMinimum);
                    CurrentAmountMinimum = business.get_SoTienGuiToiThieu();
                    IsLoading = false;
                    success = true;
                });

                while (IsLoading)
                {
                    // wait for loading to finish
                }

                throw new Exception("Cập nhật số tiền gửi tối thiểu thành công");
            }
            catch (Exception ex)
            {
                if (success)
                {
                    successTextBlock.Text = ex.Message;
                } else
                {
                    errorTextBlock.Text = ex.Message;
                }
            }

            progressBar.Visibility = Visibility.Hidden;

        }


        private void minAmountTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                errorTextBlock.Text = "";
                LongToVndConverter longToVnd = new LongToVndConverter();
                InputAmountMinimum = longToVnd.ConvertBack(minAmountTextBox.Text, null, null, null) as long? ?? -1;
                if(InputAmountMinimum < 0)
                {
                    InputAmountMinimum = 0;
                    throw new Exception("Số tiền gửi tối thiểu không hợp lệ");
                }
            }
            catch (Exception ex)
            {
                errorTextBlock.Text = ex.Message;
            }
        }

        #endregion

        #region Minimum Time
        private void minTimeSaveButton_Click(object sender, RoutedEventArgs e)
        {

            successTextBlock.Text = "";
            errorTextBlock.Text = "";

            progressBar.Visibility = Visibility.Visible;
            IsLoading = true;
            bool success = false;

            try
            {
                if (minTimeTextBox.Text.IsNullOrEmpty())
                {
                    throw new Exception("Vui lòng nhập số ngày gửi tối thiểu");
                }
                try
                {
                    InputDayMinimum = int.Parse(minTimeTextBox.Text);
                }
                catch (Exception ex)
                {
                    throw new Exception("Số ngày gửi tối thiểu không hợp lệ");
                }

                Task.Run(() =>
                {
                    business.update_NgayGuiToiThieu(InputDayMinimum);
                    CurrentDayMinimum = business.get_NgayGuiToiThieu();
                    IsLoading = false;
                    success = true;
                });

                while (IsLoading)
                {
                    // wait for loading to finish
                }

                throw new Exception("Cập nhật số ngày gửi tối thiểu thành công");
            }
            catch (Exception ex)
            {
                if (success)
                {
                    successTextBlock.Text = ex.Message;
                } else
                {
                    errorTextBlock.Text = ex.Message;
                }
            }

            progressBar.Visibility = Visibility.Hidden;

        }

        #endregion

        #region Interest Rate
        private void interestRateSaveButton_Click(object sender, RoutedEventArgs e)
        {

            successTextBlock.Text = "";
            errorTextBlock.Text = "";

            progressBar.Visibility = Visibility.Visible;
            IsLoading = true;
            bool success = false;


            FloatToPercent floatToPercent = new FloatToPercent();

            try
            {

                var periodType = interestRateComboBox.SelectedItem as LoaiTietKiem;

                if (periodType!.Kyhan == -1)
                {
                    throw new Exception("Vui lòng chọn loại tiết kiệm");
                }

                if (interestRateTextBox.Text.IsNullOrEmpty())
                {
                    throw new Exception("Vui lòng nhập lãi suất");
                }

                Task.Run(() =>
                {
                    business.update_LoaiTietKiem(periodType!.Kyhan, NewInterestRate);
                    CurrentInterestRate = business.getList_LoaiTietKiem().Find(x => x.Kyhan == periodType!.Kyhan).Laisuat!.Value;
                    IsLoading = false;
                    success = true;
                });

                while (IsLoading)
                {
                    // wait for loading to finish
                }
                throw new Exception("Cập nhật lãi suất thành công");
            }
            catch (Exception ex)
            {
                if (success)
                {
                    successTextBlock.Text = ex.Message;
                } else
                {
                    errorTextBlock.Text = ex.Message;
                }
            }

            progressBar.Visibility = Visibility.Hidden;

        }

        private void interestRateComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // set CurrentInterestRate
            var periodType = interestRateComboBox.SelectedItem as LoaiTietKiem;
            if (periodType == null) return;
            if (periodType.Kyhan == -1) return;
            progressBar.Visibility = Visibility.Visible;
            IsLoading = true;
            try
            {
                Task.Run(() =>
                {
                    var list = getListPeriod();
                    CurrentInterestRate = list.Find(x => x.Kyhan == periodType!.Kyhan).Laisuat!.Value;
                    IsLoading = false;
                });

                while (IsLoading)
                {
                    // wait for loading to finish
                }
            }
            catch (Exception ex)
            {
                errorTextBlock.Text = ex.Message;
            }
            progressBar.Visibility = Visibility.Hidden;
        }

        private void interestRateTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                errorTextBlock.Text = "";
                FloatToPercent floatToPercent = new FloatToPercent();
                NewInterestRate = floatToPercent.ConvertBack(interestRateTextBox.Text, null, null, null) as double? ?? -1;
                if (NewInterestRate < 0)
                {
                    NewInterestRate = 0;
                    throw new Exception("Lãi suất không hợp lệ");
                }
            }
            catch (Exception ex)
            {
                errorTextBlock.Text = ex.Message;
            }
        }
        #endregion


        private void closeButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

    }
}
