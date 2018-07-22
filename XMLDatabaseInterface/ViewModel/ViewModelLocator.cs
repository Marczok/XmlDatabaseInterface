/*
  In App.xaml:
  <Application.Resources>
      <vm:ViewModelLocator xmlns:vm="clr-namespace:XMLDatabaseInterface"
                           x:Key="Locator" />
  </Application.Resources>
  
  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"

  You can also use Blend to do all this with the tool's support.
  See http://www.galasoft.ch/mvvm
*/

using CommonServiceLocator;
using GalaSoft.MvvmLight.Ioc;

namespace XMLDatabaseInterface.ViewModel
{
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// </summary>
    public class ViewModelLocator
    {
        /// <summary>
        /// Initializes a new instance of the ViewModelLocator class.
        /// </summary>
        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            ////if (ViewModelBase.IsInDesignModeStatic)
            ////{
            ////    // Create design time view services and models
            ////    SimpleIoc.Default.Register<IDataService, DesignDataService>();
            ////}
            ////else
            ////{
            ////    // Create run time view services and models
            ////    SimpleIoc.Default.Register<IDataService, DataService>();
            ////}
            
            SimpleIoc.Default.Register<MainWindowViewModel>();
            SimpleIoc.Default.Register<IDataProvider, MainWindowViewModel>();
            SimpleIoc.Default.Register<CommonNamesViewModel>();
            SimpleIoc.Default.Register<CommonSurenamesViewModel>();
            SimpleIoc.Default.Register<BirthdayTodayViewModel>();
        }

        public MainWindowViewModel Main => ServiceLocator.Current.GetInstance<MainWindowViewModel>();
        public CommonNamesViewModel CommonNames => ServiceLocator.Current.GetInstance<CommonNamesViewModel>();
        public CommonSurenamesViewModel CommonSurenames => ServiceLocator.Current.GetInstance<CommonSurenamesViewModel>();
        public BirthdayTodayViewModel BirthdayToday => ServiceLocator.Current.GetInstance<BirthdayTodayViewModel>();


        public static void Cleanup()
        {
            SimpleIoc.Default.Reset();
        }
    }
}