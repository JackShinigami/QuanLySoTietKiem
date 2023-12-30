﻿using BUS_QLSTK;
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
        public long currentAmountMinimum { get; set; }
        public long inputAmountMinimum { get; set; }

        public int currentDayMinimum { get; set; }
        public int inputDayMinimum { get; set; }

        public double currentInterestRate { get; set; }
        public double newInterestRate { get; set; }

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
                    currentAmountMinimum = business.get_SoTienGuiToiThieu();
                    currentDayMinimum = business.get_NgayGuiToiThieu();
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
            progressBar.Visibility = Visibility.Hidden;
        }

        #region Period
        private List<LoaiTietKiem> getListPeriod()
        {

            List<LoaiTietKiem> list = business.getList_LoaiTietKiem();
            list.Add(new LoaiTietKiem { Kyhan = -1, Laisuat = 0 }) ;
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
            bool removeSuccess = false;

            if (periodTypeComboBox.SelectedIndex == 0 && periodTypeTextBox.Text.IsNullOrEmpty())
            {
                errorTextBlock.Text = "Vui lòng chọn hoặc nhập kì hạn cần thay đổi";
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

            // Remove
            try
            {
                if (periodTypeComboBox.SelectedIndex != 0)
                {
                    var periodType = periodTypeComboBox.SelectedItem as LoaiTietKiem;
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

                    throw new Exception("Xóa thành công");
                }
            }
            catch (Exception ex)
            {
                if(removeSuccess)
                {
                    if(successTextBlock.Text.IsNullOrEmpty()) successTextBlock.Text = ex.Message;
                    else successTextBlock.Text += "; " + ex.Message;
                } else
                {
                    if (errorTextBlock.Text.IsNullOrEmpty()) errorTextBlock.Text = ex.Message;
                    else errorTextBlock.Text += "; " + ex.Message;
                }
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

            try
            {
                if (minAmountTextBox.Text.IsNullOrEmpty())
                {
                    throw new Exception("Vui lòng nhập số tiền gửi tối thiểu");
                }
                try
                {
                    inputAmountMinimum = int.Parse(minAmountTextBox.Text);
                }
                catch (Exception ex)
                {
                    throw new Exception("Số tiền gửi tối thiểu không hợp lệ");
                }

                Task.Run(() =>
                {
                    business.update_SoTienGuiToiThieu(inputAmountMinimum);
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
                if (success) {                 
                    successTextBlock.Text = ex.Message;
                } else
                {
                    errorTextBlock.Text = ex.Message;
                }
            }

            progressBar.Visibility = Visibility.Hidden;

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
                    inputDayMinimum = int.Parse(minTimeTextBox.Text);
                }
                catch (Exception ex)
                {
                    throw new Exception("Số ngày gửi tối thiểu không hợp lệ");
                }

                Task.Run(() =>
                {
                    business.update_NgayGuiToiThieu(inputDayMinimum);
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
                }
                else
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

                try
                {
                    newInterestRate = double.Parse(interestRateTextBox.Text);
                }
                catch (Exception ex)
                {
                    throw new Exception("Lãi suất không hợp lệ");
                }

                Task.Run(() =>
                {
                    business.update_LoaiTietKiem(periodType!.Kyhan, newInterestRate);
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
                }
                else
                {
                    errorTextBlock.Text = ex.Message;
                }
            }

            progressBar.Visibility = Visibility.Hidden;

        }
        #endregion


        private void closeButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
