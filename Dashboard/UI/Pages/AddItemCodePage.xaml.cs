using BafghAutomation.Engine.Models;
using Dashboard.DataBase;
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

namespace Dashboard.UI.Pages
{
    /// <summary>
    /// Interaction logic for AddItemCodePage.xaml
    /// </summary>
    public partial class AddItemCodePage : Page
    {
        public AddItemCodePage()
        {
            InitializeComponent();
        }

        private const string CancelText = "Cancel";
        private const string AddText = "Add Item";

        private void ItemCodeTextUI_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if(ItemCodeTextUI.Text.Trim()?.Length == 0)
                {
                    if (AddButtonText.Text == AddText)
                        AddButtonText.ShowUsingLinearAnimation(150);

                    AddButtonText.Text = CancelText;
                } else
                {
                    if (AddButtonText.Text == CancelText)
                        AddButtonText.ShowUsingLinearAnimation(150);

                    AddButtonText.Text = AddText;
                }
            } catch { }
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if(AddButtonText.Text == CancelText)
            {
                App.CurrentApp.AppWindow.MainFrame.Navigate(App.CurrentApp.MainPage);
            } else
            {
                try
                {
                    DataBaseHelper.Entities.Goods.Add(
                        new Good(
                            id: 0,
                            itemCode: ItemCodeTextUI.Text,
                            diameter: DiaTextUI.Text,
                            length: LenTextUI.Text,
                            signId: GradeTextUI.Text
                        ));
                    DataBaseHelper.Entities.SaveChanges();
                } catch { }

                App.CurrentApp.AppWindow.MainFrame.Navigate(App.CurrentApp.MainPage);
            }
        }
    }
}
