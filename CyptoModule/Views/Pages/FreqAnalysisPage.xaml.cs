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

namespace CyptoModule.Views.Pages
{
    /// <summary>
    /// Interaction logic for FreqAnalysisPage.xaml
    /// </summary>
    public partial class FreqAnalysisPage : Page
    {
        public FreqAnalysisPage()
        {
            InitializeComponent();
        }

        private void ItemsControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ItemsControl itemsControl = sender as ItemsControl;
        }
    }
}
