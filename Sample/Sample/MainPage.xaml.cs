using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Sample.ViewModel;

namespace Sample
{
	public partial class MainPage : ContentPage
	{
        
        public MainPage()
		{
		    InitializeComponent();         
            this.BindingContext = new MainViewModel();
            Debug.WriteLine("MainPage Started");

           
		}


    }
}
